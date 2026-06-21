// <copyright file="ParseNumberStylesCodeHelperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Helpers;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="ParseNumberStylesCodeHelper"/>.
/// </summary>
public sealed class ParseNumberStylesCodeHelperTests
{
    /// <summary>
    /// Test <see cref="ParseNumberStylesCodeHelper.TryParseFromString"/> returns <c>null</c> for null or whitespace.
    /// </summary>
    /// <param name="value">The input value.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [UnitTest]
    public void TryParseFromStringReturnsNullForNullOrWhitespace(string? value)
    {
        ParseNumberStylesCodeHelper.TryParseFromString(value).Should().BeNull();
    }

    /// <summary>
    /// Test <see cref="ParseNumberStylesCodeHelper.TryParseFromString"/> returns <c>null</c> for invalid tokens.
    /// </summary>
    /// <param name="value">The input value.</param>
    [Theory]
    [InlineData("NotAValidNumberStyle")]
    [InlineData("AllowThousands|NotValid")]
    [UnitTest]
    public void TryParseFromStringReturnsNullForInvalidTokens(string value)
    {
        ParseNumberStylesCodeHelper.TryParseFromString(value).Should().BeNull();
    }

    /// <summary>
    /// Test <see cref="ParseNumberStylesCodeHelper.TryParseFromString"/> parses combined flags.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryParseFromStringParsesCombinedFlags()
    {
        var result = ParseNumberStylesCodeHelper.TryParseFromString("AllowThousands | AllowParentheses");

        result.Should().Be(NumberStyles.AllowThousands | NumberStyles.AllowParentheses);
    }

    /// <summary>
    /// Test <see cref="ParseNumberStylesCodeHelper.TryParseFromString"/> ignores empty tokens between separators.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryParseFromStringIgnoresEmptyTokensBetweenSeparators()
    {
        var result = ParseNumberStylesCodeHelper.TryParseFromString("AllowThousands,,| ,AllowParentheses");

        result.Should().Be(NumberStyles.AllowThousands | NumberStyles.AllowParentheses);
    }

    /// <summary>
    /// Test <see cref="ParseNumberStylesCodeHelper.BuildParseInvocation"/> emits a composite style expression.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildParseInvocationEmitsCompositeStyleExpression()
    {
        var parameters = ParseNumberStylesCodeHelper.BuildParseInvocation(
            "input",
            CultureInfoSetting.None,
            cultureName: null,
            NumberStyles.AllowThousands | NumberStyles.AllowParentheses);

        parameters.Should().Be(
            "input, System.Globalization.NumberStyles.AllowParentheses | System.Globalization.NumberStyles.AllowThousands");
    }

    /// <summary>
    /// Test <see cref="ParseNumberStylesCodeHelper.BuildParseInvocation"/> throws when user-defined culture
    /// is selected without a culture name while a number style is configured.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildParseInvocationThrowsWhenUserDefinedCultureIsMissingCultureNameAndStyleIsConfigured()
    {
        var act = () => ParseNumberStylesCodeHelper.BuildParseInvocation(
            "input",
            CultureInfoSetting.UserDefined,
            cultureName: null,
            NumberStyles.AllowThousands);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Unexpected scenario when building GeyCultureInfo without culture name");
    }

    /// <summary>
    /// Test <see cref="ParseNumberStylesCodeHelper.BuildParseInvocation"/> throws for unexpected culture settings.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildParseInvocationThrowsForUnexpectedCultureSetting()
    {
        var act = () => ParseNumberStylesCodeHelper.BuildParseInvocation(
            "input",
            (CultureInfoSetting)999,
            cultureName: null,
            NumberStyles.AllowThousands);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Unexpected culture info setting '999'.");
    }
}