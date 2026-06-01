// <copyright file="MapWithPropertiesOnBaseClassesMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MapWithPropertiesOnBaseClassesMapper"/>.
/// </summary>
internal static class MapWithPropertiesOnBaseClassesMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="MapWithPropertiesOnBaseClassesMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MapWithPropertiesOnBaseClassesMapper));
        var mapper = new MapWithPropertiesOnBaseClassesMapper();
        var source = AotSampleData.DerivedClassSourceModel;

        report.RecordInvocation(
            nameof(MapWithPropertiesOnBaseClassesMapper.MapToClassWithProperties),
            "DerivedClassSourceModel",
            "DerivedClassTargetModel",
            source,
            mapper.MapToClassWithProperties(source));

        report.RecordInvocation(
            nameof(MapWithPropertiesOnBaseClassesMapper.MapToClassWithConstructor),
            "DerivedClassSourceModel",
            "DerivedClassTargetModelWithConstructor",
            source,
            mapper.MapToClassWithConstructor(source));

        var interfaceSource = AotSampleData.DerivedInterfaceModel;
        report.RecordInvocation(
            nameof(MapWithPropertiesOnBaseClassesMapper.MapFromInterface),
            "IDerivedInterfaceModel",
            "TargetForDerivedInterfaceModel",
            interfaceSource,
            mapper.MapFromInterface(interfaceSource));
    }
}