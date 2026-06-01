// <copyright file="MapMethodStrategyWithUserCustomInstanceMethodMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MapMethodStrategyWithUserCustomInstanceMethodMapper"/>.
/// </summary>
internal static class MapMethodStrategyWithUserCustomInstanceMethodMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="MapMethodStrategyWithUserCustomInstanceMethodMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MapMethodStrategyWithUserCustomInstanceMethodMapper));
        var mapper = new MapMethodStrategyWithUserCustomInstanceMethodMapper(101);
        var input = AotSampleData.SourceClassWithInnerClassModel33One;

        report.RecordInvocation(
            nameof(MapMethodStrategyWithUserCustomInstanceMethodMapper.Map),
            nameof(SourceClassWithInnerClassModel),
            nameof(TargetClassWithInnerClassModel),
            input,
            mapper.Map(input));
    }
}