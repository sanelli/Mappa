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
    private readonly SemanticModel semanticModel;
    private readonly Compilation compilation;

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
        this.semanticModel = semanticModel;
        this.compilation = compilation;
    }

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
            this.semanticModel,
            this.compilation));

        return this;
    }

    /// <summary>
    /// Assert the compilation unit contains a file scoped namespace.
    /// </summary>
    /// <returns>The file scoped namespace declaration syntax assertions.</returns>
    public CompilationUnitSyntaxAssertions HaveFileScopedNamespace()
        => this.HaveFileScopedNamespace(_ => { /* Nothing else to test */ });

    /// <summary>
    /// Assert the compilation unit contains a namespace (non file scoped).
    /// </summary>
    /// <returns>The namespace declaration syntax assertions.</returns>
    public CompilationUnitSyntaxAssertions HaveNamespaceDeclarationSyntax()
    {
        var namespaceDeclarationSyntaxes =
            this.Subject.ChildNodes().OfType<NamespaceDeclarationSyntax>().ToArray();
        namespaceDeclarationSyntaxes.Should().HaveCount(1);

        return this;
    }

    /// <summary>
    /// Assert the compilation unit contains a namespace (non file scoped).
    /// </summary>
    /// <returns>The namespace declaration syntax assertions.</returns>
    public CompilationUnitSyntaxAssertions HaveNoNamespaceDeclarationSyntax()
    {
        var namespaceDeclarationSyntaxes =
            this.Subject.ChildNodes().OfType<BaseNamespaceDeclarationSyntax>().ToArray();
        namespaceDeclarationSyntaxes.Should().BeEmpty();

        return this;
    }
}