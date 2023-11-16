// <copyright file="ExpressionSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="ExpressionSyntax"/>.
/// </summary>
[DebuggerNonUserCode]
public sealed class ExpressionSyntaxAssertions
    : ObjectAssertions<ExpressionSyntax, ExpressionSyntaxAssertions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertion.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation.</param>
    internal ExpressionSyntaxAssertions(
        ExpressionSyntax value,
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
    /// Assert that the expression is an identifier name expression.
    /// </summary>
    /// <param name="identifierName">The identifier.</param>
    /// <returns>The assertion.</returns>
    public ExpressionSyntaxAssertions IsIdentifierName(string identifierName)
    {
        this.Subject.Should().BeOfType<IdentifierNameSyntax>();
        var identifierNameSyntax = (IdentifierNameSyntax)this.Subject;
        identifierNameSyntax.Identifier.Text.Should().Be(identifierName);
        return this;
    }

    /// <summary>
    /// Assert that the expression in a member access expression syntax
    /// whose string representation is the same as <paramref name="fullAccessPath"/>.
    /// </summary>
    /// <param name="fullAccessPath">The string representation of the expression.</param>
    /// <returns>The assertions.</returns>
    public ExpressionSyntaxAssertions IsMemberAccessExpressionSyntax(string fullAccessPath)
    {
        this.Subject.Should().BeOfType<MemberAccessExpressionSyntax>();
        var memberAccessExpressionSyntax = (MemberAccessExpressionSyntax)this.Subject;
        memberAccessExpressionSyntax.ToString().Should().Be(fullAccessPath);
        return this;
    }
}