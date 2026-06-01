// <copyright file="MappaAssignFromContextAttributeMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa;
using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MappaAssignFromContextAttributeMapper"/>.
/// </summary>
internal static class MappaAssignFromContextAttributeMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="MappaAssignFromContextAttributeMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MappaAssignFromContextAttributeMapper));
        var mapper = new MappaAssignFromContextAttributeMapper();
        var context = AotSampleData.CustomValueContext;
        var source = AotSampleData.SourceClassModel13Three;

        report.RecordInvocation(
            nameof(MappaAssignFromContextAttributeMapper.Map),
            "SourceClassModel, MappaContext",
            "TargetClassModel",
            source,
            mapper.Map(source, context));

        var nestedSource = AotSampleData.SourceClassWithInnerClassModel13Three;
        report.RecordInvocation(
            nameof(MappaAssignFromContextAttributeMapper.Map),
            "SourceClassWithInnerClassModel, MappaContext",
            "TargetClassWithInnerClassModel",
            nestedSource,
            mapper.Map(nestedSource, context));
    }
}