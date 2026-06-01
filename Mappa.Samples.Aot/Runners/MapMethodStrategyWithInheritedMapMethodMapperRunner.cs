// <copyright file="MapMethodStrategyWithInheritedMapMethodMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for inherited map method strategy sample classes.
/// </summary>
internal static class MapMethodStrategyWithInheritedMapMethodMapperRunner
{
    /// <summary>
    /// Runs all map methods on the inherited map method strategy sample classes.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var source = AotSampleData.SourceClassWithInnerClassModel33One;

        report.BeginMapper(nameof(MapMethodStrategyWithMapperBaseClass));
        var mapperWithMapperBaseClass = new MapMethodStrategyWithMapperBaseClass();
        report.RecordInvocation(
            nameof(MapMethodStrategyWithMapperBaseClass.Map),
            "SourceClassWithInnerClassModel",
            "TargetClassWithInnerClassModel",
            source,
            mapperWithMapperBaseClass.Map(source));

        report.BeginMapper(nameof(MapMethodStrategyWithDependencyPropertyBaseClass));
        var mapperWithDependencyPropertyBaseClass = new MapMethodStrategyWithDependencyPropertyBaseClass();
        report.RecordInvocation(
            nameof(MapMethodStrategyWithDependencyPropertyBaseClass.Map),
            "SourceClassWithInnerClassModel",
            "TargetClassWithInnerClassModel",
            source,
            mapperWithDependencyPropertyBaseClass.Map(source));

        report.BeginMapper(nameof(MapMethodStrategyWithDependencyFieldBaseClass));
        var mapperWithDependencyFieldBaseClass = new MapMethodStrategyWithDependencyFieldBaseClass();
        report.RecordInvocation(
            nameof(MapMethodStrategyWithDependencyFieldBaseClass.Map),
            "SourceClassWithInnerClassModel",
            "TargetClassWithInnerClassModel",
            source,
            mapperWithDependencyFieldBaseClass.Map(source));

        report.BeginMapper(nameof(MapMethodStrategyWithInheritedDependencyPropertyMapper));
        var mapperWithInheritedDependencyProperty = new MapMethodStrategyWithInheritedDependencyPropertyMapper();
        report.RecordInvocation(
            nameof(MapMethodStrategyWithInheritedDependencyPropertyMapper.Map),
            "SourceClassWithInnerClassModel",
            "TargetClassWithInnerClassModel",
            source,
            mapperWithInheritedDependencyProperty.Map(source));

        report.BeginMapper(nameof(MapMethodStrategyWithInheritedDependencyFieldMapper));
        var mapperWithInheritedDependencyField = new MapMethodStrategyWithInheritedDependencyFieldMapper();
        report.RecordInvocation(
            nameof(MapMethodStrategyWithInheritedDependencyFieldMapper.Map),
            "SourceClassWithInnerClassModel",
            "TargetClassWithInnerClassModel",
            source,
            mapperWithInheritedDependencyField.Map(source));
    }
}