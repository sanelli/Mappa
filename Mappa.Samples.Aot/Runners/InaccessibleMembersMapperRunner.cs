// <copyright file="InaccessibleMembersMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="InaccessibleMembersMapper"/>.
/// </summary>
internal static class InaccessibleMembersMapperRunner
{
    /// <summary>
    /// Runs the inaccessible-members sample mapper.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var source = AotSampleData.InaccessibleMembersSourceModelAdaThirtySix;

        report.BeginMapper(nameof(InaccessibleMembersMapper));
        var mapper = new InaccessibleMembersMapper();
        report.RecordInvocation(
            nameof(InaccessibleMembersMapper.Map),
            nameof(InaccessibleMembersSourceModel),
            nameof(InaccessibleMembersTargetModel),
            source,
            mapper.Map(source));
    }
}