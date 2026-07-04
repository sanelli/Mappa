// <copyright file="CaseInsensitiveEnumMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="CaseInsensitiveEnumMapper"/>.
/// </summary>
internal static class CaseInsensitiveEnumMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="CaseInsensitiveEnumMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(CaseInsensitiveEnumMapper));
        var mapper = new CaseInsensitiveEnumMapper();
        const string input = "one";

        report.RecordInvocation(
            nameof(CaseInsensitiveEnumMapper.MapToEnum),
            "string",
            nameof(CountingValues),
            input,
            mapper.MapToEnum(input));
    }
}