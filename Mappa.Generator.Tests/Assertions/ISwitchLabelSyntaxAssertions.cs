// <copyright file="ISwitchLabelSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions on <see cref="SwitchLabelSyntax"/>.
/// </summary>
public interface ISwitchLabelSyntaxAssertions
{
    /// <summary>
    /// Assert that the syntax is a case.
    /// </summary>
    /// <returns>The current assertion.</returns>
    public ISwitchLabelSyntaxAssertions IsCase();

    /// <summary>
    /// Assert that the syntax is a default.
    /// </summary>
    /// <returns>The current assertion.</returns>
    public ISwitchLabelSyntaxAssertions IsDefault();

    /// <summary>
    /// Returns an instance of this assertions as <see cref="CaseSwitchLabelSyntaxAssertions"/>.
    /// </summary>
    /// <returns>An instance of this assertions as <see cref="CaseSwitchLabelSyntaxAssertions"/>.</returns>
    public CaseSwitchLabelSyntaxAssertions AsCase() => (CaseSwitchLabelSyntaxAssertions)this;

    /// <summary>
    /// Returns an instance of this assertions as <see cref="DefaultSwitchLabelSyntaxAssertions"/>.
    /// </summary>
    /// <returns>An instance of this assertions as <see cref="DefaultSwitchLabelSyntaxAssertions"/>.</returns>
    public DefaultSwitchLabelSyntaxAssertions AsDefault() => (DefaultSwitchLabelSyntaxAssertions)this;
}