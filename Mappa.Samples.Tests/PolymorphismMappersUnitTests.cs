// <copyright file="PolymorphismMappersUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

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
    private static readonly PolymorphismMapperOverridingIdentityMapper PolymorphismMapperOverridingIdentityMapper = new();
    private static readonly PolymorphismMapperOverridingIdentityMapperWithNullable PolymorphismMapperOverridingIdentityMapperWithNullable = new();
    private static readonly PolymorphismMapperWithThrowDefaultBehaviour PolymorphismMapperWithThrowDefaultBehaviour = new();
    private static readonly PolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviour PolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviour = new();
    private static readonly PolymorphismMapperWithMapDefaultWithoutExplicitType PolymorphismMapperWithMapDefaultWithoutExplicitType = new();
    private static readonly PolymorphismMapperWithMapDefaultWithExplicitType PolymorphismMapperWithMapDefaultWithExplicitType = new();
    private static readonly PolymorphismMapperWithDefaultNull PolymorphismMapperWithDefaultNull = new();
    private static readonly PolymorphismMapperWithDefaultDefault PolymorphismMapperWithDefaultDefault = new();
    private static readonly PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapper PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapper = new();
    private static readonly PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParameters PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParameters = new();
    private static readonly PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContext PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContext = new();
    private static readonly PolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClass PolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClass = new();
    private static readonly PolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClass PolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClass = new();
    private static readonly PolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapper PolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapper = new();

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
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString(CultureInfo.InvariantCulture));
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
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString(CultureInfo.InvariantCulture));
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
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString(CultureInfo.InvariantCulture));
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

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperOverridingIdentityMapper.Map"/>
    /// from <see cref="Models.Polymorphism.Three.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.Three.SourceSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperOverridingIdentityMapperAndMapFromFirstToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.Three.SourceFirstClass
        {
            BaseProperty = 17,
            DerivedProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapperOverridingIdentityMapper.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.Three.SourceSecondClass>();
        var typedTarget = (Models.Polymorphism.Three.SourceSecondClass)target;
        typedTarget.BaseProperty.Should().Be(source.BaseProperty);
        typedTarget.DerivedProperty.Should().Be(source.DerivedProperty.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperOverridingIdentityMapper.Map"/>
    /// from <see cref="Models.Polymorphism.Three.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.Three.SourceFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperOverridingIdentityMapperAndMapFromSecondToFirst()
    {
        // Arrange.
        var datetime = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var source = new Models.Polymorphism.Three.SourceSecondClass
        {
            BaseProperty = 17,
            DerivedProperty = datetime.ToString(CultureInfo.InvariantCulture),
        };

        // Act.
        var target = PolymorphismMapperOverridingIdentityMapper.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.Three.SourceFirstClass>();
        var typedTarget = (Models.Polymorphism.Three.SourceFirstClass)target;
        typedTarget.BaseProperty.Should().Be(source.BaseProperty);
        typedTarget.DerivedProperty.Should().Be(datetime);
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperOverridingIdentityMapper.Map"/>
    /// from <see cref="Models.Polymorphism.Three.SourceBaseClass"/>
    /// and the method will throw.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperOverridingIdentityMapperAndMapFromBaseSoItWillThrow()
    {
        // Arrange.
        var source = new Models.Polymorphism.Three.SourceBaseClass
        {
            BaseProperty = 17,
        };

        // Act.
        var action = () => PolymorphismMapperOverridingIdentityMapper.Map(source);

        // Assert.
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperOverridingIdentityMapperWithNullable.Map"/>
    /// from <see cref="Models.Polymorphism.Three.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.Three.SourceSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void PolymorphismMapperOverridingIdentityMapperWithNullableAndMapFromFirstToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.Three.SourceFirstClass
        {
            BaseProperty = 17,
            DerivedProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapperOverridingIdentityMapperWithNullable.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.Three.SourceSecondClass>();
        var typedTarget = (Models.Polymorphism.Three.SourceSecondClass)target;
        typedTarget.BaseProperty.Should().Be(source.BaseProperty);
        typedTarget.DerivedProperty.Should().Be(source.DerivedProperty.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperOverridingIdentityMapperWithNullable.Map"/>
    /// from <see cref="Models.Polymorphism.Three.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.Three.SourceFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void PolymorphismMapperOverridingIdentityMapperWithNullableAndMapFromSecondToFirst()
    {
        // Arrange.
        var datetime = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var source = new Models.Polymorphism.Three.SourceSecondClass
        {
            BaseProperty = 17,
            DerivedProperty = datetime.ToString(CultureInfo.InvariantCulture),
        };

        // Act.
        var target = PolymorphismMapperOverridingIdentityMapperWithNullable.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.Three.SourceFirstClass>();
        var typedTarget = (Models.Polymorphism.Three.SourceFirstClass)target;
        typedTarget.BaseProperty.Should().Be(source.BaseProperty);
        typedTarget.DerivedProperty.Should().Be(datetime);
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperOverridingIdentityMapperWithNullable.Map"/>
    /// from <see cref="Models.Polymorphism.Three.SourceBaseClass"/>
    /// and the method will throw.
    /// </summary>
    [Fact]
    [UnitTest]
    public void PolymorphismMapperOverridingIdentityMapperWithNullableAndMapFromBaseSoItWillThrow()
    {
        // Arrange.
        var source = new Models.Polymorphism.Three.SourceBaseClass
        {
            BaseProperty = 17,
        };

        // Act.
        var action = () => PolymorphismMapperOverridingIdentityMapperWithNullable.Map(source);

        // Assert.
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperOverridingIdentityMapperWithNullable.Map"/>
    /// from <c>null</c> and the method will return <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void PolymorphismMapperOverridingIdentityMapperWithNullableAndMapFromNullSoItWillReturnNull()
    {
        // Arrange.
        Models.Polymorphism.Three.SourceBaseClass? source = null;

        // Act.
        var target = PolymorphismMapperOverridingIdentityMapperWithNullable.Map(source);

        // Assert.
        target.Should().BeNull();
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithThrowDefaultBehaviour.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithThrowDefaultBehaviourAndMapFromFirstToFirst()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceFirstClass
        {
            NumericProperty = 17,
            DateTimeProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapperWithThrowDefaultBehaviour.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetFirstClass>();
        var typedTarget = (Models.Polymorphism.One.TargetFirstClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithThrowDefaultBehaviour.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithThrowDefaultBehaviourAndMapFromSecondToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceSecondClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
        };

        // Act.
        var target = PolymorphismMapperWithThrowDefaultBehaviour.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetSecondClass>();
        var typedTarget = (Models.Polymorphism.One.TargetSecondClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithThrowDefaultBehaviour.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithThrowDefaultBehaviourAndMapFromThirdToThird()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceThirdClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
            Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9"],
        };

        // Act.
        var target = PolymorphismMapperWithThrowDefaultBehaviour.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetThirdClass>();
        var typedTarget = (Models.Polymorphism.One.TargetThirdClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
        typedTarget.Numbers.Should().BeEquivalentTo(source.Numbers.Select(long.Parse));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithThrowDefaultBehaviour.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// and the method will throw.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithThrowDefaultBehaviourAndMapFromBaseSoItWillThrow()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceBaseClass()
        {
            NumericProperty = 17,
        };

        // Act.
        var action = () => PolymorphismMapperWithThrowDefaultBehaviour.Map(source);

        // Assert.
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviour.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviourAndMapFromFirstToFirst()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceFirstClass
        {
            NumericProperty = 17,
            DateTimeProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviour.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetFirstClass>();
        var typedTarget = (Models.Polymorphism.One.TargetFirstClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviour.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviourAndMapFromSecondToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceSecondClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
        };

        // Act.
        var target = PolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviour.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetSecondClass>();
        var typedTarget = (Models.Polymorphism.One.TargetSecondClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviour.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviourAndMapFromThirdToThird()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceThirdClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
            Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9"],
        };

        // Act.
        var target = PolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviour.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetThirdClass>();
        var typedTarget = (Models.Polymorphism.One.TargetThirdClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
        typedTarget.Numbers.Should().BeEquivalentTo(source.Numbers.Select(long.Parse));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviour.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// and the method will throw.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviourAndMapFromBaseSoItWillThrow()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceBaseClass()
        {
            NumericProperty = 17,
        };

        // Act.
        var action = () => PolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviour.Map(source);

        // Assert.
        action.Should().Throw<Models.Polymorphism.PolymorphismCustomException>();
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithMapDefaultWithoutExplicitType.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithMapDefaultWithoutExplicitTypeAndMapFromFirstToFirst()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceFirstClass
        {
            NumericProperty = 17,
            DateTimeProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapperWithMapDefaultWithoutExplicitType.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetFirstClass>();
        var typedTarget = (Models.Polymorphism.One.TargetFirstClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithMapDefaultWithoutExplicitType.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithMapDefaultWithoutExplicitTypeAndMapFromSecondToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceSecondClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
        };

        // Act.
        var target = PolymorphismMapperWithMapDefaultWithoutExplicitType.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetSecondClass>();
        var typedTarget = (Models.Polymorphism.One.TargetSecondClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithMapDefaultWithoutExplicitType.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithMapDefaultWithoutExplicitTypeAndMapFromThirdToThird()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceThirdClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
            Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9"],
        };

        // Act.
        var target = PolymorphismMapperWithMapDefaultWithoutExplicitType.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetThirdClass>();
        var typedTarget = (Models.Polymorphism.One.TargetThirdClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
        typedTarget.Numbers.Should().BeEquivalentTo(source.Numbers.Select(long.Parse));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithMapDefaultWithoutExplicitType.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// and the method return the default target.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithMapDefaultWithoutExplicitTypeAndMapFromBaseSoItWillThrow()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceBaseClass
        {
            NumericProperty = 17,
        };

        // Act.
        var target = PolymorphismMapperWithMapDefaultWithoutExplicitType.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetBaseClass>();
        target.NumericProperty.Should().Be(source.NumericProperty);
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithMapDefaultWithExplicitType.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithMapDefaultWithExplicitTypeAndMapFromFirstToFirst()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceFirstClass
        {
            NumericProperty = 17,
            DateTimeProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapperWithMapDefaultWithExplicitType.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetFirstClass>();
        var typedTarget = (Models.Polymorphism.One.TargetFirstClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithMapDefaultWithExplicitType.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithMapDefaultWithExplicitTypeAndMapFromSecondToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceSecondClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
        };

        // Act.
        var target = PolymorphismMapperWithMapDefaultWithExplicitType.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetSecondClass>();
        var typedTarget = (Models.Polymorphism.One.TargetSecondClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithMapDefaultWithExplicitType.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithMapDefaultWithExplicitTypeAndMapFromThirdToThird()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceThirdClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
            Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9"],
        };

        // Act.
        var target = PolymorphismMapperWithMapDefaultWithExplicitType.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetThirdClass>();
        var typedTarget = (Models.Polymorphism.One.TargetThirdClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
        typedTarget.Numbers.Should().BeEquivalentTo(source.Numbers.Select(long.Parse));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithMapDefaultWithExplicitType.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// and the method will throw.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithMapDefaultWithExplicitTypeAndMapFromBaseSoItWillThrow()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceBaseClass()
        {
            NumericProperty = 17,
        };

        // Act.
        var target = PolymorphismMapperWithMapDefaultWithExplicitType.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetUnmappedBaseClass>();
        var typedTarget = (Models.Polymorphism.One.TargetUnmappedBaseClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithDefaultNull.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithDefaultNullAndMapFromFirstToFirst()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceFirstClass
        {
            NumericProperty = 17,
            DateTimeProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapperWithDefaultNull.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetFirstClass>();
        var typedTarget = (Models.Polymorphism.One.TargetFirstClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithDefaultNull.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithDefaultNullAndMapFromSecondToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceSecondClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
        };

        // Act.
        var target = PolymorphismMapperWithDefaultNull.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetSecondClass>();
        var typedTarget = (Models.Polymorphism.One.TargetSecondClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithDefaultNull.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithDefaultNullAndMapFromThirdToThird()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceThirdClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
            Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9"],
        };

        // Act.
        var target = PolymorphismMapperWithDefaultNull.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetThirdClass>();
        var typedTarget = (Models.Polymorphism.One.TargetThirdClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
        typedTarget.Numbers.Should().BeEquivalentTo(source.Numbers.Select(long.Parse));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithDefaultNull.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// and the method will return <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithDefaultNullAndMapFromBaseSoItWillThrow()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceBaseClass()
        {
            NumericProperty = 17,
        };

        // Act.
        var target = PolymorphismMapperWithDefaultNull.Map(source);

        // Assert.
        target.Should().BeNull();
    }

     /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithDefaultDefault.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithDefaultDefaultAndMapFromFirstToFirst()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceFirstClass
        {
            NumericProperty = 17,
            DateTimeProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapperWithDefaultDefault.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetFirstClass>();
        var typedTarget = (Models.Polymorphism.One.TargetFirstClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithDefaultDefault.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithDefaultDefaultAndMapFromSecondToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceSecondClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
        };

        // Act.
        var target = PolymorphismMapperWithDefaultDefault.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetSecondClass>();
        var typedTarget = (Models.Polymorphism.One.TargetSecondClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithDefaultDefault.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithDefaultDefaultAndMapFromThirdToThird()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceThirdClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
            Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9"],
        };

        // Act.
        var target = PolymorphismMapperWithDefaultDefault.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetThirdClass>();
        var typedTarget = (Models.Polymorphism.One.TargetThirdClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
        typedTarget.Numbers.Should().BeEquivalentTo(source.Numbers.Select(long.Parse));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithDefaultDefault.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// and the method will return <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithDefaultDefaultAndMapFromBaseSoItWillThrow()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceBaseClass()
        {
            NumericProperty = 17,
        };

        // Act.
        var target = PolymorphismMapperWithDefaultDefault.Map(source);

        // Assert.
        target.Should().BeNull();
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapper.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperAndMapFromFirstToFirst()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceFirstClass
        {
            NumericProperty = 17,
            DateTimeProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapper.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetFirstClass>();
        var typedTarget = (Models.Polymorphism.One.TargetFirstClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapper.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperAndMapFromSecondToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceSecondClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapper.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetSecondClass>();
        var typedTarget = (Models.Polymorphism.One.TargetSecondClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapper.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperAndMapFromThirdToThird()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceThirdClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
            Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9"],
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapper.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetThirdClass>();
        var typedTarget = (Models.Polymorphism.One.TargetThirdClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
        typedTarget.Numbers.Should().BeEquivalentTo(source.Numbers.Select(long.Parse));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapper.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// and the method will invoke the static method.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperAndMapFromBaseSoItWillInvokeTheMethod()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceBaseClass
        {
            NumericProperty = 17,
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapper.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetBaseClass>();
        target.NumericProperty.Should().Be(PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapper.InvokeMe(source).NumericProperty);
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParameters.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParametersAndMapFromFirstToFirst()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceFirstClass
        {
            NumericProperty = 17,
            DateTimeProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParameters.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetFirstClass>();
        var typedTarget = (Models.Polymorphism.One.TargetFirstClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParameters.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParametersAndMapFromSecondToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceSecondClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParameters.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetSecondClass>();
        var typedTarget = (Models.Polymorphism.One.TargetSecondClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParameters.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParametersAndMapFromThirdToThird()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceThirdClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
            Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9"],
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParameters.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetThirdClass>();
        var typedTarget = (Models.Polymorphism.One.TargetThirdClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
        typedTarget.Numbers.Should().BeEquivalentTo(source.Numbers.Select(long.Parse));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParameters.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// and the method will invoke the static method.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParametersAndMapFromBaseSoItWillInvokeTheMethod()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceBaseClass
        {
            NumericProperty = 17,
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParameters.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetBaseClass>();
        target.NumericProperty.Should().Be(PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParameters.InvokeMe().NumericProperty);
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContext.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContextAndMapFromFirstToFirst()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceFirstClass
        {
            NumericProperty = 17,
            DateTimeProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContext.Map(source, new());

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetFirstClass>();
        var typedTarget = (Models.Polymorphism.One.TargetFirstClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContext.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContextAndMapFromSecondToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceSecondClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContext.Map(source, new());

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetSecondClass>();
        var typedTarget = (Models.Polymorphism.One.TargetSecondClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContext.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContextAndMapFromThirdToThird()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceThirdClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
            Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9"],
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContext.Map(source, new());

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetThirdClass>();
        var typedTarget = (Models.Polymorphism.One.TargetThirdClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
        typedTarget.Numbers.Should().BeEquivalentTo(source.Numbers.Select(long.Parse));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContext.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// and the method will invoke the static method.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContextAndMapFromBaseSoItWillInvokeTheMethod()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceBaseClass
        {
            NumericProperty = 17,
        };

        // Act.
        var context = new MappaContext { [nameof(Models.Polymorphism.One.TargetBaseClass.NumericProperty)] = 2025L, };
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContext.Map(source, context);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetBaseClass>();
        target.NumericProperty.Should().Be(PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContext.InvokeMe(source, context).NumericProperty);
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClass.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClassAndMapFromFirstToFirst()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceFirstClass
        {
            NumericProperty = 17,
            DateTimeProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClass.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetFirstClass>();
        var typedTarget = (Models.Polymorphism.One.TargetFirstClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClass.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClassAndMapFromSecondToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceSecondClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClass.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetSecondClass>();
        var typedTarget = (Models.Polymorphism.One.TargetSecondClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClass.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClassAndMapFromThirdToThird()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceThirdClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
            Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9"],
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClass.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetThirdClass>();
        var typedTarget = (Models.Polymorphism.One.TargetThirdClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
        typedTarget.Numbers.Should().BeEquivalentTo(source.Numbers.Select(long.Parse));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClass.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// and the method will invoke the static method.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClassAndMapFromBaseSoItWillInvokeTheMethod()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceBaseClass
        {
            NumericProperty = 17,
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClass.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetBaseClass>();
        target.NumericProperty.Should().Be(Models.Polymorphism.One.MapperHelper.InvokeMe(source).NumericProperty);
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClass.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClassAndMapFromFirstToFirst()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceFirstClass
        {
            NumericProperty = 17,
            DateTimeProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClass.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetFirstClass>();
        var typedTarget = (Models.Polymorphism.One.TargetFirstClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClass.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClassAndMapFromSecondToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceSecondClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClass.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetSecondClass>();
        var typedTarget = (Models.Polymorphism.One.TargetSecondClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClass.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClassAndMapFromThirdToThird()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceThirdClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
            Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9"],
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClass.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetThirdClass>();
        var typedTarget = (Models.Polymorphism.One.TargetThirdClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
        typedTarget.Numbers.Should().BeEquivalentTo(source.Numbers.Select(long.Parse));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClass.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// and the method will invoke the static method.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClassAndMapFromBaseSoItWillInvokeTheMethod()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceBaseClass
        {
            NumericProperty = 17,
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClass.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetBaseClass>();

        // ReSharper disable once AccessToStaticMemberViaDerivedType
        target.NumericProperty.Should().Be(PolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClass.InvokeMe(source).NumericProperty);
    }

     /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapper.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceFirstClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetFirstClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapperAndMapFromFirstToFirst()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceFirstClass
        {
            NumericProperty = 17,
            DateTimeProperty = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapper.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetFirstClass>();
        var typedTarget = (Models.Polymorphism.One.TargetFirstClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.DateTimeProperty.Should().Be(source.DateTimeProperty.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapper.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceSecondClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetSecondClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapperAndMapFromSecondToSecond()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceSecondClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapper.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetSecondClass>();
        var typedTarget = (Models.Polymorphism.One.TargetSecondClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapper.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapperAndMapFromThirdToThird()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceThirdClass
        {
            NumericProperty = 17,
            GuidProperty = Guid.NewGuid(),
            Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9"],
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapper.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetThirdClass>();
        var typedTarget = (Models.Polymorphism.One.TargetThirdClass)target;
        typedTarget.NumericProperty.Should().Be(source.NumericProperty);
        typedTarget.GuidProperty.Should().Be(source.GuidProperty.ToString());
        typedTarget.Numbers.Should().BeEquivalentTo(source.Numbers.Select(long.Parse));
    }

    /// <summary>
    /// Tests mapping via <see cref="PolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapper.Map"/>
    /// from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// and the method will invoke the static method.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsePolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapperAndMapFromBaseSoItWillInvokeTheMethod()
    {
        // Arrange.
        var source = new Models.Polymorphism.One.SourceBaseClass
        {
            NumericProperty = 17,
        };

        // Act.
        var target = PolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapper.Map(source);

        // Assert.
        target.Should().BeOfType<Models.Polymorphism.One.TargetBaseClass>();
        target.NumericProperty.Should().Be(PolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapper.InvokeMe(source).NumericProperty);
    }
}