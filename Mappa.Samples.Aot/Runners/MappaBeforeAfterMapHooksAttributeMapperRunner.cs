// <copyright file="MappaBeforeAfterMapHooksAttributeMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa;
using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MappaBeforeAfterMapHooksAttributeMapper"/>.
/// </summary>
internal static class MappaBeforeAfterMapHooksAttributeMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="MappaBeforeAfterMapHooksAttributeMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MappaBeforeAfterMapHooksAttributeMapper));
        var mapper = new MappaBeforeAfterMapHooksAttributeMapper();
        MappaContext context = new Dictionary<string, object>
        {
            ["suffix"] = "ctx",
        };
        var person = new BeforeAfterMapHookPersonModel
        {
            Name = "Ada",
            Score = 0,
        };
        var counter = new BeforeAfterMapHookCounterModel
        {
            Value = 7,
        };

        report.RecordInvocation(
            nameof(MappaBeforeAfterMapHooksAttributeMapper.MapPerson),
            "BeforeAfterMapHookPersonModel, MappaContext",
            nameof(BeforeAfterMapHookPersonModel),
            person,
            mapper.MapPerson(person, context));

        report.RecordInvocation(
            nameof(MappaBeforeAfterMapHooksAttributeMapper.MapCounter),
            nameof(BeforeAfterMapHookCounterModel),
            nameof(BeforeAfterMapHookCounterModel),
            counter,
            mapper.MapCounter(counter));
    }
}