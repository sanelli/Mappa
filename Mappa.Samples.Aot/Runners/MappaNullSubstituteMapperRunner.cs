// <copyright file="MappaNullSubstituteMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MappaNullSubstituteMapper"/>.
/// </summary>
internal static class MappaNullSubstituteMapperRunner
{
    /// <summary>
    /// Runs map methods on <see cref="MappaNullSubstituteMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MappaNullSubstituteMapper));
        var mapper = new MappaNullSubstituteMapper();
        var source = new NullSubstituteSourceModel
        {
            Name = null,
            Alias = "Alias",
        };

        report.RecordInvocation(
            nameof(MappaNullSubstituteMapper.Map),
            nameof(NullSubstituteSourceModel),
            nameof(NullSubstituteTargetModel),
            source,
            mapper.Map(source));
    }
}