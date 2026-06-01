// <copyright file="MapMethodStrategyWithDependencyMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MapMethodStrategyWithDependencyMapper"/>.
/// </summary>
internal static class MapMethodStrategyWithDependencyMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="MapMethodStrategyWithDependencyMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MapMethodStrategyWithDependencyMapper));
        var mapper = new MapMethodStrategyWithDependencyMapper();
        var input = AotSampleData.SourceClassWithMultipleFieldsForDependencyModel;

        report.RecordInvocation(
            nameof(MapMethodStrategyWithDependencyMapper.Map),
            nameof(SourceClassWithMultipleFieldsForDependencyModel),
            nameof(TargetClassWithMultipleFieldForDependencyModel),
            input,
            mapper.Map(input));
    }
}