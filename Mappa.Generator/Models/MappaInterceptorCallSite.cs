// <copyright file="MappaInterceptorCallSite.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Mappa.Attributes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Models;

/// <summary>
/// A discovered <c>MappaMap</c> (or renamed) call site with collated <c>With</c> configuration.
/// </summary>
internal sealed class MappaInterceptorCallSite
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaInterceptorCallSite"/> class.
    /// </summary>
    /// <param name="invocation">The interceptable <c>MappaMap</c> invocation.</param>
    /// <param name="interceptableLocation">The Roslyn interceptable location, when available.</param>
    /// <param name="targetType">The mapping target type (generic type argument).</param>
    /// <param name="sourceType">The mapping source type (first argument).</param>
    /// <param name="hasContext"><c>true</c> when the call includes a <see cref="Mappa.MappaContext"/> argument.</param>
    /// <param name="settings">Settings instances from <c>With</c> calls in source order.</param>
    /// <param name="staticDependencies">Deduped static dependency types (class + <c>With</c>).</param>
    /// <param name="methodAttributes">Materialized method attributes from <c>With</c>.</param>
    /// <param name="hasErrors"><c>true</c> when collation reported blocking errors.</param>
    internal MappaInterceptorCallSite(
        InvocationExpressionSyntax invocation,
        InterceptableLocation? interceptableLocation,
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        bool hasContext,
        ImmutableArray<MappaSettingsAttribute> settings,
        ImmutableArray<INamedTypeSymbol> staticDependencies,
        ImmutableArray<Attribute> methodAttributes,
        bool hasErrors)
    {
        this.Invocation = invocation;
        this.InterceptableLocation = interceptableLocation;
        this.TargetType = targetType;
        this.SourceType = sourceType;
        this.HasContext = hasContext;
        this.Settings = settings;
        this.StaticDependencies = staticDependencies;
        this.MethodAttributes = methodAttributes;
        this.HasErrors = hasErrors;
    }

    /// <summary>
    /// Gets the interceptable invocation syntax.
    /// </summary>
    internal InvocationExpressionSyntax Invocation { get; }

    /// <summary>
    /// Gets the interceptable location used to emit <c>[InterceptsLocation]</c>, when available.
    /// </summary>
    internal InterceptableLocation? InterceptableLocation { get; }

    /// <summary>
    /// Gets the target type.
    /// </summary>
    internal ITypeSymbol TargetType { get; }

    /// <summary>
    /// Gets the source type.
    /// </summary>
    internal ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets a value indicating whether the call includes a context argument.
    /// </summary>
    internal bool HasContext { get; }

    /// <summary>
    /// Gets settings from <c>With</c> calls in source order.
    /// </summary>
    internal ImmutableArray<MappaSettingsAttribute> Settings { get; }

    /// <summary>
    /// Gets deduplicated static dependency types.
    /// </summary>
    internal ImmutableArray<INamedTypeSymbol> StaticDependencies { get; }

    /// <summary>
    /// Gets materialized method attributes from <c>With</c>.
    /// </summary>
    internal ImmutableArray<Attribute> MethodAttributes { get; }

    /// <summary>
    /// Gets a value indicating whether blocking errors were reported for this call site.
    /// </summary>
    internal bool HasErrors { get; }
}