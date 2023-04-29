// <copyright file="NullableMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for nullable related strategies.
/// </summary>
internal sealed class NullableMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;
    private readonly CancellationToken cancellationToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public NullableMapStrategyDetector(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        this.context = context;
        this.cancellationToken = cancellationToken;
        this.compilation = compilation;
    }

    /// <inheritdoc/>
    public bool TryDetect(out IMapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // 01. Nullable<S> -> Nullable<T> : NullableToNullableStrategy( IMapStrategy(T, S) )
        if (this.CanMapNullableToNullable(out var elementStrategy))
        {
            mapStrategy = new NullableToNullableMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                elementStrategy);
        }

        // 02. Nullable<S> -> T : SourceIsNullableStrategy ( IMapStrategy(T, S) )
        // TODO: Implement me
        // 03.S -> Nullable<T>
        // TODO: Implement me  : TargetIsNullableStrategy ( IMapStrategy(T, S) )
        return mapStrategy is not NoMapStrategy;
    }

    private bool CanMapNullableToNullable(out IMapStrategy elementStrategy)
    {
        var isSourceNullable = this.context.SourceType.IsNullable();
        var isTargetNullable = this.context.TargetType.IsNullable();

        elementStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        return isSourceNullable && isTargetNullable && this.TryGetElementStrategy(out elementStrategy);
    }

    private bool TryGetElementStrategy(out IMapStrategy elementStrategy)
    {
        var sourceElementType = this.context.SourceType.GetElementType();
        var targetElementType = this.context.TargetType.GetElementType();
        var derivedContext = new DerivedMappaMapAlgorithmContext(
            this.context,
            targetElementType,
            sourceElementType);
        var algorithm = new TypeMapIdentifierWithMapMethodAlgorithm(derivedContext, this.compilation, this.cancellationToken);
        elementStrategy = algorithm.GetStrategy();
        return elementStrategy is not NoMapStrategy;
    }
}