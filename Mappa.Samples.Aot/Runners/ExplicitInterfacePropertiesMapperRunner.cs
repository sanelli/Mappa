// <copyright file="ExplicitInterfacePropertiesMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="ExplicitInterfacePropertiesMapper"/>.
/// </summary>
internal static class ExplicitInterfacePropertiesMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="ExplicitInterfacePropertiesMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(ExplicitInterfacePropertiesMapper));
        var mapper = new ExplicitInterfacePropertiesMapper();
        var explicitSource = AotSampleData.ExplicitInterfaceSourceModel1And42;
        var publicSource = AotSampleData.ExplicitInterfacePublicSourceModel1And42;

        report.RecordInvocation(
            nameof(ExplicitInterfacePropertiesMapper.MapFromExplicitSource),
            nameof(ExplicitInterfaceSourceModel),
            nameof(ExplicitInterfacePublicTargetModel),
            explicitSource,
            mapper.MapFromExplicitSource(explicitSource));

        report.RecordInvocation(
            nameof(ExplicitInterfacePropertiesMapper.MapToExplicitTarget),
            nameof(ExplicitInterfacePublicSourceModel),
            nameof(ExplicitInterfaceTargetModel),
            publicSource,
            mapper.MapToExplicitTarget(publicSource));

        report.RecordInvocation(
            nameof(ExplicitInterfacePropertiesMapper.MapParameterizedWithLeftoverExplicitTarget),
            nameof(ExplicitInterfacePublicSourceModel),
            nameof(ExplicitInterfaceParameterizedTargetModel),
            publicSource,
            mapper.MapParameterizedWithLeftoverExplicitTarget(publicSource));
    }
}