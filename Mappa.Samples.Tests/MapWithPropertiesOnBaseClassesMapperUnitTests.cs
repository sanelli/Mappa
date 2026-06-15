// <copyright file="MapWithPropertiesOnBaseClassesMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

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

    /// <summary>
    /// Test for <see cref="MapWithPropertiesOnBaseClassesMapper.MapFromInterface"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapFromInterface()
    {
        // Arrange
        var source = new DerivedInterfaceModelImpl
        {
            LongProperty = 124,
            DoubleProperty = 12.34,
        };

        // Act
        var target = this.mapper.MapFromInterface(source);

        // Assert
        target.LongProperty.Should().Be(source.LongProperty);
        target.DoubleProperty.Should().Be(source.DoubleProperty);
    }

    private sealed class DerivedInterfaceModelImpl
        : IDerivedInterfaceModel
    {
        public long LongProperty { get; set; }

        public double DoubleProperty { get; set; }
    }
}