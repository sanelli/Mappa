// <copyright file="InvokeToStringMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using FluentAssertions;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="InvokeToStringMapper"/>.
/// </summary>
public sealed class InvokeToStringMapperUnitTest
{
    private readonly InvokeToStringMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapper.Map"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(100, "100")]
    public void CanMapInvokeToString(int value, string expected)
    {
        // Act
        var actual = this.mapper.Map(value);

        // Assert
        actual.Should().Be(expected);
    }
}