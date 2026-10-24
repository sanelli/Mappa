// <copyright file="OneOfToValueMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to unwrap a <c>OneOf</c> / <c>OneOfBase</c> to a uniquely matching arm type <c>S</c>.
/// </summary>
internal sealed class OneOfToValueMapStrategy
    : MapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneOfToValueMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type <c>S</c>.</param>
    /// <param name="sourceType">The <c>OneOf</c> / <c>OneOfBase</c> source type.</param>
    /// <param name="armIndex">The selected arm index.</param>
    /// <param name="armType">The selected arm type.</param>
    public OneOfToValueMapStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        int armIndex,
        ITypeSymbol armType)
        : base(targetType, sourceType)
    {
        this.ArmIndex = armIndex;
        this.ArmType = armType;
    }

    /// <summary>
    /// Gets the selected arm index.
    /// </summary>
    internal int ArmIndex { get; }

    /// <summary>
    /// Gets the selected arm type.
    /// </summary>
    internal ITypeSymbol ArmType { get; }

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new OneOfToValueMapStrategyBuilder(this);
}