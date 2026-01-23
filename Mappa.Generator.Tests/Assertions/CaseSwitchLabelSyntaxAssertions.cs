// <copyright file="CaseSwitchLabelSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions on <see cref="CaseSwitchLabelSyntax"/>.
/// </summary>
internal sealed class CaseSwitchLabelSyntaxAssertions
    : ObjectAssertions<CaseSwitchLabelSyntax, CaseSwitchLabelSyntaxAssertions>,
        ISwitchLabelSyntaxAssertions
{
    private readonly SemanticModel semanticModel;
    private readonly Compilation compilation;

    /// <summary>
    /// Initializes a new instance of the <see cref="CaseSwitchLabelSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation.</param>
    internal CaseSwitchLabelSyntaxAssertions(
        CaseSwitchLabelSyntax value,
        SemanticModel semanticModel,
        Compilation compilation)
        : base(value, FluentAssertions.Execution.AssertionChain.GetOrCreate())
    {
        this.semanticModel = semanticModel;
        this.compilation = compilation;
    }

    /// <summary>
    /// Assert on the value of the case.
    /// </summary>
    /// <param name="assert">The assertion on the value of the case.</param>
    /// <returns>The assertions.</returns>
    public CaseSwitchLabelSyntaxAssertions HasValue(Action<ExpressionSyntaxAssertions> assert)
    {
        ArgumentNullException.ThrowIfNull(assert);
        assert(new ExpressionSyntaxAssertions(this.Subject.Value, this.semanticModel, this.compilation));
        return this;
    }

    /// <inheritdoc/>
    public SwitchLabelSyntax GetSubject() => this.Subject;
}