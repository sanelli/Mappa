// <copyright file="NumericValueEnumToEnumMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="NumericValueEnumToEnumMapper"/>.
/// </summary>
internal static class NumericValueEnumToEnumMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="NumericValueEnumToEnumMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(NumericValueEnumToEnumMapper));
        var mapper = new NumericValueEnumToEnumMapper();
        var input = CountingValues.One;

        report.RecordInvocation(
            nameof(NumericValueEnumToEnumMapper.Map),
            nameof(CountingValues),
            nameof(CountingValuesFromTwo),
            input,
            mapper.Map(input));
    }
}