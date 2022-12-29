// <copyright file="IMappaBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;

namespace Mappa.Generator.Builders;

/// <summary>
/// Describe a mappa builder.
/// </summary>
internal interface IMappaBuilder
{
    /// <summary>
    /// Generate the source code required by this builder.
    /// </summary>
    /// <param name="mappaGlobalOptions">The Mappa global options.</param>
    /// <returns>The source code.</returns>
    string BuildSource(MappaGlobalOptions mappaGlobalOptions);
}