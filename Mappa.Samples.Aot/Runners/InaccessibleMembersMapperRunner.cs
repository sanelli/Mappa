// <copyright file="InaccessibleMembersMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for inaccessible-members sample mappers.
/// </summary>
internal static class InaccessibleMembersMapperRunner
{
    /// <summary>
    /// Runs all inaccessible-members sample mappers.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var source = AotSampleData.InaccessibleMembersSourceModelAdaThirtySix;

        report.BeginMapper(nameof(InaccessibleMembersMapper));
        var allMembersMapper = new InaccessibleMembersMapper();
        report.RecordInvocation(
            nameof(InaccessibleMembersMapper.Map),
            nameof(InaccessibleMembersSourceModel),
            nameof(InaccessibleMembersTargetModel),
            source,
            allMembersMapper.Map(source));

        report.BeginMapper(nameof(InaccessibleMembersNamedPropertiesAndConstructorMapper));
        var namedPropertiesAndConstructorMapper = new InaccessibleMembersNamedPropertiesAndConstructorMapper();
        report.RecordInvocation(
            nameof(InaccessibleMembersNamedPropertiesAndConstructorMapper.Map),
            nameof(InaccessibleMembersSourceModel),
            nameof(InaccessibleMembersTargetModel),
            source,
            namedPropertiesAndConstructorMapper.Map(source));

        report.BeginMapper(nameof(InaccessibleMembersConstructorOnlyMapper));
        var constructorOnlyMapper = new InaccessibleMembersConstructorOnlyMapper();
        report.RecordInvocation(
            nameof(InaccessibleMembersConstructorOnlyMapper.Map),
            nameof(InaccessibleMembersSourceModel),
            nameof(InaccessibleMembersPublicSettersTargetModel),
            source,
            constructorOnlyMapper.Map(source));

        report.BeginMapper(nameof(InaccessibleMembersNamedPropertiesOnlyMapper));
        var namedPropertiesOnlyMapper = new InaccessibleMembersNamedPropertiesOnlyMapper();
        report.RecordInvocation(
            nameof(InaccessibleMembersNamedPropertiesOnlyMapper.Map),
            nameof(InaccessibleMembersSourceModel),
            nameof(InaccessibleMembersPublicCtorTargetModel),
            source,
            namedPropertiesOnlyMapper.Map(source));
    }
}