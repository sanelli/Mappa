// <copyright file="InvokeMappingConstructorMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy used when invoking the mapping constructor of a class.
/// </summary>
internal sealed class InvokeMappingConstructorMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeMappingConstructorMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="constructor">The constructor.</param>
    /// <param name="parameterStrategy">The parameter strategy.</param>
    public InvokeMappingConstructorMapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType, IMethodSymbol constructor, IMapStrategy parameterStrategy)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
        this.Constructor = constructor;
        this.ParameterStrategy = parameterStrategy;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    public ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the constructor to be used.
    /// </summary>
    public IMethodSymbol Constructor { get; }

    /// <summary>
    /// Gets the strategy for the parameter.
    /// </summary>
    public IMapStrategy ParameterStrategy { get; }

    /// <inheritdoc/>
    public MappaAlgorithmRule Rule => MappaAlgorithmRule.InvokeMappingConstructor;

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new InvokeMappingConstructorMapStrategyBuilder(this);
}