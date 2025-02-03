// <copyright file="AttributeSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.CodeDom.Compiler;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="AttributeSyntax"/>.
/// </summary>
[DebuggerNonUserCode]
internal sealed class AttributeSyntaxAssertions
    : ObjectAssertions<AttributeSyntax, AttributeSyntaxAssertions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    internal AttributeSyntaxAssertions(AttributeSyntax value)
        : base(value, FluentAssertions.Execution.AssertionChain.GetOrCreate())
    {
    }

    /// <summary>
    /// Assert that the attribute is a <see cref="GeneratedCodeAttribute"/>.
    /// </summary>
    /// <param name="tool">The name of the tool.</param>
    /// <param name="version">The version of the tool.</param>
    /// <returns>The assertions.</returns>
    public AttributeSyntaxAssertions BeGeneratedCodeAttribute(string tool, string version)
    {
        this.Subject.Name.ToString().Should().Be(typeof(GeneratedCodeAttribute).FullName);
        this.Subject.ArgumentList.Should().NotBeNull();
        this.Subject.ArgumentList!.Arguments.Should().HaveCount(2);
        this.Subject.ArgumentList.Arguments.First().GetText().ToString().Should().Be(tool);
        this.Subject.ArgumentList.Arguments.Last().GetText().ToString().Should().Be(version);
        return this;
    }
}