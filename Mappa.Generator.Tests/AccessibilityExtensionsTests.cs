// <copyright file="AccessibilityExtensionsTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests for <see cref="AccessibilityExtensions"/>.
/// </summary>
public sealed class AccessibilityExtensionsTests
{
    /// <summary>
    /// Test <see cref="AccessibilityExtensions.GetAccessibilityAsCode"/>.
    /// </summary>
    /// <param name="accessibility">The input accessibility to test.</param>
    /// <param name="expected">The expected value.</param>
    [Theory]
    [InlineData(Accessibility.Internal, "internal")]
    [InlineData(Accessibility.Private, "private")]
    [InlineData(Accessibility.Protected, "protected")]
    [InlineData(Accessibility.ProtectedAndInternal, "protected internal")]
    [InlineData(Accessibility.Public, "public")]
    [InlineData(Accessibility.NotApplicable, "")]
    public void TestMapping(Accessibility accessibility, string expected)
    {
        // Act
        var actual = accessibility.GetAccessibilityAsCode();

        // Assert
        actual.Should().Be(expected);
    }
}