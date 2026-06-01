// <copyright file="IntegralToEnumMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="IntegralToEnumMapper"/>.
/// </summary>
internal static class IntegralToEnumMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="IntegralToEnumMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(IntegralToEnumMapper));
        var mapper = new IntegralToEnumMapper();
        const int intInput = 0;
        const short shortInput = 0;
        const int backwardsInput = 8;

        report.RecordInvocation(
            nameof(IntegralToEnumMapper.MapToEnum),
            "int",
            nameof(CountingValues),
            intInput,
            mapper.MapToEnum(intInput));

        report.RecordInvocation(
            nameof(IntegralToEnumMapper.MapToEnum),
            "short",
            nameof(CountingValues),
            shortInput,
            mapper.MapToEnum(shortInput));

        report.RecordInvocation(
            nameof(IntegralToEnumMapper.MapToBackwardsEnum),
            "int",
            nameof(CountingValuesBackwards),
            backwardsInput,
            mapper.MapToBackwardsEnum(backwardsInput));
    }
}