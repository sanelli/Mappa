// <copyright file="MappaMustMapTargetPropertyAttributeMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MappaMustMapTargetPropertyAttributeMapper"/> and
/// <see cref="MappaMustMapAllTargetPropertiesAttributeMapper"/>.
/// </summary>
internal static class MappaMustMapTargetPropertyAttributeMapperRunner
{
    /// <summary>
    /// Runs all map methods on the must-map sample mapper classes.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var source = AotSampleData.MappaMustMapTargetPropertySourceModel17FortyTwo;

        report.BeginMapper(nameof(MappaMustMapTargetPropertyAttributeMapper));
        var listedMapper = new MappaMustMapTargetPropertyAttributeMapper();
        report.RecordInvocation(
            nameof(MappaMustMapTargetPropertyAttributeMapper.Map),
            nameof(MappaMustMapTargetPropertySourceModel),
            nameof(MappaMustMapTargetPropertyTargetModel),
            source,
            listedMapper.Map(source));

        report.BeginMapper(nameof(MappaMustMapAllTargetPropertiesAttributeMapper));
        var allPropertiesMapper = new MappaMustMapAllTargetPropertiesAttributeMapper();
        report.RecordInvocation(
            nameof(MappaMustMapAllTargetPropertiesAttributeMapper.Map),
            nameof(MappaMustMapTargetPropertySourceModel),
            nameof(MappaMustMapTargetPropertyTargetModel),
            source,
            allPropertiesMapper.Map(source));
    }
}