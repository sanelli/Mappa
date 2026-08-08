// <copyright file="PolymorphismMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="PolymorphismMapStrategy"/> strategy.
/// </summary>
/// <param name="strategy">The strategy.</param>
internal sealed class PolymorphismMapStrategyBuilder(PolymorphismMapStrategy strategy)
    : IMappaStrategyBuilder
{
    private readonly PolymorphismMapStrategy strategy = strategy;

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();
        var targetTemporary = context.NextTemporary();

        builder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {targetTemporary};");
        builder.AppendLine($"switch ({source})");
        using (builder.CurlyBracesBlock())
        {
            foreach (var subtypeStrategy in this.strategy.SubtypesMappingsStrategies)
            {
                var subtypeStrategyTemporary = context.NextTemporary();
                builder.AppendLine($"case {subtypeStrategy.SourceType.ToDisplayString()} {subtypeStrategyTemporary}:");
                using (builder.CurlyBracesBlock())
                {
                    var (subtypeStrategyTargetVariable, subtypeStrategyCode) = ReferenceHandlingCodeGenerator.BuildNestedSource(
                        subtypeStrategy,
                        subtypeStrategyTemporary,
                        context,
                        mappaGlobalOptions);
                    builder.AppendLine(subtypeStrategyCode);
                    builder.AppendLine($"{targetTemporary} = {subtypeStrategyTargetVariable};");
                    builder.AppendLine("break;");
                }

                builder.AppendEmptyLine();
            }

            builder.AppendLine("default:");
            using (builder.CurlyBracesBlock())
            {
                AppendDefaultCode(
                    this.strategy.DefaultBehavior,
                    source,
                    targetTemporary,
                    this.strategy.DefaultBehaviorStrategy,
                    this.strategy.DefaultInvokeMethod,
                    this.strategy.MapMethodContextParameterName,
                    builder,
                    context,
                    mappaGlobalOptions);
            }
        }

        return (targetTemporary, builder.ToString());
    }

    private static void AppendDefaultCode(
        MappaTypeMappingDefaultAttribute attribute,
        string source,
        string targetTemporary,
        MapStrategy defaultBehaviorStrategy,
        IMethodSymbol? defaultInvokeMethod,
        string? contextParameterName,
        PrettyCode.StringBuilder builder,
        MappaBuilderContext context,
        MappaGlobalOptions mappaGlobalOptions)
    {
        switch (attribute.Behavior)
        {
            case MappaTypeMappingDefaultBehavior.Undefined:
                throw new MappaGeneratorException("Unexpected undefined behavior while generating default branch for type mapping.");
            case MappaTypeMappingDefaultBehavior.Throw:
                AppendThrowDefaultCode(attribute, source, context, builder);
                break;
            case MappaTypeMappingDefaultBehavior.Default:
                AppendAssignDefaultAndBreak(targetTemporary, builder);
                break;
            case MappaTypeMappingDefaultBehavior.Null:
                AppendAssignNullAndBreak(targetTemporary, builder);
                break;
            case MappaTypeMappingDefaultBehavior.MapSourceType:
                AppendMapSourceTypeDefaultCode(
                    source,
                    targetTemporary,
                    defaultBehaviorStrategy,
                    builder,
                    context,
                    mappaGlobalOptions);
                break;
            case MappaTypeMappingDefaultBehavior.InvokeMethod:
                AppendInvokeMethodDefaultCode(
                    attribute,
                    source,
                    targetTemporary,
                    defaultInvokeMethod,
                    contextParameterName,
                    builder,
                    context);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attribute));
        }
    }

    private static void AppendThrowDefaultCode(
        MappaTypeMappingDefaultAttribute attribute,
        string source,
        MappaBuilderContext context,
        PrettyCode.StringBuilder builder)
    {
        var exceptionToThrow = attribute.Type is { } exceptionType
            ? (exceptionType.FullName ?? throw new MappaGeneratorException("Cannot obtain exception type name"))
            : "System.ArgumentOutOfRangeException";

        var exceptionSymbol = context.Compilation.GetTypeByMetadataName(exceptionToThrow)
            ?? throw new MappaGeneratorException("Cannot obtain exception type by name");
        var parameters = string.Empty;

        if (exceptionSymbol.HasNamedTypeSymbolAccessibleSingleStringParametersConstructor(context.Compilation))
        {
            parameters = $"nameof({source})";
        }
        else if (!exceptionSymbol.HasNamedTypeSymbolAccessibleZeroParametersConstructor(context.Compilation))
        {
            throw new MappaGeneratorException("Cannot identify a suitable constructor to generate the exception");
        }

        builder.AppendLine($"throw new global::{exceptionToThrow}({parameters});");
    }

    private static void AppendAssignDefaultAndBreak(string targetTemporary, PrettyCode.StringBuilder builder)
    {
        builder.AppendLine($"{targetTemporary} = default;");
        builder.AppendLine("break;");
    }

    private static void AppendAssignNullAndBreak(string targetTemporary, PrettyCode.StringBuilder builder)
    {
        builder.AppendLine($"{targetTemporary} = null;");
        builder.AppendLine("break;");
    }

    private static void AppendMapSourceTypeDefaultCode(
        string source,
        string targetTemporary,
        MapStrategy defaultBehaviorStrategy,
        PrettyCode.StringBuilder builder,
        MappaBuilderContext context,
        MappaGlobalOptions mappaGlobalOptions)
    {
        var (defaultVariable, defaultCode) = ReferenceHandlingCodeGenerator.BuildNestedSource(
            defaultBehaviorStrategy,
            source,
            context,
            mappaGlobalOptions);
        if (!string.IsNullOrWhiteSpace(defaultCode))
        {
            builder.AppendLine(defaultCode);
        }

        builder.AppendLine($"{targetTemporary} = {defaultVariable};");
        builder.AppendLine("break;");
    }

    private static void AppendInvokeMethodDefaultCode(
        MappaTypeMappingDefaultAttribute attribute,
        string source,
        string targetTemporary,
        IMethodSymbol? defaultInvokeMethod,
        string? contextParameterName,
        PrettyCode.StringBuilder builder,
        MappaBuilderContext context)
    {
        if (defaultInvokeMethod is null)
        {
            throw new MappaGeneratorException("Cannot identify the method to be invoked.");
        }

        var invokeMethodTypeSymbol =
            (attribute.Type is { } invokingType && !string.IsNullOrWhiteSpace(invokingType.FullName))
                ? context.Compilation.GetTypeByMetadataName(invokingType.FullName)
                : context.GetMapMethod().ContainingType as ITypeSymbol;
        if (invokeMethodTypeSymbol is null)
        {
            throw new MappaGeneratorException("Cannot identify the type on which the method is being invoked on.");
        }

        var methodInvocationCode = BuildMethodInvocationCode(
            context.GetMapMethod().ContainingType,
            invokeMethodTypeSymbol,
            defaultInvokeMethod,
            source,
            contextParameterName);
        builder.AppendLine($"{targetTemporary} = {methodInvocationCode};");
        builder.AppendLine("break;");
    }

    private static string BuildMethodInvocationCode(
        ITypeSymbol mapMethodTypeSymbol,
        ITypeSymbol? typeSymbol,
        IMethodSymbol method,
        string source,
        string? contextParameterName)
    {
        var head = string.Empty;
        if (typeSymbol is not null && !SymbolEqualityComparer.Default.Equals(typeSymbol, mapMethodTypeSymbol))
        {
            head = $"global::{typeSymbol.ToDisplayString()}.";
        }
        else if (!method.IsStatic)
        {
            head = "this.";
        }

        string parameters;
        switch (method.Parameters.Length)
        {
            case 0:
                parameters = string.Empty;
                break;

            case 1:
                parameters = source;
                break;

            case 2:
                if (string.IsNullOrWhiteSpace(contextParameterName))
                {
                    throw new MappaGeneratorException("Default mapping method requires to parameters but context on original mapping is not provided.");
                }

                parameters = $"{source}, {contextParameterName}";
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(method), $@"Unexpected number of parameters for method '{method.Name}'.");
        }

        return $"{head}{method.Name}({parameters})";
    }
}