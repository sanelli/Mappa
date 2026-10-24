// <copyright file="OneOfOptionToValueMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to unwrap a <c>OneOf&lt;T, None&gt;</c> / <c>OneOfBase&lt;T, None&gt;</c> Option to <c>T</c> or <c>T?</c>.
/// </summary>
internal sealed class OneOfOptionToValueMapStrategy
    : MapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneOfOptionToValueMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type (<c>T</c> or <c>T?</c>).</param>
    /// <param name="sourceType">The Option source type.</param>
    /// <param name="valueType">The Option value arm type (<c>T0</c>).</param>
    /// <param name="absentBehavior">How to handle <c>None</c>.</param>
    public OneOfOptionToValueMapStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        ITypeSymbol valueType,
        OneOfOptionAbsentBehavior absentBehavior)
        : base(targetType, sourceType)
    {
        this.ValueType = valueType;
        this.AbsentBehavior = absentBehavior;
    }

    /// <summary>
    /// Gets the Option value arm type (<c>T0</c>).
    /// </summary>
    internal ITypeSymbol ValueType { get; }

    /// <summary>
    /// Gets the behavior when the Option is <c>None</c>.
    /// </summary>
    internal OneOfOptionAbsentBehavior AbsentBehavior { get; }

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new OneOfOptionToValueMapStrategyBuilder(this);
}