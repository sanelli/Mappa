// <copyright file="CompilationUnitSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="SyntaxTree"/>.
/// </summary>
[DebuggerNonUserCode]
public sealed class CompilationUnitSyntaxAssertions
    : ObjectAssertions<CompilationUnitSyntax, CompilationUnitSyntaxAssertions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompilationUnitSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation unit.</param>
    internal CompilationUnitSyntaxAssertions(
        CompilationUnitSyntax value,
        SemanticModel semanticModel,
        Compilation compilation)
        : base(value)
    {
        this.SemanticModel = semanticModel;
        this.Compilation = compilation;
    }

    /// <summary>
    /// Gets the semantic model.
    /// </summary>
    public SemanticModel SemanticModel { get; }

    /// <summary>
    /// Gets the compilation.
    /// </summary>
    public Compilation Compilation { get; }

    /// <summary>
    /// Assert the compilation unit contains a file scoped namespace.
    /// </summary>
    /// <param name="assert">Assertions on the file scoped namespace.</param>
    /// <returns>The file scoped namespace declaration syntax assertions.</returns>
    public CompilationUnitSyntaxAssertions HaveFileScopedNamespace(Action<FileScopedNamespaceDeclarationSyntaxAssertions> assert)
    {
        ArgumentNullException.ThrowIfNull(assert);

        var fileScopedNamespaceDeclarationSyntaxes =
            this.Subject.ChildNodes().OfType<FileScopedNamespaceDeclarationSyntax>().ToArray();
        fileScopedNamespaceDeclarationSyntaxes.Should().HaveCount(1);

        assert(new FileScopedNamespaceDeclarationSyntaxAssertions(
            fileScopedNamespaceDeclarationSyntaxes.Single(),
            this.SemanticModel,
            this.Compilation));

        return this;
    }
}