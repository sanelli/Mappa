// <copyright file="ValueTransformerMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ValueTransformerMapStrategy"/>.
/// </summary>
internal sealed class ValueTransformerMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly ValueTransformerMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueTransformerMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public ValueTransformerMapStrategyBuilder(ValueTransformerMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var code = new List<string>();

        var innerSource = ChainTransformers(this.strategy.SourceTransformers, source, context, code);

        var (innerVariableName, innerCode) = this.strategy.InnerStrategy.GetBuilder().BuildSource(
            innerSource,
            context,
            mappaGlobalOptions);
        if (!string.IsNullOrWhiteSpace(innerCode))
        {
            code.Add(innerCode);
        }

        var result = ChainTransformers(this.strategy.TargetTransformers, innerVariableName, context, code);
        return (result, string.Join("\n", code));
    }

    private static string ChainTransformers(
        IReadOnlyList<ValueTransformer> transformers,
        string value,
        MappaBuilderContext context,
        List<string> code)
    {
        var current = value;
        foreach (var transformer in transformers)
        {
            var temporary = context.NextTemporary();
            code.Add($"var {temporary} = {GetAccessor(transformer)}{transformer.Method.Name}({GetArguments(transformer, current, context)});");
            current = temporary;
        }

        return current;
    }

    private static string GetAccessor(ValueTransformer transformer)
    {
        if (transformer.ExplicitType is not null)
        {
            return $"{transformer.ExplicitType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.";
        }

        if (transformer.FieldOrProperty is not null)
        {
            if (transformer.Method.IsStatic)
            {
                var fieldOrPropertyType = ValueTransformer.GetFieldOrPropertyType(transformer.FieldOrProperty);
                return $"{fieldOrPropertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.";
            }

            var fieldOrPropertyAccessor = transformer.FieldOrProperty.IsStatic ? string.Empty : "this.";
            return $"{fieldOrPropertyAccessor}{transformer.FieldOrProperty.Name}.";
        }

        return transformer.Method.IsStatic ? string.Empty : "this.";
    }

    private static string GetArguments(
        ValueTransformer transformer,
        string value,
        MappaBuilderContext context)
    {
        // The signature is validated during discovery: a transformer has either one or two parameters.
        return transformer.RequiresContext
            ? $"{value}, {context.GetMapMethod().GetMappaContextParameterName()}"
            : value;
    }
}