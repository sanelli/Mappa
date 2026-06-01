// <copyright file="ParamsAndInMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa;
using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="ParamsAndInMapper"/>.
/// </summary>
internal static class ParamsAndInMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="ParamsAndInMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(ParamsAndInMapper));
        var mapper = new ParamsAndInMapper();
        var classInput = AotSampleData.SourceClassModel17Three;
        var classInput2 = AotSampleData.SourceClassModel13One;
        var recordInput = AotSampleData.SourceRecordModel17Three;
        var context = AotSampleData.ParamBContext33;

        report.RecordInvocation(
            nameof(ParamsAndInMapper.MapWithIn),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            classInput,
            mapper.MapWithIn(classInput));

        report.RecordInvocation(
            nameof(ParamsAndInMapper.MapWithParams),
            "SourceClassModel[]",
            "TargetClassModel[]",
            new[] { classInput, classInput2 },
            mapper.MapWithParams(classInput, classInput2));

        report.RecordInvocation(
            nameof(ParamsAndInMapper.MapWithInOnContext),
            $"{nameof(SourceRecordModel)}, {nameof(MappaContext)}",
            nameof(TargetRecordModel),
            recordInput,
            mapper.MapWithInOnContext(recordInput, context));
    }
}