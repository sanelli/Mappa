// <copyright file="MappaClassBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;

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
    private MappaClassGeneratorContext ClassContext { get; }

    /// <inheritdoc/>
    public string BuildSource()
    {
        const string space = " ";
        var modifiers = string.Join(
            space,
            this.ClassContext.DeclaredClassSymbol.GetClassAccessibility(),
            this.ClassContext.DeclaredClassSymbol.GetClassModifiers(),
            "partial");

        var builder = new IndentStringBuilder();
        builder
            .AppendLine(new MappaGeneratedCodeAttributeBuilder().BuildSource())
            .AppendLine($"{modifiers} class {this.ClassContext.ClassDeclarationSyntax.Identifier}");

        using (builder.BeginCodeBlock())
        using (builder.Indent())
        {
            // Build all map  methods.
            foreach (var mapMethod in this.ClassContext.MapMethods.Where(mapMethod => mapMethod.HasStrategy))
            {
                var methodBuilder = new MappaMethodBuilder(this.ClassContext, mapMethod);
                var methodSourceCode = methodBuilder.BuildSource();
                builder.AppendLine(methodSourceCode);
            }
        }

        return builder.ToString();
    }
}