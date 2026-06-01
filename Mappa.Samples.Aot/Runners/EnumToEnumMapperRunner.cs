// <copyright file="EnumToEnumMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="EnumToEnumMapper"/>.
/// </summary>
internal static class EnumToEnumMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="EnumToEnumMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(EnumToEnumMapper));
        var mapper = new EnumToEnumMapper();
        var input = CountingValues.Two;

        report.RecordInvocation(
            nameof(EnumToEnumMapper.Map),
            nameof(CountingValues),
            nameof(CountingValuesFromTwo),
            input,
            mapper.Map(input));
    }
}