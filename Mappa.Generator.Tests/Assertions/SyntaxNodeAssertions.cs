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
    public SyntaxNodeAssertions(SyntaxNode value)
        : base(value)
    {
    }

    /// <summary>
    /// Assert that the syntax node is a return statement.
    /// </summary>
    /// <param name="assert">The assertion.</param>
    /// <returns>The assertions.</returns>
    public SyntaxNodeAssertions IsReturnStatement(Action<ExpressionSyntaxAssertions>? assert = null)
    {
        this.Subject.Should().BeOfType<ReturnStatementSyntax>();
        var returnStatement = (ReturnStatementSyntax)this.Subject;
        if (assert is not null)
        {
            returnStatement.Expression!.Should().NotBeNull();
            assert(new ExpressionSyntaxAssertions(returnStatement.Expression!));
        }

        return this;
    }
}