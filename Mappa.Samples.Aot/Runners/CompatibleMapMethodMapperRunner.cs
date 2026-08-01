// <copyright file="CompatibleMapMethodMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="CompatibleMapMethodMapper"/>.
/// </summary>
internal static class CompatibleMapMethodMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="CompatibleMapMethodMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(CompatibleMapMethodMapper));
        var mapper = new CompatibleMapMethodMapper();
        var input = new CompatibleMapMethodSource
        {
            Property = new CompatibleMapMethodDerivedSource
            {
                Value = 42,
            },
        };

        report.RecordInvocation(
            nameof(CompatibleMapMethodMapper.Map),
            nameof(CompatibleMapMethodSource),
            nameof(CompatibleMapMethodTarget),
            input,
            mapper.Map(input));
    }
}