// <copyright file="TypeMapIdentifierAlgorithm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Algorithm.StrategyDetectors;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Helpers;
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
    internal virtual MapStrategy GetStrategy()
    {
        IMapStrategyDetector[] detectors = [

            // 01. Identity strategy.
            new IdentityMapStrategyDetector(this.Context, this.Compilation, this.CancellationToken),

            // 02. Nullable related strategies.
            new NullableMapStrategyDetector(this.Context, this.Compilation, this.CancellationToken),

            // 03. Type mapping.
            new PolymorphismMapStrategyDetector(this.Context, this.Compilation, this.CancellationToken),

            // 04. Enum related strategies.
            new EnumMapStrategyDetector(this.Context, this.Compilation),

            // 05. String related strategies.
            new StringMapStrategyDetector(this.Context, this.Compilation),

            // 06. Date and time related strategies.
            new DateAndTimeMapStrategyDetector(this.Context, this.Compilation),

            // 07. IQueryable projection strategies.
            new QueryableProjectionMapStrategyDetector(this.Context, this.Compilation, this.CancellationToken),

            // 08. Container related strategies.
            new ContainerMapStrategyDetector(this.Context, this.Compilation, this.CancellationToken),

            // 09. Tuple related strategies.
            new TupleMapStrategyDetector(this.Context, this.Compilation, this.CancellationToken),

            // 10. Guid related strategies.
            new GuidStrategyDetector(this.Context, this.Compilation),

            // 11. Constructor related strategies.
            new ConstructorMapStrategyDetector(this.Context, this.Compilation, this.CancellationToken),
        ];

        foreach (var detector in detectors)
        {
            // If the operation has been cancelled: stop!
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
                case NullableMapStrategyDetector when !this.Context.AlgorithmSettings.UseNullableMapStrategyDetector:

                // Skip the identity strategy when resolving nested field mappings for nested deep copy,
                // or when nested property-path attributes require constructor-based property mapping.
                case IdentityMapStrategyDetector when !this.Context.AlgorithmSettings.UseIdentityMapStrategyDetector:
                case IdentityMapStrategyDetector when this.Context.PropertyPathContext is { IsNestedAttributeScope: true }:
                case IdentityMapStrategyDetector when this.Context.PropertyPathContext?.RemainingTargetSegments.Length > 0:
                    continue;

                // Polymorphism detector can only run at the root or if following a
                // nullable detector.
                case PolymorphismMapStrategyDetector:
                    if (!CanExecutePolymorphismMapStrategyDetector(this.Context.AlgorithmSettings.Detectors, this.Context.GetRootMapMethod()))
                    {
                        continue;
                    }

                    break;

                // Do not run the identity mapper if we could instead run the mappa polymorphism strategy
                // where there is at least one TypeMapping attribute.
                case IdentityMapStrategyDetector:
                    if (CanExecutePolymorphismMapStrategyDetector(this.Context.AlgorithmSettings.Detectors, this.Context.GetRootMapMethod()))
                    {
                        continue;
                    }

                    break;
            }

            using (this.Context.AlgorithmSettings.ApplyAlgorithmContextDefaults())
            {
                using (this.Context.AlgorithmSettings.Detectors.Apply(detector.GetType()))
                {
                    if (detector.TryDetect(out var detectedStrategy))
                    {
                        return detectedStrategy;
                    }
                }
            }

            // If any error diagnostic has been reported there is no point in going ahead.
            if (this.Context.HasErrorDiagnostics)
            {
                break;
            }
        }

        // Report error because a mapping cannot be identified but only if no other error
        // diagnostic has been reported before that.
        if (!this.Context.HasErrorDiagnostics)
        {
            this.Context.ReportDiagnostic(MappaDiagnostics.CannotIdentifyStrategy(
                this.Context.TargetType,
                this.Context.SourceType,
                this.Context.GetLocation()));
        }

        return new NoMapStrategy(this.Context.TargetType, this.Context.SourceType);

        // Identify if the polymorphism detector can actually be executed.
        static bool CanExecutePolymorphismMapStrategyDetector(StackSetting<Type> detectorsStack, MapMethod mapMethod)
            => mapMethod.HasAnyAttribute<MappaTypeMappingAttribute>() && detectorsStack.Count switch
            {
                // If only one item is present on the stack then
                // there is actually nothing on the stack beside
                // the default value null.
                1 => true,

                // If one detector is on the stack you can apply the polymorphism
                // detector only if that detector is the nullability detector.
                2 => detectorsStack.CurrentValue == typeof(NullableMapStrategyDetector),

                // In any other scenario the polymorphism detector
                // cannot be used.
                _ => false,
            };
    }
}