// <copyright file="MapMethodStrategyWithUserCustomStaticMethodMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MapMethodStrategyWithUserCustomStaticMethodMapper"/>.
/// </summary>
internal static class MapMethodStrategyWithUserCustomStaticMethodMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="MapMethodStrategyWithUserCustomStaticMethodMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MapMethodStrategyWithUserCustomStaticMethodMapper));
        var mapper = new MapMethodStrategyWithUserCustomStaticMethodMapper();
        var input = AotSampleData.SourceClassWithInnerClassModel33One;

        report.RecordInvocation(
            nameof(MapMethodStrategyWithUserCustomStaticMethodMapper.Map),
            nameof(SourceClassWithInnerClassModel),
            nameof(TargetClassWithInnerClassModel),
            input,
            mapper.Map(input));
    }
}