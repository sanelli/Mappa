// <copyright file="NestedPropertyPathAttributeMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa;
using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for nested property path attribute sample mappers.
/// </summary>
internal static class NestedPropertyPathAttributeMapperRunner
{
    /// <summary>
    /// Runs all nested property path sample mappers.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var personSource = new NestedPropertyPathPersonSourceModel
        {
            Address = new NestedPropertyPathAddressModel
            {
                City = "Rome",
                ZipCode = "00100",
            },
        };

        var locationSource = new NestedPropertyPathLocationSourceModel
        {
            Location = new NestedPropertyPathLocationModel
            {
                Address = new NestedPropertyPathAddressModel
                {
                    City = "Milan",
                    ZipCode = "20100",
                },
            },
        };

        report.BeginMapper(nameof(NestedPropertyPathAttributeMapper));
        var twoSegmentMapper = new NestedPropertyPathAttributeMapper();
        report.RecordInvocation(
            nameof(NestedPropertyPathAttributeMapper.MapWithTwoSegmentUseProperty),
            "NestedPropertyPathPersonSourceModel",
            "NestedPropertyPathPersonTargetModel",
            personSource,
            twoSegmentMapper.MapWithTwoSegmentUseProperty(personSource));

        report.BeginMapper(nameof(NestedPropertyPathThreeSegmentUsePropertyAttributeMapper));
        var threeSegmentMapper = new NestedPropertyPathThreeSegmentUsePropertyAttributeMapper();
        report.RecordInvocation(
            nameof(NestedPropertyPathThreeSegmentUsePropertyAttributeMapper.Map),
            "NestedPropertyPathLocationSourceModel",
            "NestedPropertyPathPersonTargetModel",
            locationSource,
            threeSegmentMapper.Map(locationSource));

        report.BeginMapper(nameof(NestedPropertyPathNestedSourceOnFlatTargetAttributeMapper));
        var nestedSourceOnFlatTargetMapper = new NestedPropertyPathNestedSourceOnFlatTargetAttributeMapper();
        report.RecordInvocation(
            nameof(NestedPropertyPathNestedSourceOnFlatTargetAttributeMapper.Map),
            "NestedPropertyPathLocationSourceModel",
            "NestedPropertyPathCityTargetModel",
            locationSource,
            nestedSourceOnFlatTargetMapper.Map(locationSource));

        report.BeginMapper(nameof(NestedPropertyPathInvokeMethodAttributeMapper));
        var invokeMethodMapper = new NestedPropertyPathInvokeMethodAttributeMapper();
        report.RecordInvocation(
            nameof(NestedPropertyPathInvokeMethodAttributeMapper.Map),
            "NestedPropertyPathPersonSourceModel",
            "NestedPropertyPathPersonTargetModel",
            personSource,
            invokeMethodMapper.Map(personSource));

        report.BeginMapper(nameof(NestedPropertyPathAssignFromConstantAttributeMapper));
        var assignFromConstantMapper = new NestedPropertyPathAssignFromConstantAttributeMapper();
        report.RecordInvocation(
            nameof(NestedPropertyPathAssignFromConstantAttributeMapper.Map),
            "NestedPropertyPathPersonSourceModel",
            "NestedPropertyPathPersonTargetModel",
            personSource,
            assignFromConstantMapper.Map(personSource));

        report.BeginMapper(nameof(NestedPropertyPathAssignFromContextAttributeMapper));
        var assignFromContextMapper = new NestedPropertyPathAssignFromContextAttributeMapper();
        MappaContext fromContext = new Dictionary<string, object> { ["city"] = "Florence" };
        report.RecordInvocation(
            nameof(NestedPropertyPathAssignFromContextAttributeMapper.Map),
            "NestedPropertyPathPersonSourceModel, MappaContext",
            "NestedPropertyPathPersonTargetModel",
            personSource,
            assignFromContextMapper.Map(personSource, fromContext));

        report.BeginMapper(nameof(NestedPropertyPathAssignToContextAttributeMapper));
        var assignToContextMapper = new NestedPropertyPathAssignToContextAttributeMapper();
        MappaContext toContext = new Dictionary<string, object>();
        report.RecordInvocation(
            nameof(NestedPropertyPathAssignToContextAttributeMapper.Map),
            "NestedPropertyPathPersonSourceModel, MappaContext",
            "NestedPropertyPathPersonTargetModel",
            personSource,
            assignToContextMapper.Map(personSource, toContext));

        report.BeginMapper(nameof(NestedPropertyPathIgnoreTargetPropertyAttributeMapper));
        var ignoreMapper = new NestedPropertyPathIgnoreTargetPropertyAttributeMapper();
        report.RecordInvocation(
            nameof(NestedPropertyPathIgnoreTargetPropertyAttributeMapper.Map),
            "NestedPropertyPathPersonSourceModel",
            "NestedPropertyPathPersonTargetModel",
            personSource,
            ignoreMapper.Map(personSource));
    }
}