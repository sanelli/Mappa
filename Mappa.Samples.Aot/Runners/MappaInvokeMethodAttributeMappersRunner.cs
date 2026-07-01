// <copyright file="MappaInvokeMethodAttributeMappersRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa;
using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for mappers that use <c>MappaInvokeMethod</c> attributes.
/// </summary>
internal static class MappaInvokeMethodAttributeMappersRunner
{
    /// <summary>
    /// Runs all map methods on the invoke-method attribute mapper sample classes.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var sourceClass = AotSampleData.SourceClassModel10Three;
        var sourceRecord = AotSampleData.SourceRecordModel17Three;

        report.BeginMapper(nameof(MapEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput));
        var localStatic = new MapEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput();
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            localStatic.Map(sourceClass));

        report.BeginMapper(nameof(MapNonEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput));
        var nonEmptyStatic = new MapNonEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput();
        report.RecordInvocation(
            nameof(MapNonEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput.Map),
            nameof(SourceRecordModel),
            nameof(TargetRecordModel),
            sourceRecord,
            nonEmptyStatic.Map(sourceRecord));

        report.BeginMapper(nameof(MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndPropertyInput));
        var localNonStatic = new MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndPropertyInput();
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndPropertyInput.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            localNonStatic.Map(sourceClass));

        report.BeginMapper(nameof(MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndPropertyInput));
        var implicitSource = new MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndPropertyInput();
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndPropertyInput.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            implicitSource.Map(sourceClass));

        report.BeginMapper(nameof(MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndImplicitConvertiblePropertyInput));
        var implicitProperty = new MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndImplicitConvertiblePropertyInput();
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndImplicitConvertiblePropertyInput.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            implicitProperty.Map(sourceClass));

        report.BeginMapper(nameof(MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndImplicitConvertiblePropertyInput));
        var implicitBoth = new MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndImplicitConvertiblePropertyInput();
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndImplicitConvertiblePropertyInput.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            implicitBoth.Map(sourceClass));

        report.BeginMapper(nameof(MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassInput));
        var sourceClassInput = new MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassInput();
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassInput.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            sourceClassInput.Map(sourceClass));

        report.BeginMapper(nameof(MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConversionFromSourceClassInput));
        var implicitSourceClass = new MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConversionFromSourceClassInput();
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConversionFromSourceClassInput.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            implicitSourceClass.Map(sourceClass));

        report.BeginMapper(nameof(MapEmptyConstructorWithLocalMethodWithSourcePropertyTypeInput));
        var propertyTypeInput = new MapEmptyConstructorWithLocalMethodWithSourcePropertyTypeInput();
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithLocalMethodWithSourcePropertyTypeInput.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            propertyTypeInput.Map(sourceClass));

        report.BeginMapper(nameof(MapEmptyConstructorWithLocalMethodWithImplicitConversionFromSourcePropertyTypeInput));
        var implicitPropertyType = new MapEmptyConstructorWithLocalMethodWithImplicitConversionFromSourcePropertyTypeInput();
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithLocalMethodWithImplicitConversionFromSourcePropertyTypeInput.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            implicitPropertyType.Map(sourceClass));

        report.BeginMapper(nameof(MapEmptyConstructorWithLocalMethodWithNoParameters));
        var noParameters = new MapEmptyConstructorWithLocalMethodWithNoParameters();
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithLocalMethodWithNoParameters.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            noParameters.Map(sourceClass));

        report.BeginMapper(nameof(MapEmptyConstructorWithTypeLocatedMethodWithSourceClassAndPropertyInput));
        var typeLocated = new MapEmptyConstructorWithTypeLocatedMethodWithSourceClassAndPropertyInput();
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithTypeLocatedMethodWithSourceClassAndPropertyInput.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            typeLocated.Map(sourceClass));

        report.BeginMapper(nameof(MapEmptyConstructorWithFieldLocatedMethodWithSourceClassAndPropertyInput));
        var fieldLocated = new MapEmptyConstructorWithFieldLocatedMethodWithSourceClassAndPropertyInput();
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithFieldLocatedMethodWithSourceClassAndPropertyInput.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            fieldLocated.Map(sourceClass));

        report.BeginMapper(nameof(MapEmptyConstructorWithPropertyLocatedMethodWithSourceClassAndPropertyInput));
        var propertyLocated = new MapEmptyConstructorWithPropertyLocatedMethodWithSourceClassAndPropertyInput();
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithPropertyLocatedMethodWithSourceClassAndPropertyInput.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            propertyLocated.Map(sourceClass));

        report.BeginMapper(nameof(MapEmptyConstructorWithStaticPropertyAndStaticMapMethodMapper));
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithStaticPropertyAndStaticMapMethodMapper.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            MapEmptyConstructorWithStaticPropertyAndStaticMapMethodMapper.Map(sourceClass));

        report.BeginMapper(nameof(MapEmptyConstructorWithStaticFieldAndStaticMapMethodMapper));
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithStaticFieldAndStaticMapMethodMapper.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            MapEmptyConstructorWithStaticFieldAndStaticMapMethodMapper.Map(sourceClass));

        report.BeginMapper(nameof(MapEmptyConstructorWithMethodFromMapperBaseClass));
        var methodFromMapperBase = new MapEmptyConstructorWithMethodFromMapperBaseClass();
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithMethodFromMapperBaseClass.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            methodFromMapperBase.Map(sourceClass));

        report.BeginMapper(nameof(MapEmptyConstructorWithFieldFromMapperBaseClass));
        var fieldFromMapperBase = new MapEmptyConstructorWithFieldFromMapperBaseClass();
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithFieldFromMapperBaseClass.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            fieldFromMapperBase.Map(sourceClass));

        report.BeginMapper(nameof(MapEmptyConstructorWithLocalStaticMethodWithSourceClassPropertyAndMappaContextInput));
        var withMappaContext = new MapEmptyConstructorWithLocalStaticMethodWithSourceClassPropertyAndMappaContextInput();
        var context = AotSampleData.CustomValueContext;
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithLocalStaticMethodWithSourceClassPropertyAndMappaContextInput.Map),
            "SourceClassModel, MappaContext",
            nameof(TargetClassModel),
            sourceClass,
            withMappaContext.Map(sourceClass, context));

        report.BeginMapper(nameof(MapEmptyConstructorWithLocalMethodUsingSourcePropertyName));
        var withSourcePropertyName = new MapEmptyConstructorWithLocalMethodUsingSourcePropertyName();
        report.RecordInvocation(
            nameof(MapEmptyConstructorWithLocalMethodUsingSourcePropertyName.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            sourceClass,
            withSourcePropertyName.Map(sourceClass));
    }
}