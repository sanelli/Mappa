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
    /// <param name="forceBlockScopedNamespace">
    /// When <see langword="true"/>, emit a block-scoped namespace even if the user type
    /// uses a file-scoped namespace (required when file-local members must precede the namespace).
    /// </param>
    public MappaNamespaceBuilder(
        MappaClassGeneratorContext classContext,
        string classSourceCode,
        bool forceBlockScopedNamespace = false)
    {
        this.ClassContext = classContext;
        this.ClassSourceCode = classSourceCode;
        this.ForceBlockScopedNamespace = forceBlockScopedNamespace;
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
    /// Gets a value indicating whether a block-scoped namespace must be emitted.
    /// </summary>
    private bool ForceBlockScopedNamespace { get; }

    /// <inheritdoc/>
    public string BuildSource(MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();
        var @namespace = this.ClassContext.ClassSymbol.ContainingNamespace.ToDisplayString();

        var isFileScoped = this.ClassContext.ClassDeclarationSyntax.Parent is FileScopedNamespaceDeclarationSyntax;
        var isBlockScoped = this.ClassContext.ClassDeclarationSyntax.Parent is NamespaceDeclarationSyntax;

        // File scoped namespace (unless forced to a block so preceding file-local members are valid).
        if (isFileScoped && !this.ForceBlockScopedNamespace)
        {
            builder.AppendLine($"namespace {@namespace};");
            builder.AppendEmptyLine();
            builder.AppendLine(this.ClassSourceCode);
        }

        // Standard namespace block (including file-scoped types forced to block form).
        else if (isBlockScoped || (isFileScoped && this.ForceBlockScopedNamespace))
        {
            builder.AppendLine($"namespace {@namespace}");
            using (builder.CurlyBracesBlock())
            {
                builder.AppendLine(this.ClassSourceCode);
            }
        }

        // No namespace at all
        else
        {
            builder.AppendLine(this.ClassSourceCode);
        }

        return builder.ToString();
    }
}