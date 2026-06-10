// <copyright file="PropertyMapNameSettingsMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="PropertyMapNameSettingsMapper"/>.
/// </summary>
internal static class PropertyMapNameSettingsMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="PropertyMapNameSettingsMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(PropertyMapNameSettingsMapper));
        var mapper = new PropertyMapNameSettingsMapper();
        var source = AotSampleData.PropertyMapNameSettingsSourceModel42Seven;

        report.RecordInvocation(
            nameof(PropertyMapNameSettingsMapper.MapWithClassDefaults),
            nameof(PropertyMapNameSettingsSourceModel),
            nameof(PropertyMapNameSettingsTargetModel),
            source,
            mapper.MapWithClassDefaults(source));

        report.RecordInvocation(
            nameof(PropertyMapNameSettingsMapper.MapWithMethodOverrideDisablingUnderscoreMatching),
            nameof(PropertyMapNameSettingsSourceModel),
            nameof(PropertyMapNameSettingsPartialTargetModel),
            source,
            mapper.MapWithMethodOverrideDisablingUnderscoreMatching(source));
    }
}