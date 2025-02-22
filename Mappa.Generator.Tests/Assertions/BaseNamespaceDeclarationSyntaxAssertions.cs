// <copyright file="BaseNamespaceDeclarationSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="BaseNamespaceDeclarationSyntax"/>.
/// </summary>
/// <typeparam name="TNamespaceSyntax">The namespace syntax.</typeparam>
/// <typeparam name="TDerivedAssertion">The derived return type.</typeparam>
[DebuggerNonUserCode]
internal abstract class BaseNamespaceDeclarationSyntaxAssertions<TNamespaceSyntax, TDerivedAssertion>
    : ObjectAssertions<TNamespaceSyntax, TDerivedAssertion>
    where TNamespaceSyntax : BaseNamespaceDeclarationSyntax
    where TDerivedAssertion : BaseNamespaceDeclarationSyntaxAssertions<TNamespaceSyntax, TDerivedAssertion>
{
    private readonly SemanticModel semanticModel;
    private readonly Compilation compilation;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseNamespaceDeclarationSyntaxAssertions{TNamespaceSyntax, TDerivedAssertion}"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation unit.</param>
    protected internal BaseNamespaceDeclarationSyntaxAssertions(
        TNamespaceSyntax value,
        SemanticModel semanticModel,
        Compilation compilation)
        : base(value, FluentAssertions.Execution.AssertionChain.GetOrCreate())
    {
        this.semanticModel = semanticModel;
        this.compilation = compilation;
    }

    /// <summary>
    /// Assert that the namespace name is <paramref name="identifier"/>.
    /// </summary>
    /// <param name="identifier">The expected namespace identifier.</param>
    /// <returns>The assertions.</returns>
    public TDerivedAssertion HaveNamespaceIdentifier(string identifier)
    {
        this.Subject.Name.ToString().Should().Be(identifier);
        return (TDerivedAssertion)this;
    }

    /// <summary>
    /// Assert that the namespace contains <paramref name="count"/> classes.
    /// </summary>
    /// <param name="count">The number of expected classes in the namespace.</param>
    /// <returns>The assertions.</returns>
    public TDerivedAssertion HaveClasses(int count)
    {
        var classDeclarationSyntaxes = this.Subject.ChildNodes().OfType<ClassDeclarationSyntax>().ToArray();
        classDeclarationSyntaxes.Should().HaveCount(count);
        return (TDerivedAssertion)this;
    }

    /// <summary>
    /// Assert that the namespace contains a specific class.
    /// </summary>
    /// <param name="identifier">The identifier of the class.</param>
    /// <param name="assert">The assertions.</param>
    /// <returns>The class declaration syntax.</returns>
    public TDerivedAssertion HaveClass(string identifier, Action<ClassDeclarationSyntaxAssertions> assert)
    {
        ArgumentNullException.ThrowIfNull(assert);

        var classDeclarationSyntaxes = this.Subject.ChildNodes().OfType<ClassDeclarationSyntax>().ToArray();
        classDeclarationSyntaxes.Should().Contain(classDeclarationSyntax =>
            classDeclarationSyntax.Identifier.ToString().Equals(identifier, StringComparison.Ordinal));
        var classDeclarationSyntax = classDeclarationSyntaxes.Single(classDeclarationSyntax =>
            classDeclarationSyntax.Identifier.ToString().Equals(identifier, StringComparison.Ordinal));
        assert(new ClassDeclarationSyntaxAssertions(classDeclarationSyntax, this.semanticModel, this.compilation));
        return (TDerivedAssertion)this;
    }
}