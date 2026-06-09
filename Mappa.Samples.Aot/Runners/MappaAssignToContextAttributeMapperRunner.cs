// <copyright file="MappaAssignToContextAttributeMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa;
using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MappaAssignToContextAttributeMapper"/>.
/// </summary>
internal static class MappaAssignToContextAttributeMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="MappaAssignToContextAttributeMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MappaAssignToContextAttributeMapper));
        var mapper = new MappaAssignToContextAttributeMapper();
        MappaContext context = new Dictionary<string, object>();
        var source = AotSampleData.SourceClassModel13Three;

        report.RecordInvocation(
            nameof(MappaAssignToContextAttributeMapper.Map),
            "SourceClassModel, MappaContext",
            "TargetClassModel",
            source,
            mapper.Map(source, context));
    }
}