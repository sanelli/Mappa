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
        IMapStrategyDetector[] detectors = [

            // 01. Identity strategy.
            new IdentityMapStrategyDetector(this.Context, this.Compilation),

            // 02. Nullable related strategies.
            new NullableMapStrategyDetector(this.Context, this.Compilation, this.CancellationToken),

            // 03. Reference nullable related strategies.
            new ReferenceNullableMapStrategyDetector(this.Context, this.Compilation, this.CancellationToken),

            // 04. Enum related strategies.
            new EnumMapStrategyDetector(this.Context, this.Compilation),

            // 05. String related strategies.
            new StringMapStrategyDetector(this.Context, this.Compilation),

            // 06. Date and time related strategies.
            new DateAndTimeMapStrategyDetector(this.Context, this.Compilation),

            // 07. Container related strategies.
            new ContainerMapStrategyDetector(this.Context, this.Compilation, this.CancellationToken),

            // 08. Tuple related strategies.
            new TupleMapStrategyDetector(this.Context, this.Compilation, this.CancellationToken),

            // 09. Constructor related strategies.
            new ConstructorMapStrategyDetector(this.Context, this.Compilation, this.CancellationToken),
        ];

        foreach (var detector in detectors)
        {
            this.CancellationToken.ThrowIfCancellationRequested();

            switch (detector)
            {
                // Skip the constructor strategy in order to avoid infinite loops
                // in the case this algorithm is run inside the constructor strategy
                // detector itself.
                case ConstructorMapStrategyDetector when !this.Context.AlgorithmSettings.UseConstructorMapStrategyDetector:

                // Skip the nullable reference strategy in order to avoid infinite loops
                // in the case this algorithm is run inside the nullable reference strategy
                // detector itself.
                case ReferenceNullableMapStrategyDetector when !this.Context.AlgorithmSettings.UseReferenceNullableMapStrategyDetector:
                    continue;
            }

            using (this.Context.AlgorithmSettings.ApplyAlgorithmContextDefaults())
            {
                if (detector.TryDetect(out var detectedStrategy))
                {
                    return detectedStrategy;
                }
            }
        }

        // Report error
        this.Context.ReportDiagnostic(MappaDiagnostics.CannotIdentifyStrategy(
            this.Context.TargetType,
            this.Context.SourceType,
            this.Context.GetLocation()));
        return new NoMapStrategy(this.Context.TargetType, this.Context.SourceType);
    }
}