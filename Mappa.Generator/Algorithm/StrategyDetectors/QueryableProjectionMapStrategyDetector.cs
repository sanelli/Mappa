// <copyright file="QueryableProjectionMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
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

        if (!this.TryGetQueryableElementMapping(
                out var sourceElementType,
                out var targetElementType,
                out var normalizedElementStrategy))
        {
            return false;
        }

        var mapMethodSymbol = this.context.MapMethod?.MethodSymbol;
        if (mapMethodSymbol is null)
        {
            return false;
        }

        mapStrategy = new QueryableProjectionMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            normalizedElementStrategy,
            sourceElementType,
            targetElementType,
            mapMethodSymbol);
        return true;
    }

    private bool TryGetQueryableElementMapping(
        out ITypeSymbol sourceElementType,
        out ITypeSymbol targetElementType,
        out MapStrategy normalizedElementStrategy)
    {
        normalizedElementStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        sourceElementType = this.context.SourceType;
        targetElementType = this.context.TargetType;

        if (!this.CanStartQueryableElementMapping())
        {
            return false;
        }

        if (!this.TryGetDistinctQueryableElementTypes(out sourceElementType, out targetElementType))
        {
            return false;
        }

        var mapMethod = this.context.MapMethod;
        if (mapMethod is null)
        {
            return false;
        }

        return this.TryGetProjectableQueryableElementStrategy(
            mapMethod,
            sourceElementType,
            targetElementType,
            out normalizedElementStrategy);
    }

    private bool CanStartQueryableElementMapping()
    {
        if (this.context.MapMethod is null)
        {
            return false;
        }

        if (ReferenceHandlingCodeGenerator.IsReferenceHandlingRequested(this.context.MappaUserSettings))
        {
            return false;
        }

        return this.context.SourceType.IsOrImplementIQueryable(this.compilation)
               && this.context.TargetType.IsOrImplementIQueryable(this.compilation);
    }

    private bool TryGetDistinctQueryableElementTypes(
        out ITypeSymbol sourceElementType,
        out ITypeSymbol targetElementType)
    {
        sourceElementType = this.context.SourceType;
        targetElementType = this.context.TargetType;

        if (!this.context.SourceType.TryGetQueryableElementType(this.compilation, out sourceElementType)
            || !this.context.TargetType.TryGetQueryableElementType(this.compilation, out targetElementType))
        {
            return false;
        }

        return !SymbolEqualityComparer.Default.Equals(sourceElementType, targetElementType);
    }

    private bool TryGetProjectableQueryableElementStrategy(
        MapMethod mapMethod,
        ITypeSymbol sourceElementType,
        ITypeSymbol targetElementType,
        out MapStrategy normalizedElementStrategy)
    {
        normalizedElementStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        var derivedContext = new DerivedMappaMapAlgorithmContext(
            this.context,
            targetElementType,
            sourceElementType);
        var elementAlgorithm = new TypeMapIdentifierWithMapMethodAlgorithm(
            derivedContext,
            this.compilation,
            this.cancellationToken);
        var elementStrategy = elementAlgorithm.GetStrategy();
        if (elementStrategy is NoMapStrategy)
        {
            return false;
        }

        return ProjectionCapabilityAnalyzer.TryAnalyze(
            elementStrategy,
            new ProjectionCapabilityAnalysisContext(
                this.context,
                this.compilation,
                mapMethod.MethodName,
                mapMethod.MethodDeclarationSyntax?.GetLocation(),
                this.cancellationToken),
            out normalizedElementStrategy);
    }
}