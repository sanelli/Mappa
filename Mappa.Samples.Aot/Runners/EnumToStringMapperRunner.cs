// <copyright file="EnumToStringMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="EnumToStringMapper"/>.
/// </summary>
internal static class EnumToStringMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="EnumToStringMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(EnumToStringMapper));
        var mapper = new EnumToStringMapper();
        var input = CountingValues.One;

        report.RecordInvocation(
            nameof(EnumToStringMapper.MapToString),
            nameof(CountingValues),
            "string",
            input,
            mapper.MapToString(input));
    }
}