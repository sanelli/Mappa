// <copyright file="MappaObjectFactoryMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa;
using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for object factory sample mappers.
/// </summary>
internal static class MappaObjectFactoryMapperRunner
{
    /// <summary>
    /// Runs all object factory sample map methods.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var input = new ObjectFactorySourceModel
        {
            Name = "Ada",
            Value = 5,
        };

        report.BeginMapper(nameof(MappaObjectFactoryEmptyParameterMapper));
        var emptyMapper = new MappaObjectFactoryEmptyParameterMapper();
        report.RecordInvocation(
            nameof(MappaObjectFactoryEmptyParameterMapper.Map),
            nameof(ObjectFactorySourceModel),
            nameof(ObjectFactoryTargetModel),
            input,
            emptyMapper.Map(input));

        report.BeginMapper(nameof(MappaObjectFactoryContextParameterMapper));
        var contextMapper = new MappaObjectFactoryContextParameterMapper();
        MappaContext contextTag = new Dictionary<string, object>
        {
            ["factory-tag"] = "from-context",
        };
        report.RecordInvocation(
            nameof(MappaObjectFactoryContextParameterMapper.Map),
            "ObjectFactorySourceModel, MappaContext",
            nameof(ObjectFactoryTargetModel),
            input,
            contextMapper.Map(input, contextTag));

        report.BeginMapper(nameof(MappaObjectFactorySourceAndContextMapper));
        var sourceAndContextMapper = new MappaObjectFactorySourceAndContextMapper();
        MappaContext contextSuffix = new Dictionary<string, object>
        {
            ["suffix"] = "ctx",
        };
        report.RecordInvocation(
            nameof(MappaObjectFactorySourceAndContextMapper.Map),
            "ObjectFactorySourceModel, MappaContext",
            nameof(ObjectFactoryTargetModel),
            input,
            sourceAndContextMapper.Map(input, contextSuffix));

        report.BeginMapper(nameof(MappaObjectFactorySourceParameterMapper));
        var sourceMapper = new MappaObjectFactorySourceParameterMapper();
        report.RecordInvocation(
            nameof(MappaObjectFactorySourceParameterMapper.Map),
            nameof(ObjectFactorySourceModel),
            nameof(ObjectFactoryTargetModel),
            input,
            sourceMapper.Map(input));

        report.BeginMapper(nameof(MappaObjectFactoryParameterizedMapper));
        var parameterizedMapper = new MappaObjectFactoryParameterizedMapper();
        report.RecordInvocation(
            nameof(MappaObjectFactoryParameterizedMapper.Map),
            nameof(ObjectFactorySourceModel),
            nameof(ObjectFactoryTargetModel),
            input,
            parameterizedMapper.Map(input));
    }
}