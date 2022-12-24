// <copyright file="MappaGeneratedCodeAttributeBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System;
using System.CodeDom.Compiler;

namespace Mappa.Generator.Builders;

/// <summary>
/// Builds the <see cref="GeneratedCodeAttribute"/>.
/// </summary>
internal sealed class MappaGeneratedCodeAttributeBuilder
    : IMappaBuilder
{
    /// <inheritdoc/>
    public string BuildSource()
        => $"[{typeof(GeneratedCodeAttribute).FullName}(\"Mappa\", \"{typeof(MappaGenerator).Assembly.GetName().Version}\")]";
}