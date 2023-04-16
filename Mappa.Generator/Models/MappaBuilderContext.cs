// <copyright file="MappaBuilderContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
namespace Mappa.Generator.Models;

/// <summary>
/// The content of the building.
/// </summary>
internal sealed class MappaBuilderContext
{
    private uint temporaryCounter;

    /// <summary>
    /// Gets a new unique temporary value.
    /// </summary>
    /// <returns>A new temporary value.</returns>
    internal string NextTemporary()
        => $"__mappa_tmp_{++this.temporaryCounter}";
}