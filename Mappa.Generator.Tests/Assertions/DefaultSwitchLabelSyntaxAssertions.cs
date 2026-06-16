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
        : base(value, AwesomeAssertions.Execution.AssertionChain.GetOrCreate())
    {
    }

    /// <inheritdoc/>
    public SwitchLabelSyntax GetSubject() => this.Subject;
}