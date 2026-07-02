// <copyright file="CaseInsensitiveStringToEnumMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="CaseInsensitiveStringToEnumMapper"/>.
/// </summary>
internal static class CaseInsensitiveStringToEnumMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="CaseInsensitiveStringToEnumMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(CaseInsensitiveStringToEnumMapper));
        var mapper = new CaseInsensitiveStringToEnumMapper();
        const string input = "one";

        report.RecordInvocation(
            nameof(CaseInsensitiveStringToEnumMapper.MapToEnum),
            "string",
            nameof(CountingValues),
            input,
            mapper.MapToEnum(input));
    }
}