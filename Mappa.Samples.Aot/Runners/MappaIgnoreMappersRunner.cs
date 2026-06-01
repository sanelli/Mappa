// <copyright file="MappaIgnoreMappersRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MappaIgnoreLocalMethodMapper"/> and <see cref="MappaIgnoreDependencyMethodMapper"/>.
/// </summary>
internal static class MappaIgnoreMappersRunner
{
    /// <summary>
    /// Runs all map methods on the ignore mapper sample classes.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var source = AotSampleData.SourceClassModel10Three;

        report.BeginMapper(nameof(MappaIgnoreLocalMethodMapper));
        var localMapper = new MappaIgnoreLocalMethodMapper();
        report.RecordInvocation(
            nameof(MappaIgnoreLocalMethodMapper.Map),
            "SourceClassModel",
            "TargetClassModel",
            source,
            localMapper.Map(source));

        report.BeginMapper(nameof(MappaIgnoreDependencyMethodMapper));
        var dependencyMapper = new MappaIgnoreDependencyMethodMapper();
        report.RecordInvocation(
            nameof(MappaIgnoreDependencyMethodMapper.Map),
            "SourceClassModel",
            "TargetClassModel",
            source,
            dependencyMapper.Map(source));
    }
}