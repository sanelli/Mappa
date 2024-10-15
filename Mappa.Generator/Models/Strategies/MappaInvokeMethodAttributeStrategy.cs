// <copyright file="MappaInvokeMethodAttributeStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to be used when we are mapping
/// using the <see cref="MappaInvokeMethodAttribute"/>.
/// </summary>
internal sealed class MappaInvokeMethodAttributeStrategy
     : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaInvokeMethodAttributeStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The type of the target.</param>
    /// <param name="attribute">The attribute, as specified by the user on the mapper method.</param>
    /// <param name="method">The method to be invoked.</param>
    /// <param name="sourceProperty">The optional source property to be used by the method.</param>
    public MappaInvokeMethodAttributeStrategy(
        ITypeSymbol targetType,
        MappaInvokeMethodAttribute attribute,
        IMethodSymbol method,
        IPropertySymbol? sourceProperty)
    {
        this.TargetType = targetType;
        this.SourceType = null!;
        this.Attribute = attribute;
        this.Method = method;
        this.SourceProperty = sourceProperty;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    public ITypeSymbol SourceType { get; }

    /// <inheritdoc/>
    public MappaAlgorithmRule Rule => MappaAlgorithmRule.InvokeMethodFromAttribute;

    /// <summary>
    /// Gets the method that should be invoked.
    /// </summary>
    internal IMethodSymbol Method { get; }

    /// <summary>
    /// Gets the source property that can be used to invoke the mapper.
    /// </summary>
    internal IPropertySymbol? SourceProperty { get; }

    /// <summary>
    /// Gets the attribute as specified by the user.
    /// </summary>
    internal MappaInvokeMethodAttribute Attribute { get; }

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder()
    {
        return new MappaInvokeMethodAttributeStrategyBuilder(this);
    }
}