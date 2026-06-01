// <copyright file="InvokeMappingConstructorStrategyMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="InvokeMappingConstructorStrategyMapper"/>.
/// </summary>
internal static class InvokeMappingConstructorStrategyMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="InvokeMappingConstructorStrategyMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(InvokeMappingConstructorStrategyMapper));
        var mapper = new InvokeMappingConstructorStrategyMapper();
        var classInput = AotSampleData.SourceClassModel123Three;
        var enumInput = CountingValues.Three;

        report.RecordInvocation(
            nameof(InvokeMappingConstructorStrategyMapper.MapToClassWithSingleMappingConstructor),
            nameof(SourceClassModel),
            nameof(TargetClassModelWithSingleMapperConstructorFromSourceClassModel),
            classInput,
            mapper.MapToClassWithSingleMappingConstructor(classInput));

        report.RecordInvocation(
            nameof(InvokeMappingConstructorStrategyMapper.MapToClassWithMultipleMappingConstructors),
            nameof(CountingValues),
            nameof(TargetClassModelWithMultipleMapperConstructors),
            enumInput,
            mapper.MapToClassWithMultipleMappingConstructors(enumInput));
    }
}