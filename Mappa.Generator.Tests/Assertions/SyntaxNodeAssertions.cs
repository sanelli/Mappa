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

        var expectedType = this.Compilation.GetTypeSymbol(type);
        var localSymbol = this.SemanticModel.GetDeclaredSymbol(localDeclarationStatementSyntax.Declaration.Variables[0]) as ILocalSymbol;

        localSymbol.Should().NotBeNull();

        SymbolEqualityComparer
            .Default
            .Equals(localSymbol!.Type, expectedType)
            .Should().BeTrue();

        return this;
    }

    /// <summary>
    /// Assert that the syntax node is a switch statement.
    /// </summary>
    /// <param name="assertExpression">Assertions on the expression of the switch statement.</param>
    /// <param name="assertCase">Assertions on the case statements.</param>
    /// <returns>The assertions.</returns>
    public SyntaxNodeAssertions IsSwitchStatementSyntax(
        Action<ExpressionSyntaxAssertions> assertExpression,
        params Action<ISwitchLabelSyntaxAssertions[], IStatementSyntaxBaseAssertions[]>[] assertCase)
    {
        ArgumentNullException.ThrowIfNull(assertExpression);
        ArgumentNullException.ThrowIfNull(assertCase);

        this.Subject.Should().BeOfType<SwitchStatementSyntax>();
        var switchStatementSyntax = (SwitchStatementSyntax)this.Subject;
        assertExpression(new ExpressionSyntaxAssertions(switchStatementSyntax.Expression));

        var caseStatements = switchStatementSyntax
            .ChildNodes()
            .Skip(1)
            .ToArray();

        caseStatements.Should().HaveCount(assertCase.Length);
        for (var index = 0; index < assertCase.Length; ++index)
        {
            caseStatements[index].Should().BeOfType<SwitchSectionSyntax>();
            var switchSectionSyntax = (SwitchSectionSyntax)caseStatements[index];

            var labelAssertions = switchSectionSyntax.Labels.ToAssertions(this.SemanticModel, this.Compilation);
            var statementAssertions = switchSectionSyntax.Statements.ToAssertions(this.SemanticModel, this.Compilation);

            assertCase[index](labelAssertions, statementAssertions);
        }

        return this;
    }
}