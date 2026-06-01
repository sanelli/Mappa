// <copyright file="ReadOnlyTargetCollectionMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="ReadOnlyTargetCollectionMapper"/>.
/// </summary>
internal static class ReadOnlyTargetCollectionMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="ReadOnlyTargetCollectionMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(ReadOnlyTargetCollectionMapper));
        var mapper = new ReadOnlyTargetCollectionMapper();
        var source = AotSampleData.SourceClassWithCollections;

        report.RecordInvocation(
            nameof(ReadOnlyTargetCollectionMapper.Map),
            "SourceClassWithCollections",
            "TargetClassWithCollections",
            source,
            mapper.Map(source));

        report.RecordInvocation(
            nameof(ReadOnlyTargetCollectionMapper.MapWithPrivateSetters),
            "SourceClassWithCollections",
            "TargetClassWithPrivateSetterPropertyCollections",
            source,
            mapper.MapWithPrivateSetters(source));

        report.RecordInvocation(
            nameof(ReadOnlyTargetCollectionMapper.MapToProtobuf),
            "SourceClassWithCollections",
            "TargetProtobufClassWithCollections",
            source,
            mapper.MapToProtobuf(source));
    }
}