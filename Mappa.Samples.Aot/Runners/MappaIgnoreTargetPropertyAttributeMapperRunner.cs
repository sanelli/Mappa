// <copyright file="MappaIgnoreTargetPropertyAttributeMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MappaIgnoreTargetPropertyAttributeMapper"/>.
/// </summary>
internal static class MappaIgnoreTargetPropertyAttributeMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="MappaIgnoreTargetPropertyAttributeMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MappaIgnoreTargetPropertyAttributeMapper));
        var mapper = new MappaIgnoreTargetPropertyAttributeMapper();

        var source = AotSampleData.MappaIgnoreTargetPropertySourceModel17;
        report.RecordInvocation(
            nameof(MappaIgnoreTargetPropertyAttributeMapper.Map),
            nameof(MappaIgnoreTargetPropertySourceModel),
            nameof(MappaIgnoreTargetPropertyTargetModel),
            source,
            mapper.Map(source));
    }
}