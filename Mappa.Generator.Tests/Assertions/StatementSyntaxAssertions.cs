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
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation.</param>
    public StatementSyntaxAssertions(StatementSyntax value, SemanticModel semanticModel, Compilation compilation)
        : base(value, FluentAssertions.Execution.AssertionChain.GetOrCreate())
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

    /// <inheritdoc/>
    public IStatementSyntaxBaseAssertions IsBlockStatement()
    {
        this.Subject.Should().BeOfType<BlockSyntax>();
        return this;
    }
}