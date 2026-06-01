// <copyright file="PolymorphismMappersRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models.Polymorphism.One;
using Mappa.Samples.Models.Polymorphism.Two;

using PolymorphismThree = Mappa.Samples.Models.Polymorphism.Three;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for polymorphism mapper sample classes.
/// </summary>
internal static class PolymorphismMappersRunner
{
    /// <summary>
    /// Runs all map methods on the polymorphism mapper sample classes.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var first = AotSampleData.PolymorphismOneSourceFirstClass;
        var second = AotSampleData.PolymorphismOneSourceSecondClass;
        var third = AotSampleData.PolymorphismOneSourceThirdClass;
        var sourceBase = AotSampleData.PolymorphismOneSourceBaseClass;

        report.BeginMapper(nameof(PolymorphismMapper));
        var mapper = new PolymorphismMapper();
        RecordOneMap(report, mapper, first);
        RecordOneMap(report, mapper, second);
        RecordOneMap(report, mapper, third);

        report.BeginMapper(nameof(PolymorphismMapperNullable));
        var mapperNullable = new PolymorphismMapperNullable();
        RecordOneMap(report, mapperNullable, first);
        RecordOneMap(report, mapperNullable, second);
        RecordOneMap(report, mapperNullable, third);
        RecordOneMap(report, mapperNullable, (SourceBaseClass?)null);

        report.BeginMapper(nameof(PolymorphismMapperBetweenInterfaces));
        var mapperInterfaces = new PolymorphismMapperBetweenInterfaces();
        RecordOneMap(report, mapperInterfaces, AotSampleData.PolymorphismTwoSourceFirstClass);
        RecordOneMap(report, mapperInterfaces, AotSampleData.PolymorphismTwoSourceSecondClass);
        RecordOneMap(report, mapperInterfaces, AotSampleData.PolymorphismTwoSourceThirdClass);

        report.BeginMapper(nameof(PolymorphismMapperOverridingIdentityMapper));
        var mapperIdentity = new PolymorphismMapperOverridingIdentityMapper();
        RecordOneMap(report, mapperIdentity, AotSampleData.PolymorphismThreeSourceFirstClass);
        RecordOneMap(report, mapperIdentity, AotSampleData.PolymorphismThreeSourceSecondClass);

        report.BeginMapper(nameof(PolymorphismMapperOverridingIdentityMapperWithNullable));
        var mapperIdentityNullable = new PolymorphismMapperOverridingIdentityMapperWithNullable();
        RecordOneMap(report, mapperIdentityNullable, AotSampleData.PolymorphismThreeSourceFirstClass);
        RecordOneMap(report, mapperIdentityNullable, AotSampleData.PolymorphismThreeSourceSecondClass);
        RecordOneMap(report, mapperIdentityNullable, (PolymorphismThree.SourceBaseClass?)null);

        report.BeginMapper(nameof(PolymorphismMapperWithThrowDefaultBehaviour));
        var mapperThrow = new PolymorphismMapperWithThrowDefaultBehaviour();
        RecordOneMap(report, mapperThrow, first);
        RecordOneMap(report, mapperThrow, second);
        RecordOneMap(report, mapperThrow, third);

        report.BeginMapper(nameof(PolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviour));
        var mapperCustomThrow = new PolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviour();
        RecordOneMap(report, mapperCustomThrow, first);
        RecordOneMap(report, mapperCustomThrow, second);
        RecordOneMap(report, mapperCustomThrow, third);

        report.BeginMapper(nameof(PolymorphismMapperWithMapDefaultWithoutExplicitType));
        var mapperMapDefault = new PolymorphismMapperWithMapDefaultWithoutExplicitType();
        RecordOneMap(report, mapperMapDefault, first);
        RecordOneMap(report, mapperMapDefault, second);
        RecordOneMap(report, mapperMapDefault, third);
        RecordOneMap(report, mapperMapDefault, sourceBase);

        report.BeginMapper(nameof(PolymorphismMapperWithMapDefaultWithExplicitType));
        var mapperMapDefaultExplicit = new PolymorphismMapperWithMapDefaultWithExplicitType();
        RecordOneMap(report, mapperMapDefaultExplicit, first);
        RecordOneMap(report, mapperMapDefaultExplicit, second);
        RecordOneMap(report, mapperMapDefaultExplicit, third);
        RecordOneMap(report, mapperMapDefaultExplicit, sourceBase);

