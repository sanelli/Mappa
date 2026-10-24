// <copyright file="ReplaceNullWithEmptyContainerMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="ReplaceNullWithEmptyContainerMapper"/>.
/// </summary>
internal static class ReplaceNullWithEmptyContainerMapperRunner
{
    /// <summary>
    /// Runs map methods on <see cref="ReplaceNullWithEmptyContainerMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(ReplaceNullWithEmptyContainerMapper));
        var mapper = new ReplaceNullWithEmptyContainerMapper();

        report.RecordInvocation(
            nameof(ReplaceNullWithEmptyContainerMapper.Map),
            "List<int>?",
            "List<string>",
            null,
            mapper.Map(null));

        report.RecordInvocation(
            nameof(ReplaceNullWithEmptyContainerMapper.Map),
            "List<int>?",
            "List<string>",
            new List<int> { 1 },
            mapper.Map([1]));
    }
}