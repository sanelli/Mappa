// <copyright file="StringToDateTimeMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using System.Globalization;

using FluentAssertions;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="StringToDateTimeMapper"/>.
/// </summary>
public sealed class StringToDateTimeMapperUnitTest
{
    private readonly StringToDateTimeMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="StringToDateTimeMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringDateTime()
    {
        // Arrange
        var expected = DateTime.UtcNow;
        var input = expected.ToString("yyyy-MM-dd HH:mm:ss.ffffff", DateTimeFormatInfo.CurrentInfo);

        // Act
        var actual = this.mapper.Map(input);

        // Assert
        actual.Should().Be(expected);
    }
}