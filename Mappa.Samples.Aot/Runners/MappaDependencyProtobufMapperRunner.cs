// <copyright file="MappaDependencyProtobufMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MappaDependencyProtobufMapper"/>.
/// </summary>
internal static class MappaDependencyProtobufMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="MappaDependencyProtobufMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MappaDependencyProtobufMapper));
        var mapper = new MappaDependencyProtobufMapper();
        var source = AotSampleData.MappaDependencySourceRecord;

        report.RecordInvocation(
            nameof(MappaDependencyProtobufMapper.MapWithDependencies),
            "MappaDependencySourceRecord",
            "MappaDependencyTargetModel",
            source,
            mapper.MapWithDependencies(source));
    }
}