// <copyright file="BlockSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="BlockSyntax"/>.
/// </summary>
public sealed class BlockSyntaxAssertions
    : ObjectAssertions<BlockSyntax, BlockSyntaxAssertions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BlockSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation unit.</param>
    public BlockSyntaxAssertions(
        BlockSyntax value,
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
    /// Assert that the block contains a single return statement. The return statement
    /// also return an identifier expression (i.e. <c>return <paramref name="identifierName"/></c>).
    /// </summary>
    /// <param name="identifierName">The name of the identifier in the return expression.</param>
    /// <returns>The assertions.</returns>
    public BlockSyntaxAssertions HaveSingleReturnStatementWithIdentifierExpression(string identifierName)
    {
        this.Subject.ChildNodes().Should().HaveCount(1);
        var singleStatement = this.Subject.ChildNodes().Single();
        AssertIsReturnStatementWithIdentifierExpression(singleStatement, identifierName);
        return this;
    }

    private static void AssertIsReturnStatementWithIdentifierExpression(SyntaxNode syntaxNode, string identifierName)
    {
        syntaxNode.Should().BeOfType<ReturnStatementSyntax>();
        var returnStatement = (ReturnStatementSyntax)syntaxNode;
        returnStatement.Expression.Should().NotBeNull();
        returnStatement.Expression.Should().BeOfType<IdentifierNameSyntax>();
        ((IdentifierNameSyntax)returnStatement.Expression!).Identifier.Text.Should().Be(identifierName);
    }
}