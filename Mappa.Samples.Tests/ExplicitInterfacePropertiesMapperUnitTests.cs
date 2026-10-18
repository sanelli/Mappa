// <copyright file="ExplicitInterfacePropertiesMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="ExplicitInterfacePropertiesMapper"/>.
/// </summary>
public sealed class ExplicitInterfacePropertiesMapperUnitTests
{
    private readonly ExplicitInterfacePropertiesMapper mapper = new();

    /// <summary>
    /// Tests <see cref="ExplicitInterfacePropertiesMapper.MapFromExplicitSource"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromExplicitSource()
    {
        // Arrange
        var source = new ExplicitInterfaceSourceModel { Id = 1 };
        ((IHasExplicitValue)source).Value = 42;

        // Act
        var actual = this.mapper.MapFromExplicitSource(source);

        // Assert
        actual.Id.Should().Be(1);
        actual.Value.Should().Be(42);
    }

    /// <summary>
    /// Tests <see cref="ExplicitInterfacePropertiesMapper.MapToExplicitTarget"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToExplicitTarget()
    {
        // Arrange
        var source = new ExplicitInterfacePublicSourceModel
        {
            Id = 1,
            Value = 42,
        };

        // Act
        var actual = this.mapper.MapToExplicitTarget(source);

        // Assert
        actual.Id.Should().Be(1);
        ((IHasExplicitValue)actual).Value.Should().Be(42);
    }

    /// <summary>
    /// Tests <see cref="ExplicitInterfacePropertiesMapper.MapParameterizedWithLeftoverExplicitTarget"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapParameterizedWithLeftoverExplicitTarget()
    {
        // Arrange
        var source = new ExplicitInterfacePublicSourceModel
        {
            Id = 7,
            Value = 99,
        };

        // Act
        var actual = this.mapper.MapParameterizedWithLeftoverExplicitTarget(source);

        // Assert
        actual.Id.Should().Be(7);
        ((IHasExplicitValue)actual).Value.Should().Be(99);
    }
}