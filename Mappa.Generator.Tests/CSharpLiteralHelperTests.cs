// <copyright file="CSharpLiteralHelperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Helpers;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="CSharpLiteralHelper"/>.
/// </summary>
public sealed class CSharpLiteralHelperTests
{
    /// <summary>
    /// Test <see cref="CSharpLiteralHelper.ToRequiredStringLiteral"/> throws for null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ToRequiredStringLiteralThrowsForNull()
    {
        var act = () => CSharpLiteralHelper.ToRequiredStringLiteral(null);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Cannot emit a string literal for a null value.");
    }

    /// <summary>
    /// Test <see cref="CSharpLiteralHelper.ToRequiredStringLiteral"/> throws for whitespace.
    /// </summary>
    /// <param name="value">The input value.</param>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [UnitTest]
    public void ToRequiredStringLiteralThrowsForWhitespace(string value)
    {
        var act = () => CSharpLiteralHelper.ToRequiredStringLiteral(value);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Cannot emit a string literal for a whitespace value.");
    }

    /// <summary>
    /// Test <see cref="CSharpLiteralHelper.ToStringLiteral"/> escapes special characters.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <param name="expectedLiteral">The expected C# literal.</param>
    [Theory]
    [InlineData("plain", "\"plain\"")]
    [InlineData("say \"hi\"", "\"say \\\"hi\\\"\"")]
    [InlineData("line\nbreak", "\"line\\nbreak\"")]
    [InlineData("tab\there", "\"tab\\there\"")]
    [UnitTest]
    public void ToStringLiteralEscapesSpecialCharacters(string value, string expectedLiteral)
    {
        CSharpLiteralHelper.ToStringLiteral(value).Should().Be(expectedLiteral);
    }

    /// <summary>
    /// Test <see cref="CSharpLiteralHelper.ToRequiredStringLiteral"/> returns the same escaped literal as <see cref="CSharpLiteralHelper.ToStringLiteral"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ToRequiredStringLiteralReturnsEscapedLiteral()
    {
        const string value = "culture-name";

        CSharpLiteralHelper.ToRequiredStringLiteral(value).Should().Be(CSharpLiteralHelper.ToStringLiteral(value));
    }

    /// <summary>
    /// Test <see cref="CSharpLiteralHelper.ToCharLiteral"/> emits an escaped character literal.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ToCharLiteralEscapesSpecialCharacters()
    {
        CSharpLiteralHelper.ToCharLiteral('\'').Should().Be("'\\''");
    }
}