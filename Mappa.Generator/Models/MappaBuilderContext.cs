// <copyright file="MappaBuilderContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// The content of the building.
/// </summary>
internal sealed class MappaBuilderContext
{
    private readonly Compilation compilation;
    private uint temporaryCounter;

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaBuilderContext"/> class.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    public MappaBuilderContext(Compilation compilation)
    {
        this.compilation = compilation;
    }

    /// <summary>
    /// Gets the compilation.
    /// </summary>
    internal Compilation Compilation => this.compilation;

    /// <summary>
    /// Gets a new unique temporary value.
    /// </summary>
    /// <returns>A new temporary value.</returns>
    internal string NextTemporary()
        => $"__mappa_tmp_{++this.temporaryCounter}";
}