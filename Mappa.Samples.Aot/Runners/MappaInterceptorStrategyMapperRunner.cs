// <copyright file="MappaInterceptorStrategyMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MappaInterceptorStrategyMapper"/>.
/// </summary>
internal static class MappaInterceptorStrategyMapperRunner
{
    /// <summary>
    /// Runs map methods on <see cref="MappaInterceptorStrategyMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MappaInterceptorStrategyMapper));
        var mapper = new MappaInterceptorStrategyMapper();

        report.RecordInvocation(
            nameof(MappaInterceptorStrategyMapper.MapNullable),
            "int?",
            "long",
            7,
            mapper.MapNullable(7));

        report.RecordInvocation(
            nameof(MappaInterceptorStrategyMapper.MapEnum),
            nameof(InterceptorSourceStatus),
            nameof(InterceptorTargetStatus),
            InterceptorSourceStatus.Active,
            mapper.MapEnum(InterceptorSourceStatus.Active));

        report.RecordInvocation(
            nameof(MappaInterceptorStrategyMapper.MapStringToInt),
            "string",
            "int",
            "42",
            mapper.MapStringToInt("42"));

        report.RecordInvocation(
            nameof(MappaInterceptorStrategyMapper.MapLongToDateTimeOffset),
            "long",
            nameof(DateTimeOffset),
            1_700_000_000L,
            mapper.MapLongToDateTimeOffset(1_700_000_000L));

        report.RecordInvocation(
            nameof(MappaInterceptorStrategyMapper.MapList),
            "List<int>",
            "List<string>",
            new List<int> { 1, 2, 3 },
            mapper.MapList([1, 2, 3]));

        report.RecordInvocation(
            nameof(MappaInterceptorStrategyMapper.MapDictionary),
            "Dictionary<string, int>",
            "Dictionary<string, long>",
            new Dictionary<string, int> { ["a"] = 1 },
            mapper.MapDictionary(new Dictionary<string, int> { ["a"] = 1 }));

        report.RecordInvocation(
            nameof(MappaInterceptorStrategyMapper.MapTuple),
            "Tuple<int, string>",
            "Tuple<string, string>",
            Tuple.Create(9, "x"),
            mapper.MapTuple(Tuple.Create(9, "x")));

        var guid = AotSampleData.SampleGuid;
        report.RecordInvocation(
            nameof(MappaInterceptorStrategyMapper.MapGuid),
            nameof(Guid),
            "byte[]",
            guid,
            mapper.MapGuid(guid));

        var person = new InterceptorPersonSource { Name = "Ada", Age = 36 };
        report.RecordInvocation(
            nameof(MappaInterceptorStrategyMapper.MapPerson),
            nameof(InterceptorPersonSource),
            nameof(InterceptorPersonTarget),
            person,
            mapper.MapPerson(person));
    }
}