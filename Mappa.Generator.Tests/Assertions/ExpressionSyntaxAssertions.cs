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
    public ExpressionSyntaxAssertions BeIdentifierName(string identifierName)
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
    public ExpressionSyntaxAssertions BeMemberAccessExpressionSyntax(string fullAccessPath)
    {
        this.Subject.Should().BeOfType<MemberAccessExpressionSyntax>();
        var memberAccessExpressionSyntax = (MemberAccessExpressionSyntax)this.Subject;
        memberAccessExpressionSyntax.ToString().Should().Be(fullAccessPath);
        return this;
    }

    /// <summary>
    /// Assert that the expression is a literal expression.
    /// </summary>
    /// <param name="value">The expected value of the expression.</param>
    /// <returns>The expression.</returns>
    public ExpressionSyntaxAssertions BeLiteralExpressionSyntax(object value)
    {
        this.Subject.Should().BeOfType<LiteralExpressionSyntax>();
        var literalExpressionSyntax = (LiteralExpressionSyntax)this.Subject;
        literalExpressionSyntax.Token.Should().BeOfType<SyntaxToken>();
        literalExpressionSyntax.Token.Value.Should().NotBeNull();
        literalExpressionSyntax.Token.Value.Should().Be(value);
        return this;
    }

    /// <summary>
    /// Assert that the expression is a <c>nameof</c> expression.
    /// </summary>
    /// <param name="name">The expected value of the expression.</param>
    /// <returns>The expression.</returns>
    public ExpressionSyntaxAssertions BeNameofWithMemberAccess(string name)
    {
        this.Subject.Should().BeOfType<InvocationExpressionSyntax>();
        var invocationExpressionSyntax = (InvocationExpressionSyntax)this.Subject;
        new ExpressionSyntaxAssertions(invocationExpressionSyntax.Expression, this.SemanticModel, this.Compilation)
            .BeIdentifierName("nameof");
        invocationExpressionSyntax.ArgumentList.Arguments.Should().HaveCount(1);
        new ExpressionSyntaxAssertions(invocationExpressionSyntax.ArgumentList.Arguments[0].Expression, this.SemanticModel, this.Compilation)
            .BeMemberAccessExpressionSyntax(name);
        return this;
    }
}