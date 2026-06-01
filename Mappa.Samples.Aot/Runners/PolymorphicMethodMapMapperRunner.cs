// <copyright file="PolymorphicMethodMapMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models.Polymorphism.One;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for polymorphic method map mapper sample classes.
/// </summary>
internal static class PolymorphicMethodMapMapperRunner
{
    /// <summary>
    /// Runs all map methods on the polymorphic method map mapper sample classes.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var sourceThird = AotSampleData.PolymorphismOneSourceThirdClass;
        var sourceWithDependency = AotSampleData.PolymorphismOneSourceWithDependency;
        var sourceWithDependencyBase = AotSampleData.PolymorphismOneSourceWithDependencyWithSourceBaseClass;

        report.BeginMapper(nameof(PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeMapper));
        var viaTypeMapping = new PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeMapper();
        report.RecordInvocation(
            nameof(PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeMapper.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            sourceThird,
            viaTypeMapping.Map(sourceThird));
        report.RecordInvocation(
            nameof(PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeMapper.Map),
            nameof(SourceWithDependency),
            nameof(TargetWithDependency),
            sourceWithDependency,
            viaTypeMapping.Map(sourceWithDependency));

        report.BeginMapper(nameof(PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingDefaultAttributeMapper));
        var viaDefault = new PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingDefaultAttributeMapper();
        report.RecordInvocation(
            nameof(PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingDefaultAttributeMapper.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            AotSampleData.PolymorphismOneSourceBaseClass,
            viaDefault.Map(AotSampleData.PolymorphismOneSourceBaseClass));
        report.RecordInvocation(
            nameof(PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingDefaultAttributeMapper.Map),
            nameof(SourceWithDependencyWithSourceBaseClass),
            nameof(TargetWithDependencyWithUnmappedBaseClass),
            sourceWithDependencyBase,
            viaDefault.Map(sourceWithDependencyBase));

        report.BeginMapper(nameof(PolymorphicMethodMapDependency));
        var dependency = new PolymorphicMethodMapDependency();
        report.RecordInvocation(
            nameof(PolymorphicMethodMapDependency.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            sourceThird,
            dependency.Map(sourceThird));

        report.BeginMapper(nameof(PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeUsingFieldDependencyMapper));
        var fieldDependency = new PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeUsingFieldDependencyMapper();
        report.RecordInvocation(
            nameof(PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeUsingFieldDependencyMapper.Map),
            nameof(SourceWithDependency),
            nameof(TargetWithDependency),
            sourceWithDependency,
            fieldDependency.Map(sourceWithDependency));

        report.BeginMapper(nameof(NonStaticPolymorphicMethodNotInvokedByStaticContextMapper));
        report.RecordInvocation(
            nameof(NonStaticPolymorphicMethodNotInvokedByStaticContextMapper.Map),
            nameof(SourceWithDependency),
            nameof(TargetWithDependency),
            sourceWithDependency,
            NonStaticPolymorphicMethodNotInvokedByStaticContextMapper.Map(sourceWithDependency));

        report.BeginMapper(nameof(PolymorphicMethodMapMapperBase));
        var mapperBase = new PolymorphicMethodMapMapperBase();
        report.RecordInvocation(
            nameof(PolymorphicMethodMapMapperBase.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            sourceThird,
            mapperBase.Map(sourceThird));

        report.BeginMapper(nameof(PolymorphicMethodMapMapperWithMapperBaseClass));
        var withMapperBase = new PolymorphicMethodMapMapperWithMapperBaseClass();
        report.RecordInvocation(
            nameof(PolymorphicMethodMapMapperWithMapperBaseClass.Map),
            nameof(SourceWithDependency),
            nameof(TargetWithDependency),
            sourceWithDependency,
            withMapperBase.Map(sourceWithDependency));

        report.BeginMapper(nameof(PolymorphicMethodMapMapperWithDependencyPropertyBaseClass));
        var withDependencyProperty = new PolymorphicMethodMapMapperWithDependencyPropertyBaseClass();
        report.RecordInvocation(
            nameof(PolymorphicMethodMapMapperWithDependencyPropertyBaseClass.Map),
            nameof(SourceWithDependency),
            nameof(TargetWithDependency),
            sourceWithDependency,
            withDependencyProperty.Map(sourceWithDependency));

        report.BeginMapper(nameof(PolymorphicMethodMapMapperWithDependencyFieldBaseClass));
        var withDependencyField = new PolymorphicMethodMapMapperWithDependencyFieldBaseClass();
        report.RecordInvocation(
            nameof(PolymorphicMethodMapMapperWithDependencyFieldBaseClass.Map),
            nameof(SourceWithDependency),
            nameof(TargetWithDependency),
            sourceWithDependency,
            withDependencyField.Map(sourceWithDependency));

        report.BeginMapper(nameof(PolymorphicMethodMapDependencyBase));
        var dependencyBase = new PolymorphicMethodMapDerivedDependency();
        report.RecordInvocation(
            nameof(PolymorphicMethodMapDependencyBase.Map),
            nameof(SourceBaseClass),
            nameof(TargetBaseClass),
            sourceThird,
            dependencyBase.Map(sourceThird));
    }
}