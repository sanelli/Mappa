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

        var existingStrategy = this.TryGetExistingMapMethodStrategy();
        if (existingStrategy is not null)
        {
            return existingStrategy;
        }

        return this.ComputeStrategy();
    }

    private MapStrategy? TryGetExistingMapMethodStrategy()
    {
        if (this.Context.PropertyPathContext is not null)
        {
            return null;
        }

        return this.TryGetDirectMethodStrategy()
               ?? this.TryGetCompatibleMethodStrategy()
               ?? this.TryGetPolymorphicMethodStrategy();
    }

    private MapStrategy? TryGetDirectMethodStrategy()
    {
        if (!this.Context.TryGetMethod(this.Context.TargetType, this.Context.SourceType, out var mapMethod))
        {
            return null;
        }

        return this.TryCreateWrappedMapMethodStrategy(
            mapMethod,
            (method, contextParameterName) => new MethodMapStrategy(method, contextParameterName));
    }

    private MapStrategy? TryGetCompatibleMethodStrategy()
    {
        if (this.Context.MappaUserSettings.CompatibleMapMethod is not BooleanSetting.Enable)
        {
            return null;
        }

        if (!this.Context.TryGetCompatibleMethod(this.Context.TargetType, this.Context.SourceType, this.Compilation, out var mapMethod))
        {
            return null;
        }

        if (ReferenceEquals(mapMethod, this.Context.GetRootMapMethod()))
        {
            return null;
        }

        return this.TryCreateWrappedMapMethodStrategy(
            mapMethod,
            (method, contextParameterName) => new CompatibleMethodMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType,
                method,
                contextParameterName));
    }

    private MapStrategy? TryGetPolymorphicMethodStrategy()
    {
        if (!this.Context.TryGetPolymorphicMethod(this.Context.TargetType, this.Context.SourceType, this.Context.MappaUserSettings, out var mapMethod))
        {
            return null;
        }

        if (ReferenceEquals(mapMethod, this.Context.GetRootMapMethod()))
        {
            return null;
        }

        return this.TryCreateWrappedMapMethodStrategy(
            mapMethod,
            (method, contextParameterName) => new PolymorphicMethodMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType,
                method,
                contextParameterName));
    }

    private MapStrategy? TryCreateWrappedMapMethodStrategy(
        MapMethod mapMethod,
        Func<MapMethod, string?, MapStrategy> createStrategy)
    {
        var mapMethodRequireMappaContext = mapMethod.RequireMappaContextWhenInvoked();
        var rootMapMethod = this.Context.GetRootMapMethod();
        var callerMethodProvideMappaContext = rootMapMethod.ProvideMappaContextWhenInvoked();

        if (mapMethodRequireMappaContext && !callerMethodProvideMappaContext)
        {
            return null;
        }

        this.MaybeReportNestedMapWithoutMappaContext(mapMethod, mapMethodRequireMappaContext);
        return this.WrapIfNullableReferenceSource(
            createStrategy(mapMethod, rootMapMethod.MaybeGetMappaContextParameterName()));
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

        if (ReferenceEquals(mapMethod, rootMapMethod))
        {
            return;
        }

        this.Context.ReportDiagnostic(MappaDiagnostics.ReferenceHandlingNestedMapWithoutMappaContext(
            this.Context.GetLocation(),
            mapMethod.MethodName));
    }
}