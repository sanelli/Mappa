// <copyright file="IdentityStrategyMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Test class for the identity strategy.
/// </summary>
public sealed class IdentityStrategyMapperUnitTest
{
    private readonly IdentityStrategyMapper identityStrategyMapper = new();

    /// <summary>
    /// Test the method <see cref="IdentityStrategyMapper.MapStringToObjectWithNullableDisabled"/>.
    /// </summary>
    /// <param name="input">The input string to be mapped.</param>
    [Theory]
    [UnitTest]
    [InlineData("Test string")]
    public void CanMapFromStringToObject(string input)
    {
        // Act
        var output = this.identityStrategyMapper.MapStringToObjectWithNullableDisabled(input);

        // Assert
        output.Should().Be(input);
    }
}