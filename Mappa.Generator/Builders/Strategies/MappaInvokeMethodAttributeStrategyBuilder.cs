// <copyright file="MappaInvokeMethodAttributeStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="MappaInvokeMethodAttributeStrategy"/>.
/// </summary>
internal sealed class MappaInvokeMethodAttributeStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly MappaInvokeMethodAttributeStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaInvokeMethodAttributeStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy to build.</param>
    public MappaInvokeMethodAttributeStrategyBuilder(MappaInvokeMethodAttributeStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var targetTemporary = context.NextTemporary();
        var accessor = this.GetAccessor();
        var parameters = this.GetParameters(source, context);
        var code = $"{this.strategy.TargetType.ToDisplayString()} {targetTemporary} = {accessor}{this.strategy.Method.Name}({parameters});";
        return (targetTemporary, code);
    }

    private string GetAccessor()
    {
        string accessor;
        if (this.strategy.Attribute.ClassType is not null)
        {
            var classTypeFullName = this.strategy.Attribute.ClassType.FullName ?? throw new MappaGeneratorException($"Cannot detect name for type {this.strategy.Attribute.ClassType}");
            accessor = $"{classTypeFullName}.";
        }
        else if (this.strategy.FieldOrProperty is not null)
        {
            var fieldAccessor = this.strategy.FieldOrProperty.IsStatic ? string.Empty : "this.";
            accessor = $"{fieldAccessor}{this.strategy.FieldOrProperty.Name}.";
        }
        else
        {
            accessor = this.strategy.Method.IsStatic ? string.Empty : "this.";
        }

        return accessor;
    }

    private string GetParameters(string source, MappaBuilderContext context)
    {
        var compilation = context.Compilation;
        var method = this.strategy.Method;
        var compositeSource = context.GetCompositeTypeSourceName();

        return method.Parameters.Length switch
        {
            0 => string.Empty,
            1 => this.GetParametersForSingleArgumentMethod(compilation, source, compositeSource),
            2 => this.GetParametersForTwoArgumentMethod(compilation, source, compositeSource),
            3 => $"{compositeSource}, {source}, {this.GetContextArgument()}",
            _ => throw new MappaGeneratorException("Unexpected parameter type"),
        };
    }

    private string GetParametersForSingleArgumentMethod(Compilation compilation, string source, string compositeSource)
    {
        var method = this.strategy.Method;
        if (method.ParameterIsMappaContext(compilation, 0))
        {
            return this.GetContextArgument();
        }

        if (this.ParameterAcceptsCompositeSource(method.Parameters[0].Type, compilation))
        {
            return compositeSource;
        }

        if (this.strategy.SourceProperty is not null &&
            this.ParameterAcceptsPropertyType(method.Parameters[0].Type, compilation))
        {
            return source;
        }

        throw new MappaGeneratorException("Unexpected parameter type");
    }

    private string GetParametersForTwoArgumentMethod(Compilation compilation, string source, string compositeSource)
    {
        var method = this.strategy.Method;
        if (method.ParameterIsMappaContext(compilation, 1))
        {
            if (this.ParameterAcceptsCompositeSource(method.Parameters[0].Type, compilation))
            {
                return $"{compositeSource}, {this.GetContextArgument()}";
            }

            return $"{source}, {this.GetContextArgument()}";
        }

        return $"{compositeSource}, {source}";
    }

    private bool ParameterAcceptsCompositeSource(ITypeSymbol parameterType, Compilation compilation)
        => parameterType.IsEqualTo(this.strategy.SourceType, this.strategy.IsNullableEnabled) ||
           compilation.HasImplicitConversion(this.strategy.SourceType, parameterType);

    private bool ParameterAcceptsPropertyType(ITypeSymbol parameterType, Compilation compilation)
        => this.strategy.SourceProperty is not null &&
           (parameterType.IsEqualTo(this.strategy.SourceProperty.Type, this.strategy.IsNullableEnabled) ||
            compilation.HasImplicitConversion(this.strategy.SourceProperty.Type, parameterType));

    private string GetContextArgument()
        => this.strategy.ContextParameterName
           ?? throw new MappaGeneratorException("Invoked method requires MappaContext but the root map method does not provide one.");
}