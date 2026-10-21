// <copyright file="MappaUseConstructorMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MappaUseConstructorMapper"/>.
/// </summary>
internal static class MappaUseConstructorMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="MappaUseConstructorMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MappaUseConstructorMapper));
        var mapper = new MappaUseConstructorMapper();
        var source = new UseConstructorSource { PropertyA = 42, PropertyB = "hello" };
        var nestedSource = new UseConstructorNestedOuterSource
        {
            Nested = new UseConstructorNestedInnerSource { PropertyA = "nested" },
        };

        report.RecordInvocation(
            nameof(MappaUseConstructorMapper.MapForceEmpty),
            nameof(UseConstructorSource),
            nameof(UseConstructorEmptyPreferredTarget),
            source,
            mapper.MapForceEmpty(source));

        report.RecordInvocation(
            nameof(MappaUseConstructorMapper.MapForceParameterized),
            nameof(UseConstructorSource),
            nameof(UseConstructorParameterizedPreferredTarget),
            source,
            mapper.MapForceParameterized(source));

        report.RecordInvocation(
            nameof(MappaUseConstructorMapper.MapExactSignature),
            nameof(UseConstructorSource),
            nameof(UseConstructorExactSignatureTarget),
            source,
            mapper.MapExactSignature(source));

        report.RecordInvocation(
            nameof(MappaUseConstructorMapper.MapNestedTyped),
            nameof(UseConstructorNestedOuterSource),
            nameof(UseConstructorNestedOuterTarget),
            nestedSource,
            mapper.MapNestedTyped(nestedSource));
    }
}