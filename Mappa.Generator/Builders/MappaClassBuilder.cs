// <copyright file="MappaClassBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System;

using Mappa.Generator.Models;
using Mappa.Generator.Models.Helpers;

namespace Mappa.Generator.Builders;

/// <summary>
/// Builder to generate a the mappa mapper class.
/// </summary>
internal sealed class MappaClassBuilder
    : IMappaBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaClassBuilder"/> class.
    /// </summary>
    /// <param name="classContext">The class generator context.</param>
    public MappaClassBuilder(MappaClassGeneratorContext classContext)
    {
        this.ClassContext = classContext;
    }

    /// <summary>
    /// Gets the class generator context.
    /// </summary>
    internal MappaClassGeneratorContext ClassContext { get; }

    /// <inheritdoc/>
    public string BuildSource()
    {
        var builder = new IndentStringBuilder();
        builder.AppendLine(new MappaGeneratedCodeAttributeBuilder().BuildSource())
               .AppendLine($"partial class {this.ClassContext.ClassDeclarationSyntax.Identifier}");
        using (builder.BeginCodeBlock())
        using (builder.Indent())
        {
            // TODO: Implement me
        }

        return builder.ToString();
    }
}