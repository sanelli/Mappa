// <copyright file="ISwitchLabelSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions on <see cref="SwitchLabelSyntax"/>.
/// </summary>
internal interface ISwitchLabelSyntaxAssertions
{
    /// <summary>
    /// Get the subject of the assertions.
    /// </summary>
    /// <returns>The subject of assertions.</returns>
    SwitchLabelSyntax GetSubject();

    /// <summary>
    /// Assert that the syntax is a case.
    /// </summary>
    /// <returns>The current assertion.</returns>
    sealed ISwitchLabelSyntaxAssertions IsCase()
    {
        this.GetSubject().Should().BeOfType<CaseSwitchLabelSyntax>();
        return this;
    }

    /// <summary>
    /// Assert that the syntax is a default.
    /// </summary>
    /// <returns>The current assertion.</returns>
    sealed ISwitchLabelSyntaxAssertions IsDefault()
    {
        this.GetSubject().Should().BeOfType<DefaultSwitchLabelSyntax>();
        return this;
    }

    /// <summary>
    /// Assert that the syntax is a case with pattern match.
    /// </summary>
    /// <returns>The current assertion.</returns>
    sealed ISwitchLabelSyntaxAssertions IsCasePattern()
    {
        this.GetSubject().Should().BeOfType<CasePatternSwitchLabelSyntax>();
        return this;
    }

    /// <summary>
    /// Returns an instance of the assertions as <see cref="CaseSwitchLabelSyntaxAssertions"/>.
    /// </summary>
    /// <returns>An instance of the assertions as <see cref="CaseSwitchLabelSyntaxAssertions"/>.</returns>
    sealed CaseSwitchLabelSyntaxAssertions AsCase() => (CaseSwitchLabelSyntaxAssertions)this;

    /// <summary>
    /// Returns an instance of the assertions as <see cref="DefaultSwitchLabelSyntaxAssertions"/>.
    /// </summary>
    /// <returns>An instance of the assertions as <see cref="DefaultSwitchLabelSyntaxAssertions"/>.</returns>
    sealed DefaultSwitchLabelSyntaxAssertions AsDefault() => (DefaultSwitchLabelSyntaxAssertions)this;

    /// <summary>
    /// Returns an instance of the assertions as <see cref="CasePatternSwitchLabelSyntaxAssertions"/>.
    /// </summary>
    /// <returns>An instance of the assertions as <see cref="CasePatternSwitchLabelSyntaxAssertions"/>.</returns>
    sealed CasePatternSwitchLabelSyntaxAssertions AsCasePattern() => (CasePatternSwitchLabelSyntaxAssertions)this;
}