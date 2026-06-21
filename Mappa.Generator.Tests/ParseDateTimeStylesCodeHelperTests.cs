// <copyright file="ParseDateTimeStylesCodeHelperTests.cs" company="Stefano Anelli">
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
/// Unit tests for <see cref="ParseDateTimeStylesCodeHelper"/>.
/// </summary>
public sealed class ParseDateTimeStylesCodeHelperTests
{
    /// <summary>
    /// Test <see cref="ParseDateTimeStylesCodeHelper.TryParseFromString"/> returns <c>null</c> for null or whitespace.
    /// </summary>
    /// <param name="value">The input value.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [UnitTest]
    public void TryParseFromStringReturnsNullForNullOrWhitespace(string? value)
    {
        ParseDateTimeStylesCodeHelper.TryParseFromString(value).Should().BeNull();
    }

    /// <summary>
    /// Test <see cref="ParseDateTimeStylesCodeHelper.TryParseFromString"/> returns <c>null</c> for invalid tokens.
    /// </summary>
    /// <param name="value">The input value.</param>
    [Theory]
    [InlineData("NotAValidDateTimeStyle")]
    [InlineData("AllowWhiteSpaces|NotValid")]
    [UnitTest]
    public void TryParseFromStringReturnsNullForInvalidTokens(string value)
    {
        ParseDateTimeStylesCodeHelper.TryParseFromString(value).Should().BeNull();
    }

    /// <summary>
    /// Test <see cref="ParseDateTimeStylesCodeHelper.TryParseFromString"/> parses combined flags.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryParseFromStringParsesCombinedFlags()
    {
        var result = ParseDateTimeStylesCodeHelper.TryParseFromString("AllowWhiteSpaces | AssumeUniversal");

        result.Should().Be(DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal);
    }

    /// <summary>
    /// Test <see cref="ParseDateTimeStylesCodeHelper.BuildParseInvocation"/> emits a composite style expression.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildParseInvocationEmitsCompositeStyleExpression()
    {
        var (parseMethod, parameters) = ParseDateTimeStylesCodeHelper.BuildParseInvocation(
            "input",
            format: null,
            CultureInfoSetting.None,
            cultureName: null,
            DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal);

        parseMethod.Should().Be("Parse");
        parameters.Should().Be(
            "input, null, System.Globalization.DateTimeStyles.AllowWhiteSpaces | System.Globalization.DateTimeStyles.AssumeUniversal");
    }

    /// <summary>
    /// Test <see cref="ParseDateTimeStylesCodeHelper.BuildDateTimeOrDateTimeOffsetParseInvocation"/> emits
    /// <c>ParseExact</c> with culture, format, and composite style.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildDateTimeOrDateTimeOffsetParseInvocationEmitsParseExactWithCultureFormatAndStyle()
    {
        var (parseMethod, parameters) = ParseDateTimeStylesCodeHelper.BuildDateTimeOrDateTimeOffsetParseInvocation(
            "input",
            format: "d",
            CultureInfoSetting.InvariantCulture,
            cultureName: null,
            DateTimeStyles.AdjustToUniversal);

        parseMethod.Should().Be("ParseExact");
        parameters.Should().Be(
            "input, \"d\", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal");
    }

    /// <summary>
    /// Test <see cref="ParseDateTimeStylesCodeHelper.BuildParseInvocation"/> throws when user-defined culture
    /// is selected without a culture name while a date/time style is configured.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildParseInvocationThrowsWhenUserDefinedCultureIsMissingCultureNameAndStyleIsConfigured()
    {
        var act = () => ParseDateTimeStylesCodeHelper.BuildParseInvocation(
            "input",
            format: null,
            CultureInfoSetting.UserDefined,
            cultureName: "   ",
            DateTimeStyles.AllowWhiteSpaces);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Unexpected scenario when building GeyCultureInfo without culture name");
    }

    /// <summary>
    /// Test <see cref="ParseDateTimeStylesCodeHelper.BuildParseInvocation"/> throws for unexpected culture settings.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildParseInvocationThrowsForUnexpectedCultureSetting()
    {
        var act = () => ParseDateTimeStylesCodeHelper.BuildParseInvocation(
            "input",
            format: null,
            (CultureInfoSetting)999,
            cultureName: null,
            DateTimeStyles.AllowWhiteSpaces);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Unexpected culture info setting '999'.");
    }
}