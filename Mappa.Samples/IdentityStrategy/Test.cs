// <copyright file="Test.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.IdentityStrategy;

/// <summary>
/// Test class for the identity strategy.
/// </summary>
public sealed class Test
{
    private readonly Mapper mapper = new();

    /// <summary>
    /// Test the method <see cref="Mapper.MapStringToObjectWithNullableDisabled"/>.
    /// </summary>
    /// <param name="input">The input string to be mapped.</param>
    [Theory]
    [UnitTest]
    [InlineData("Test string")]
    public void CanMapFromStringToObject(string input)
    {
        // Act
        var output = this.mapper.MapStringToObjectWithNullableDisabled(input);

        // Assert
        output.Should().Be(input);
    }
}