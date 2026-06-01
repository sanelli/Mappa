// <copyright file="ContainersWithCapacityConstructorMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="ContainersWithCapacityConstructorMapper"/>.
/// </summary>
internal static class ContainersWithCapacityConstructorMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="ContainersWithCapacityConstructorMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(ContainersWithCapacityConstructorMapper));
        var mapper = new ContainersWithCapacityConstructorMapper();
        var array = AotSampleData.IntArray;
        var enumerable = AotSampleData.IntEnumerable;

        report.RecordInvocation(
            nameof(ContainersWithCapacityConstructorMapper.MapFromArrayToCustomCollection),
            "int[]",
            "CustomICollectionWithCapacityConstructor<string>",
            array,
            mapper.MapFromArrayToCustomCollection(array));

        report.RecordInvocation(
            nameof(ContainersWithCapacityConstructorMapper.MapFromEnumerableToCustomCollection),
            "IEnumerable<int>",
            "CustomICollectionWithCapacityConstructor<string>",
            enumerable,
            mapper.MapFromEnumerableToCustomCollection(enumerable));

        report.RecordInvocation(
            nameof(ContainersWithCapacityConstructorMapper.MapFromArrayToCustomSet),
            "int[]",
            "CustomISetWithCapacityConstructor<string>",
            array,
            mapper.MapFromArrayToCustomSet(array));

        report.RecordInvocation(
            nameof(ContainersWithCapacityConstructorMapper.MapFromEnumerableToCustomSet),
            "IEnumerable<int>",
            "CustomISetWithCapacityConstructor<string>",
            enumerable,
            mapper.MapFromEnumerableToCustomSet(enumerable));

        report.RecordInvocation(
            nameof(ContainersWithCapacityConstructorMapper.MapFromArrayToCustomStack),
            "int[]",
            "CustomStackWithCapacityConstructor<string>",
            array,
            mapper.MapFromArrayToCustomStack(array));

        report.RecordInvocation(
            nameof(ContainersWithCapacityConstructorMapper.MapFromEnumerableToCustomStack),
            "IEnumerable<int>",
            "CustomStackWithCapacityConstructor<string>",
            enumerable,
            mapper.MapFromEnumerableToCustomStack(enumerable));

        report.RecordInvocation(
            nameof(ContainersWithCapacityConstructorMapper.MapFromArrayToCustomQueue),
            "int[]",
            "CustomQueueWithCapacityConstructor<string>",
            array,
            mapper.MapFromArrayToCustomQueue(array));

        report.RecordInvocation(
            nameof(ContainersWithCapacityConstructorMapper.MapFromEnumerableToCustomQueue),
            "IEnumerable<int>",
            "CustomQueueWithCapacityConstructor<string>",
            enumerable,
            mapper.MapFromEnumerableToCustomQueue(enumerable));

        report.RecordInvocation(
            nameof(ContainersWithCapacityConstructorMapper.MapFromArrayToCustomBlockingCollection),
            "int[]",
            "CustomBlockingCollectionWithCapacityConstructor<string>",
            array,
            mapper.MapFromArrayToCustomBlockingCollection(array));

        report.RecordInvocation(
            nameof(ContainersWithCapacityConstructorMapper.MapFromEnumerableToCustomBlockingCollection),
            "IEnumerable<int>",
            "CustomBlockingCollectionWithCapacityConstructor<string>",
            enumerable,
            mapper.MapFromEnumerableToCustomBlockingCollection(enumerable));
    }
}