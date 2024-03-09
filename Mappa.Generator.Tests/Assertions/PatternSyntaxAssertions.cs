// <copyright file="PatternSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions on the <see cref="PatternSyntax"/>.
/// </summary>
public sealed class PatternSyntaxAssertions
 : ObjectAssertions<PatternSyntax, PatternSyntaxAssertions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PatternSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation.</param>
    public PatternSyntaxAssertions(PatternSyntax value, SemanticModel semanticModel, Compilation compilation)
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
    /// Assert that the pattern is a unary pattern.
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

        argumentAssertions(new PatternSyntaxAssertions(unaryPatternSyntax.Pattern, this.SemanticModel, this.Compilation));

        return this;
    }

    /// <summary>
    /// Assert that the pattern is a constant pattern.
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
}