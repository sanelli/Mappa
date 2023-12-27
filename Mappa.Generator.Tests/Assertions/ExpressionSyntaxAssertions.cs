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
    public ExpressionSyntaxAssertions BeIdentifierNameSyntax(string identifierName)
    {
        this.Subject.Should().BeOfType<IdentifierNameSyntax>();
        var identifierNameSyntax = (IdentifierNameSyntax)this.Subject;
        identifierNameSyntax.Identifier.Text.Should().Be(identifierName);
        return this;
    }

    /// <summary>
    /// Assert that the expression is a cast expression.
    /// </summary>
    /// <param name="type">The type the expression is being cast to.</param>
    /// <param name="assert">Assert the expression.</param>
    /// <returns>The assertion.</returns>
    public ExpressionSyntaxAssertions BeCastExpressionSyntax(
        string type,
        Action<ExpressionSyntaxAssertions> assert)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(assert);

        this.Subject.Should().BeOfType<CastExpressionSyntax>();
        var castExpressionSyntax = (CastExpressionSyntax)this.Subject;

        var isTypeNullable = type.EndsWith('?');
        type = type.Replace("?", string.Empty, StringComparison.Ordinal);

        var actualStringType = castExpressionSyntax.Type.ToString();
        var isActualTypeNullable = actualStringType!.EndsWith('?');
        actualStringType = actualStringType.Replace("?", string.Empty, StringComparison.Ordinal);

        var expectedType = this.Compilation.GetTypeSymbol(type);
        var actualType = this.Compilation.GetTypeSymbol(actualStringType);

        SymbolEqualityComparer
            .Default
            .Equals(actualType, expectedType)
            .Should().BeTrue();

        isTypeNullable.Should().Be(isActualTypeNullable);

        assert(new ExpressionSyntaxAssertions(castExpressionSyntax.Expression, this.SemanticModel, this.Compilation));
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
    public ExpressionSyntaxAssertions BeLiteralExpressionSyntax(object? value)
    {
        this.Subject.Should().BeOfType<LiteralExpressionSyntax>();
        var literalExpressionSyntax = (LiteralExpressionSyntax)this.Subject;
        literalExpressionSyntax.Token.Should().BeOfType<SyntaxToken>();
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
            .BeIdentifierNameSyntax("nameof");
        invocationExpressionSyntax.ArgumentList.Arguments.Should().HaveCount(1);
        new ExpressionSyntaxAssertions(invocationExpressionSyntax.ArgumentList.Arguments[0].Expression, this.SemanticModel, this.Compilation)
            .BeMemberAccessExpressionSyntax(name);
        return this;
    }

    /// <summary>
    /// Assert that the expression is an invocation expression.
    /// </summary>
    /// <param name="memberAccess">The name of the method being invoked.</param>
    /// <param name="assertArguments">Assertions on the arguments.</param>
    /// <returns>The assertion.</returns>
    public ExpressionSyntaxAssertions BeInvocationExpressionSyntax(
        string memberAccess,
        params Action<ExpressionSyntaxAssertions>[] assertArguments)
    {
        ArgumentNullException.ThrowIfNull(assertArguments);

        this.Subject.Should().BeOfType<InvocationExpressionSyntax>();
        var invocationExpressionSyntax = (InvocationExpressionSyntax)this.Subject;
        new ExpressionSyntaxAssertions(invocationExpressionSyntax.Expression, this.SemanticModel, this.Compilation)
            .BeMemberAccessExpressionSyntax(memberAccess);
        invocationExpressionSyntax.ArgumentList.Arguments.Should().HaveCount(assertArguments.Length);

        for (int index = 0; index < assertArguments.Length; ++index)
        {
            assertArguments[index](new ExpressionSyntaxAssertions(invocationExpressionSyntax.ArgumentList.Arguments[index].Expression, this.SemanticModel, this.Compilation));
        }

        return this;
    }

    /// <summary>
    /// Assert that the expression is an array creation expression.
    /// </summary>
    /// <param name="type">The type being created.</param>
    /// <param name="sizeAssertion">The assertions on the size expression.</param>
    /// <returns>The assertion.</returns>
    public ExpressionSyntaxAssertions BeArrayCreationExpressionSyntax(string type, Action<ExpressionSyntaxAssertions> sizeAssertion)
    {
        ArgumentNullException.ThrowIfNull(sizeAssertion);

        this.Subject.Should().BeOfType<ArrayCreationExpressionSyntax>();
        var arrayCreationExpressionSyntax = (ArrayCreationExpressionSyntax)this.Subject;

        var expectedType = this.Compilation.GetTypeSymbol(type);
        var localSymbol = this.Compilation.GetTypeSymbol(arrayCreationExpressionSyntax.Type.ElementType.ToString());

        SymbolEqualityComparer
            .Default
            .Equals(localSymbol, expectedType)
            .Should().BeTrue();

        arrayCreationExpressionSyntax.Type.RankSpecifiers.Should().HaveCount(1);
        arrayCreationExpressionSyntax.Type.RankSpecifiers[0].Sizes.Should().HaveCount(1);
        var size = arrayCreationExpressionSyntax.Type.RankSpecifiers[0].Sizes[0];
        sizeAssertion(new ExpressionSyntaxAssertions(size, this.SemanticModel, this.Compilation));

        return this;
    }

    /// <summary>
    /// Assert that the expression in a element access expression syntax having
    /// identifier name syntax for accessing both the array and the index.
    /// </summary>
    /// <param name="arrayName">The name of the array.</param>
    /// <param name="indexVariableName">The name of the index variable.</param>
    /// <returns>The assertions.</returns>
    public ExpressionSyntaxAssertions BeElementAccessExpressionSyntaxWithIdentifierNameSyntax(string arrayName, string indexVariableName)
    {
        this.Subject.Should().BeOfType<ElementAccessExpressionSyntax>();

        var elementAccessExpressionSyntax = (ElementAccessExpressionSyntax)this.Subject;
        var expression = elementAccessExpressionSyntax.Expression;
        expression.Should().BeOfType<IdentifierNameSyntax>();
        var arrayAccess = (IdentifierNameSyntax)expression;
        arrayAccess.Identifier.ToString().Should().Be(arrayName);

        var argumentList = elementAccessExpressionSyntax.ArgumentList;
        argumentList.Arguments.Should().HaveCount(1);
        argumentList.Arguments[0].Expression.Should().BeOfType<IdentifierNameSyntax>();
        var indexExpression = (IdentifierNameSyntax)argumentList.Arguments[0].Expression;
        indexExpression.Identifier.ToString().Should().Be(indexVariableName);

        return this;
    }

    /// <summary>
    /// Assert that the expression is a binary expression.
    /// </summary>
    /// <param name="leftExpressionAssertions">The left expression assertion.</param>
    /// <param name="operator">The operator token.</param>
    /// <param name="rightExpressionAssertions">The right expression assertion.</param>
    /// <returns>The assertions.</returns>
    public ExpressionSyntaxAssertions BeBinaryExpressionSyntax(
        Action<ExpressionSyntaxAssertions> leftExpressionAssertions,
        SyntaxKind @operator,
        Action<ExpressionSyntaxAssertions> rightExpressionAssertions)
    {
        ArgumentNullException.ThrowIfNull(leftExpressionAssertions);
        ArgumentNullException.ThrowIfNull(rightExpressionAssertions);

        this.Subject.Should().BeOfType<BinaryExpressionSyntax>();
        var binaryExpressionSyntax = (BinaryExpressionSyntax)this.Subject;

        binaryExpressionSyntax.OperatorToken.Kind().Should().Be(@operator);

        leftExpressionAssertions(new ExpressionSyntaxAssertions(binaryExpressionSyntax.Left, this.SemanticModel, this.Compilation));
        rightExpressionAssertions(new ExpressionSyntaxAssertions(binaryExpressionSyntax.Right, this.SemanticModel, this.Compilation));

        return this;
    }

    /// <summary>
    /// Assert that the expression is a prefix unary expression syntax.
    /// </summary>
    /// <param name="operator">The operator token.</param>
    /// <param name="operandAssertions">The operand expression assertion.</param>
    /// <returns>The assertions.</returns>
    public ExpressionSyntaxAssertions BePrefixUnaryExpressionSyntax(
        SyntaxKind @operator,
        Action<ExpressionSyntaxAssertions> operandAssertions)
    {
        ArgumentNullException.ThrowIfNull(operandAssertions);

        this.Subject.Should().BeOfType<PrefixUnaryExpressionSyntax>();
        var prefixUnaryExpressionSyntax = (PrefixUnaryExpressionSyntax)this.Subject;

        prefixUnaryExpressionSyntax.OperatorToken.Kind().Should().Be(@operator);
        operandAssertions(new ExpressionSyntaxAssertions(prefixUnaryExpressionSyntax.Operand, this.SemanticModel, this.Compilation));

        return this;
    }

    /// <summary>
    /// Assert that the expression is an object creation expression.
    /// </summary>
    /// <param name="type">The type being created.</param>
    /// <param name="argumentExpressionAssertions">The assertions on the argument expressions.</param>
    /// <param name="initializationAssertions">The assertions on the initialization expressions.</param>
    /// <returns>The assertions.</returns>
    public ExpressionSyntaxAssertions BeObjectCreationExpressionSyntax(
        string type,
        Action<ExpressionSyntaxAssertions>[] argumentExpressionAssertions,
        (string PropertyName, Action<ExpressionSyntaxAssertions> Assertions)[] initializationAssertions)
    {
        ArgumentNullException.ThrowIfNull(argumentExpressionAssertions);
        ArgumentNullException.ThrowIfNull(initializationAssertions);

        this.Subject.Should().BeOfType<ObjectCreationExpressionSyntax>();
        var objectCreationExpressionSyntax = (ObjectCreationExpressionSyntax)this.Subject;

        var expectedType = this.Compilation.GetTypeSymbol(type);
        var actualType = this.Compilation.GetTypeSymbol(objectCreationExpressionSyntax.Type.ToString());

        SymbolEqualityComparer
            .Default
            .Equals(actualType, expectedType)
            .Should().BeTrue();

        if (argumentExpressionAssertions.Length == 0)
        {
            objectCreationExpressionSyntax.ArgumentList.Should().NotBeNull();
            objectCreationExpressionSyntax.ArgumentList!.Arguments.Should().BeNullOrEmpty();
        }
        else
        {
            objectCreationExpressionSyntax.ArgumentList.Should().NotBeNull();
            objectCreationExpressionSyntax.ArgumentList!.Arguments.Should().HaveCount(argumentExpressionAssertions.Length);

            for (var argumentIndex = 0; argumentIndex < argumentExpressionAssertions.Length; ++argumentIndex)
            {
                argumentExpressionAssertions[argumentIndex](new ExpressionSyntaxAssertions(objectCreationExpressionSyntax.ArgumentList.Arguments[argumentIndex].Expression, this.SemanticModel, this.Compilation));
            }
        }

        if (initializationAssertions.Length == 0)
        {
            objectCreationExpressionSyntax.Initializer.Should().BeNull();
        }
        else
        {
            objectCreationExpressionSyntax.Initializer.Should().NotBeNull();
            objectCreationExpressionSyntax.Initializer!.Expressions.Should().HaveCount(initializationAssertions.Length);

            for (var initializerIndex = 0; initializerIndex < initializationAssertions.Length; ++initializerIndex)
            {
                objectCreationExpressionSyntax.Initializer!.Expressions[initializerIndex].Should().BeOfType<AssignmentExpressionSyntax>();
                var assignmentExpression = (AssignmentExpressionSyntax)objectCreationExpressionSyntax.Initializer!.Expressions[initializerIndex];

                assignmentExpression.Left.Should().BeOfType<IdentifierNameSyntax>();
                var leftExpression = (IdentifierNameSyntax)assignmentExpression.Left;
                leftExpression.Identifier.Text.Should().Be(initializationAssertions[initializerIndex].PropertyName);

                initializationAssertions[initializerIndex].Assertions(new ExpressionSyntaxAssertions(assignmentExpression.Right, this.SemanticModel, this.Compilation));
            }
        }

        return this;
    }

    /// <summary>
    /// Assert that the expression is an object creation expression.
    /// </summary>
    /// <param name="type">The type being created.</param>
    /// <param name="argumentExpressionAssertions">The assertions on the argument expressions.</param>
    /// <returns>The assertions.</returns>
    public ExpressionSyntaxAssertions BeObjectCreationExpressionSyntax(
        string type,
        params Action<ExpressionSyntaxAssertions>[] argumentExpressionAssertions)
        => this.BeObjectCreationExpressionSyntax(type, argumentExpressionAssertions, Array.Empty<(string PropertyName, Action<ExpressionSyntaxAssertions> Assertions)>());

    /// <summary>
    /// Assert that the expression is an object creation expression.
    /// </summary>
    /// <param name="type">The type being created.</param>
    /// <param name="initializationAssertions">The assertions on the initialization expressions.</param>
    /// <returns>The assertions.</returns>
    public ExpressionSyntaxAssertions BeObjectCreationExpressionSyntax(
        string type,
        params (string PropertyName, Action<ExpressionSyntaxAssertions> Assertions)[] initializationAssertions)
        => this.BeObjectCreationExpressionSyntax(type, Array.Empty<Action<ExpressionSyntaxAssertions>>(), initializationAssertions);

    /// <summary>
    /// Assert that the expression is an object creation expression.
    /// </summary>
    /// <param name="type">The type being created.</param>
    /// <returns>The assertions.</returns>
    public ExpressionSyntaxAssertions BeObjectCreationExpressionSyntax(string type)
        => this.BeObjectCreationExpressionSyntax(type, Array.Empty<Action<ExpressionSyntaxAssertions>>(), Array.Empty<(string PropertyName, Action<ExpressionSyntaxAssertions> Assertions)>());

    /// <summary>
    /// Assert that the expression is an <c>is</c> pattern expression.
    /// </summary>
    /// <param name="expressionAssertions">The assertions for the expression on the left side of the <c>is</c>.</param>
    /// <param name="patternAssertions">The assertions on the pattern.</param>
    /// <returns>The assertions.</returns>
    public ExpressionSyntaxAssertions IsIsPatternExpressionSyntax(
        Action<ExpressionSyntaxAssertions> expressionAssertions,
        Action<PatternSyntaxAssertions> patternAssertions)
    {
        ArgumentNullException.ThrowIfNull(expressionAssertions);
        ArgumentNullException.ThrowIfNull(patternAssertions);

        this.Subject.Should().BeOfType<IsPatternExpressionSyntax>();
        var isPatternExpressionSyntax = (IsPatternExpressionSyntax)this.Subject;

        expressionAssertions(new ExpressionSyntaxAssertions(isPatternExpressionSyntax.Expression, this.SemanticModel, this.Compilation));
        patternAssertions(new PatternSyntaxAssertions(isPatternExpressionSyntax.Pattern, this.SemanticModel, this.Compilation));

        return this;
    }
}