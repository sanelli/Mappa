// <copyright file="PragmaWarningSettingMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="PragmaWarningSettingMapper"/>.
/// </summary>
internal static class PragmaWarningSettingMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="PragmaWarningSettingMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(PragmaWarningSettingMapper));
        var mapper = new PragmaWarningSettingMapper();
        const int input = 100;

        report.RecordInvocation(
            nameof(PragmaWarningSettingMapper.Map),
            "int",
            "long",
            input,
            mapper.Map(input));
    }
}