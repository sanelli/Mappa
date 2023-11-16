// <copyright file="IStatementSyntaxBaseAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Base interface to assert on <see cref="StatementSyntax"/>.
/// </summary>
public interface IStatementSyntaxBaseAssertions
{
    /// <summary>
    /// Assert the subject is a block statement.
    /// </summary>
    /// <returns>The assertion.</returns>
    IStatementSyntaxBaseAssertions IsBlockStatement();
}