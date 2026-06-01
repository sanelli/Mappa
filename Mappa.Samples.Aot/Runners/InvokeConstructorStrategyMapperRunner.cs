// <copyright file="InvokeConstructorStrategyMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="InvokeConstructorStrategyMapper"/>.
/// </summary>
internal static class InvokeConstructorStrategyMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="InvokeConstructorStrategyMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(InvokeConstructorStrategyMapper));
        var mapper = new InvokeConstructorStrategyMapper();
        var input = AotSampleData.SourceRecordModel123Three;

        report.RecordInvocation(
            nameof(InvokeConstructorStrategyMapper.Map),
            nameof(SourceRecordModel),
            nameof(TargetRecordModel),
            input,
            mapper.Map(input));
    }
}