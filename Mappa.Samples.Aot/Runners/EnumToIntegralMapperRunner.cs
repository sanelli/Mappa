// <copyright file="EnumToIntegralMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="EnumToIntegralMapper"/>.
/// </summary>
internal static class EnumToIntegralMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="EnumToIntegralMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(EnumToIntegralMapper));
        var mapper = new EnumToIntegralMapper();
        var countingValue = CountingValues.One;
        var backwardsValue = CountingValuesBackwards.Eight;

        report.RecordInvocation(
            nameof(EnumToIntegralMapper.MapToInteger),
            nameof(CountingValues),
            "int",
            countingValue,
            mapper.MapToInteger(countingValue));

        report.RecordInvocation(
            nameof(EnumToIntegralMapper.MapToInteger),
            nameof(CountingValuesBackwards),
            "int",
            backwardsValue,
            mapper.MapToInteger(backwardsValue));

        report.RecordInvocation(
            nameof(EnumToIntegralMapper.MapToLong),
            nameof(CountingValues),
            "long",
            countingValue,
            mapper.MapToLong(countingValue));
    }
}