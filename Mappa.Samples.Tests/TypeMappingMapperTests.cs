// <copyright file="TypeMappingMapperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for <see cref="Samples.TypeMappingMapper"/> mappers.
/// </summary>
public sealed class TypeMappingMapperTests
{
    private static readonly TypeMappingMapper TypeMappingMapper = new();

    /// <summary>
    /// Tests mapping via <see cref="TypeMappingMapper.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UseTypeMappingMapperAndMapFromFirstToFirst()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceFirstClass
        {
            NumericProperty = 17,
            DateTimeProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = TypeMappingMapper.Map(source);

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
    /// Tests mapping via <see cref="TypeMappingMapper.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UseTypeMappingMapperAndMapFromSecondToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceSecondClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
        };

        // Act.
        var target = TypeMappingMapper.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetSecondClass>();
        var typedTarget = (Models.Polymorphism.One.TargetSecondClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
    }

    /// <summary>
    /// Tests mapping via <see cref="TypeMappingMapper.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UseTypeMappingMapperAndMapFromThirdToThird()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceThirdClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
            Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9"],
        };

        // Act.
        var target = TypeMappingMapper.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetThirdClass>();
        var typedTarget = (Models.Polymorphism.One.TargetThirdClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
        typedTarget.Numbers.Should().BeEquivalentTo(source.Numbers.Select(long.Parse));
    }

    /// <summary>
    /// Tests mapping via <see cref="TypeMappingMapper.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// and the method will throw.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UseTypeMappingMapperAndMapFromBaseSoItWillThrow()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceBaseClass()
        {
            NumericProperty = 17,
        };

        // Act.
        var action = () => TypeMappingMapper.Map(source);

        // Assert.
        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}