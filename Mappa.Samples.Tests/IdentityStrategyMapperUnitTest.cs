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
    private readonly IdentityStrategyMapperDup identityStrategyMapperDup = new();

    /// <summary>
    /// Test the method <see cref="IdentityStrategyMapper.MapIntToIntWhenNullableIsDisabled"/>.
    /// </summary>
    /// <param name="input">The input integer to be mapped.</param>
    [Theory]
    [UnitTest]
    [InlineData(17)]
    public void CanMapIntToIntWhenNullableIsDisabled(int input)
    {
        // Act
        var output = this.identityStrategyMapper.MapIntToIntWhenNullableIsDisabled(input);

        // Assert
        output.Should().Be(input);
    }

    /// <summary>
    /// Test the method <see cref="IdentityStrategyMapper.MapIntToNullableIntWhenNullableIsDisabled"/>.
    /// </summary>
    /// <param name="input">The input integer to be mapped.</param>
    [Theory]
    [UnitTest]
    [InlineData(17)]
    public void CanMapIntToNullableIntWhenNullableIsDisabled(int input)
    {
        // Act
        var output = this.identityStrategyMapper.MapIntToNullableIntWhenNullableIsDisabled(input);

        // Assert
        output.Should().Be(input);
    }

    /// <summary>
    /// Test the method <see cref="IdentityStrategyMapperDup.MapIntToNullableIntWhenNullableIsEnabled"/>.
    /// </summary>
    /// <param name="input">The input integer to be mapped.</param>
    [Theory]
    [UnitTest]
    [InlineData(17)]
    public void CanMapIntToNullableIntWhenNullableIsEnabled(int input)
    {
        // Act
        var output = this.identityStrategyMapperDup.MapIntToNullableIntWhenNullableIsEnabled(input);

        // Assert
        output.Should().Be(input);
    }

    /// <summary>
    /// Test the method <see cref="IdentityStrategyMapper.MapIntToObjectWhenNullableIsDisabled"/>.
    /// </summary>
    /// <param name="input">The input integer to be mapped.</param>
    [Theory]
    [UnitTest]
    [InlineData(17)]
    public void CanMapIntToObjectWhenNullableIsDisabled(int input)
    {
        // Act
        var output = this.identityStrategyMapper.MapIntToObjectWhenNullableIsDisabled(input);

        // Assert
        output.Should().Be(input);
    }

    /// <summary>
    /// Test the method <see cref="IdentityStrategyMapper.MapIntToNullableObjectWhenNullableIsEnabled"/>.
    /// </summary>
    /// <param name="input">The input integer to be mapped.</param>
    [Theory]
    [UnitTest]
    [InlineData(17)]
    public void CanMapIntToNullableObjectWhenNullableIsEnabled(int input)
    {
        // Act
        var output = this.identityStrategyMapper.MapIntToNullableObjectWhenNullableIsEnabled(input);

        // Assert
        output.Should().Be(input);
    }

    /// <summary>
    /// Test the method <see cref="IdentityStrategyMapper.MapStringToObjectWhenNullableIsDisabled"/>.
    /// </summary>
    /// <param name="input">The input string to be mapped.</param>
    [Theory]
    [UnitTest]
    [InlineData("Test string")]
    public void CanMapStringToObjectWithNullableDisabled(string input)
    {
        // Act
        var output = this.identityStrategyMapper.MapStringToObjectWhenNullableIsDisabled(input);

        // Assert
        output.Should().Be(input);
    }

    /// <summary>
    /// Test the method <see cref="IdentityStrategyMapperDup.MapStringToObjectWhenNullableIsEnabled"/>.
    /// </summary>
    /// <param name="input">The input string to be mapped.</param>
    [Theory]
    [UnitTest]
    [InlineData("Test string")]
    public void CanMapStringToObjectWhenNullableIsEnabled(string input)
    {
        // Act
        var output = this.identityStrategyMapperDup.MapStringToObjectWhenNullableIsEnabled(input);

        // Assert
        output.Should().Be(input);
    }
}