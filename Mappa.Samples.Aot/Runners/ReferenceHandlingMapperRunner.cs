// <copyright file="ReferenceHandlingMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for the reference-handling sample mappers.
/// </summary>
internal static class ReferenceHandlingMapperRunner
{
    /// <summary>
    /// Runs the reference-reusing and max-runtime-depth sample mappers.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(ReferenceReusingCycleMapper));
        var cycleMapper = new ReferenceReusingCycleMapper();
        var closedPerson = new ReferenceHandlingPersonSource { Id = 1 };
        var closedAddress = new ReferenceHandlingAddressSource { Id = 2, Owner = closedPerson };
        closedPerson.Address = closedAddress;
        var closedResult = cycleMapper.MapPerson(closedPerson, new MappaContext());
        report.RecordInvocation(
            nameof(ReferenceReusingCycleMapper.MapPerson),
            nameof(ReferenceHandlingPersonSource),
            nameof(ReferenceHandlingPersonTarget),
            closedPerson,
            $"id={closedResult.Id};addressId={closedResult.Address?.Id};ownerSame={ReferenceEquals(closedResult, closedResult.Address?.Owner)}");

        var truncatedPerson = new ReferenceHandlingPersonSource
        {
            Id = 3,
            Address = new ReferenceHandlingAddressSource { Id = 4, Owner = null },
        };
        var truncatedResult = cycleMapper.MapPerson(truncatedPerson, new MappaContext());
        report.RecordInvocation(
            nameof(ReferenceReusingCycleMapper.MapPerson),
            nameof(ReferenceHandlingPersonSource),
            nameof(ReferenceHandlingPersonTarget),
            truncatedPerson,
            $"id={truncatedResult.Id};addressId={truncatedResult.Address?.Id};ownerNull={truncatedResult.Address?.Owner is null}");

        report.BeginMapper(nameof(MaxRuntimeDepthMapper));
        var depthMapper = new MaxRuntimeDepthMapper();
        var depthSource = new ReferenceHandlingLevel0Source
        {
            Child = new ReferenceHandlingLevel1Source
            {
                Child = new ReferenceHandlingLevel2Source { Value = 42 },
            },
        };
        var depthResult = depthMapper.Map(depthSource, new MappaContext());
        report.RecordInvocation(
            nameof(MaxRuntimeDepthMapper.Map),
            nameof(ReferenceHandlingLevel0Source),
            nameof(ReferenceHandlingLevel0Target),
            depthSource,
            $"value={depthResult.Child.Child.Value}");

        report.BeginMapper(nameof(MaxRuntimeDepthOverflowMapper));
        var overflowMapper = new MaxRuntimeDepthOverflowMapper();
        try
        {
            _ = overflowMapper.Map(depthSource, new MappaContext());
            report.RecordInvocation(
                nameof(MaxRuntimeDepthOverflowMapper.Map),
                nameof(ReferenceHandlingLevel0Source),
                nameof(ReferenceHandlingLevel0Target),
                depthSource,
                "unexpected-success");
        }
        catch (MappaException exception)
        {
            report.RecordInvocation(
                nameof(MaxRuntimeDepthOverflowMapper.Map),
                nameof(ReferenceHandlingLevel0Source),
                nameof(ReferenceHandlingLevel0Target),
                depthSource,
                $"threw={exception.GetType().Name};message={exception.Message}");
        }
    }
}