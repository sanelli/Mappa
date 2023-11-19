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
    public SyntaxNodeAssertions BeReturnStatement(Action<ExpressionSyntaxAssertions>? expressionAssertion = null)
    {
        this.Subject.Should().BeOfType<ReturnStatementSyntax>();
        var returnStatement = (ReturnStatementSyntax)this.Subject;
        if (expressionAssertion is not null)
        {
            returnStatement.Expression.Should().NotBeNull();
            expressionAssertion(new ExpressionSyntaxAssertions(returnStatement.Expression!, this.SemanticModel, this.Compilation));
        }

        return this;
    }

    /// <summary>
    /// Validate that the syntax node is a variable declaration syntax without initialization.
    /// </summary>
    /// <param name="type">The type of the variable.</param>
    /// <param name="names">The names of the variables declared.</param>
    /// <returns>The assertion.</returns>
    public SyntaxNodeAssertions BeLocalDeclarationStatementSyntax(string type, params string[] names)
        => this.BeLocalDeclarationStatementSyntax(type, names, null);

    /// <summary>
    /// Validate that the syntax node is a variable declaration syntax
    /// for a single variable.
    /// </summary>
    /// <param name="type">The type of the variable.</param>
    /// <param name="name">The names of the variable declared.</param>
    /// <param name="assertInitialization">Assert the initialization expression.</param>
    /// <returns>The assertion.</returns>
    public SyntaxNodeAssertions BeLocalDeclarationStatementSyntax(string type, string name, Action<ExpressionSyntaxAssertions> assertInitialization)
        => this.BeLocalDeclarationStatementSyntax(type, new[] { name }, new[] { assertInitialization });

    /// <summary>
    /// Validate that the syntax node is a variable declaration syntax.
    /// </summary>
    /// <param name="type">The type of the variable.</param>
    /// <param name="names">The names of the variables declared.</param>
    /// <param name="assertInitializations">Assert the initialization expressions.</param>
    /// <returns>The assertion.</returns>
    public SyntaxNodeAssertions BeLocalDeclarationStatementSyntax(string type, string[] names, Action<ExpressionSyntaxAssertions>[]? assertInitializations)
    {
        ArgumentNullException.ThrowIfNull(names);

        if (assertInitializations is not null)
        {
            if (names.Length != assertInitializations.Length)
            {
                throw new ArgumentException($"'{nameof(names)}' and '{nameof(assertInitializations)}' must have the same length", nameof(assertInitializations));
            }
        }

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

        if (assertInitializations is null)
        {
            foreach (var variableDeclaratorSyntax in localDeclarationStatementSyntax.Declaration.Variables)
            {
                variableDeclaratorSyntax.Initializer.Should().BeNull();
            }
        }
        else
        {
            for (int index = 0; index < assertInitializations.Length; ++index)
            {
                localDeclarationStatementSyntax.Declaration.Variables[index].Initializer.Should().NotBeNull();
                var initializer = localDeclarationStatementSyntax.Declaration.Variables[index].Initializer!;
                assertInitializations[index](new ExpressionSyntaxAssertions(initializer.Value, this.SemanticModel, this.Compilation));
            }
        }

        return this;
    }

    /// <summary>
    /// Assert that the syntax node is a switch statement.
    /// </summary>
    /// <param name="assertExpression">Assertions on the expression of the switch statement.</param>
    /// <param name="assertCase">Assertions on the case statements.</param>
    /// <returns>The assertions.</returns>
    public SyntaxNodeAssertions BeSwitchStatementSyntax(
        Action<ExpressionSyntaxAssertions> assertExpression,
        params Action<ISwitchLabelSyntaxAssertions[], IStatementSyntaxBaseAssertions[]>[] assertCase)
    {
        ArgumentNullException.ThrowIfNull(assertExpression);
        ArgumentNullException.ThrowIfNull(assertCase);

        this.Subject.Should().BeOfType<SwitchStatementSyntax>();
        var switchStatementSyntax = (SwitchStatementSyntax)this.Subject;
        assertExpression(new ExpressionSyntaxAssertions(switchStatementSyntax.Expression, this.SemanticModel, this.Compilation));

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

    /// <summary>
    /// Assert that the syntax node is a break statement.
    /// </summary>
    /// <returns>The assertions.</returns>
    public SyntaxNodeAssertions BeBreakStatement()
    {
        this.Subject.Should().BeOfType<BreakStatementSyntax>();
        return this;
    }

    /// <summary>
    /// Assert that that the syntax node is an assignment.
    /// </summary>
    /// <param name="identifierName">The name of the identifier.</param>
    /// <param name="assert">Assertion on the right hande side expression.</param>
    /// <returns>The assertions.</returns>
    public SyntaxNodeAssertions BeAssignmentExpressionStatement(string identifierName, Action<ExpressionSyntaxAssertions> assert)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifierName);
        ArgumentNullException.ThrowIfNull(assert);

        this.Subject.Should().BeOfType<ExpressionStatementSyntax>();
        var expressionStatementSyntax = (ExpressionStatementSyntax)this.Subject;
        expressionStatementSyntax.Expression.Should().BeOfType<AssignmentExpressionSyntax>();
        var assignmentExpressionSyntax = (AssignmentExpressionSyntax)expressionStatementSyntax.Expression;

        assignmentExpressionSyntax.Left.Should().BeOfType<IdentifierNameSyntax>();
        var identifier = (IdentifierNameSyntax)assignmentExpressionSyntax.Left;
        identifier.Identifier.Text.Should().Be(identifierName);

        assert(new ExpressionSyntaxAssertions(assignmentExpressionSyntax.Right, this.SemanticModel, this.Compilation));

        return this;
    }

    /// <summary>
    /// Assert that the syntax node is a throw statement.
    /// </summary>
    /// <param name="parameterAssertions">Assertions on the parameters of the created exception.</param>
    /// <returns>The assertions.</returns>
    /// <typeparam name="TException">The type of the exception thrown.</typeparam>
    public SyntaxNodeAssertions BeThrowStatementSyntax<TException>(
        params Action<ExpressionSyntaxAssertions>[] parameterAssertions)
    {
        ArgumentNullException.ThrowIfNull(parameterAssertions);

        this.Subject.Should().BeOfType<ThrowStatementSyntax>();
        var throwStatementSyntax = (ThrowStatementSyntax)this.Subject;
        throwStatementSyntax.Expression.Should().NotBeNull();
        throwStatementSyntax.Expression.Should().BeOfType<ObjectCreationExpressionSyntax>();
        var objectCreationExpressionSyntax = (ObjectCreationExpressionSyntax)throwStatementSyntax.Expression!;
        objectCreationExpressionSyntax.Type.ToString().Should().Be(typeof(TException).FullName);
        objectCreationExpressionSyntax.ArgumentList.Should().NotBeNull();
        objectCreationExpressionSyntax.ArgumentList!.Arguments.Should().HaveCount(parameterAssertions.Length);

        for (int index = 0; index < parameterAssertions.Length; ++index)
        {
            parameterAssertions[index](new ExpressionSyntaxAssertions(objectCreationExpressionSyntax.ArgumentList!.Arguments[index].Expression, this.SemanticModel, this.Compilation));
        }

        return this;
    }
}