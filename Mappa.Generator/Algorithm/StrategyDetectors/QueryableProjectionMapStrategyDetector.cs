// <copyright file="QueryableProjectionMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for <see cref="QueryableProjectionMapStrategy"/>.
/// </summary>
internal sealed class QueryableProjectionMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;
    private readonly CancellationToken cancellationToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryableProjectionMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public QueryableProjectionMapStrategyDetector(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        this.context = context;
        this.compilation = compilation;
        this.cancellationToken = cancellationToken;
    }

    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        if (this.context.MapMethod is null)
        {
            return false;
        }

        if (!this.context.SourceType.IsOrImplementIQueryable(this.compilation)
            || !this.context.TargetType.IsOrImplementIQueryable(this.compilation))
        {
            return false;
        }

        if (!this.context.SourceType.TryGetQueryableElementType(this.compilation, out var sourceElementType)
            || !this.context.TargetType.TryGetQueryableElementType(this.compilation, out var targetElementType))
        {
            return false;
        }

        if (SymbolEqualityComparer.Default.Equals(sourceElementType, targetElementType))
        {
            return false;
        }

        if (!this.context.TryGetElementStrategy(
                targetElementType,
                sourceElementType,
                this.compilation,
                out var elementStrategy,
                this.cancellationToken))
        {
            return false;
        }

        if (!ProjectionCapabilityAnalyzer.IsSupported(elementStrategy))
        {
            return false;
        }

        mapStrategy = new QueryableProjectionMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            elementStrategy,
            sourceElementType,
            targetElementType,
            this.context.MapMethod.MethodSymbol);
        return true;
    }
}