// <copyright file="SyntaxNodeAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Syntax node assertions.
/// </summary>
[DebuggerNonUserCode]
public sealed class SyntaxNodeAssertions
    : ObjectAssertions<SyntaxNode, SyntaxNodeAssertions>
{
    private readonly SemanticModel semanticModel;
    private readonly Compilation compilation;

    /// <summary>
    /// Initializes a new instance of the <see cref="SyntaxNodeAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertion.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation.</param>
    public SyntaxNodeAssertions(
        SyntaxNode value,
        SemanticModel semanticModel,
        Compilation compilation)
        : base(value)
    {
        this.semanticModel = semanticModel;
        this.compilation = compilation;
    }

    /// <summary>
    /// Assert that the syntax node is a return statement.
    /// </summary>
    /// <param name="expressionAssertion">The assertion on the expression returned.</param>
    /// <returns>The assertions.</returns>
    public SyntaxNodeAssertions IsReturnStatement(Action<ExpressionSyntaxAssertions>? expressionAssertion = null)
    {
        this.Subject.Should().BeOfType<ReturnStatementSyntax>();
        var returnStatement = (ReturnStatementSyntax)this.Subject;
        if (expressionAssertion is not null)
        {
            returnStatement.Expression!.Should().NotBeNull();
            expressionAssertion(new ExpressionSyntaxAssertions(returnStatement.Expression!));
        }

        return this;
    }

    /// <summary>
    /// Validate that the syntax node is a variable declaration syntax.
    /// </summary>
    /// <param name="type">The type of the variable.</param>
    /// <param name="names">The names of the variables declared.</param>
    /// <returns>The assertion.</returns>
    public SyntaxNodeAssertions IsLocalDeclarationStatementSyntax(string type, params string[] names)
    {
        ArgumentNullException.ThrowIfNull(names);

        this.Subject.Should().BeOfType<LocalDeclarationStatementSyntax>();
        var localDeclarationStatementSyntax = (LocalDeclarationStatementSyntax)this.Subject;
        localDeclarationStatementSyntax.Declaration.Variables.Should().HaveCount(names.Length);
        localDeclarationStatementSyntax.Declaration.Variables
            .Select(syntax => syntax.Identifier.Text)
            .Should()
            .BeEquivalentTo(names);

        var expectedType = AssertionsHelpers.GetTypeSymbol(this.compilation, type);
        var localSymbol = this.semanticModel.GetDeclaredSymbol(localDeclarationStatementSyntax.Declaration.Variables[0]) as ILocalSymbol;

        localSymbol.Should().NotBeNull();

        SymbolEqualityComparer
            .Default
            .Equals(localSymbol!.Type, expectedType)
            .Should().BeTrue();

        return this;
    }
}