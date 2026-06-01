// <copyright file="MappaUsePropertyAttributeMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MappaUsePropertyAttributeMapper"/>.
/// </summary>
internal static class MappaUsePropertyAttributeMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="MappaUsePropertyAttributeMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MappaUsePropertyAttributeMapper));
        var mapper = new MappaUsePropertyAttributeMapper();

        var classSource = AotSampleData.SourceClassModel17Three;
        report.RecordInvocation(
            nameof(MappaUsePropertyAttributeMapper.MapWithEmptyConstructor),
            "SourceClassModel",
            "TargetClassModel",
            classSource,
            mapper.MapWithEmptyConstructor(classSource));

        var recordSource = AotSampleData.SourceRecordModel17Three;
        report.RecordInvocation(
            nameof(MappaUsePropertyAttributeMapper.MapWithConstructorWithParameters),
            "SourceRecordModel",
            "TargetRecordModel",
            recordSource,
            mapper.MapWithConstructorWithParameters(recordSource));
    }
}