// <copyright file="DescriptionEnumToEnumMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="DescriptionEnumToEnumMapper"/>.
/// </summary>
internal static class DescriptionEnumToEnumMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="DescriptionEnumToEnumMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(DescriptionEnumToEnumMapper));
        var mapper = new DescriptionEnumToEnumMapper();
        var input = DescribedSourceValues.Alpha;

        report.RecordInvocation(
            nameof(DescriptionEnumToEnumMapper.Map),
            nameof(DescribedSourceValues),
            nameof(DescribedTargetValues),
            input,
            mapper.Map(input));
    }
}