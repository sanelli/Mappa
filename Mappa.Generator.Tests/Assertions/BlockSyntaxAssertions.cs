// <copyright file="BlockSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="BlockSyntax"/>.
/// </summary>
[DebuggerNonUserCode]
public sealed class BlockSyntaxAssertions
    : ObjectAssertions<BlockSyntax, BlockSyntaxAssertions>,
        IStatementSyntaxBaseAssertions
{
    private readonly SemanticModel semanticModel;
    private readonly Compilation compilation;
    private int nextSyntaxNodePosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlockSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation.</param>
    internal BlockSyntaxAssertions(
        BlockSyntax value,
        SemanticModel semanticModel,
        Compilation compilation)
        : base(value)
    {
        this.semanticModel = semanticModel;
        this.compilation = compilation;
    }

    /// <summary>
    /// Asset the number of nodes in the code block.
    /// </summary>
    /// <param name="count">The number of expected syntax nodes in the code block.</param>
    /// <returns>The assertion.</returns>
    public BlockSyntaxAssertions HasSyntaxNodesCount(int count)
    {
        this.Subject.ChildNodes().Should().HaveCount(count);
        return this;
    }

    /// <summary>
    /// Assert that the syntax node at a given position match the given criteria.
    /// </summary>
    /// <param name="position">The position of the token.</param>
    /// <param name="assert">The assertions on the node.</param>
    /// <returns>The assertions.</returns>
    public BlockSyntaxAssertions HasSyntaxNode(int position, Action<SyntaxNodeAssertions> assert)
    {
        ArgumentNullException.ThrowIfNull(assert);

        var syntaxNode = this.Subject.ChildNodes().ElementAt(position);
        assert(new SyntaxNodeAssertions(syntaxNode, this.semanticModel, this.compilation));
        return this;
    }

    /// <summary>
    /// Assert that the syntax node matched the given criteria.
    /// </summary>
    /// <param name="assert">The assertions on the node.</param>
    /// <returns>The assertions.</returns>
    public BlockSyntaxAssertions HasNextSyntaxNode(Action<SyntaxNodeAssertions> assert)
    {
        return this.HasSyntaxNode(this.nextSyntaxNodePosition++, assert);
    }

    /// <inheritdoc/>
    public IStatementSyntaxBaseAssertions IsBlockStatement()
    {
        this.Subject.Should().BeOfType<BlockSyntax>();
        return this;
    }
}