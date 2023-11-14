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
    public ExpressionSyntaxAssertions(
        ExpressionSyntax value)
        : base(value)
    {
    }

    /// <summary>
    /// Assert that the expression is an identifier name expression.
    /// </summary>
    /// <param name="identifierName">The identifier.</param>
    /// <returns>The assertion.</returns>
    public ExpressionSyntaxAssertions IsIdentifierName(string identifierName)
    {
        this.Subject.Should().BeOfType<IdentifierNameSyntax>();
        ((IdentifierNameSyntax)this.Subject).Identifier.Text.Should().Be(identifierName);
        return this;
    }
}