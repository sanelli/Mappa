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

    private string GetParameters(string source, MappaBuilderContext context)
    {
        var compilation = context.Compilation;
        var method = this.strategy.Method;
        var compositeSource = context.GetCompositeTypeSourceName();

        switch (method.Parameters.Length)
        {
            case 0:
                return string.Empty;

            case 3:
                return $"{compositeSource}, {source}, {this.GetContextArgument()}";

            case 2:
                if (method.ParameterIsMappaContext(compilation, 1))
                {
                    if (method.Parameters[0].Type.IsEqualTo(this.strategy.SourceType, this.strategy.IsNullableEnabled) ||
                        compilation.HasImplicitConversion(this.strategy.SourceType, method.Parameters[0].Type))
                    {
                        return $"{compositeSource}, {this.GetContextArgument()}";
                    }

                    return $"{source}, {this.GetContextArgument()}";
                }

                return $"{compositeSource}, {source}";

            case 1:
                if (method.ParameterIsMappaContext(compilation, 0))
                {
                    return this.GetContextArgument();
                }

                if (method.Parameters[0].Type.IsEqualTo(this.strategy.SourceType, this.strategy.IsNullableEnabled) ||
                    compilation.HasImplicitConversion(this.strategy.SourceType, method.Parameters[0].Type))
                {
                    return compositeSource;
                }

                if (this.strategy.SourceProperty is not null &&
                    (method.Parameters[0].Type.IsEqualTo(this.strategy.SourceProperty.Type, this.strategy.IsNullableEnabled) ||
                    compilation.HasImplicitConversion(this.strategy.SourceProperty.Type, method.Parameters[0].Type)))
                {
                    return source;
                }

                break;
        }

        throw new MappaGeneratorException("Unexpected parameter type");
    }

    private string GetContextArgument()
        => this.strategy.ContextParameterName
           ?? throw new MappaGeneratorException("Invoked method requires MappaContext but the root map method does not provide one.");
}