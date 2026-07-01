// <copyright file="MappaInvokeMethodAttributeMappersTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for <see cref="MappaInvokeMethodAttribute"/> mappers.
/// </summary>
public sealed class MappaInvokeMethodAttributeMappersTests
{
    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithLocalStaticMethodWithSourceClassInput()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput)}/static/({nameof(SourceClassModel)},int)/{source.ParamA}/{source.ParamB}/{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapNonEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapNonEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput()
    {
        // Arrange
        var mapper = new MapNonEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput();
        var source = new SourceRecordModel(10, CountingValues.Three);

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapNonEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput)}/static/({nameof(SourceRecordModel)},int)/{source.ParamA}/{source.ParamB}/{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndPropertyInput"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndPropertyInput()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndPropertyInput();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndPropertyInput)}/non-static/({nameof(SourceClassModel)},int)/{source.ParamA}/{source.ParamB}/{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndPropertyInput"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndPropertyInput()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndPropertyInput();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndPropertyInput)}/non-static/(object,int)/{source}/{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndImplicitConvertiblePropertyInput"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndImplicitConvertiblePropertyInput()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndImplicitConvertiblePropertyInput();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndImplicitConvertiblePropertyInput)}/non-static/({nameof(SourceClassModel)},long)/{source.ParamA}/{source.ParamB}/{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndImplicitConvertiblePropertyInput"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndImplicitConvertiblePropertyInput()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndImplicitConvertiblePropertyInput();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndImplicitConvertiblePropertyInput)}/non-static/(object,long)/{source}/{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassInput"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithLocalNonStaticMethodWithSourceClassInput()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassInput();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassInput)}/not-static/({nameof(SourceClassModel)})/{source.ParamA}/{source.ParamB}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConversionFromSourceClassInput"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithLocalNonStaticMethodWithImplicitConversionFromSourceClassInput()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConversionFromSourceClassInput();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConversionFromSourceClassInput)}/not-static/(object))/{source}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithLocalMethodWithSourcePropertyTypeInput"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithLocalMethodWithSourcePropertyTypeInput()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithLocalMethodWithSourcePropertyTypeInput();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapEmptyConstructorWithLocalMethodWithSourcePropertyTypeInput)}/static/(int)/{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithLocalMethodWithImplicitConversionFromSourcePropertyTypeInput"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithLocalMethodWithImplicitConversionFromSourcePropertyTypeInput()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithLocalMethodWithImplicitConversionFromSourcePropertyTypeInput();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapEmptyConstructorWithLocalMethodWithImplicitConversionFromSourcePropertyTypeInput)}/static/(long)/{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithLocalMethodWithNoParameters"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithLocalMethodWithNoParameters()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithLocalMethodWithNoParameters();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapEmptyConstructorWithLocalMethodWithNoParameters)}/static/()");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithTypeLocatedMethodWithSourceClassAndPropertyInput"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithTypeLocatedMethodWithSourceClassAndPropertyInput()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithTypeLocatedMethodWithSourceClassAndPropertyInput();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapperDependencyHelper.StaticMap)}/{source.ParamA}/{source.ParamB}/{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithFieldLocatedMethodWithSourceClassAndPropertyInput"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithFieldLocatedMethodWithSourceClassAndPropertyInput()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithFieldLocatedMethodWithSourceClassAndPropertyInput();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapperDependencyHelper.Map)}/{source.ParamA}/{source.ParamB}/{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithPropertyLocatedMethodWithSourceClassAndPropertyInput"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithPropertyLocatedMethodWithSourceClassAndPropertyInput()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithPropertyLocatedMethodWithSourceClassAndPropertyInput();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapperDependencyHelper.Map)}/{source.ParamA}/{source.ParamB}/{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithStaticPropertyAndStaticMapMethodMapper"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithStaticPropertyAndStaticMapMethodMapper()
    {
        // Arrange
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = MapEmptyConstructorWithStaticPropertyAndStaticMapMethodMapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapperDependencyHelper.Map)}/{source.ParamA}/{source.ParamB}/{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithStaticFieldAndStaticMapMethodMapper"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithStaticFieldAndStaticMapMethodMapper()
    {
        // Arrange
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = MapEmptyConstructorWithStaticFieldAndStaticMapMethodMapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapperDependencyHelper.Map)}/{source.ParamA}/{source.ParamB}/{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithMethodFromMapperBaseClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithMethodFromMapperBaseClass()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithMethodFromMapperBaseClass();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapEmptyConstructorWithMethodFromMapperBaseClass)}/mapper-base/static/({nameof(SourceClassModel)},int)/{source.ParamA}/{source.ParamB}/{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithFieldFromMapperBaseClass"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithFieldFromMapperBaseClass()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithFieldFromMapperBaseClass();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapperDependencyHelper.Map)}/{source.ParamA}/{source.ParamB}/{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithLocalStaticMethodWithSourceClassPropertyAndMappaContextInput"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithLocalStaticMethodWithSourceClassPropertyAndMappaContextInput()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithLocalStaticMethodWithSourceClassPropertyAndMappaContextInput();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };
        MappaContext context = new Dictionary<string, object> { ["CustomValue"] = "Use the custom value" };

        // Act
        var actual = mapper.Map(source, context);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapEmptyConstructorWithLocalStaticMethodWithSourceClassPropertyAndMappaContextInput)}/static/({nameof(SourceClassModel)},int,{nameof(MappaContext)})/{source.ParamA}/{source.ParamB}/{source.ParamA}/Use the custom value");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test for <see cref="MapEmptyConstructorWithLocalMethodUsingSourcePropertyName"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapEmptyConstructorWithLocalMethodUsingSourcePropertyName()
    {
        // Arrange
        var mapper = new MapEmptyConstructorWithLocalMethodUsingSourcePropertyName();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{nameof(MapEmptyConstructorWithLocalMethodUsingSourcePropertyName)}/static/({nameof(CountingValues)})/{source.ParamB}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }
}