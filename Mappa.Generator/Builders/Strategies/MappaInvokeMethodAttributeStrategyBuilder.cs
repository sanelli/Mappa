// <copyright file="MappaInvokeMethodAttributeStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

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

    // TODO [#70] Support method with MappaContext.
    private string GetParameters(string source, MappaBuilderContext context)
    {
        switch (this.strategy.Method.Parameters.Length)
        {
            case 0:
                return string.Empty;

            case 1:

                if (this.strategy.Method.Parameters[0].Type.IsEqualTo(this.strategy.SourceType, this.strategy.IsNullableEnabled) ||
                    context.Compilation.HasImplicitConversion(this.strategy.SourceType, this.strategy.Method.Parameters[0].Type))
                {
                    return context.GetCompositeTypeSourceName();
                }

                if (this.strategy.SourceProperty is not null &&
                    (this.strategy.Method.Parameters[0].Type.IsEqualTo(this.strategy.SourceProperty.Type, this.strategy.IsNullableEnabled) ||
                    context.Compilation.HasImplicitConversion(this.strategy.SourceProperty.Type, this.strategy.Method.Parameters[0].Type)))
                {
                    return $"{source}";
                }

                throw new MappaGeneratorException("Unexpected parameter type");

            case 2:
                if (this.strategy.SourceProperty is not null)
                {
                    return $"{context.GetCompositeTypeSourceName()}, {source}";
                }

                break;
        }

        throw new MappaGeneratorException("Unsupported number of parameters.");
    }
}