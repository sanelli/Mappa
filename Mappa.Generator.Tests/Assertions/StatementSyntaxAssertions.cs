// <copyright file="StatementSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions on <see cref="StatementSyntax"/>.
/// </summary>
internal sealed class StatementSyntaxAssertions
    : ObjectAssertions<StatementSyntax, StatementSyntaxAssertions>,
        IStatementSyntaxBaseAssertions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StatementSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of assertions.</param>
    public StatementSyntaxAssertions(StatementSyntax value)
        : base(value, FluentAssertions.Execution.AssertionChain.GetOrCreate())
    {
    }

    /// <inheritdoc/>
    public IStatementSyntaxBaseAssertions IsBlockStatement()
    {
        this.Subject.Should().BeOfType<BlockSyntax>();
        return this;
    }
}