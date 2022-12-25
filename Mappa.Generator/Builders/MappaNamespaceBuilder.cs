// <copyright file="MappaNamespaceBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Helpers;

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
    internal MappaClassGeneratorContext ClassContext { get; }

    /// <summary>
    /// Gets the class source code.
    /// </summary>
    internal string ClassSourceCode { get; }

    /// <inheritdoc/>
    public string BuildSource()
    {
        var builder = new IndentStringBuilder();
        var @namespace = this.ClassContext.DeclaredClassSymbol.ContainingNamespace.ToDisplayString();
        var fileScopedNamespace = (this.ClassContext.Compilation as CSharpCompilation)?.LanguageVersion >= LanguageVersion.CSharp10;
        if (fileScopedNamespace)
        {
            builder.AppendLine($"namespace {@namespace};");
            builder.AppendLine(this.ClassSourceCode);
        }
        else
        {
            builder.AppendLine($"namespace {@namespace}");
            using (builder.BeginCodeBlock())
            using (builder.Indent())
            {
                builder.AppendLine(this.ClassSourceCode);
            }
        }

        return builder.ToString();
    }
}