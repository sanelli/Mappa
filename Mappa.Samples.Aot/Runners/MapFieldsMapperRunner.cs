// <copyright file="MapFieldsMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MapFieldsMapper"/>.
/// </summary>
internal static class MapFieldsMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="MapFieldsMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MapFieldsMapper));
        var mapper = new MapFieldsMapper();
        var source = AotSampleData.MapFieldsSourceModelAdaThirtySix;

        report.RecordInvocation(
            nameof(MapFieldsMapper.MapFieldsToFields),
            nameof(MapFieldsSourceModel),
            nameof(MapFieldsTargetModel),
            source,
            mapper.MapFieldsToFields(source));

        report.RecordInvocation(
            nameof(MapFieldsMapper.MapFieldsToProperties),
            nameof(MapFieldsSourceModel),
            nameof(MapFieldsPropertyTargetModel),
            source,
            mapper.MapFieldsToProperties(source));
    }
}