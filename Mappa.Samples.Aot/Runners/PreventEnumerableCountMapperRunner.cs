// <copyright file="PreventEnumerableCountMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="PreventEnumerableCountMapper"/>.
/// </summary>
internal static class PreventEnumerableCountMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="PreventEnumerableCountMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(PreventEnumerableCountMapper));
        var mapper = new PreventEnumerableCountMapper();
        var enumerable = AotSampleData.CountingValuesOneThreeEnumerable;

        report.RecordInvocation(
            nameof(PreventEnumerableCountMapper.MapEnumerableToArray),
            "IEnumerable<CountingValues>",
            "int[]",
            enumerable,
            mapper.MapEnumerableToArray(enumerable));

        report.RecordInvocation(
            nameof(PreventEnumerableCountMapper.MapEnumerableToSpan),
            "IEnumerable<CountingValues>",
            "Span<int>",
            enumerable,
            mapper.MapEnumerableToSpan(enumerable).ToArray());
    }
}