// <copyright file="MappaNamespaceBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Builders;

/// <summary>
/// The namespace builder.
/// </summary>
internal sealed class MappaNamespaceBuilder
   : IMappaBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaNamespaceBuilder"/> class.
    /// </summary>
    /// <param name="classContext">The class generator context.</param>
    /// <param name="classSourceCode">The source code of the class.</param>
    /// <param name="preambleSourceCode">
    /// Optional source emitted inside the namespace before the class
    /// (for example file-local inaccessible accessors).
    /// </param>
    public MappaNamespaceBuilder(
        MappaClassGeneratorContext classContext,
        string classSourceCode,
        string? preambleSourceCode = null)
    {
        this.ClassContext = classContext;
        this.ClassSourceCode = classSourceCode;
        this.PreambleSourceCode = preambleSourceCode;
    }

    /// <summary>
    /// Gets the class generator context.
    /// </summary>
    private MappaClassGeneratorContext ClassContext { get; }

    /// <summary>
    /// Gets the class source code.
    /// </summary>
    private string ClassSourceCode { get; }

    /// <summary>
    /// Gets optional source emitted inside the namespace before the class.
    /// </summary>
    private string? PreambleSourceCode { get; }

    /// <inheritdoc/>
    public string BuildSource(MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();
        var @namespace = this.ClassContext.ClassSymbol.ContainingNamespace.ToDisplayString();

        // File scoped namespace
        if (this.ClassContext.ClassDeclarationSyntax.Parent is FileScopedNamespaceDeclarationSyntax)
        {
            builder.AppendLine($"namespace {@namespace};");
            builder.AppendEmptyLine();
            this.AppendPreamble(builder);
            builder.AppendLine(this.ClassSourceCode);
        }

        // Standard namespace block
        else if (this.ClassContext.ClassDeclarationSyntax.Parent is NamespaceDeclarationSyntax)
        {
            builder.AppendLine($"namespace {@namespace}");
            using (builder.CurlyBracesBlock())
            {
                this.AppendPreamble(builder);
                builder.AppendLine(this.ClassSourceCode);
            }
        }

        // No namespace at all
        else
        {
            this.AppendPreamble(builder);
            builder.AppendLine(this.ClassSourceCode);
        }

        return builder.ToString();
    }

    private void AppendPreamble(PrettyCode.StringBuilder builder)
    {
        if (this.PreambleSourceCode is not { Length: > 0 } preambleSourceCode)
        {
            return;
        }

        builder.AppendLine(preambleSourceCode);
        builder.AppendEmptyLine();
    }
}