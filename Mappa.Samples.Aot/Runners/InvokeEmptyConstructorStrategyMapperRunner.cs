// <copyright file="InvokeEmptyConstructorStrategyMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="InvokeEmptyConstructorStrategyMapper"/>.
/// </summary>
internal static class InvokeEmptyConstructorStrategyMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="InvokeEmptyConstructorStrategyMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(InvokeEmptyConstructorStrategyMapper));
        var mapper = new InvokeEmptyConstructorStrategyMapper();
        var classInput = AotSampleData.SourceClassModel123Three;
        var recordInput = AotSampleData.SourceRecordModelWithEmptyConstructor123Three;

        report.RecordInvocation(
            nameof(InvokeEmptyConstructorStrategyMapper.Map),
            nameof(SourceClassModel),
            nameof(TargetClassModel),
            classInput,
            mapper.Map(classInput));

        report.RecordInvocation(
            nameof(InvokeEmptyConstructorStrategyMapper.Map),
            nameof(SourceRecordModelWithEmptyConstructor),
            nameof(TargetRecordModelWithEmptyConstructor),
            recordInput,
            mapper.Map(recordInput));

        report.RecordInvocation(
            nameof(InvokeEmptyConstructorStrategyMapper.MapWithPrivateSetter),
            nameof(SourceClassModel),
            nameof(TargetClassModelWithOnePrivateSetterProperty),
            classInput,
            mapper.MapWithPrivateSetter(classInput));
    }
}