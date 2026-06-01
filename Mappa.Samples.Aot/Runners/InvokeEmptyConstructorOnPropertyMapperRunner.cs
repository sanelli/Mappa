// <copyright file="InvokeEmptyConstructorOnPropertyMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="InvokeEmptyConstructorOnPropertyMapper"/>.
/// </summary>
internal static class InvokeEmptyConstructorOnPropertyMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="InvokeEmptyConstructorOnPropertyMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(InvokeEmptyConstructorOnPropertyMapper));
        var mapper = new InvokeEmptyConstructorOnPropertyMapper();
        var input = AotSampleData.SourceClassWithInnerClassModel33One;

        report.RecordInvocation(
            nameof(InvokeEmptyConstructorOnPropertyMapper.Map),
            nameof(SourceClassWithInnerClassModel),
            nameof(TargetClassWithInnerClassModel),
            input,
            mapper.Map(input));
    }
}