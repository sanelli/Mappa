// <copyright file="IStatementSyntaxBaseAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Base interface to assert on <see cref="StatementSyntax"/>.
/// </summary>
internal interface IStatementSyntaxBaseAssertions
{
    /// <summary>
    /// Assert the subject is a block statement.
    /// </summary>
    /// <returns>The assertion.</returns>
    IStatementSyntaxBaseAssertions BeBlockStatement();

    /// <summary>
    /// Returns this instance as a <see cref="BlockSyntaxAssertions"/>.
    /// </summary>
    /// <returns>This instance as a <see cref="BlockSyntaxAssertions"/>.</returns>
    public BlockSyntaxAssertions AsBlock() => (BlockSyntaxAssertions)this;

    /// <summary>
    /// Returns this instance as a <see cref="StatementSyntaxAssertions"/>.
    /// </summary>
    /// <returns>This instance as a <see cref="StatementSyntaxAssertions"/>.</returns>
    public StatementSyntaxAssertions AsStatement() => (StatementSyntaxAssertions)this;
}