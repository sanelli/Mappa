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
internal sealed class CompilationUnitSyntaxAssertions
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
        : base(value, AwesomeAssertions.Execution.AssertionChain.GetOrCreate())
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
    /// Assert the compilation unit contains a non file-scoped namespace declaration.
    /// </summary>
    /// <returns>The compilation unit syntax assertions.</returns>
    public CompilationUnitSyntaxAssertions HaveNamespaceDeclarationSyntax()
        => this.HaveNamespaceDeclarationSyntax(_ => { /* Nothing else to test */ });

    /// <summary>
    /// Assert the compilation unit contains a non file-scoped namespace and apply nested assertions.
    /// </summary>
    /// <param name="assert">Assertions on the namespace declaration.</param>
    /// <returns>The compilation unit syntax assertions.</returns>
    public CompilationUnitSyntaxAssertions HaveNamespaceDeclarationSyntax(Action<NamespaceDeclarationSyntaxAssertions> assert)
    {
        ArgumentNullException.ThrowIfNull(assert);

        var namespaceDeclarationSyntaxes =
            this.Subject.ChildNodes().OfType<NamespaceDeclarationSyntax>().ToArray();
        namespaceDeclarationSyntaxes.Should().HaveCount(1);

        assert(new NamespaceDeclarationSyntaxAssertions(
            namespaceDeclarationSyntaxes.Single(),
            this.semanticModel,
            this.compilation));

        return this;
    }

    /// <summary>
    /// Assert the compilation unit contains no namespace declaration.
    /// </summary>
    /// <returns>The compilation unit syntax assertions.</returns>
    public CompilationUnitSyntaxAssertions HaveNoNamespaceDeclarationSyntax()
    {
        var namespaceDeclarationSyntaxes =
            this.Subject.ChildNodes().OfType<BaseNamespaceDeclarationSyntax>().ToArray();
        namespaceDeclarationSyntaxes.Should().BeEmpty();

        return this;
    }

    /// <summary>
    /// Assert that the compilation unit contains <paramref name="count"/> classes.
    /// </summary>
    /// <param name="count">The number of expected classes.</param>
    /// <returns>The compilation unit syntax assertions.</returns>
    public CompilationUnitSyntaxAssertions HaveClasses(int count)
    {
        var classDeclarationSyntaxes = this.Subject.ChildNodes().OfType<ClassDeclarationSyntax>().ToArray();
        classDeclarationSyntaxes.Should().HaveCount(count);
        return this;
    }

    /// <summary>
    /// Assert the compilation unit contains exactly one namespace (file-scoped or block-scoped)
    /// with <paramref name="classCount"/> classes, and assert on the class named
    /// <paramref name="className"/>.
    /// </summary>
    /// <param name="classCount">The expected number of classes in the namespace.</param>
    /// <param name="className">The mapper class identifier.</param>
    /// <param name="assert">Assertions on the mapper class.</param>
    /// <returns>The compilation unit syntax assertions.</returns>
    public CompilationUnitSyntaxAssertions HaveNamespaceWithClass(
        int classCount,
        string className,
        Action<ClassDeclarationSyntaxAssertions> assert)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(className);
        ArgumentNullException.ThrowIfNull(assert);

        var fileScopedNamespaces = this.Subject.ChildNodes().OfType<FileScopedNamespaceDeclarationSyntax>().ToArray();
        var blockNamespaces = this.Subject.ChildNodes().OfType<NamespaceDeclarationSyntax>().ToArray();

        BaseNamespaceDeclarationSyntax namespaceDeclarationSyntax;
        if (fileScopedNamespaces.Length == 1 && blockNamespaces.Length == 0)
        {
            namespaceDeclarationSyntax = fileScopedNamespaces[0];
        }
        else if (blockNamespaces.Length == 1 && fileScopedNamespaces.Length == 0)
        {
            namespaceDeclarationSyntax = blockNamespaces[0];
        }
        else
        {
            (fileScopedNamespaces.Length + blockNamespaces.Length).Should().Be(
                1,
                "exactly one file-scoped or block-scoped namespace is expected");
            namespaceDeclarationSyntax = fileScopedNamespaces.Length == 1
                ? fileScopedNamespaces[0]
                : blockNamespaces[0];
        }

        var classDeclarationSyntaxes = namespaceDeclarationSyntax.ChildNodes().OfType<ClassDeclarationSyntax>().ToArray();
        classDeclarationSyntaxes.Should().HaveCount(classCount);
        classDeclarationSyntaxes.Should().Contain(classDeclarationSyntax =>
            classDeclarationSyntax.Identifier.ToString().Equals(className, StringComparison.Ordinal));
        var classDeclarationSyntax = classDeclarationSyntaxes.Single(classDeclarationSyntax =>
            classDeclarationSyntax.Identifier.ToString().Equals(className, StringComparison.Ordinal));
        assert(new ClassDeclarationSyntaxAssertions(classDeclarationSyntax, this.semanticModel, this.compilation));
        return this;
    }

    /// <summary>
    /// Assert that the compilation unit contains a class named <paramref name="identifier"/>.
    /// </summary>
    /// <param name="identifier">The identifier of the class.</param>
    /// <param name="assert">The assertions on the class declaration.</param>
    /// <returns>The compilation unit syntax assertions.</returns>
    public CompilationUnitSyntaxAssertions HaveClass(string identifier, Action<ClassDeclarationSyntaxAssertions> assert)
    {
        ArgumentNullException.ThrowIfNull(assert);

        var classDeclarationSyntaxes = this.Subject.ChildNodes().OfType<ClassDeclarationSyntax>().ToArray();
        classDeclarationSyntaxes.Should().Contain(classDeclarationSyntax =>
            classDeclarationSyntax.Identifier.ToString().Equals(identifier, StringComparison.Ordinal));
        var classDeclarationSyntax = classDeclarationSyntaxes.Single(classDeclarationSyntax =>
            classDeclarationSyntax.Identifier.ToString().Equals(identifier, StringComparison.Ordinal));
        assert(new ClassDeclarationSyntaxAssertions(classDeclarationSyntax, this.semanticModel, this.compilation));
        return this;
    }
}