        report.BeginMapper(nameof(PolymorphismMapperWithDefaultNull));
        var mapperDefaultNull = new PolymorphismMapperWithDefaultNull();
        RecordOneMap(report, mapperDefaultNull, first);
        RecordOneMap(report, mapperDefaultNull, second);
        RecordOneMap(report, mapperDefaultNull, third);
        RecordOneMap(report, mapperDefaultNull, sourceBase);

        report.BeginMapper(nameof(PolymorphismMapperWithDefaultDefault));
        var mapperDefaultDefault = new PolymorphismMapperWithDefaultDefault();
        RecordOneMap(report, mapperDefaultDefault, first);
        RecordOneMap(report, mapperDefaultDefault, second);
        RecordOneMap(report, mapperDefaultDefault, third);
        RecordOneMap(report, mapperDefaultDefault, sourceBase);

        report.BeginMapper(nameof(PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapper));
        var mapperInvokeStatic = new PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapper();
        RecordOneMap(report, mapperInvokeStatic, first);
        RecordOneMap(report, mapperInvokeStatic, second);
        RecordOneMap(report, mapperInvokeStatic, third);
        RecordOneMap(report, mapperInvokeStatic, sourceBase);

        report.BeginMapper(nameof(PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParameters));
        var mapperInvokeStaticNoParams = new PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParameters();
        RecordOneMap(report, mapperInvokeStaticNoParams, first);
        RecordOneMap(report, mapperInvokeStaticNoParams, second);
        RecordOneMap(report, mapperInvokeStaticNoParams, third);
        RecordOneMap(report, mapperInvokeStaticNoParams, sourceBase);

        report.BeginMapper(nameof(PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContext));
        var mapperInvokeStaticContext = new PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContext();
        RecordOneMap(report, mapperInvokeStaticContext, first, new MappaContext());
        RecordOneMap(report, mapperInvokeStaticContext, second, new MappaContext());
        RecordOneMap(report, mapperInvokeStaticContext, third, new MappaContext());
        RecordOneMap(report, mapperInvokeStaticContext, sourceBase, AotSampleData.PolymorphismNumericPropertyContext2025);

        report.BeginMapper(nameof(PolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClass));
        var mapperInvokeDifferentClass = new PolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClass();
        RecordOneMap(report, mapperInvokeDifferentClass, first);
        RecordOneMap(report, mapperInvokeDifferentClass, second);
        RecordOneMap(report, mapperInvokeDifferentClass, third);
        RecordOneMap(report, mapperInvokeDifferentClass, sourceBase);

        report.BeginMapper(nameof(PolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClass));
        var mapperInvokeBaseClass = new PolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClass();
        RecordOneMap(report, mapperInvokeBaseClass, first);
        RecordOneMap(report, mapperInvokeBaseClass, second);
        RecordOneMap(report, mapperInvokeBaseClass, third);
        RecordOneMap(report, mapperInvokeBaseClass, sourceBase);

