// <copyright file="MethodParameterMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="MethodParameterMapStrategy"/> strategy.
/// </summary>
internal sealed class MethodParameterMapStrategyBuilder
    : IMappaStrategyBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MethodParameterMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="methodParameterMapStrategy">The strategy.</param>
    public MethodParameterMapStrategyBuilder(MethodParameterMapStrategy methodParameterMapStrategy)
    {
        this.MethodParameterMapStrategy = methodParameterMapStrategy;
    }

    /// <summary>
    /// Gets the strategy.
    /// </summary>
    private MethodParameterMapStrategy MethodParameterMapStrategy { get; }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var maxRuntimeDepthInitialization = ReferenceHandlingCodeGenerator.BuildMaxRuntimeDepthInitialization(context);
        var beforeMapHooks = this.MethodParameterMapStrategy.BeforeMapHooks;
        var afterMapHooks = this.MethodParameterMapStrategy.AfterMapHooks;
        if (beforeMapHooks.Count == 0 && afterMapHooks.Count == 0)
        {
            var (strategySource, header) = ReferenceHandlingCodeGenerator.BuildRootSource(
                this.MethodParameterMapStrategy.Strategy,
                source,
                context,
                mappaGlobalOptions);
            return ($"return {strategySource};", JoinHeader(maxRuntimeDepthInitialization, header));
        }

        return this.BuildSourceWithMapHooks(
            source,
            context,
            mappaGlobalOptions,
            maxRuntimeDepthInitialization,
            beforeMapHooks,
            afterMapHooks);
    }

    private static string JoinHeader(string? maxRuntimeDepthInitialization, string header)
    {
        if (maxRuntimeDepthInitialization is null)
        {
            return header;
        }

        if (string.IsNullOrWhiteSpace(header))
        {
            return maxRuntimeDepthInitialization;
        }

        return $"{maxRuntimeDepthInitialization}\n{header}";
    }

    private static bool RequiresMappedValue(MapHook hook, Compilation compilation)
        => hook.Method.Parameters.Length > 0 &&
           !hook.Method.ParameterIsMappaContext(compilation, 0);

    private static string BuildHookInvocation(
        MapHook hook,
        string mappedValue,
        MappaBuilderContext context)
    {
        var accessor = GetAccessor(hook);
        var arguments = GetArguments(hook, mappedValue, context);
        return $"{accessor}{hook.Method.Name}({arguments});";
    }

    private static string GetAccessor(MapHook hook)
    {
        if (hook.ExplicitType is not null)
        {
            return $"{hook.ExplicitType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.";
        }

        if (hook.FieldOrProperty is not null)
        {
            if (hook.Method.IsStatic)
            {
                var fieldOrPropertyType = GetFieldOrPropertyType(hook.FieldOrProperty);
                return $"{fieldOrPropertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.";
            }

            var fieldOrPropertyAccessor = hook.FieldOrProperty.IsStatic ? string.Empty : "this.";
            return $"{fieldOrPropertyAccessor}{hook.FieldOrProperty.Name}.";
        }

        return hook.Method.IsStatic ? string.Empty : "this.";
    }

    private static string GetArguments(
        MapHook hook,
        string mappedValue,
        MappaBuilderContext context)
    {
        return hook.Method.Parameters.Length switch
        {
            0 => string.Empty,
            1 when hook.Method.ParameterIsMappaContext(context.Compilation, 0)
                => context.GetMapMethod().GetMappaContextParameterName(),
            1 => $"ref {mappedValue}",
            2 => $"ref {mappedValue}, {context.GetMapMethod().GetMappaContextParameterName()}",
            _ => throw new MappaGeneratorException($"Unexpected hook method signature '{hook.Method.ToDisplayString()}'."),
        };
    }

    private static ITypeSymbol GetFieldOrPropertyType(ISymbol fieldOrProperty)
        => fieldOrProperty switch
        {
            IFieldSymbol field => field.Type,
            IPropertySymbol property => property.Type,
            _ => throw new MappaGeneratorException($"Unexpected symbol kind '{fieldOrProperty.Kind}' for field or property '{fieldOrProperty.Name}'."),
        };

    private (string VariableName, string Code) BuildSourceWithMapHooks(
        string source,
        MappaBuilderContext context,
        MappaGlobalOptions mappaGlobalOptions,
        string? maxRuntimeDepthInitialization,
        IReadOnlyList<MapHook> beforeMapHooks,
        IReadOnlyList<MapHook> afterMapHooks)
    {
        var code = new List<string>();
        if (maxRuntimeDepthInitialization is not null)
        {
            code.Add(maxRuntimeDepthInitialization);
        }

        var strategyInput = this.ResolveStrategyInputForBeforeMapHooks(source, context, beforeMapHooks, code);

        foreach (var hook in beforeMapHooks)
        {
            code.Add(BuildHookInvocation(hook, strategyInput, context));
        }

        var (mappedValue, mappingCode) = ReferenceHandlingCodeGenerator.BuildRootSource(
            this.MethodParameterMapStrategy.Strategy,
            strategyInput,
            context,
            mappaGlobalOptions);
        if (!string.IsNullOrWhiteSpace(mappingCode))
        {
            code.Add(mappingCode);
        }

        if (afterMapHooks.Count == 0)
        {
            return ($"return {mappedValue};", string.Join("\n", code));
        }

        var targetTemporary = context.NextTemporary();
        code.Add($"{this.MethodParameterMapStrategy.TargetType.ToDisplayString()} {targetTemporary} = {mappedValue};");
        foreach (var hook in afterMapHooks)
        {
            code.Add(BuildHookInvocation(hook, targetTemporary, context));
        }

        return ($"return {targetTemporary};", string.Join("\n", code));
    }

    private string ResolveStrategyInputForBeforeMapHooks(
        string source,
        MappaBuilderContext context,
        IReadOnlyList<MapHook> beforeMapHooks,
        List<string> code)
    {
        if (!beforeMapHooks.Any(hook => RequiresMappedValue(hook, context.Compilation)))
        {
            return source;
        }

        if (context.GetMapMethod().GetSourceParameterRefKind() is not RefKind.In)
        {
            return source;
        }

        var strategyInput = context.NextTemporary();
        code.Add($"{this.MethodParameterMapStrategy.SourceType.ToDisplayString()} {strategyInput} = {source};");
        return strategyInput;
    }
}