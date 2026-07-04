// <copyright file="DescriptionEnumToStringMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="DescriptionEnumToStringMapper"/>.
/// </summary>
internal static class DescriptionEnumToStringMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="DescriptionEnumToStringMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(DescriptionEnumToStringMapper));
        var mapper = new DescriptionEnumToStringMapper();
        var input = DescribedCountingValues.One;

        report.RecordInvocation(
            nameof(DescriptionEnumToStringMapper.MapToString),
            nameof(DescribedCountingValues),
            "string",
            input,
            mapper.MapToString(input));
    }
}