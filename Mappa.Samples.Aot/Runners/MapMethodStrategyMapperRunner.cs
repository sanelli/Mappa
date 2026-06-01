// <copyright file="MapMethodStrategyMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MapMethodStrategyMapper"/> and <see cref="StaticMapMethodStrategyMapper"/>.
/// </summary>
internal static class MapMethodStrategyMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="MapMethodStrategyMapper"/> and <see cref="StaticMapMethodStrategyMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MapMethodStrategyMapper));
        var mapper = new MapMethodStrategyMapper();
        var classInput = AotSampleData.SourceClassModel123Three;
        var innerClassInput = AotSampleData.SourceClassWithInnerClassModel33One;

        report.RecordInvocation(
            nameof(MapMethodStrategyMapper.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            classInput,
            mapper.Map(classInput));

        report.RecordInvocation(
            nameof(MapMethodStrategyMapper.Map),
            nameof(SourceClassWithInnerClassModel),
            nameof(TargetClassWithInnerClassModel),
            innerClassInput,
            mapper.Map(innerClassInput));

        report.BeginMapper(nameof(StaticMapMethodStrategyMapper));
        var staticMapper = new StaticMapMethodStrategyMapper();

        report.RecordInvocation(
            nameof(StaticMapMethodStrategyMapper.Map),
            nameof(SourceClassWithInnerClassModel),
            nameof(TargetClassWithInnerClassModel),
            innerClassInput,
            StaticMapMethodStrategyMapper.Map(innerClassInput));

        report.RecordInvocation(
            nameof(StaticMapMethodStrategyMapper.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            classInput,
            staticMapper.Map(classInput));
    }
}