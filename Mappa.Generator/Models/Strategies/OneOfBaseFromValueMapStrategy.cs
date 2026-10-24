// <copyright file="OneOfBaseFromValueMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to wrap a value into a constructible <c>OneOfBase</c> (or derived) target.
/// </summary>
internal sealed class OneOfBaseFromValueMapStrategy
    : MapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneOfBaseFromValueMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The <c>OneOfBase</c> or derived target type.</param>
    /// <param name="sourceType">The source value type.</param>
    /// <param name="armIndex">The selected arm index.</param>
    /// <param name="armType">The selected arm type.</param>
    /// <param name="constructionKind">How to construct the target.</param>
    /// <param name="optionStructType">The underlying <c>OneOf</c> struct when using <see cref="OneOfBaseConstructionKind.ConstructorFromOneOf"/>.</param>
    public OneOfBaseFromValueMapStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        int armIndex,
        ITypeSymbol armType,
        OneOfBaseConstructionKind constructionKind,
        INamedTypeSymbol? optionStructType)
        : base(targetType, sourceType)
    {
        this.ArmIndex = armIndex;
        this.ArmType = armType;
        this.ConstructionKind = constructionKind;
        this.OptionStructType = optionStructType;
    }

    /// <summary>
    /// Gets the selected arm index.
    /// </summary>
    internal int ArmIndex { get; }

    /// <summary>
    /// Gets the selected arm type.
    /// </summary>
    internal ITypeSymbol ArmType { get; }

    /// <summary>
    /// Gets the construction kind.
    /// </summary>
    internal OneOfBaseConstructionKind ConstructionKind { get; }

    /// <summary>
    /// Gets the underlying <c>OneOf</c> struct type when required for construction.
    /// </summary>
    internal INamedTypeSymbol? OptionStructType { get; }

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new OneOfBaseFromValueMapStrategyBuilder(this);
}