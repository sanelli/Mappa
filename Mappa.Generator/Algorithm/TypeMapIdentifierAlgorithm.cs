// <copyright file="TypeMapIdentifierAlgorithm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Algorithm.StrategyDetectors;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm;

/// <summary>
/// Algorithm to identify a suitable map strategy from <see cref="MappaMapAlgorithmContext.SourceType"/> to
/// <see cref="MappaMapAlgorithmContext.TargetType"/>.
/// </summary>
internal class TypeMapIdentifierAlgorithm
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeMapIdentifierAlgorithm"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public TypeMapIdentifierAlgorithm(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        this.Context = context;
        this.Compilation = compilation;
        this.CancellationToken = cancellationToken;
    }

    /// <summary>
    /// Gets the context.
    /// </summary>
    protected MappaMapAlgorithmContext Context { get; }

    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    protected CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets the compilation.
    /// </summary>
    private Compilation Compilation { get; }

    /// <summary>
    /// Compute a suitable strategy from type <see cref="MappaMapAlgorithmContext.SourceType"/> to
    /// <see cref="MappaMapAlgorithmContext.TargetType"/>.
    /// </summary>
    /// <returns>The strategy computed.</returns>
    internal virtual IMapStrategy GetStrategy()
    {
        this.CancellationToken.ThrowIfCancellationRequested();
        IMapStrategyDetector[] detectors =
        {
            // 01. Identity strategy.
            new IdentityMapStrategyDetector(this.Context, this.Compilation),

            // 02. Enum related strategies.
            new EnumMapStrategyDetector(this.Context, this.Compilation),

            // 03. String related strategies.
            new StringMapStrategyDetector(this.Context),

            // 04. Nullable related strategies.
            new NullableMapStrategyDetector(this.Context, this.Compilation, this.CancellationToken),

            // 05. Container related strategies.
            new ContainerMapStrategyDetector(this.Context, this.Compilation, this.CancellationToken),

            // 06. Tuple related strategies.
            new TupleMapStrategyDetector(this.Context, this.Compilation, this.CancellationToken),
        };

        foreach (var detector in detectors)
        {
            this.CancellationToken.ThrowIfCancellationRequested();
            if (detector.TryDetect(out var detectedStrategy))
            {
                return detectedStrategy;
            }
        }

        // 21. (nullable enabled) S? -> T? : ReferenceNullableToReferenceNullableStrategy( IMapStrategy(T,S) )
        // TODO: Implement me
        // 22. (nullable enabled) S? -> T : SourceReferenceNullableStrategy ( IMapStrategy(T ,S) )
        // TODO: Implement me
        // 23. (nullable enabled) S -> T? : TargetReferenceNullableStrategy ( IMapStrategy(T, S) )
        // TODO: Implement me
        // 24. S -> T : ConstructorStrategy(T, S)
        // TODO: Implement me
        // Report error
        this.Context.ReportDiagnostic(MappaDiagnostics.CannotIdentifyStrategy(
            this.Context.TargetType,
            this.Context.SourceType,
            this.Context.GetLocation()));
        return new NoMapStrategy(this.Context.TargetType, this.Context.SourceType);
    }
}