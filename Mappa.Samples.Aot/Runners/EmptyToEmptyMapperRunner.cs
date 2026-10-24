// <copyright file="EmptyToEmptyMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="EmptyToEmptyMapper"/>.
/// </summary>
internal static class EmptyToEmptyMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="EmptyToEmptyMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(EmptyToEmptyMapper));
        var mapper = new EmptyToEmptyMapper();

        var emptySource = AotSampleData.EmptySourceModel;
        report.RecordInvocation(
            nameof(EmptyToEmptyMapper.Map),
            nameof(EmptySourceModel),
            nameof(EmptyTargetModel),
            emptySource,
            mapper.Map(emptySource));

        var nestedSource = AotSampleData.SourceWithEmptyNestedModel;
        report.RecordInvocation(
            nameof(EmptyToEmptyMapper.Map),
            nameof(SourceWithEmptyNestedModel),
            nameof(TargetWithEmptyNestedModel),
            nestedSource,
            mapper.Map(nestedSource));
    }
}