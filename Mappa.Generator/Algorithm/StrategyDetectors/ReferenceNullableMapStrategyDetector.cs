// <copyright file="ReferenceNullableMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for reference nullable strategies.
/// </summary>
internal sealed class ReferenceNullableMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceNullableMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public ReferenceNullableMapStrategyDetector(MappaMapAlgorithmContext context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public bool TryDetect(out IMapStrategy mapStrategy)
    {
        // 01. (nullable enabled) S? -> T? : ReferenceNullableToReferenceNullableStrategy( IMapStrategy(T, S) )
        // TODO: Implement me
        // 02. (nullable enabled) S? -> T : SourceReferenceNullableStrategy ( IMapStrategy(T ,S) )
        // TODO: Implement me
        // 03. (nullable enabled) S -> T? : TargetReferenceNullableStrategy ( IMapStrategy(T, S) )
        // TODO: Implement me
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        return mapStrategy is not NoMapStrategy;
    }
}