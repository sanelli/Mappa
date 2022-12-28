// <copyright file="BaseNamespaceDeclarationSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="BaseNamespaceDeclarationSyntax"/>.
/// </summary>
/// <typeparam name="TNamespaceSyntax">The namespace syntax.</typeparam>
public sealed class BaseNamespaceDeclarationSyntaxAssertions<TNamespaceSyntax>
    : ObjectAssertions<TNamespaceSyntax, BaseNamespaceDeclarationSyntaxAssertions<TNamespaceSyntax>>
    where TNamespaceSyntax : BaseNamespaceDeclarationSyntax
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseNamespaceDeclarationSyntaxAssertions{TNamespaceSyntax}"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation unit.</param>
    public BaseNamespaceDeclarationSyntaxAssertions(
        TNamespaceSyntax value,
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
    private SemanticModel SemanticModel { get; }

    /// <summary>
    /// Gets the compilation.
    /// </summary>
    private Compilation Compilation { get; }

    /// <summary>
    /// Assert that the namespace name is <paramref name="identifier"/>.
    /// </summary>
    /// <param name="identifier">The expected namespace identifier.</param>
    /// <returns>The assertions.</returns>
    public BaseNamespaceDeclarationSyntaxAssertions<TNamespaceSyntax> HaveNamespaceIdentifier(string identifier)
    {
        this.Subject.Name.ToString().Should().Be(identifier);
        return this;
    }

    /// <summary>
    /// Assert that the namespace contains <paramref name="count"/> classes.
    /// </summary>
    /// <param name="count">The number of expected classes in the namespace.</param>
    /// <returns>The assertions.</returns>
    public BaseNamespaceDeclarationSyntaxAssertions<TNamespaceSyntax> HaveClasses(int count)
    {
        var classDeclarationSyntaxes = this.Subject.ChildNodes().OfType<ClassDeclarationSyntax>().ToArray();
        classDeclarationSyntaxes.Should().HaveCount(count);
        return this;
    }

    /// <summary>
    /// Assert that the namespace contains a specific class.
    /// </summary>
    /// <param name="identifier">The identifier of the class.</param>
    /// <returns>The class declaration syntax.</returns>
    public ClassDeclarationSyntaxAssertions HaveClass(string identifier)
    {
        var classDeclarationSyntaxes = this.Subject.ChildNodes().OfType<ClassDeclarationSyntax>().ToArray();
        classDeclarationSyntaxes.Should().Contain(classDeclarationSyntax =>
            classDeclarationSyntax.Identifier.ToString().Equals(identifier, StringComparison.Ordinal));
        var classDeclarationSyntax = classDeclarationSyntaxes.Single(classDeclarationSyntax =>
            classDeclarationSyntax.Identifier.ToString().Equals(identifier, StringComparison.Ordinal));
        return new ClassDeclarationSyntaxAssertions(classDeclarationSyntax, this.SemanticModel, this.Compilation);
    }
}