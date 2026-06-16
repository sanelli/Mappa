// <copyright file="CasePatternSwitchLabelSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="CasePatternSwitchLabelSyntax"/>.
/// </summary>
internal sealed class CasePatternSwitchLabelSyntaxAssertions
    : ObjectAssertions<CasePatternSwitchLabelSyntax, CasePatternSwitchLabelSyntaxAssertions>,
        ISwitchLabelSyntaxAssertions
{
    private readonly SemanticModel semanticModel;
    private readonly Compilation compilation;

    /// <summary>
    /// Initializes a new instance of the <see cref="CasePatternSwitchLabelSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation.</param>
    public CasePatternSwitchLabelSyntaxAssertions(
        CasePatternSwitchLabelSyntax value, SemanticModel semanticModel, Compilation compilation)
        : base(value, AwesomeAssertions.Execution.AssertionChain.GetOrCreate())
    {
        this.semanticModel = semanticModel;
        this.compilation = compilation;
    }

    /// <summary>
    /// Assert on the pattern.
    /// </summary>
    /// <returns>The assertions.</returns>
    /// <param name="assert">The assertion on the pattern.</param>
    public CasePatternSwitchLabelSyntaxAssertions HasPattern(Action<PatternSyntaxAssertions> assert)
    {
        assert(new PatternSyntaxAssertions(this.Subject.Pattern, this.semanticModel, this.compilation));
        return this;
    }

    /// <inheritdoc/>
    public SwitchLabelSyntax GetSubject() => this.Subject;
}