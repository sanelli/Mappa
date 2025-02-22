// <copyright file="DefaultSwitchLabelSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions on <see cref="DefaultSwitchLabelSyntax"/>.
/// </summary>
internal sealed class DefaultSwitchLabelSyntaxAssertions
    : ObjectAssertions<DefaultSwitchLabelSyntax, DefaultSwitchLabelSyntaxAssertions>,
        ISwitchLabelSyntaxAssertions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultSwitchLabelSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    internal DefaultSwitchLabelSyntaxAssertions(
        DefaultSwitchLabelSyntax value)
        : base(value, FluentAssertions.Execution.AssertionChain.GetOrCreate())
    {
    }

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