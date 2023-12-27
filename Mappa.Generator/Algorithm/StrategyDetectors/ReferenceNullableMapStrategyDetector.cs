// <copyright file="ReferenceNullableMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for reference nullable strategies.
/// </summary>
internal sealed class ReferenceNullableMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;
    private readonly CancellationToken cancellationToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceNullableMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public ReferenceNullableMapStrategyDetector(MappaMapAlgorithmContext context, Compilation compilation, CancellationToken cancellationToken)
    {
        this.context = context;
        this.compilation = compilation;
        this.cancellationToken = cancellationToken;
    }

    /// <inheritdoc/>
    public bool TryDetect(out IMapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // 01. (nullable disabled) S -> T : ReferenceNullableToReferenceNullableStrategy( IMapStrategy(T, S) )
        if (this.CanMapReferenceToReferenceWhenNullableIsEnabled(out var referenceToReferenceWhenNullableIsDisabledStrategy))
        {
            mapStrategy = new ReferenceNullableToReferenceNullableMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                referenceToReferenceWhenNullableIsDisabledStrategy);
        }

        // 02. (nullable enabled) S? -> T? : ReferenceNullableToReferenceNullableStrategy( IMapStrategy(T, S) )
        else if (this.CanMapReferenceNullableToReferenceNullable(out var nullableToNullableInnerStrategy))
        {
            mapStrategy = new ReferenceNullableToReferenceNullableMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                nullableToNullableInnerStrategy);
        }

        // 03. (nullable enabled) S? -> T : SourceReferenceNullableStrategy ( IMapStrategy(T ,S) )
        else if (this.CanMapFromReferenceNullable(out var fromNullableInnerStrategy))
        {
            mapStrategy = new FromReferenceNullableMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                fromNullableInnerStrategy);
        }

        // 04. (nullable enabled) S -> T? : TargetReferenceNullableStrategy ( IMapStrategy(T, S) )
        else if (this.CanMapToReferenceNullable(out var toNullableInnerStrategy))
        {
            mapStrategy = new ToReferenceNullableMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                toNullableInnerStrategy);
        }

        return mapStrategy is not NoMapStrategy;
    }

    private bool CanMapReferenceToReferenceWhenNullableIsEnabled(out IMapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        if (this.context.IsNullableEnabled())
        {
            return false;
        }

        return this.TryGetStrategyWithReferenceNullableDisabled(out mapStrategy);
    }

    private bool CanMapReferenceNullableToReferenceNullable(out IMapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        var isSourceNullable = this.context.SourceType.IsReferenceNullable();
        var isTargetNullable = this.context.TargetType.IsReferenceNullable();

        return isSourceNullable && isTargetNullable && this.TryGetStrategyWithReferenceNullableDisabled(out mapStrategy);
    }

    private bool CanMapFromReferenceNullable(out IMapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        var isSourceNullable = this.context.SourceType.IsReferenceNullable();

        return isSourceNullable && this.TryGetStrategyWithReferenceNullableDisabled(out mapStrategy);
    }

    private bool CanMapToReferenceNullable(out IMapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        var isTargetNullable = this.context.TargetType.IsReferenceNullable();

        return isTargetNullable && this.TryGetStrategyWithReferenceNullableDisabled(out mapStrategy);
    }

    private bool TryGetStrategyWithReferenceNullableDisabled(out IMapStrategy elementStrategy)
    {
        var derivedContext = new DerivedMappaMapAlgorithmContext(
            this.context,
            this.context.TargetType,
            this.context.SourceType);
        using (this.context.Settings.UseReferenceNullableMapStrategyDetector.Apply(false))
        {
            // Do not attempt to obtain a method: we would obtain the very same.
            var algorithm = new TypeMapIdentifierAlgorithm(derivedContext, this.compilation, this.cancellationToken);
            elementStrategy = algorithm.GetStrategy();
            return elementStrategy is not NoMapStrategy;
        }
    }
}