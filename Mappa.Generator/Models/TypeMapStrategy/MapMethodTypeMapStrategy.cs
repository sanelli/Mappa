// <copyright file="MapMethodTypeMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.TypeMapStrategy;

/// <summary>
/// DEscribe a startegy for mapping between two types that uses a map method.
/// </summary>
internal sealed class MapMethodTypeMapStrategy
    : ITypeMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapMethodTypeMapStrategy"/> class.
    /// </summary>
    /// <param name="mapMethod">The method to be used for the mapping.</param>
    /// <param name="source">The source of the mapping.</param>
    public MapMethodTypeMapStrategy(MapMethod mapMethod, string source)
    {
        this.MapMethod = mapMethod;
        this.Source = source;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType => this.MapMethod.TargetType;

    /// <inheritdoc/>
    public ITypeSymbol SourceType => this.MapMethod.SourceType;

    /// <inheritdoc/>
    public string Source { get; }

    /// <summary>
    /// Gets the method used for the mapping.
    /// </summary>
    internal MapMethod MapMethod { get; }
}