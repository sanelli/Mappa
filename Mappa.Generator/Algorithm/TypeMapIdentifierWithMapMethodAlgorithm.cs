// <copyright file="TypeMapIdentifierWithMapMethodAlgorithm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm;

/// <summary>
/// Algorithm to identify a suitable map strategy from <see cref="MappaMapAlgorithmContext.SourceType"/>
/// to <see cref="MappaMapAlgorithmContext.TargetType"/>. This is similar to <see cref="TypeMapIdentifierAlgorithm"/>
/// but it first check if a suitable map already exists in <see cref="TypeMapIdentifierAlgorithm.Context"/>.
/// </summary>
internal sealed class TypeMapIdentifierWithMapMethodAlgorithm
    : TypeMapIdentifierAlgorithm
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeMapIdentifierWithMapMethodAlgorithm"/> class.
    /// </summary>
    /// <param name="methodContext">The mappa method generator context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public TypeMapIdentifierWithMapMethodAlgorithm(
        MappaMapAlgorithmContext methodContext,
        Compilation compilation,
        CancellationToken cancellationToken)
        : base(methodContext, compilation, cancellationToken)
    {
    }

    /// <inheritdoc/>
    internal override MapStrategy GetStrategy()
        => this.WithGetStrategyGuards(this.ComputeStrategyWithMapMethod);

    private MapStrategy ComputeStrategyWithMapMethod()
    {
        this.CancellationToken.ThrowIfCancellationRequested();

        if (this.Context.PropertyPathContext is null)
        {
            if (this.Context.TryGetMethod(this.Context.TargetType, this.Context.SourceType, out var mapMethod))
            {
                var mapMethodRequireMappaContext = mapMethod.RequireMappaContextWhenInvoked();
                var rootMapMethod = this.Context.GetRootMapMethod();
                var callerMethodProvideMappaContext = rootMapMethod.ProvideMappaContextWhenInvoked();

                if (!mapMethodRequireMappaContext || /* mapMethodRequireMappaContext && */ callerMethodProvideMappaContext)
                {
                    this.MaybeReportNestedMapWithoutMappaContext(mapMethod, mapMethodRequireMappaContext);
                    return this.WrapIfNullableReferenceSource(
                        new MethodMapStrategy(mapMethod, rootMapMethod.MaybeGetMappaContextParameterName()));
                }
            }

            if (this.Context.MappaUserSettings.CompatibleMapMethod is BooleanSetting.Enable
                && this.Context.TryGetCompatibleMethod(this.Context.TargetType, this.Context.SourceType, this.Compilation, out mapMethod)
                && !ReferenceEquals(mapMethod.MethodSymbol, this.Context.GetRootMapMethod().MethodSymbol))
            {
                var mapMethodRequireMappaContext = mapMethod.RequireMappaContextWhenInvoked();
                var rootMapMethod = this.Context.GetRootMapMethod();
                var callerMethodProvideMappaContext = rootMapMethod.ProvideMappaContextWhenInvoked();

                if (!mapMethodRequireMappaContext || /* mapMethodRequireMappaContext && */ callerMethodProvideMappaContext)
                {
                    this.MaybeReportNestedMapWithoutMappaContext(mapMethod, mapMethodRequireMappaContext);
                    return this.WrapIfNullableReferenceSource(
                        new CompatibleMethodMapStrategy(
                            this.Context.TargetType,
                            this.Context.SourceType,
                            mapMethod,
                            rootMapMethod.MaybeGetMappaContextParameterName()));
                }
            }

            if (this.Context.TryGetPolymorphicMethod(this.Context.TargetType, this.Context.SourceType, this.Context.MappaUserSettings, out mapMethod)
                && !ReferenceEquals(mapMethod.MethodSymbol, this.Context.GetRootMapMethod().MethodSymbol))
            {
                var mapMethodRequireMappaContext = mapMethod.RequireMappaContextWhenInvoked();
                var rootMapMethod = this.Context.GetRootMapMethod();
                var callerMethodProvideMappaContext = rootMapMethod.ProvideMappaContextWhenInvoked();

                if (!mapMethodRequireMappaContext || /* mapMethodRequireMappaContext && */ callerMethodProvideMappaContext)
                {
                    this.MaybeReportNestedMapWithoutMappaContext(mapMethod, mapMethodRequireMappaContext);
                    return this.WrapIfNullableReferenceSource(
                        new PolymorphicMethodMapStrategy(
                            this.Context.TargetType,
                            this.Context.SourceType,
                            mapMethod,
                            rootMapMethod.MaybeGetMappaContextParameterName()));
                }
            }
        }

        return this.ComputeStrategy();
    }

    /// <summary>
    /// Wraps a map-method strategy in <see cref="NullableStrategy"/> when the source is a nullable
    /// reference type, so <c>null</c> edges short-circuit before invoking the nested map method
    /// (required for ReferenceReusing cycle edges).
    /// </summary>
    /// <param name="strategy">The strategy that maps the non-null source.</param>
    /// <returns>The original strategy, or a nullable wrapper around it.</returns>
    private MapStrategy WrapIfNullableReferenceSource(MapStrategy strategy)
    {
        var sourceType = this.Context.SourceType;
        if (sourceType is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated })
        {
            return new NullableStrategy(this.Context.TargetType, sourceType, strategy);
        }

        return strategy;
    }

    private void MaybeReportNestedMapWithoutMappaContext(MapMethod mapMethod, bool mapMethodRequireMappaContext)
    {
        if (mapMethodRequireMappaContext)
        {
            return;
        }

        if (!ReferenceHandlingCodeGenerator.IsReferenceHandlingRequested(this.Context.MappaUserSettings))
        {
            return;
        }

        var rootMapMethod = this.Context.GetRootMapMethod();
        if (!rootMapMethod.ProvideMappaContextWhenInvoked())
        {
            return;
        }

        if (ReferenceEquals(mapMethod.MethodSymbol, rootMapMethod.MethodSymbol))
        {
            return;
        }

        this.Context.ReportDiagnostic(MappaDiagnostics.ReferenceHandlingNestedMapWithoutMappaContext(
            this.Context.GetLocation(),
            mapMethod.MethodName));
    }
}