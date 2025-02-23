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
/// <param name="targetType">The type of the target.</param>
/// <param name="sourceClassType">The type of the source.</param>
/// <param name="attribute">The attribute, as specified by the user on the mapper method.</param>
/// <param name="method">The method to be invoked.</param>
/// <param name="sourceProperty">The optional source property to be used by the method.</param>
/// <param name="isNullableEnabled"><c>true</c> if nullable is enabled at this invocation point.</param>
internal sealed class MappaInvokeMethodAttributeStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceClassType,
    MappaInvokeMethodAttribute attribute,
    IMethodSymbol method,
    IPropertySymbol? sourceProperty,
    bool isNullableEnabled)
          : MapStrategy(targetType, sourceClassType)
{
    /// <summary>
    /// Gets the method that should be invoked.
    /// </summary>
    internal IMethodSymbol Method { get; } = method;

    /// <summary>
    /// Gets the source property that can be used to invoke the mapper.
    /// </summary>
    internal IPropertySymbol? SourceProperty { get; } = sourceProperty;

    /// <summary>
    /// Gets the attribute as specified by the user.
    /// </summary>
    internal MappaInvokeMethodAttribute Attribute { get; } = attribute;

    /// <summary>
    /// Gets a value indicating whether <c>nullable</c> is enabled for reference types.
    /// </summary>
    internal bool IsNullableEnabled { get; } = isNullableEnabled;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new MappaInvokeMethodAttributeStrategyBuilder(this);
}