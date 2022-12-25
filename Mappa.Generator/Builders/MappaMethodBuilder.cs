// <copyright file="MappaMethodBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;

namespace Mappa.Generator.Builders;

/// <summary>
/// Build a method.
/// </summary>
internal sealed class MappaMethodBuilder
    : IMappaBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaMethodBuilder"/> class.
    /// </summary>
    /// <param name="classContext">The class generator context.</param>
    /// <param name="mapMethod">THe method to be generated.</param>
    public MappaMethodBuilder(MappaClassGeneratorContext classContext, MapMethod mapMethod)
    {
        this.ClassContext = classContext;
        this.MapMethod = mapMethod;
    }

    /// <summary>
    /// Gets the class generator context.
    /// </summary>
    private MappaClassGeneratorContext ClassContext { get; }

    /// <summary>
    /// Gets the map method.
    /// </summary>
    private MapMethod MapMethod { get; }

    /// <inheritdoc/>
    public string BuildSource()
    {
        throw new NotImplementedException();
    }
}