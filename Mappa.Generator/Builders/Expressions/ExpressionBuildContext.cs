// <copyright file="ExpressionBuildContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;

namespace Mappa.Generator.Builders.Expressions;

/// <summary>
/// Context used while building projection expressions.
/// </summary>
internal sealed class ExpressionBuildContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionBuildContext"/> class.
    /// </summary>
    /// <param name="builderContext">The mappa builder context.</param>
    /// <param name="mappaGlobalOptions">The global options.</param>
    internal ExpressionBuildContext(MappaBuilderContext builderContext, MappaGlobalOptions mappaGlobalOptions)
    {
        this.BuilderContext = builderContext;
        this.MappaGlobalOptions = mappaGlobalOptions;
    }

    /// <summary>
    /// Gets the mappa builder context.
    /// </summary>
    internal MappaBuilderContext BuilderContext { get; }

    /// <summary>
    /// Gets the global options.
    /// </summary>
    internal MappaGlobalOptions MappaGlobalOptions { get; }
}