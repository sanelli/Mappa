// <copyright file="StyleEnumCodeHelperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Attributes;
using Mappa.Generator.Helpers;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="StyleEnumCodeHelper"/>.
/// </summary>
public sealed class StyleEnumCodeHelperTests
{
    /// <summary>
    /// Test <see cref="StyleEnumCodeHelper.IsValidDateTimeStyle"/> accepts <c>None</c> and single flags.
    /// </summary>
    /// <param name="styles">The style value.</param>
    [Theory]
    [InlineData(DateTimeStyles.None)]
    [InlineData(DateTimeStyles.AllowWhiteSpaces)]
    [InlineData(DateTimeStyles.AssumeUniversal)]
    [UnitTest]
    public void IsValidDateTimeStyleReturnsTrueForKnownValues(DateTimeStyles styles)
    {
        StyleEnumCodeHelper.IsValidDateTimeStyle(styles).Should().BeTrue();
    }

    /// <summary>
    /// Test <see cref="StyleEnumCodeHelper.IsValidDateTimeStyle"/> accepts combined flags.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsValidDateTimeStyleReturnsTrueForCombinedFlags()
    {
        var styles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal;

        StyleEnumCodeHelper.IsValidDateTimeStyle(styles).Should().BeTrue();
    }

    /// <summary>
    /// Test <see cref="StyleEnumCodeHelper.IsValidDateTimeStyle"/> accepts the undefined sentinel.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsValidDateTimeStyleReturnsTrueForUndefinedSentinel()
    {
        StyleEnumCodeHelper.IsValidDateTimeStyle(MappaSettingsAttribute.UndefinedDateTimeStyle).Should().BeTrue();
    }

    /// <summary>
    /// Test <see cref="StyleEnumCodeHelper.IsValidDateTimeStyle"/> rejects arbitrary integer casts.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsValidDateTimeStyleReturnsFalseForArbitraryIntegerCast()
    {
        StyleEnumCodeHelper.IsValidDateTimeStyle((DateTimeStyles)999).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="StyleEnumCodeHelper.IsValidDateTimeStyle"/> rejects known flags combined with orphan bits.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsValidDateTimeStyleReturnsFalseForKnownFlagsWithOrphanBits()
    {
        var styles = DateTimeStyles.AllowWhiteSpaces | (DateTimeStyles)1048576;

        StyleEnumCodeHelper.IsValidDateTimeStyle(styles).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="StyleEnumCodeHelper.IsValidNumberStyle"/> accepts <c>None</c> and single flags.
    /// </summary>
    /// <param name="styles">The style value.</param>
    [Theory]
    [InlineData(NumberStyles.None)]
    [InlineData(NumberStyles.AllowThousands)]
    [InlineData(NumberStyles.AllowParentheses)]
    [UnitTest]
    public void IsValidNumberStyleReturnsTrueForKnownValues(NumberStyles styles)
    {
        StyleEnumCodeHelper.IsValidNumberStyle(styles).Should().BeTrue();
    }

    /// <summary>
    /// Test <see cref="StyleEnumCodeHelper.IsValidNumberStyle"/> accepts combined flags.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsValidNumberStyleReturnsTrueForCombinedFlags()
    {
        var styles = NumberStyles.AllowThousands | NumberStyles.AllowParentheses;

        StyleEnumCodeHelper.IsValidNumberStyle(styles).Should().BeTrue();
    }

    /// <summary>
    /// Test <see cref="StyleEnumCodeHelper.IsValidNumberStyle"/> accepts the undefined sentinel.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsValidNumberStyleReturnsTrueForUndefinedSentinel()
    {
        StyleEnumCodeHelper.IsValidNumberStyle(MappaSettingsAttribute.UndefinedNumberStyle).Should().BeTrue();
    }

    /// <summary>
    /// Test <see cref="StyleEnumCodeHelper.IsValidNumberStyle"/> rejects arbitrary integer casts.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsValidNumberStyleReturnsFalseForArbitraryIntegerCast()
    {
        StyleEnumCodeHelper.IsValidNumberStyle((NumberStyles)1048576).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="StyleEnumCodeHelper.IsValidNumberStyle"/> rejects known flags combined with orphan bits.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsValidNumberStyleReturnsFalseForKnownFlagsWithOrphanBits()
    {
        var styles = NumberStyles.AllowThousands | (NumberStyles)1048576;

        StyleEnumCodeHelper.IsValidNumberStyle(styles).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="StyleEnumCodeHelper.GetDateTimeStyleExpression"/> emits a composite expression for valid combined flags.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetDateTimeStyleExpressionEmitsCompositeExpressionForValidCombinedFlags()
    {
        var expression = StyleEnumCodeHelper.GetDateTimeStyleExpression(
            DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal);

        expression.Should().Be(
            "System.Globalization.DateTimeStyles.AllowWhiteSpaces | System.Globalization.DateTimeStyles.AssumeUniversal");
    }

    /// <summary>
    /// Test <see cref="StyleEnumCodeHelper.GetNumberStyleExpression"/> emits <c>None</c> when no known flags decompose.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetNumberStyleExpressionEmitsNoneForArbitraryIntegerCast()
    {
        StyleEnumCodeHelper.GetNumberStyleExpression((NumberStyles)1048576)
            .Should().Be("System.Globalization.NumberStyles.None");
    }
}