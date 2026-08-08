// <copyright file="MappaTypeMappingDefaultAttributeExtensionsTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Tests.Abstractions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaTypeMappingDefaultAttributeExtensions"/>.
/// </summary>
public sealed class MappaTypeMappingDefaultAttributeExtensionsTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="MappaTypeMappingDefaultAttributeExtensions.IsValid"/> throws when
    /// <see cref="MappaTypeMappingDefaultBehavior.Throw"/> references a type missing from the compilation.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsValidThrowBehaviorThrowsWhenExceptionTypeCannotBeLoaded()
    {
        var (compilation, sourceType, targetType, parentClass) = CreateMinimalSymbols();
        var attribute = new MappaTypeMappingDefaultAttribute(
            MappaTypeMappingDefaultBehavior.Throw,
            typeof(MappaTypeMappingDefaultAttributeExtensionsTests));

        var act = () => attribute.IsValid(
            targetType,
            sourceType,
            parentClass,
            nullableEnabled: true,
            mapMethodHasTwoParameters: false,
            compilation,
            location: null,
            out _,
            out _);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage($"Type '{typeof(MappaTypeMappingDefaultAttributeExtensionsTests).FullName}' cannot be loaded at compile time.");
    }

    /// <summary>
    /// Test <see cref="MappaTypeMappingDefaultAttributeExtensions.IsValid"/> throws when
    /// <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/> references a type missing from the compilation.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsValidMapSourceTypeBehaviorThrowsWhenTargetTypeCannotBeLoaded()
    {
        var (compilation, sourceType, targetType, parentClass) = CreateMinimalSymbols();
        var attribute = new MappaTypeMappingDefaultAttribute(
            MappaTypeMappingDefaultBehavior.MapSourceType,
            typeof(MappaTypeMappingDefaultAttributeExtensionsTests));

        var act = () => attribute.IsValid(
            targetType,
            sourceType,
            parentClass,
            nullableEnabled: true,
            mapMethodHasTwoParameters: false,
            compilation,
            location: null,
            out _,
            out _);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage($"Type '{typeof(MappaTypeMappingDefaultAttributeExtensionsTests).FullName}' cannot be loaded at compile time.");
    }

    /// <summary>
    /// Test <see cref="MappaTypeMappingDefaultAttributeExtensions.IsValid"/> throws when
    /// <see cref="MappaTypeMappingDefaultBehavior.InvokeMethod"/> references a declaring type missing from the compilation.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsValidInvokeMethodBehaviorThrowsWhenDeclaringTypeCannotBeLoaded()
    {
        var (compilation, sourceType, targetType, parentClass) = CreateMinimalSymbols();
        var attribute = new MappaTypeMappingDefaultAttribute(
            typeof(MappaTypeMappingDefaultAttributeExtensionsTests),
            "MapDefault");

        var act = () => attribute.IsValid(
            targetType,
            sourceType,
            parentClass,
            nullableEnabled: true,
            mapMethodHasTwoParameters: false,
            compilation,
            location: null,
            out _,
            out _);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Type that can be used to identify the method to invoke cannot be loaded.");
    }

    /// <summary>
    /// Test <see cref="MappaTypeMappingDefaultAttributeExtensions.IsValid"/> throws for an unsupported behavior value.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsValidThrowsForUnsupportedBehavior()
    {
        var (compilation, sourceType, targetType, parentClass) = CreateMinimalSymbols();
        var attribute = new MappaTypeMappingDefaultAttribute((MappaTypeMappingDefaultBehavior)999);

        var act = () => attribute.IsValid(
            targetType,
            sourceType,
            parentClass,
            nullableEnabled: true,
            mapMethodHasTwoParameters: false,
            compilation,
            location: null,
            out _,
            out _);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    private static (Compilation Compilation, ITypeSymbol SourceType, ITypeSymbol TargetType, ITypeSymbol ParentClass) CreateMinimalSymbols()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Source
                              {
                              }

                              public sealed class Target
                              {
                              }

                              public sealed class Mapper
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")!;
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")!;
        var parentClass = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Mapper")!;
        return (compilation, sourceType, targetType, parentClass);
    }
}