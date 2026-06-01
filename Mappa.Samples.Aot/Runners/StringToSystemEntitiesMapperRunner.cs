// <copyright file="StringToSystemEntitiesMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="StringToSystemEntitiesMapper"/> and <see cref="StringToSystemEntitiesWithSettingsMapper"/>.
/// </summary>
internal static class StringToSystemEntitiesMapperRunner
{
    /// <summary>
    /// Runs all map methods on the string-to-system-entities mapper sample classes.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(StringToSystemEntitiesMapper));
        var mapper = new StringToSystemEntitiesMapper();

        report.RecordInvocation(
            nameof(StringToSystemEntitiesMapper.MapToDateTime),
            "string",
            "DateTime",
            AotSampleData.StringToSystemEntitiesDateTimeInput,
            mapper.MapToDateTime(AotSampleData.StringToSystemEntitiesDateTimeInput));

        report.RecordInvocation(
            nameof(StringToSystemEntitiesMapper.MapToDateTimeOffset),
            "string",
            "DateTimeOffset",
            AotSampleData.StringToSystemEntitiesDateTimeOffsetInput,
            mapper.MapToDateTimeOffset(AotSampleData.StringToSystemEntitiesDateTimeOffsetInput));

        report.RecordInvocation(
            nameof(StringToSystemEntitiesMapper.MapToTimeSpan),
            "string",
            "TimeSpan",
            AotSampleData.StringToSystemEntitiesTimeSpanInput,
            mapper.MapToTimeSpan(AotSampleData.StringToSystemEntitiesTimeSpanInput));

        report.RecordInvocation(
            nameof(StringToSystemEntitiesMapper.MapToTimeOnly),
            "string",
            "TimeOnly",
            AotSampleData.StringToSystemEntitiesTimeOnlyInput,
            mapper.MapToTimeOnly(AotSampleData.StringToSystemEntitiesTimeOnlyInput));

        report.RecordInvocation(
            nameof(StringToSystemEntitiesMapper.MapToDateOnly),
            "string",
            "DateOnly",
            AotSampleData.StringToSystemEntitiesDateOnlyInput,
            mapper.MapToDateOnly(AotSampleData.StringToSystemEntitiesDateOnlyInput));

        report.RecordInvocation(
            nameof(StringToSystemEntitiesMapper.MapToGuid),
            "string",
            "Guid",
            AotSampleData.StringToSystemEntitiesGuidInput,
            mapper.MapToGuid(AotSampleData.StringToSystemEntitiesGuidInput));

        report.RecordInvocation(
            nameof(StringToSystemEntitiesMapper.MapToUri),
            "string",
            "Uri",
            AotSampleData.StringToSystemEntitiesUriInput,
            mapper.MapToUri(AotSampleData.StringToSystemEntitiesUriInput));

        report.BeginMapper(nameof(StringToSystemEntitiesWithSettingsMapper));
        var mapperWithSettings = new StringToSystemEntitiesWithSettingsMapper();

        report.RecordInvocation(
            nameof(StringToSystemEntitiesWithSettingsMapper.MapToDateTime),
            "string",
            "DateTime",
            AotSampleData.StringToSystemEntitiesSettingsDateTimeInput,
            mapperWithSettings.MapToDateTime(AotSampleData.StringToSystemEntitiesSettingsDateTimeInput));

        report.RecordInvocation(
            nameof(StringToSystemEntitiesWithSettingsMapper.MapToDateTimeOffset),
            "string",
            "DateTimeOffset",
            AotSampleData.StringToSystemEntitiesSettingsDateTimeOffsetInput,
            mapperWithSettings.MapToDateTimeOffset(AotSampleData.StringToSystemEntitiesSettingsDateTimeOffsetInput));

        report.RecordInvocation(
            nameof(StringToSystemEntitiesWithSettingsMapper.MapToTimeSpan),
            "string",
            "TimeSpan",
            AotSampleData.StringToSystemEntitiesSettingsTimeSpanInput,
            mapperWithSettings.MapToTimeSpan(AotSampleData.StringToSystemEntitiesSettingsTimeSpanInput));

        report.RecordInvocation(
            nameof(StringToSystemEntitiesWithSettingsMapper.MapToTimeOnly),
            "string",
            "TimeOnly",
            AotSampleData.StringToSystemEntitiesSettingsTimeOnlyInput,
            mapperWithSettings.MapToTimeOnly(AotSampleData.StringToSystemEntitiesSettingsTimeOnlyInput));

        report.RecordInvocation(
            nameof(StringToSystemEntitiesWithSettingsMapper.MapToDateOnly),
            "string",
            "DateOnly",
            AotSampleData.StringToSystemEntitiesSettingsDateOnlyInput,
            mapperWithSettings.MapToDateOnly(AotSampleData.StringToSystemEntitiesSettingsDateOnlyInput));

        report.RecordInvocation(
            nameof(StringToSystemEntitiesWithSettingsMapper.MapToGuid),
            "string",
            "Guid",
            AotSampleData.StringToSystemEntitiesSettingsGuidInput,
            mapperWithSettings.MapToGuid(AotSampleData.StringToSystemEntitiesSettingsGuidInput));
    }
}