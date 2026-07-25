// <copyright file="EnumMappingConfigurationMappersRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for enum mapping configuration sample mappers.
/// </summary>
internal static class EnumMappingConfigurationMappersRunner
{
    /// <summary>
    /// Runs representative map methods for enum mapping configuration samples.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(EnumMemberIntMapper));
        var memberIntMapper = new EnumMemberIntMapper();
        report.RecordInvocation(
            nameof(EnumMemberIntMapper.Map),
            nameof(ConfigStatus),
            nameof(Int32),
            ConfigStatus.Inactive,
            memberIntMapper.Map(ConfigStatus.Inactive));

        report.BeginMapper(nameof(EnumMemberStringMapper));
        var memberStringMapper = new EnumMemberStringMapper();
        report.RecordInvocation(
            nameof(EnumMemberStringMapper.Map),
            nameof(ConfigStatus),
            nameof(String),
            ConfigStatus.Inactive,
            memberStringMapper.Map(ConfigStatus.Inactive));

        report.BeginMapper(nameof(EnumMemberTwoEnumMapper));
        var memberTwoEnumMapper = new EnumMemberTwoEnumMapper();
        report.RecordInvocation(
            nameof(EnumMemberTwoEnumMapper.Map),
            nameof(ConfigSourceStatus),
            nameof(ConfigTargetStatus),
            ConfigSourceStatus.Offline,
            memberTwoEnumMapper.Map(ConfigSourceStatus.Offline));

        report.BeginMapper(nameof(EnumIgnoreMapper));
        var ignoreMapper = new EnumIgnoreMapper();
        report.RecordInvocation(
            nameof(EnumIgnoreMapper.Map),
            nameof(ConfigStatus),
            nameof(Int32),
            ConfigStatus.Active,
            ignoreMapper.Map(ConfigStatus.Active));

        report.BeginMapper(nameof(EnumDefaultUseDefaultValueIntegralMapper));
        var defaultIntegralMapper = new EnumDefaultUseDefaultValueIntegralMapper();
        report.RecordInvocation(
            nameof(EnumDefaultUseDefaultValueIntegralMapper.Map),
            nameof(ConfigStatus),
            nameof(Int32),
            ConfigStatus.Active,
            defaultIntegralMapper.Map(ConfigStatus.Active));

        report.BeginMapper(nameof(EnumDefaultUseDefaultValueStringMapper));
        var defaultStringMapper = new EnumDefaultUseDefaultValueStringMapper();
        report.RecordInvocation(
            nameof(EnumDefaultUseDefaultValueStringMapper.Map),
            nameof(ConfigStatus),
            nameof(String),
            ConfigStatus.Active,
            defaultStringMapper.Map(ConfigStatus.Active));

        report.BeginMapper(nameof(EnumDefaultUseDefaultValueEnumMapper));
        var defaultEnumMapper = new EnumDefaultUseDefaultValueEnumMapper();
        report.RecordInvocation(
            nameof(EnumDefaultUseDefaultValueEnumMapper.Map),
            nameof(ConfigSourceStatus),
            nameof(ConfigTargetStatus),
            ConfigSourceStatus.Legacy,
            defaultEnumMapper.Map(ConfigSourceStatus.Legacy));

        report.BeginMapper(nameof(EnumIgnoreAndDefaultMapper));
        var ignoreAndDefaultMapper = new EnumIgnoreAndDefaultMapper();
        report.RecordInvocation(
            nameof(EnumIgnoreAndDefaultMapper.Map),
            nameof(ConfigStatus),
            nameof(Int32),
            ConfigStatus.Inactive,
            ignoreAndDefaultMapper.Map(ConfigStatus.Inactive));

        report.BeginMapper(nameof(EnumDefaultThrowMapper));
        var defaultThrowMapper = new EnumDefaultThrowMapper();
        report.RecordInvocation(
            nameof(EnumDefaultThrowMapper.Map),
            nameof(ConfigStatus),
            nameof(Int32),
            ConfigStatus.Active,
            defaultThrowMapper.Map(ConfigStatus.Active));

        var classSource = new EnumConfigSourceModel
        {
            Status = ConfigStatus.Inactive,
            Priority = ConfigPriority.High,
        };
        report.BeginMapper(nameof(EnumConfigClassPropertyMapper));
        var classPropertyMapper = new EnumConfigClassPropertyMapper();
        report.RecordInvocation(
            nameof(EnumConfigClassPropertyMapper.Map),
            nameof(EnumConfigSourceModel),
            nameof(EnumConfigTargetModel),
            classSource,
            classPropertyMapper.Map(classSource));

        var multiDefaultSource = new EnumConfigMultiDefaultSourceModel
        {
            Status = ConfigStatus.Active,
            Priority = ConfigPriority.Normal,
        };
        report.BeginMapper(nameof(EnumConfigMultiDefaultClassMapper));
        var multiDefaultMapper = new EnumConfigMultiDefaultClassMapper();
        report.RecordInvocation(
            nameof(EnumConfigMultiDefaultClassMapper.Map),
            nameof(EnumConfigMultiDefaultSourceModel),
            nameof(EnumConfigMultiDefaultTargetModel),
            multiDefaultSource,
            multiDefaultMapper.Map(multiDefaultSource));
    }
}