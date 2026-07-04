// <copyright file="DescriptionStringToEnumMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="DescriptionStringToEnumMapper"/>.
/// </summary>
internal static class DescriptionStringToEnumMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="DescriptionStringToEnumMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(DescriptionStringToEnumMapper));
        var mapper = new DescriptionStringToEnumMapper();
        const string input = "First";

        report.RecordInvocation(
            nameof(DescriptionStringToEnumMapper.MapToEnum),
            "string",
            nameof(DescribedCountingValues),
            input,
            mapper.MapToEnum(input));
    }
}