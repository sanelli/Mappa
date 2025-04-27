// <copyright file="MapWithPropertiesOnBaseClassesMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="MapWithPropertiesOnBaseClassesMapper"/>.
/// </summary>
public sealed class MapWithPropertiesOnBaseClassesMapperUnitTests
{
    private readonly MapWithPropertiesOnBaseClassesMapper mapper = new();

    /// <summary>
    /// Test for <see cref="MapWithPropertiesOnBaseClassesMapper.MapToClassWithProperties"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapToClassWithProperties()
    {
        // Arrange
        var source = new DerivedClassSourceModel
        {
            BooleanProperty = true,
            ByteProperty = 17,
            CharProperty = 'C',
            StringProperty = "hello",
            IntegerProperty = 123,
            LongProperty = long.MaxValue,
        };

        // Act
        var target = this.mapper.MapToClassWithProperties(source);

        // Assert
        target.BooleanProperty.Should().Be(source.BooleanProperty);
        target.ByteProperty.Should().Be(source.ByteProperty);
        target.CharProperty.Should().Be(source.CharProperty);
        target.StringProperty.Should().Be(source.StringProperty);
        target.IntegerProperty.Should().Be(source.IntegerProperty);
        target.LongProperty.Should().Be(source.LongProperty);
    }

    /// <summary>
    /// Test for <see cref="MapWithPropertiesOnBaseClassesMapper.MapToClassWithConstructor"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapToClassWithConstructor()
    {
        // Arrange
        var source = new DerivedClassSourceModel
        {
            BooleanProperty = true,
            ByteProperty = 17,
            CharProperty = 'C',
            StringProperty = "hello",
            IntegerProperty = 123,
            LongProperty = long.MaxValue,
        };

        // Act
        var target = this.mapper.MapToClassWithConstructor(source);

        // Assert
        target.BooleanProperty.Should().Be(source.BooleanProperty);
        target.ByteProperty.Should().Be(source.ByteProperty);
        target.CharProperty.Should().Be(source.CharProperty);
        target.StringProperty.Should().Be(source.StringProperty);
        target.IntegerProperty.Should().Be(source.IntegerProperty);
        target.LongProperty.Should().Be(source.LongProperty);
    }
}