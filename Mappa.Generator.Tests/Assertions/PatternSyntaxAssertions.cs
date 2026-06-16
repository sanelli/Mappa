// <copyright file="PatternSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions on the <see cref="PatternSyntax"/>.
/// </summary>
internal sealed class PatternSyntaxAssertions
 : ObjectAssertions<PatternSyntax, PatternSyntaxAssertions>
{
    private readonly SemanticModel semanticModel;
    private readonly Compilation compilation;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatternSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation.</param>
    public PatternSyntaxAssertions(PatternSyntax value, SemanticModel semanticModel, Compilation compilation)
        : base(value, AwesomeAssertions.Execution.AssertionChain.GetOrCreate())
    {
        this.semanticModel = semanticModel;
        this.compilation = compilation;
    }

    /// <summary>
    /// Assert that the pattern is a <see cref="UnaryPatternSyntax"/>.
    /// </summary>
    /// <param name="kind">The kind of the unary pattern.</param>
    /// <param name="argumentAssertions">Assertions on the argument expression.</param>
    /// <returns>The assertions.</returns>
    public PatternSyntaxAssertions BeUnaryPatternSyntax(SyntaxKind kind, Action<PatternSyntaxAssertions> argumentAssertions)
    {
        ArgumentNullException.ThrowIfNull(argumentAssertions);

        this.Subject.Should().BeOfType<UnaryPatternSyntax>();
        var unaryPatternSyntax = (UnaryPatternSyntax)this.Subject;

        unaryPatternSyntax.OperatorToken.Kind().Should().Be(kind);

        argumentAssertions(new PatternSyntaxAssertions(unaryPatternSyntax.Pattern, this.semanticModel, this.compilation));

        return this;
    }

    /// <summary>
    /// Assert that the pattern is a <see cref="ConstantPatternSyntax"/>.
    /// </summary>
    /// <param name="value">The value of the constant.</param>
    /// <returns>The assertions.</returns>
    public PatternSyntaxAssertions BeConstantPatternSyntax(object? value)
    {
        this.Subject.Should().BeOfType<ConstantPatternSyntax>();
        var constantPatternSyntax = (ConstantPatternSyntax)this.Subject;

        constantPatternSyntax.Expression.Should().BeOfType<LiteralExpressionSyntax>();
        var literalExpressionSyntax = (LiteralExpressionSyntax)constantPatternSyntax.Expression;

        literalExpressionSyntax.Token.Should().BeOfType<SyntaxToken>();
        literalExpressionSyntax.Token.Value.Should().Be(value);

        return this;
    }

    /// <summary>
    /// Assert that the pattern is a <see cref="DeclarationPatternSyntax"/>.
    /// </summary>
    /// <param name="type">The type of the declaration pattern.</param>
    /// <param name="variableName">The name of the variable.</param>
    /// <returns>The assertions.</returns>
    public PatternSyntaxAssertions BeDeclarationPatternSyntax(string type, string variableName)
    {
        this.Subject.Should().BeOfType<DeclarationPatternSyntax>();
        var declarationPatternSyntax = (DeclarationPatternSyntax)this.Subject;

        declarationPatternSyntax.Type.ToString().Should().MatchRegex($"(global::)*{Regex.Escape(type)}");
        declarationPatternSyntax.Designation.Should().BeOfType<SingleVariableDesignationSyntax>();
        var designation = (SingleVariableDesignationSyntax)declarationPatternSyntax.Designation;
        designation.Identifier.ValueText.Should().Be(variableName);

        return this;
    }
}