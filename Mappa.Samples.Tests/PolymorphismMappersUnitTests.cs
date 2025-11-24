// <copyright file="PolymorphismMappersUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for <see cref="PolymorphismMapper"/> mappers.
/// </summary>
public sealed class PolymorphismMappersUnitTests
{
    private static readonly PolymorphismMapper PolymorphismMapper = new();
    private static readonly PolymorphismMapperNullable PolymorphismMapperNullable = new();
    private static readonly PolymorphismMapperBetweenInterfaces PolymorphismMapperBetweenInterfaces = new();

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapper.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperAndMapFromFirstToFirst()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceFirstClass
        {
            NumericProperty = 17,
            DateTimeProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapper.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetFirstClass>();
        var typedTarget = (Models.Polymorphism.One.TargetFirstClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
#pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString());
#pragma warning restore CA1305
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapper.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperAndMapFromSecondToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceSecondClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
        };

        // Act.
        var target = PolymorphismMapper.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetSecondClass>();
        var typedTarget = (Models.Polymorphism.One.TargetSecondClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapper.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperAndMapFromThirdToThird()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceThirdClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
            Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9"],
        };

        // Act.
        var target = PolymorphismMapper.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetThirdClass>();
        var typedTarget = (Models.Polymorphism.One.TargetThirdClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
        typedTarget.Numbers.Should().BeEquivalentTo(source.Numbers.Select(long.Parse));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapper.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// and the method will throw.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperAndMapFromBaseSoItWillThrow()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceBaseClass()
        {
            NumericProperty = 17,
        };

        // Act.
        var action = () => PolymorphismMapper.Map(source);

        // Assert.
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperNullable.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperNullableAndMapFromFirstToFirst()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceFirstClass
        {
            NumericProperty = 17,
            DateTimeProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapperNullable.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetFirstClass>();
        var typedTarget = (Models.Polymorphism.One.TargetFirstClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
#pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString());
#pragma warning restore CA1305
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperNullable.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperNullableAndMapFromSecondToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceSecondClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
        };

        // Act.
        var target = PolymorphismMapperNullable.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetSecondClass>();
        var typedTarget = (Models.Polymorphism.One.TargetSecondClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperNullable.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperNullableAndMapFromThirdToThird()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceThirdClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
            Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9"],
        };

        // Act.
        var target = PolymorphismMapperNullable.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetThirdClass>();
        var typedTarget = (Models.Polymorphism.One.TargetThirdClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
        typedTarget.Numbers.Should().BeEquivalentTo(source.Numbers.Select(long.Parse));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperNullable.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// and the method will throw.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperNullableAndMapFromBaseSoItWillThrow()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceBaseClass()
        {
            NumericProperty = 17,
        };

        // Act.
        var action = () => PolymorphismMapperNullable.Map(source);

        // Assert.
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperNullable.Map"/>
    /// from <c>null</c> and the method will return <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperNullableAndMapFromNullWillReturnNull()
    {
        // Arrange.
        Models.Polymorphism.One.SourceBaseClass? source = null;

        // Act.
        var target = PolymorphismMapperNullable.Map(source);

        // Assert.
        target.Should().BeNull();
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperBetweenInterfaces.Map"/>
    /// from <see cref="Models.Polymorphism.Two.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.Two.TargetFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperBetweenInterfacesAndMapFromFirstToFirst()
    {
        // Arrange.
        var source = new Models.Polymorphism.Two.SourceFirstClass
        {
            NumericProperty = 17,
            DateTimeProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapperBetweenInterfaces.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.Two.TargetFirstClass>();
        var typedTarget = (Models.Polymorphism.Two.TargetFirstClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
#pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString());
#pragma warning restore CA1305
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperBetweenInterfaces.Map"/>
    /// from <see cref="Models.Polymorphism.Two.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.Two.TargetSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperBetweenInterfacesAndMapFromSecondToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.Two.SourceSecondClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
        };

        // Act.
        var target = PolymorphismMapperBetweenInterfaces.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.Two.TargetSecondClass>();
        var typedTarget = (Models.Polymorphism.Two.TargetSecondClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperBetweenInterfaces.Map"/>
    /// from <see cref="Models.Polymorphism.Two.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.Two.TargetThirdClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperBetweenInterfacesAndMapFromThirdToThird()
    {
        // Arrange.
        var source = new Models.Polymorphism.Two.SourceThirdClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
            Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9"],
        };

        // Act.
        var target = PolymorphismMapperBetweenInterfaces.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.Two.TargetThirdClass>();
        var typedTarget = (Models.Polymorphism.Two.TargetThirdClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
        typedTarget.Numbers.Should().BeEquivalentTo(source.Numbers.Select(long.Parse));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperBetweenInterfaces.Map"/>
    /// from <see cref="Models.Polymorphism.Two.SourceUnmappedClass"/>
    /// and the method will throw.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperBetweenInterfacesAndMapFromBaseSoItWillThrow()
    {
        // Arrange.
        var source = new Models.Polymorphism.Two.SourceUnmappedClass
        {
            UnmappedProperty = "Ciao",
            NumericProperty = 17,
        };

        // Act.
        var action = () => PolymorphismMapperBetweenInterfaces.Map(source);

        // Assert.
        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}