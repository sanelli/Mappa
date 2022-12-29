// <copyright file="MappaGeneratedCodeAttributeBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.CodeDom.Compiler;

using Mappa.Generator.Models;

namespace Mappa.Generator.Builders;

/// <summary>
/// Builds the <see cref="GeneratedCodeAttribute"/>.
/// </summary>
internal sealed class MappaGeneratedCodeAttributeBuilder
    : IMappaBuilder
{
    /// <inheritdoc/>
    public string BuildSource(MappaGlobalOptions mappaGlobalOptions)
        => $"[{typeof(GeneratedCodeAttribute).FullName}(\"Mappa\", \"{typeof(MappaGenerator).Assembly.GetName().Version}\")]";
}