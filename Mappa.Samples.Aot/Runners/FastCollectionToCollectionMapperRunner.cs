// <copyright file="FastCollectionToCollectionMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="FastCollectionToCollectionMapper"/>.
/// </summary>
internal static class FastCollectionToCollectionMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="FastCollectionToCollectionMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(FastCollectionToCollectionMapper));
        var mapper = new FastCollectionToCollectionMapper();
        var array = AotSampleData.CountingValuesArray;
        var list = AotSampleData.CountingValuesList;

        report.RecordInvocation(
            nameof(FastCollectionToCollectionMapper.MapArrayToArray),
            "CountingValues[]",
            "int[]",
            array,
            mapper.MapArrayToArray(array));

        report.RecordInvocation(
            nameof(FastCollectionToCollectionMapper.MapArrayToList),
            "CountingValues[]",
            "List<int>",
            array,
            mapper.MapArrayToList(array));

        report.RecordInvocation(
            nameof(FastCollectionToCollectionMapper.MapListToArray),
            "List<CountingValues>",
            "int[]",
            list,
            mapper.MapListToArray(list));

        report.RecordInvocation(
            nameof(FastCollectionToCollectionMapper.MapListToList),
            "List<CountingValues>",
            "List<int>",
            list,
            mapper.MapListToList(list));
    }
}