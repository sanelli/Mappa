// <copyright file="StringToEnumMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="StringToEnumMapper"/>.
/// </summary>
internal static class StringToEnumMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="StringToEnumMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(StringToEnumMapper));
        var mapper = new StringToEnumMapper();
        var input = nameof(CountingValues.One);

        report.RecordInvocation(
            nameof(StringToEnumMapper.MapToEnum),
            "string",
            nameof(CountingValues),
            input,
            mapper.MapToEnum(input));
    }
}