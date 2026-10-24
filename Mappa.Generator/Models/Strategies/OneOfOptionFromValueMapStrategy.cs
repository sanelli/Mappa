// <copyright file="OneOfOptionFromValueMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to wrap a value into a <c>OneOf&lt;T, None&gt;</c> / <c>OneOfBase&lt;T, None&gt;</c> Option.
/// </summary>
internal sealed class OneOfOptionFromValueMapStrategy
    : MapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneOfOptionFromValueMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The Option target type.</param>
    /// <param name="sourceType">The source value type (<c>T</c> or <c>T?</c>).</param>
    /// <param name="valueType">The Option value arm type (<c>T0</c>).</param>
    /// <param name="optionStructType">The underlying <c>OneOf&lt;T, None&gt;</c> struct type used for <c>FromT0</c>/<c>FromT1</c>.</param>
    /// <param name="wrapBehavior">How to decide between Some and <c>None</c>.</param>
    /// <param name="isOneOfBaseTarget">Whether <paramref name="targetType"/> derives from <c>OneOfBase</c> and must be constructed from a <c>OneOf</c>.</param>
    public OneOfOptionFromValueMapStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        ITypeSymbol valueType,
        INamedTypeSymbol optionStructType,
        OneOfOptionWrapBehavior wrapBehavior,
        bool isOneOfBaseTarget)
        : base(targetType, sourceType)
    {
        this.ValueType = valueType;
        this.OptionStructType = optionStructType;
        this.WrapBehavior = wrapBehavior;
        this.IsOneOfBaseTarget = isOneOfBaseTarget;
    }

    /// <summary>
    /// Gets the Option value arm type (<c>T0</c>).
    /// </summary>
    internal ITypeSymbol ValueType { get; }

    /// <summary>
    /// Gets the underlying <c>OneOf&lt;T, None&gt;</c> struct type.
    /// </summary>
    internal INamedTypeSymbol OptionStructType { get; }

    /// <summary>
    /// Gets the wrap behavior.
    /// </summary>
    internal OneOfOptionWrapBehavior WrapBehavior { get; }

    /// <summary>
    /// Gets a value indicating whether the target derives from <c>OneOfBase</c>.
    /// </summary>
    internal bool IsOneOfBaseTarget { get; }

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new OneOfOptionFromValueMapStrategyBuilder(this);
}