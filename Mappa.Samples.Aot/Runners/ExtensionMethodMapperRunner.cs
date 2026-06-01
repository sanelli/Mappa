// <copyright file="ExtensionMethodMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="ExtensionMethodMapper"/>.
/// </summary>
internal static class ExtensionMethodMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="ExtensionMethodMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(ExtensionMethodMapper));
        const int input = 123;

        report.RecordInvocation(
            nameof(ExtensionMethodMapper.MapToLong),
            "int",
            "long",
            input,
            input.MapToLong());
    }
}