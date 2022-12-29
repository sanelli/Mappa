// <copyright file="MappaNamespaceBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis.CSharp;

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
    public MappaNamespaceBuilder(MappaClassGeneratorContext classContext, string classSourceCode)
    {
        this.ClassContext = classContext;
        this.ClassSourceCode = classSourceCode;
    }

    /// <summary>
    /// Gets the class generator context.
    /// </summary>
    private MappaClassGeneratorContext ClassContext { get; }

    /// <summary>
    /// Gets the class source code.
    /// </summary>
    private string ClassSourceCode { get; }

    /// <inheritdoc/>
    public string BuildSource(MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new IndentStringBuilder();
        var @namespace = this.ClassContext.ClassSymbol.ContainingNamespace.ToDisplayString();
        var fileScopedNamespace = (this.ClassContext.Compilation as CSharpCompilation)?.LanguageVersion >= LanguageVersion.CSharp10;
        if (fileScopedNamespace)
        {
            builder.AppendLine($"namespace {@namespace};");
            builder.AppendEmptyLine();
            builder.AppendLine(this.ClassSourceCode);
        }
        else
        {
            builder.AppendLine($"namespace {@namespace}");
            using (builder.CodeBlock())
            using (builder.Indent())
            {
                builder.AppendLine(this.ClassSourceCode);
            }
        }

        return builder.ToString();
    }
}