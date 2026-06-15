// <copyright file="ExtensionMethodMapperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Test class for the testing it is possible
/// generating extension method mappers.
/// </summary>
public class ExtensionMethodMapperTests
{
    /// <summary>
    /// Test the mapping from <see cref="int"/> to <see cref="long"/>
    /// using an extension method.
    /// </summary>
    [Fact]
    public void CanMapFromIntToLongUsingAnExtensionMethod()
    {
        // Arrange
        const int input = 123;

        // Act
        var actual = input.MapToLong();

        // Assert
        actual.Should().Be(input);
    }
}