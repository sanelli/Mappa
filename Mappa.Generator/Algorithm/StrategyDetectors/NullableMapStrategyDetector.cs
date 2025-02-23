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
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // 01. Nullable<S> -> Nullable<T> : NullableToNullableStrategy( IMapStrategy(T, S) )
        if (this.CanMapNullableToNullable(out var nullableToNullableElementStrategy))
        {
            mapStrategy = new NullableToNullableMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                nullableToNullableElementStrategy);
        }

        // 02. Nullable<S> -> T : SourceIsNullableStrategy ( IMapStrategy(T, S) )
        else if (this.CanMapNullableToNonNullable(out var nullableToNonNullableElementStrategy))
        {
            mapStrategy = new NullableToNonNullableMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                nullableToNonNullableElementStrategy);
        }

        // 03. S -> Nullable<T>
        else if (this.CanMapNonNullableToNullable(out var nonNullableToNullableElementStrategy))
        {
            mapStrategy = new NonNullableToNullableMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                nonNullableToNullableElementStrategy);
        }

        return mapStrategy is not NoMapStrategy;
    }

    private bool CanMapNullableToNullable(out MapStrategy elementStrategy)
    {
        var isSourceNullable = this.context.SourceType.IsNullable();
        var isTargetNullable = this.context.TargetType.IsNullable();

        elementStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        return isSourceNullable && isTargetNullable && this.TryGetElementStrategy(out elementStrategy);
    }

    private bool CanMapNullableToNonNullable(out MapStrategy elementStrategy)
    {
        var isSourceNullable = this.context.SourceType.IsNullable();

        elementStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        return isSourceNullable && this.TryGetSourceElementStrategy(out elementStrategy);
    }

    private bool CanMapNonNullableToNullable(out MapStrategy elementStrategy)
    {
        var isTargetTypeNullable = this.context.TargetType.IsNullable();

        elementStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        return isTargetTypeNullable && this.TryGetTargetElementStrategy(out elementStrategy);
    }

    private bool TryGetElementStrategy(out MapStrategy elementStrategy)
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

    private bool TryGetSourceElementStrategy(out MapStrategy elementStrategy)
    {
        var sourceElementType = this.context.SourceType.GetElementType();
        var targetType = this.context.TargetType;
        var derivedContext = new DerivedMappaMapAlgorithmContext(
            this.context,
            targetType,
            sourceElementType);
        var algorithm = new TypeMapIdentifierWithMapMethodAlgorithm(derivedContext, this.compilation, this.cancellationToken);
        elementStrategy = algorithm.GetStrategy();
        return elementStrategy is not NoMapStrategy;
    }

    private bool TryGetTargetElementStrategy(out MapStrategy elementStrategy)
    {
        var sourceType = this.context.SourceType;
        var targetElementType = this.context.TargetType.GetElementType();
        var derivedContext = new DerivedMappaMapAlgorithmContext(
            this.context,
            targetElementType,
            sourceType);
        var algorithm = new TypeMapIdentifierWithMapMethodAlgorithm(derivedContext, this.compilation, this.cancellationToken);
        elementStrategy = algorithm.GetStrategy();
        return elementStrategy is not NoMapStrategy;
    }
}