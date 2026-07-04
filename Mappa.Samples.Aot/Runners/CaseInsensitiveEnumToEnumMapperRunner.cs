// <copyright file="CaseInsensitiveEnumToEnumMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="CaseInsensitiveEnumToEnumMapper"/>.
/// </summary>
internal static class CaseInsensitiveEnumToEnumMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="CaseInsensitiveEnumToEnumMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(CaseInsensitiveEnumToEnumMapper));
        var mapper = new CaseInsensitiveEnumToEnumMapper();
        var input = CaseInsensitiveSourceValues.ONe;

        report.RecordInvocation(
            nameof(CaseInsensitiveEnumToEnumMapper.Map),
            nameof(CaseInsensitiveSourceValues),
            nameof(CaseInsensitiveTargetValues),
            input,
            mapper.Map(input));
    }
}