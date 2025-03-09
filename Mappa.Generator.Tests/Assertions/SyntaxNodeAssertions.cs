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
internal sealed class SyntaxNodeAssertions
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
        : base(value, FluentAssertions.Execution.AssertionChain.GetOrCreate())
    {
        this.semanticModel = semanticModel;
        this.compilation = compilation;
    }

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
            expressionAssertion(new ExpressionSyntaxAssertions(returnStatement.Expression!, this.semanticModel, this.compilation));
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
        => this.BeLocalDeclarationStatementSyntax(type, [name], [assertInitialization]);

    /// <summary>
    /// Validate that the syntax node is a variable declaration syntax.
    /// </summary>
    /// <param name="type">The type of the variable.</param>
    /// <param name="names">The names of the variables declared.</param>
    /// <param name="assertInitializations">Assert the initialization expressions.</param>
    /// <returns>The assertion.</returns>
    public SyntaxNodeAssertions BeLocalDeclarationStatementSyntax(string type, string[] names, Action<ExpressionSyntaxAssertions>[]? assertInitializations)
    {
        ArgumentNullException.ThrowIfNull(type);
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

        var isTypeNullable = type.EndsWith('?');
        type = type.Replace("?", string.Empty, StringComparison.Ordinal);

        var localSymbol = this.semanticModel.GetDeclaredSymbol(localDeclarationStatementSyntax.Declaration.Variables[0]) as ILocalSymbol;
        localSymbol.Should().NotBeNull();

        ITypeSymbol expectedType;
        if (localSymbol.Type.IsValueType && isTypeNullable)
        {
            var valueTypeNullableStringSymbol = $"{typeof(Nullable)}<{type}>";
            expectedType = this.compilation.GetTypeSymbol(valueTypeNullableStringSymbol);
        }
        else
        {
            expectedType = this.compilation.GetTypeSymbol(type);
        }

        SymbolEqualityComparer
            .Default
            .Equals(localSymbol.Type, expectedType)
            .Should().BeTrue();

        if (expectedType.IsReferenceType)
        {
            if (isTypeNullable)
            {
                localSymbol.NullableAnnotation.Should().Be(NullableAnnotation.Annotated);
            }
            else
            {
                localSymbol.NullableAnnotation.Should().BeOneOf([NullableAnnotation.None, NullableAnnotation.NotAnnotated]);
            }
        }

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
                assertInitializations[index](new ExpressionSyntaxAssertions(initializer.Value, this.semanticModel, this.compilation));
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
        assertExpression(new ExpressionSyntaxAssertions(switchStatementSyntax.Expression, this.semanticModel, this.compilation));

        var caseStatements = switchStatementSyntax
            .ChildNodes()
            .Skip(1)
            .ToArray();

        caseStatements.Should().HaveCount(assertCase.Length);
        for (var index = 0; index < assertCase.Length; ++index)
        {
            caseStatements[index].Should().BeOfType<SwitchSectionSyntax>();
            var switchSectionSyntax = (SwitchSectionSyntax)caseStatements[index];

            var labelAssertions = switchSectionSyntax.Labels.ToAssertions(this.semanticModel, this.compilation);
            var statementAssertions = switchSectionSyntax.Statements.ToAssertions(this.semanticModel, this.compilation);

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

        return this.BeAssignmentExpressionStatement(
            identifierAssertion => identifierAssertion.BeIdentifierNameSyntax(identifierName),
            assert);
    }

    /// <summary>
    /// Assert that that the syntax node is an assignment.
    /// </summary>
    /// <param name="leftExpressionAssertions">The left expression assertions.</param>
    /// <param name="rightExpressionAssertions">The right expression assertions.</param>
    /// <returns>The assertions.</returns>
    public SyntaxNodeAssertions BeAssignmentExpressionStatement(Action<ExpressionSyntaxAssertions> leftExpressionAssertions, Action<ExpressionSyntaxAssertions> rightExpressionAssertions)
    {
        ArgumentNullException.ThrowIfNull(leftExpressionAssertions);
        ArgumentNullException.ThrowIfNull(rightExpressionAssertions);

        this.Subject.Should().BeOfType<ExpressionStatementSyntax>();
        var expressionStatementSyntax = (ExpressionStatementSyntax)this.Subject;
        expressionStatementSyntax.Expression.Should().BeOfType<AssignmentExpressionSyntax>();
        var assignmentExpressionSyntax = (AssignmentExpressionSyntax)expressionStatementSyntax.Expression;

        leftExpressionAssertions(new ExpressionSyntaxAssertions(assignmentExpressionSyntax.Left, this.semanticModel, this.compilation));
        rightExpressionAssertions(new ExpressionSyntaxAssertions(assignmentExpressionSyntax.Right, this.semanticModel, this.compilation));

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
            parameterAssertions[index](new ExpressionSyntaxAssertions(objectCreationExpressionSyntax.ArgumentList!.Arguments[index].Expression, this.semanticModel, this.compilation));
        }

        return this;
    }

    /// <summary>
    /// Assert that the syntax node is a for statement.
    /// </summary>
    /// <param name="declarationAssertion">Assertions on the variables declaration.</param>
    /// <param name="conditionAssertion">Assertions on the condition.</param>
    /// <param name="incrementorAssertion">Assertions on the incrementor expression.</param>
    /// <param name="statementAssertion">Assertion on the statement.</param>
    /// <returns>The assertions.</returns>
    public SyntaxNodeAssertions BeForStatementSyntax(
        Action<VariableDeclarationSyntaxAssertions>? declarationAssertion,
        Action<ExpressionSyntaxAssertions>? conditionAssertion,
        Action<ExpressionSyntaxAssertions>? incrementorAssertion,
        Action<IStatementSyntaxBaseAssertions> statementAssertion)
        => this.BeForStatementSyntax(
            declarationAssertion,
            conditionAssertion,
            incrementorAssertion is null ? null : [incrementorAssertion],
            statementAssertion);

    /// <summary>
    /// Assert that the syntax node is a for statement.
    /// </summary>
    /// <param name="declarationAssertion">Assertions on the variables declaration.</param>
    /// <param name="conditionAssertion">Assertions on the condition.</param>
    /// <param name="incrementorAssertions">Assertions on the incrementor expressions.</param>
    /// <param name="statementAssertion">Assertion on the statement.</param>
    /// <returns>The assertions.</returns>
    public SyntaxNodeAssertions BeForStatementSyntax(
        Action<VariableDeclarationSyntaxAssertions>? declarationAssertion,
        Action<ExpressionSyntaxAssertions>? conditionAssertion,
        Action<ExpressionSyntaxAssertions>[]? incrementorAssertions,
        Action<IStatementSyntaxBaseAssertions> statementAssertion)
    {
        ArgumentNullException.ThrowIfNull(statementAssertion);

        this.Subject.Should().BeOfType<ForStatementSyntax>();
        var forStatementSyntax = (ForStatementSyntax)this.Subject;

        if (declarationAssertion is null)
        {
            forStatementSyntax.Declaration.Should().BeNull();
        }
        else
        {
            forStatementSyntax.Declaration.Should().NotBeNull();
            declarationAssertion(new VariableDeclarationSyntaxAssertions(forStatementSyntax.Declaration!, this.semanticModel, this.compilation));
        }

        if (conditionAssertion is null)
        {
            forStatementSyntax.Condition.Should().BeNull();
        }
        else
        {
            forStatementSyntax.Condition.Should().NotBeNull();
            conditionAssertion(new ExpressionSyntaxAssertions(forStatementSyntax.Condition!, this.semanticModel, this.compilation));
        }

        if (incrementorAssertions is null)
        {
            forStatementSyntax.Incrementors.Should().BeNullOrEmpty();
        }
        else
        {
            forStatementSyntax.Incrementors.Should().NotBeNull();
            forStatementSyntax.Incrementors.Should().HaveCount(incrementorAssertions.Length);

            for (int index = 0; index < incrementorAssertions.Length; ++index)
            {
                incrementorAssertions[index](new ExpressionSyntaxAssertions(forStatementSyntax.Incrementors[index], this.semanticModel, this.compilation));
            }
        }

        statementAssertion(forStatementSyntax.Statement.ToAssertion(this.semanticModel, this.compilation));

        return this;
    }

    /// <summary>
    /// Assert that that the syntax node is an assignment.
    /// </summary>
    /// <param name="accessIdentifier">Describe the access to the method.</param>
    /// <param name="argumentExpressionAssertions">The assertions on the arguments.</param>
    /// <returns>The assertions.</returns>
    public SyntaxNodeAssertions BeInvocationExpressionSyntaxStatement(
        string accessIdentifier,
        params Action<ExpressionSyntaxAssertions>[] argumentExpressionAssertions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessIdentifier);
        ArgumentNullException.ThrowIfNull(argumentExpressionAssertions);

        this.Subject.Should().BeOfType<ExpressionStatementSyntax>();
        var expressionStatementSyntax = (ExpressionStatementSyntax)this.Subject;

        expressionStatementSyntax.Expression.Should().BeOfType<InvocationExpressionSyntax>();
        var invocationExpressionSyntax = (InvocationExpressionSyntax)expressionStatementSyntax.Expression;

        new ExpressionSyntaxAssertions(invocationExpressionSyntax.Expression, this.semanticModel, this.compilation)
            .BeMemberAccessExpressionSyntax(accessIdentifier);

        invocationExpressionSyntax.ArgumentList.Arguments.Should().HaveCount(argumentExpressionAssertions.Length);
        for (int index = 0; index < argumentExpressionAssertions.Length; ++index)
        {
            argumentExpressionAssertions[index](new ExpressionSyntaxAssertions(invocationExpressionSyntax.ArgumentList.Arguments[index].Expression, this.semanticModel, this.compilation));
        }

        return this;
    }

    /// <summary>
    /// Assert that the statement is a <c>foreach</c> statement.
    /// </summary>
    /// <param name="type">The type of the identifier being defined.</param>
    /// <param name="identifier">The identifier used in the for loop.</param>
    /// <param name="expressionAssertions">The expression assertions.</param>
    /// <param name="statementAssertions">Assertions on the statement.</param>
    /// <returns>The assertions.</returns>
    public SyntaxNodeAssertions BeForEachStatementSyntax(
        string type,
        string identifier,
        Action<ExpressionSyntaxAssertions> expressionAssertions,
        Action<IStatementSyntaxBaseAssertions> statementAssertions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        ArgumentNullException.ThrowIfNull(expressionAssertions);
        ArgumentNullException.ThrowIfNull(statementAssertions);

        this.Subject.Should().BeOfType<ForEachStatementSyntax>();
        var forEachStatementSyntax = (ForEachStatementSyntax)this.Subject;

        var expectedType = this.compilation.GetTypeSymbol(forEachStatementSyntax.Type.ToString());
        var actualType = this.compilation.GetTypeSymbol(forEachStatementSyntax.Type.ToString());

        SymbolEqualityComparer
            .Default
            .Equals(actualType, expectedType)
            .Should().BeTrue();

        forEachStatementSyntax.Identifier.Text.Should().Be(identifier);
        expressionAssertions(new ExpressionSyntaxAssertions(forEachStatementSyntax.Expression, this.semanticModel, this.compilation));
        statementAssertions(forEachStatementSyntax.Statement.ToAssertion(this.semanticModel, this.compilation));

        return this;
    }

    /// <summary>
    /// Assert the statement is an <c>if</c> statement.
    /// </summary>
    /// <param name="conditionAssertions">The assertions on the if condition.</param>
    /// <param name="thenStatementAssertions">The assertions on the then statement.</param>
    /// <param name="elseStatementAssertions">The assertions on the else condition.</param>
    /// <returns>The assertions.</returns>
    public SyntaxNodeAssertions BeIfStatementSyntax(
        Action<ExpressionSyntaxAssertions> conditionAssertions,
        Action<IStatementSyntaxBaseAssertions> thenStatementAssertions,
        Action<IStatementSyntaxBaseAssertions>? elseStatementAssertions = null)
    {
        ArgumentNullException.ThrowIfNull(conditionAssertions);
        ArgumentNullException.ThrowIfNull(thenStatementAssertions);

        this.Subject.Should().BeOfType<IfStatementSyntax>();
        var ifStatementSyntax = (IfStatementSyntax)this.Subject;

        conditionAssertions(new ExpressionSyntaxAssertions(ifStatementSyntax.Condition, this.semanticModel, this.compilation));
        thenStatementAssertions(ifStatementSyntax.Statement.ToAssertion(this.semanticModel, this.compilation));

        if (elseStatementAssertions is null)
        {
            ifStatementSyntax.Else.Should().BeNull();
        }
        else
        {
            ifStatementSyntax.Else.Should().NotBeNull();
            elseStatementAssertions(ifStatementSyntax.Else!.Statement.ToAssertion(this.semanticModel, this.compilation));
        }

        return this;
    }
}