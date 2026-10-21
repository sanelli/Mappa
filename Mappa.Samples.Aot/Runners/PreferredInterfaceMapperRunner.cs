// <copyright file="PreferredInterfaceMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="PreferredInterfaceMapper"/>.
/// </summary>
internal static class PreferredInterfaceMapperRunner
{
    /// <summary>
    /// Runs preferred-interface sample map methods.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(PreferredInterfaceMapper));
        var mapper = new PreferredInterfaceMapper();
        var dualEnumerable = new DualEnumerable();
        var sourceDto = new PreferredInterfaceSourceDto { Items = dualEnumerable };
        var intsForDualCollection = new List<int> { 7, 8 };
        var ints = new List<int> { 9, 8 };

        report.RecordInvocation(
            nameof(PreferredInterfaceMapper.MapImplicitInt),
            nameof(DualEnumerable),
            "List<int>",
            dualEnumerable,
            mapper.MapImplicitInt(dualEnumerable));

        report.RecordInvocation(
            nameof(PreferredInterfaceMapper.MapExplicitString),
            nameof(DualEnumerable),
            "List<string>",
            dualEnumerable,
            mapper.MapExplicitString(dualEnumerable));

        report.RecordInvocation(
            nameof(PreferredInterfaceMapper.MapNestedImplicit),
            nameof(PreferredInterfaceSourceDto),
            nameof(PreferredInterfaceIntTargetDto),
            sourceDto,
            mapper.MapNestedImplicit(sourceDto));

        report.RecordInvocation(
            nameof(PreferredInterfaceMapper.MapNestedExplicitString),
            nameof(PreferredInterfaceSourceDto),
            nameof(PreferredInterfaceStringTargetDto),
            sourceDto,
            mapper.MapNestedExplicitString(sourceDto));

        report.RecordInvocation(
            nameof(PreferredInterfaceMapper.MapToDualCollectionStrings),
            "List<int>",
            nameof(DualCollection),
            intsForDualCollection,
            mapper.MapToDualCollectionStrings(intsForDualCollection));

        report.RecordInvocation(
            nameof(PreferredInterfaceMapper.MapWithUnusedPreferredInterfaceAttribute),
            "List<int>",
            "List<int>",
            ints,
            mapper.MapWithUnusedPreferredInterfaceAttribute(ints));
    }
}