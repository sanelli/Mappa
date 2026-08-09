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
    protected Compilation Compilation { get; }

    /// <summary>
    /// Compute a suitable strategy from type <see cref="MappaMapAlgorithmContext.SourceType"/> to
    /// <see cref="MappaMapAlgorithmContext.TargetType"/>.
    /// </summary>
    /// <returns>The strategy computed.</returns>
    internal virtual MapStrategy GetStrategy()
        => this.WithGetStrategyGuards(this.ComputeStrategy);

    /// <summary>
    /// Runs <paramref name="computeStrategy"/> under the compile-time mapping-cycle stack and
    /// depth guards shared by all <see cref="GetStrategy"/> entry points.
    /// </summary>
    /// <param name="computeStrategy">The strategy identification callback.</param>
    /// <returns>
    /// The strategy computed, or <see cref="NoMapStrategy"/> when a mapping cycle is detected
    /// or the compile-time depth limit is exceeded.
    /// </returns>
    protected MapStrategy WithGetStrategyGuards(Func<MapStrategy> computeStrategy)
    {
        this.CancellationToken.ThrowIfCancellationRequested();

        IDisposable? typePairScope = null;
        try
        {
            typePairScope = this.Context.TryPushMappingTypePair(
                this.Context.TargetType,
                this.Context.SourceType);
            if (typePairScope is null)
            {
                // Detectors may intentionally re-enter GetStrategy for the same type pair while one of
                // the recursive-guard settings is disabled (nullable unwrap, constructor single-arg,
                // identity nested fields) or while DetectMappingCycles is disabled (e.g. polymorphism
                // MapSourceType defaults). Those paths already prevent unbounded recursion.
                if (this.ShouldReportMappingCycle())
                {
                    // Prefer an existing map method for the cycling pair (user or synthetic) so a
                    // synthetic method body can self-invoke even when BreakCompileTimeCycles is no
                    // longer Enable on the settings stack (e.g. method-level Enable only).
                    if (this.Context.TryGetMethod(
                            this.Context.TargetType,
                            this.Context.SourceType,
                            out var existingMapMethodOnCycle))
                    {
                        var rootMapMethod = this.Context.GetRootMapMethod();
                        return this.WrapIfNullableReferenceSource(
                            new MethodMapStrategy(
                                existingMapMethodOnCycle,
                                rootMapMethod.MaybeGetMappaContextParameterName()));
                    }

                    if (this.Context.MappaUserSettings.BreakCompileTimeCycles is BooleanSetting.Enable)
                    {
                        return this.BreakCompileTimeCycleWithSyntheticOrExistingMethod();
                    }

                    this.Context.ReportDiagnostic(MappaDiagnostics.MappingCycleDetected(
                        this.Context.GetLocation(),
                        this.Context.SourceType,
                        this.Context.TargetType));
                    return new NoMapStrategy(this.Context.TargetType, this.Context.SourceType);
                }

                return this.WithCompileTimeDepthGuard(computeStrategy);
            }

            return this.WithCompileTimeDepthGuard(computeStrategy);
        }
        finally
        {
            typePairScope?.Dispose();
        }
    }

    /// <summary>
    /// Identify a strategy by running the detector pipeline without looking up existing map methods first.
    /// </summary>
    /// <returns>The strategy computed.</returns>
    protected MapStrategy ComputeStrategy()
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

            if (this.ShouldSkipDetector(detector))
            {
                continue;
            }

            if (this.TryDetectWithDetector(detector, out var detectedStrategy))
            {
                return detectedStrategy;
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
    }

    /// <summary>
    /// Wraps a map-method strategy in <see cref="NullableStrategy"/> when the source is a nullable
    /// reference type, so <c>null</c> edges short-circuit before invoking the nested map method.
    /// </summary>
    /// <param name="strategy">The strategy that maps the non-null source.</param>
    /// <returns>The original strategy, or a nullable wrapper around it.</returns>
    protected MapStrategy WrapIfNullableReferenceSource(MapStrategy strategy)
    {
        var sourceType = this.Context.SourceType;
        if (sourceType is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated })
        {
            return new NullableStrategy(this.Context.TargetType, sourceType, strategy);
        }

        return strategy;
    }

    private static bool CanExecutePolymorphismMapStrategyDetector(StackSetting<Type> detectorsStack, MapMethod mapMethod)
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

    private bool ShouldSkipDetector(IMapStrategyDetector detector)
    {
        if (this.ShouldSkipConstructorOrNullableDetector(detector)
            || this.ShouldSkipIdentityDetectorForSettingsOrNestedPaths(detector))
        {
            return true;
        }

        if (detector is PolymorphismMapStrategyDetector)
        {
            return !CanExecutePolymorphismMapStrategyDetector(this.Context.AlgorithmSettings.Detectors, this.Context.GetRootMapMethod());
        }

        if (detector is IdentityMapStrategyDetector)
        {
            return CanExecutePolymorphismMapStrategyDetector(this.Context.AlgorithmSettings.Detectors, this.Context.GetRootMapMethod());
        }

        return false;
    }

    private bool ShouldSkipConstructorOrNullableDetector(IMapStrategyDetector detector)
        => (detector is ConstructorMapStrategyDetector && !this.Context.AlgorithmSettings.UseConstructorMapStrategyDetector)
           || (detector is NullableMapStrategyDetector && !this.Context.AlgorithmSettings.UseNullableMapStrategyDetector);

    private bool ShouldSkipIdentityDetectorForSettingsOrNestedPaths(IMapStrategyDetector detector)
        => detector is IdentityMapStrategyDetector
           && (!this.Context.AlgorithmSettings.UseIdentityMapStrategyDetector
               || this.Context.PropertyPathContext is { IsNestedAttributeScope: true }
               || this.Context.PropertyPathContext?.RemainingTargetSegments.Length > 0);

    private bool TryDetectWithDetector(IMapStrategyDetector detector, out MapStrategy detectedStrategy)
    {
        detectedStrategy = new NoMapStrategy(this.Context.TargetType, this.Context.SourceType);
        using (this.Context.AlgorithmSettings.ApplyAlgorithmContextDefaults())
        {
            using (this.Context.AlgorithmSettings.Detectors.Apply(detector.GetType()))
            {
                if (detector.TryDetect(out detectedStrategy))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool ShouldReportMappingCycle()
        => this.Context.AlgorithmSettings.DetectMappingCycles.CurrentValue
           && this.Context.AlgorithmSettings.UseNullableMapStrategyDetector.CurrentValue
           && this.Context.AlgorithmSettings.UseConstructorMapStrategyDetector.CurrentValue
           && this.Context.AlgorithmSettings.UseIdentityMapStrategyDetector.CurrentValue;

    /// <summary>
    /// Breaks a compile-time mapping cycle by synthesizing a private map method and returning
    /// a <see cref="MethodMapStrategy"/>. Callers must only invoke this when
    /// <see cref="MappaMapAlgorithmContext.TryGetMethod"/> has already failed for the cycling pair.
    /// </summary>
    /// <returns>The method invocation strategy for the cycling type pair.</returns>
    private MapStrategy BreakCompileTimeCycleWithSyntheticOrExistingMethod()
    {
        var targetType = this.Context.TargetType;
        var sourceType = this.Context.SourceType;
        var rootMapMethod = this.Context.GetRootMapMethod();
        var contextParameterName = rootMapMethod.MaybeGetMappaContextParameterName();

        if (!this.Context.TryGetClassGeneratorContext(out var classContext) || classContext is null)
        {
            this.Context.ReportDiagnostic(MappaDiagnostics.MappingCycleDetected(
                this.Context.GetLocation(),
                sourceType,
                targetType));
            return new NoMapStrategy(targetType, sourceType);
        }

        var methodName = SyntheticMapMethodNaming.AllocateName(classContext, sourceType, targetType);
        var syntheticMapMethod = MapMethod.CreateSynthetic(
            methodName,
            sourceType,
            targetType,
            classContext.ClassSymbol,
            this.Context.IsNullableEnabled(),
            rootMapMethod.CanBeUsedByStaticMethod,
            sourceParameterName: "source",
            mappaContextParameterName: contextParameterName,
            location: this.Context.GetLocation());

        if (!classContext.TryAddMethod(syntheticMapMethod))
        {
            // A map for the pair exists but is not usable from this call site
            // (for example an instance-only method while the root map is static).
            this.Context.ReportDiagnostic(MappaDiagnostics.MappingCycleDetected(
                this.Context.GetLocation(),
                sourceType,
                targetType));
            return new NoMapStrategy(targetType, sourceType);
        }

        this.Context.ReportDiagnostic(MappaDiagnostics.MappingCycleAutoBroken(
            this.Context.GetLocation(),
            sourceType,
            targetType,
            methodName));

        return this.WrapIfNullableReferenceSource(
            new MethodMapStrategy(syntheticMapMethod, contextParameterName));
    }

    /// <summary>
    /// Runs <paramref name="computeStrategy"/> under the compile-time depth guard when
    /// <see cref="IMappaUserSettings.MaxCompileTimeDepth"/> is greater than zero.
    /// </summary>
    /// <param name="computeStrategy">The strategy identification callback.</param>
    /// <returns>The strategy computed, or <see cref="NoMapStrategy"/> when the depth limit is exceeded.</returns>
    private MapStrategy WithCompileTimeDepthGuard(Func<MapStrategy> computeStrategy)
    {
        var maxCompileTimeDepth = this.Context.MappaUserSettings.MaxCompileTimeDepth;
        if (maxCompileTimeDepth == 0)
        {
            return computeStrategy();
        }

        using (this.Context.IncreaseCompileTimeDepth())
        {
            if (this.Context.CurrentDepth > maxCompileTimeDepth)
            {
                this.Context.ReportDiagnostic(MappaDiagnostics.MaxCompileTimeDepthReached(
                    this.Context.GetLocation(),
                    this.Context.SourceType,
                    this.Context.TargetType,
                    maxCompileTimeDepth));
                return new NoMapStrategy(this.Context.TargetType, this.Context.SourceType);
            }

            return computeStrategy();
        }
    }
}