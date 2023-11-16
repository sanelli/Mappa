// <copyright file="CaseSwitchLabelSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions on <see cref="CaseSwitchLabelSyntax"/>.
/// </summary>
public sealed class CaseSwitchLabelSyntaxAssertions
    : ObjectAssertions<CaseSwitchLabelSyntax, CaseSwitchLabelSyntaxAssertions>,
        ISwitchLabelSyntaxAssertions
{
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
    /// Assert that the syntax is a case.
    /// </summary>
    /// <returns>The current assertion.</returns>
    public ISwitchLabelSyntaxAssertions IsCase()
    {
        this.Subject.Should().BeOfType<CaseSwitchLabelSyntax>();
        return this;
    }

    /// <summary>
    /// Assert that the syntax is a default.
    /// </summary>
    /// <returns>The current assertion.</returns>
    public ISwitchLabelSyntaxAssertions IsDefault()
    {
        this.Subject.Should().BeOfType<DefaultSwitchLabelSyntax>();
        return this;
    }
}