        report.BeginMapper(nameof(PolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapper));
        var mapperInvokeNonStatic = new PolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapper();
        RecordOneMap(report, mapperInvokeNonStatic, first);
        RecordOneMap(report, mapperInvokeNonStatic, second);
        RecordOneMap(report, mapperInvokeNonStatic, third);
        RecordOneMap(report, mapperInvokeNonStatic, sourceBase);
    }

    private static void RecordOneMap(AotReport report, PolymorphismMapper mapper, SourceBaseClass source)
        => report.RecordInvocation(
            nameof(PolymorphismMapper.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            source,
            mapper.Map(source));

    private static void RecordOneMap(AotReport report, PolymorphismMapperNullable mapper, SourceBaseClass? source)
        => report.RecordInvocation(
            nameof(PolymorphismMapperNullable.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            source,
            mapper.Map(source));

    private static void RecordOneMap(AotReport report, PolymorphismMapperBetweenInterfaces mapper, ISourceBaseClass source)
        => report.RecordInvocation(
            nameof(PolymorphismMapperBetweenInterfaces.Map),
            nameof(ISourceBaseClass),
            nameof(ITargetBaseClass),
            source,
            mapper.Map(source));

    private static void RecordOneMap(AotReport report, PolymorphismMapperOverridingIdentityMapper mapper, PolymorphismThree.SourceBaseClass source)
        => report.RecordInvocation(
            nameof(PolymorphismMapperOverridingIdentityMapper.Map),
            nameof(PolymorphismThree.SourceBaseClass),
            nameof(PolymorphismThree.SourceBaseClass),
            source,
            mapper.Map(source));

    private static void RecordOneMap(AotReport report, PolymorphismMapperOverridingIdentityMapperWithNullable mapper, PolymorphismThree.SourceBaseClass? source)
        => report.RecordInvocation(
            nameof(PolymorphismMapperOverridingIdentityMapperWithNullable.Map),
            nameof(PolymorphismThree.SourceBaseClass),
            nameof(PolymorphismThree.SourceBaseClass),
            source,
            mapper.Map(source));

    private static void RecordOneMap(AotReport report, PolymorphismMapperWithThrowDefaultBehaviour mapper, SourceBaseClass source)
        => report.RecordInvocation(
            nameof(PolymorphismMapperWithThrowDefaultBehaviour.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            source,
            mapper.Map(source));

    private static void RecordOneMap(AotReport report, PolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviour mapper, SourceBaseClass source)
        => report.RecordInvocation(
            nameof(PolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviour.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            source,
            mapper.Map(source));

    private static void RecordOneMap(AotReport report, PolymorphismMapperWithMapDefaultWithoutExplicitType mapper, SourceBaseClass source)
        => report.RecordInvocation(
            nameof(PolymorphismMapperWithMapDefaultWithoutExplicitType.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            source,
            mapper.Map(source));

    private static void RecordOneMap(AotReport report, PolymorphismMapperWithMapDefaultWithExplicitType mapper, SourceBaseClass source)
        => report.RecordInvocation(
            nameof(PolymorphismMapperWithMapDefaultWithExplicitType.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            source,
            mapper.Map(source));

    private static void RecordOneMap(AotReport report, PolymorphismMapperWithDefaultNull mapper, SourceBaseClass source)
        => report.RecordInvocation(
            nameof(PolymorphismMapperWithDefaultNull.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            source,
            mapper.Map(source));

    private static void RecordOneMap(AotReport report, PolymorphismMapperWithDefaultDefault mapper, SourceBaseClass source)
        => report.RecordInvocation(
            nameof(PolymorphismMapperWithDefaultDefault.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            source,
            mapper.Map(source));

    private static void RecordOneMap(AotReport report, PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapper mapper, SourceBaseClass source)
        => report.RecordInvocation(
            nameof(PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapper.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            source,
            mapper.Map(source));

    private static void RecordOneMap(AotReport report, PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParameters mapper, SourceBaseClass source)
        => report.RecordInvocation(
            nameof(PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithoutParameters.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            source,
            mapper.Map(source));

    private static void RecordOneMap(AotReport report, PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContext mapper, SourceBaseClass source, MappaContext context)
        => report.RecordInvocation(
            nameof(PolymorphismMapperWithInvokeMethodAndStaticMethodInTheMapperWithContext.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            source,
            mapper.Map(source, context));

    private static void RecordOneMap(AotReport report, PolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClass mapper, SourceBaseClass source)
        => report.RecordInvocation(
            nameof(PolymorphismMapperWithInvokeMethodAndStaticMethodInADifferentClass.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            source,
            mapper.Map(source));

    private static void RecordOneMap(AotReport report, PolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClass mapper, SourceBaseClass source)
        => report.RecordInvocation(
            nameof(PolymorphismMapperWithInvokeMethodAndStaticMethodInTheBaseClass.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            source,
            mapper.Map(source));

    private static void RecordOneMap(AotReport report, PolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapper mapper, SourceBaseClass source)
        => report.RecordInvocation(
            nameof(PolymorphismMapperWithInvokeMethodAndNonStaticMethodInTheMapper.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            source,
            mapper.Map(source));
}