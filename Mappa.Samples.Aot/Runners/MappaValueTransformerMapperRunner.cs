// <copyright file="MappaValueTransformerMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MappaValueTransformerMapper"/>.
/// </summary>
internal static class MappaValueTransformerMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="MappaValueTransformerMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MappaValueTransformerMapper));
        var mapper = new MappaValueTransformerMapper();
        MappaContext context = new Dictionary<string, object>
        {
            ["maxLength"] = 5,
        };
        var input = new ValueTransformerSourceModel
        {
            Name = "  Ada   Lovelace  ",
            Code = "  ab-1  ",
            Note = "  hello world  ",
        };

        report.RecordInvocation(
            nameof(MappaValueTransformerMapper.MapTrimmed),
            nameof(ValueTransformerSourceModel),
            nameof(ValueTransformerTargetModel),
            input,
            mapper.MapTrimmed(input));

        report.RecordInvocation(
            nameof(MappaValueTransformerMapper.MapTruncated),
            "ValueTransformerSourceModel, MappaContext",
            nameof(ValueTransformerNoteModel),
            input,
            mapper.MapTruncated(input, context));

        report.RecordInvocation(
            nameof(MappaValueTransformerMapper.MapWithDependencyTransformers),
            nameof(ValueTransformerSourceModel),
            nameof(ValueTransformerLabelModel),
            input,
            mapper.MapWithDependencyTransformers(input));
    }
}