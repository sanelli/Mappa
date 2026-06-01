// <copyright file="MappaAssignFromConstantAttributeMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MappaAssignFromConstantAttributeMapper"/>.
/// </summary>
internal static class MappaAssignFromConstantAttributeMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="MappaAssignFromConstantAttributeMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MappaAssignFromConstantAttributeMapper));
        var mapper = new MappaAssignFromConstantAttributeMapper();
        var source = AotSampleData.UnusedObject;

        report.RecordInvocation(
            nameof(MappaAssignFromConstantAttributeMapper.MapToClassModel),
            "object",
            "MappaAssignFromConstantTargetClassModel",
            source,
            mapper.MapToClassModel(source));

        report.RecordInvocation(
            nameof(MappaAssignFromConstantAttributeMapper.MapToRecordModel),
            "object",
            "MappaAssignFromConstantTargetRecordModel",
            source,
            mapper.MapToRecordModel(source));
    }
}