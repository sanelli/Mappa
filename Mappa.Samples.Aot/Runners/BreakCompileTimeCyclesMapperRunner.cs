// <copyright file="BreakCompileTimeCyclesMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="BreakCompileTimeCyclesMapper"/>.
/// </summary>
internal static class BreakCompileTimeCyclesMapperRunner
{
    /// <summary>
    /// Runs map methods on <see cref="BreakCompileTimeCyclesMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(BreakCompileTimeCyclesMapper));
        var mapper = new BreakCompileTimeCyclesMapper();

        var closedPerson = new ReferenceHandlingPersonSource { Id = 1 };
        var closedAddress = new ReferenceHandlingAddressSource { Id = 2, Owner = closedPerson };
        closedPerson.Address = closedAddress;
        var closedResult = mapper.Map(closedPerson, new MappaContext());
        report.RecordInvocation(
            nameof(BreakCompileTimeCyclesMapper.Map),
            nameof(ReferenceHandlingPersonSource),
            nameof(ReferenceHandlingPersonTarget),
            closedPerson,
            $"id={closedResult.Id};addressId={closedResult.Address?.Id};ownerSame={ReferenceEquals(closedResult, closedResult.Address?.Owner)}");

        var truncatedPerson = new ReferenceHandlingPersonSource
        {
            Id = 3,
            Address = new ReferenceHandlingAddressSource { Id = 4, Owner = null },
        };
        var truncatedResult = mapper.Map(truncatedPerson, new MappaContext());
        report.RecordInvocation(
            nameof(BreakCompileTimeCyclesMapper.Map),
            nameof(ReferenceHandlingPersonSource),
            nameof(ReferenceHandlingPersonTarget),
            truncatedPerson,
            $"id={truncatedResult.Id};addressId={truncatedResult.Address?.Id};ownerNull={truncatedResult.Address?.Owner is null}");
    }
}