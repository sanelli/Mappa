// <copyright file="InvokeConstructorMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy used to invoke the constructor.
/// </summary>
internal sealed class InvokeConstructorMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeConstructorMapStrategy"/> class.
    /// </summary>
    /// <param name="rule">The rule applied.</param>
    /// <param name="targetType">The source type.</param>
    /// <param name="sourceType">The target type.</param>
    /// <param name="constructor">Gets the constructor.</param>
    /// <param name="argumentsStrategies">The strategies to be applied via constructor parameters.</param>
    /// <param name="initializerStrategies">The strategies to be applied via initializers.</param>
    public InvokeConstructorMapStrategy(
        MappaAlgorithmRule rule,
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        IMethodSymbol constructor,
        PropertyMapStrategy[] argumentsStrategies,
        PropertyMapStrategy[] initializerStrategies)
    {
        if (rule is not MappaAlgorithmRule.InvokeEmptyConstructor and not MappaAlgorithmRule.InvokeConstructor)
        {
            throw new ArgumentException($"Only {MappaAlgorithmRule.InvokeEmptyConstructor} and {MappaAlgorithmRule.InvokeConstructor} are valid rules for {nameof(InvokeConstructorMapStrategy)}", nameof(rule));
        }

        this.Rule = rule;
        this.TargetType = targetType;
        this.SourceType = sourceType;
        this.Constructor = constructor;
        this.ArgumentsStrategies = argumentsStrategies;
        this.InitializerStrategies = initializerStrategies;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    public ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the constructor used.
    /// </summary>
    public IMethodSymbol Constructor { get; private set; }

    /// <summary>
    /// Gets the strategies that can be applied via constructor parameters.
    /// </summary>
    public PropertyMapStrategy[] ArgumentsStrategies { get; }

    /// <summary>
    /// Gets the strategies that can be applied via initializers.
    /// </summary>
    public PropertyMapStrategy[] InitializerStrategies { get; }

    /// <inheritdoc/>
    public MappaAlgorithmRule Rule { get; }

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new InvokeConstructorMapStrategyBuilder(this);
}