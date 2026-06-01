// <copyright file="InvokeToStringMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Samples;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for invoke-to-string strategy sample mappers.
/// </summary>
internal static class InvokeToStringMapperRunner
{
    private const int SampleInt = 100;
    private static readonly DateTime SampleDateTime = new(2025, 2, 1, 22, 17, 34, DateTimeKind.Utc);
    private static readonly DateTimeOffset SampleDateTimeOffset = new(SampleDateTime);
    private static readonly DateOnly SampleDateOnly = new(2025, 2, 1);
    private static readonly TimeOnly SampleTimeOnly = new(22, 20, 5);
    private static readonly TimeSpan SampleTimeSpan = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3));
    private static readonly Guid SampleGuid = AotSampleData.SampleGuid;

    /// <summary>
    /// Runs all map methods on invoke-to-string strategy sample mappers.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        RunInvokeToStringMapper(report);
        RunInvokeToStringMapperWithFormatSettingsOnMethod(report);
        RunInvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod(report);
        RunInvokeToStringMapperWithInvariantCultureSettingsOnMethod(report);
        RunInvokeToStringMapperWithCurrentCultureSettingsOnMethod(report);
        RunInvokeToStringMapperWithCustomCultureSettingsOnMethod(report);
        RunInvokeToStringMapperWithFormatSettingsOnClass(report);
        RunInvokeToStringMapperWithFormatAndInvariantCultureSettingsOnClass(report);
        RunInvokeToStringMapperWithInvariantCultureSettingsOnClass(report);
        RunInvokeToStringMapperWithCurrentCultureSettingsOnClass(report);
        RunInvokeToStringMapperWithCustomCultureSettingsOnClass(report);
        RunInvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethodSupersedingTheOnesOnClass(report);
    }

    private static void RunWithInvariantCulture(Action action)
    {
        var culture = CultureInfo.CurrentCulture;
        var uiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
        try
        {
            action();
        }
        finally
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = uiCulture;
        }
    }

    private static void RunInvokeToStringMapper(AotReport report)
    {
        report.BeginMapper(nameof(InvokeToStringMapper));
        var mapper = new InvokeToStringMapper();

        report.RecordInvocation(nameof(InvokeToStringMapper.MapInt), "int", "string", SampleInt, mapper.MapInt(SampleInt));
        report.RecordInvocation(nameof(InvokeToStringMapper.MapDateTime), "DateTime", "string", SampleDateTime, mapper.MapDateTime(SampleDateTime));
        report.RecordInvocation(nameof(InvokeToStringMapper.MapDateTimeOffset), "DateTimeOffset", "string", SampleDateTimeOffset, mapper.MapDateTimeOffset(SampleDateTimeOffset));
        report.RecordInvocation(nameof(InvokeToStringMapper.MapDateOnly), "DateOnly", "string", SampleDateOnly, mapper.MapDateOnly(SampleDateOnly));
        report.RecordInvocation(nameof(InvokeToStringMapper.MapTimeOnly), "TimeOnly", "string", SampleTimeOnly, mapper.MapTimeOnly(SampleTimeOnly));
        report.RecordInvocation(nameof(InvokeToStringMapper.MapTimeSpan), "TimeSpan", "string", SampleTimeSpan, mapper.MapTimeSpan(SampleTimeSpan));
        report.RecordInvocation(nameof(InvokeToStringMapper.MapGuid), "Guid", "string", SampleGuid, mapper.MapGuid(SampleGuid));
    }

    private static void RunInvokeToStringMapperWithFormatSettingsOnMethod(AotReport report)
    {
        report.BeginMapper(nameof(InvokeToStringMapperWithFormatSettingsOnMethod));
        var mapper = new InvokeToStringMapperWithFormatSettingsOnMethod();

        report.RecordInvocation(nameof(InvokeToStringMapperWithFormatSettingsOnMethod.MapDateTime), "DateTime", "string", SampleDateTime, mapper.MapDateTime(SampleDateTime));
        report.RecordInvocation(nameof(InvokeToStringMapperWithFormatSettingsOnMethod.MapDateTimeOffset), "DateTimeOffset", "string", SampleDateTimeOffset, mapper.MapDateTimeOffset(SampleDateTimeOffset));
        report.RecordInvocation(nameof(InvokeToStringMapperWithFormatSettingsOnMethod.MapDateOnly), "DateOnly", "string", SampleDateOnly, mapper.MapDateOnly(SampleDateOnly));
        report.RecordInvocation(nameof(InvokeToStringMapperWithFormatSettingsOnMethod.MapTimeOnly), "TimeOnly", "string", SampleTimeOnly, mapper.MapTimeOnly(SampleTimeOnly));
        report.RecordInvocation(nameof(InvokeToStringMapperWithFormatSettingsOnMethod.MapTimeSpan), "TimeSpan", "string", SampleTimeSpan, mapper.MapTimeSpan(SampleTimeSpan));
        report.RecordInvocation(nameof(InvokeToStringMapperWithFormatSettingsOnMethod.MapGuid), "Guid", "string", SampleGuid, mapper.MapGuid(SampleGuid));
    }

    private static void RunInvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod(AotReport report)
    {
        RunWithInvariantCulture(() =>
        {
            report.BeginMapper(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod));
            var mapper = new InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod();

            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateTime), "DateTime", "string", SampleDateTime, mapper.MapDateTime(SampleDateTime));
            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateTimeOffset), "DateTimeOffset", "string", SampleDateTimeOffset, mapper.MapDateTimeOffset(SampleDateTimeOffset));
            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateOnly), "DateOnly", "string", SampleDateOnly, mapper.MapDateOnly(SampleDateOnly));
            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod.MapTimeOnly), "TimeOnly", "string", SampleTimeOnly, mapper.MapTimeOnly(SampleTimeOnly));
            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod.MapTimeSpan), "TimeSpan", "string", SampleTimeSpan, mapper.MapTimeSpan(SampleTimeSpan));
            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod.MapGuid), "Guid", "string", SampleGuid, mapper.MapGuid(SampleGuid));
        });
    }

    private static void RunInvokeToStringMapperWithInvariantCultureSettingsOnMethod(AotReport report)
    {
        RunWithInvariantCulture(() =>
        {
            report.BeginMapper(nameof(InvokeToStringMapperWithInvariantCultureSettingsOnMethod));
            var mapper = new InvokeToStringMapperWithInvariantCultureSettingsOnMethod();

            report.RecordInvocation(nameof(InvokeToStringMapperWithInvariantCultureSettingsOnMethod.MapDateTime), "DateTime", "string", SampleDateTime, mapper.MapDateTime(SampleDateTime));
            report.RecordInvocation(nameof(InvokeToStringMapperWithInvariantCultureSettingsOnMethod.MapDateTimeOffset), "DateTimeOffset", "string", SampleDateTimeOffset, mapper.MapDateTimeOffset(SampleDateTimeOffset));
            report.RecordInvocation(nameof(InvokeToStringMapperWithInvariantCultureSettingsOnMethod.MapDateOnly), "DateOnly", "string", SampleDateOnly, mapper.MapDateOnly(SampleDateOnly));
            report.RecordInvocation(nameof(InvokeToStringMapperWithInvariantCultureSettingsOnMethod.MapTimeOnly), "TimeOnly", "string", SampleTimeOnly, mapper.MapTimeOnly(SampleTimeOnly));
            report.RecordInvocation(nameof(InvokeToStringMapperWithInvariantCultureSettingsOnMethod.MapTimeSpan), "TimeSpan", "string", SampleTimeSpan, mapper.MapTimeSpan(SampleTimeSpan));
            report.RecordInvocation(nameof(InvokeToStringMapperWithInvariantCultureSettingsOnMethod.MapGuid), "Guid", "string", SampleGuid, mapper.MapGuid(SampleGuid));
        });
    }

    private static void RunInvokeToStringMapperWithCurrentCultureSettingsOnMethod(AotReport report)
    {
        report.BeginMapper(nameof(InvokeToStringMapperWithCurrentCultureSettingsOnMethod));
        var mapper = new InvokeToStringMapperWithCurrentCultureSettingsOnMethod();

        report.RecordInvocation(nameof(InvokeToStringMapperWithCurrentCultureSettingsOnMethod.MapDateTime), "DateTime", "string", SampleDateTime, mapper.MapDateTime(SampleDateTime));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCurrentCultureSettingsOnMethod.MapDateTimeOffset), "DateTimeOffset", "string", SampleDateTimeOffset, mapper.MapDateTimeOffset(SampleDateTimeOffset));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCurrentCultureSettingsOnMethod.MapDateOnly), "DateOnly", "string", SampleDateOnly, mapper.MapDateOnly(SampleDateOnly));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCurrentCultureSettingsOnMethod.MapTimeOnly), "TimeOnly", "string", SampleTimeOnly, mapper.MapTimeOnly(SampleTimeOnly));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCurrentCultureSettingsOnMethod.MapTimeSpan), "TimeSpan", "string", SampleTimeSpan, mapper.MapTimeSpan(SampleTimeSpan));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCurrentCultureSettingsOnMethod.MapGuid), "Guid", "string", SampleGuid, mapper.MapGuid(SampleGuid));
    }

    private static void RunInvokeToStringMapperWithCustomCultureSettingsOnMethod(AotReport report)
    {
        report.BeginMapper(nameof(InvokeToStringMapperWithCustomCultureSettingsOnMethod));
        var mapper = new InvokeToStringMapperWithCustomCultureSettingsOnMethod();

        report.RecordInvocation(nameof(InvokeToStringMapperWithCustomCultureSettingsOnMethod.MapDateTime), "DateTime", "string", SampleDateTime, mapper.MapDateTime(SampleDateTime));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCustomCultureSettingsOnMethod.MapDateTimeOffset), "DateTimeOffset", "string", SampleDateTimeOffset, mapper.MapDateTimeOffset(SampleDateTimeOffset));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCustomCultureSettingsOnMethod.MapDateOnly), "DateOnly", "string", SampleDateOnly, mapper.MapDateOnly(SampleDateOnly));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCustomCultureSettingsOnMethod.MapTimeOnly), "TimeOnly", "string", SampleTimeOnly, mapper.MapTimeOnly(SampleTimeOnly));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCustomCultureSettingsOnMethod.MapTimeSpan), "TimeSpan", "string", SampleTimeSpan, mapper.MapTimeSpan(SampleTimeSpan));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCustomCultureSettingsOnMethod.MapGuid), "Guid", "string", SampleGuid, mapper.MapGuid(SampleGuid));
    }

    private static void RunInvokeToStringMapperWithFormatSettingsOnClass(AotReport report)
    {
        report.BeginMapper(nameof(InvokeToStringMapperWithFormatSettingsOnClass));
        var mapper = new InvokeToStringMapperWithFormatSettingsOnClass();

        report.RecordInvocation(nameof(InvokeToStringMapperWithFormatSettingsOnClass.MapDateTime), "DateTime", "string", SampleDateTime, mapper.MapDateTime(SampleDateTime));
        report.RecordInvocation(nameof(InvokeToStringMapperWithFormatSettingsOnClass.MapDateTimeOffset), "DateTimeOffset", "string", SampleDateTimeOffset, mapper.MapDateTimeOffset(SampleDateTimeOffset));
        report.RecordInvocation(nameof(InvokeToStringMapperWithFormatSettingsOnClass.MapDateOnly), "DateOnly", "string", SampleDateOnly, mapper.MapDateOnly(SampleDateOnly));
        report.RecordInvocation(nameof(InvokeToStringMapperWithFormatSettingsOnClass.MapTimeOnly), "TimeOnly", "string", SampleTimeOnly, mapper.MapTimeOnly(SampleTimeOnly));
        report.RecordInvocation(nameof(InvokeToStringMapperWithFormatSettingsOnClass.MapTimeSpan), "TimeSpan", "string", SampleTimeSpan, mapper.MapTimeSpan(SampleTimeSpan));
        report.RecordInvocation(nameof(InvokeToStringMapperWithFormatSettingsOnClass.MapGuid), "Guid", "string", SampleGuid, mapper.MapGuid(SampleGuid));
    }

    private static void RunInvokeToStringMapperWithFormatAndInvariantCultureSettingsOnClass(AotReport report)
    {
        RunWithInvariantCulture(() =>
        {
            report.BeginMapper(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnClass));
            var mapper = new InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnClass();

            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnClass.MapDateTime), "DateTime", "string", SampleDateTime, mapper.MapDateTime(SampleDateTime));
            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnClass.MapDateTimeOffset), "DateTimeOffset", "string", SampleDateTimeOffset, mapper.MapDateTimeOffset(SampleDateTimeOffset));
            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnClass.MapDateOnly), "DateOnly", "string", SampleDateOnly, mapper.MapDateOnly(SampleDateOnly));
            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnClass.MapTimeOnly), "TimeOnly", "string", SampleTimeOnly, mapper.MapTimeOnly(SampleTimeOnly));
            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnClass.MapTimeSpan), "TimeSpan", "string", SampleTimeSpan, mapper.MapTimeSpan(SampleTimeSpan));
            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnClass.MapGuid), "Guid", "string", SampleGuid, mapper.MapGuid(SampleGuid));
        });
    }

    private static void RunInvokeToStringMapperWithInvariantCultureSettingsOnClass(AotReport report)
    {
        RunWithInvariantCulture(() =>
        {
            report.BeginMapper(nameof(InvokeToStringMapperWithInvariantCultureSettingsOnClass));
            var mapper = new InvokeToStringMapperWithInvariantCultureSettingsOnClass();

            report.RecordInvocation(nameof(InvokeToStringMapperWithInvariantCultureSettingsOnClass.MapDateTime), "DateTime", "string", SampleDateTime, mapper.MapDateTime(SampleDateTime));
            report.RecordInvocation(nameof(InvokeToStringMapperWithInvariantCultureSettingsOnClass.MapDateTimeOffset), "DateTimeOffset", "string", SampleDateTimeOffset, mapper.MapDateTimeOffset(SampleDateTimeOffset));
            report.RecordInvocation(nameof(InvokeToStringMapperWithInvariantCultureSettingsOnClass.MapDateOnly), "DateOnly", "string", SampleDateOnly, mapper.MapDateOnly(SampleDateOnly));
            report.RecordInvocation(nameof(InvokeToStringMapperWithInvariantCultureSettingsOnClass.MapTimeOnly), "TimeOnly", "string", SampleTimeOnly, mapper.MapTimeOnly(SampleTimeOnly));
            report.RecordInvocation(nameof(InvokeToStringMapperWithInvariantCultureSettingsOnClass.MapTimeSpan), "TimeSpan", "string", SampleTimeSpan, mapper.MapTimeSpan(SampleTimeSpan));
            report.RecordInvocation(nameof(InvokeToStringMapperWithInvariantCultureSettingsOnClass.MapGuid), "Guid", "string", SampleGuid, mapper.MapGuid(SampleGuid));
        });
    }

    private static void RunInvokeToStringMapperWithCurrentCultureSettingsOnClass(AotReport report)
    {
        report.BeginMapper(nameof(InvokeToStringMapperWithCurrentCultureSettingsOnClass));
        var mapper = new InvokeToStringMapperWithCurrentCultureSettingsOnClass();

        report.RecordInvocation(nameof(InvokeToStringMapperWithCurrentCultureSettingsOnClass.MapDateTime), "DateTime", "string", SampleDateTime, mapper.MapDateTime(SampleDateTime));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCurrentCultureSettingsOnClass.MapDateTimeOffset), "DateTimeOffset", "string", SampleDateTimeOffset, mapper.MapDateTimeOffset(SampleDateTimeOffset));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCurrentCultureSettingsOnClass.MapDateOnly), "DateOnly", "string", SampleDateOnly, mapper.MapDateOnly(SampleDateOnly));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCurrentCultureSettingsOnClass.MapTimeOnly), "TimeOnly", "string", SampleTimeOnly, mapper.MapTimeOnly(SampleTimeOnly));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCurrentCultureSettingsOnClass.MapTimeSpan), "TimeSpan", "string", SampleTimeSpan, mapper.MapTimeSpan(SampleTimeSpan));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCurrentCultureSettingsOnClass.MapGuid), "Guid", "string", SampleGuid, mapper.MapGuid(SampleGuid));
    }

    private static void RunInvokeToStringMapperWithCustomCultureSettingsOnClass(AotReport report)
    {
        report.BeginMapper(nameof(InvokeToStringMapperWithCustomCultureSettingsOnClass));
        var mapper = new InvokeToStringMapperWithCustomCultureSettingsOnClass();

        report.RecordInvocation(nameof(InvokeToStringMapperWithCustomCultureSettingsOnClass.MapDateTime), "DateTime", "string", SampleDateTime, mapper.MapDateTime(SampleDateTime));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCustomCultureSettingsOnClass.MapDateTimeOffset), "DateTimeOffset", "string", SampleDateTimeOffset, mapper.MapDateTimeOffset(SampleDateTimeOffset));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCustomCultureSettingsOnClass.MapDateOnly), "DateOnly", "string", SampleDateOnly, mapper.MapDateOnly(SampleDateOnly));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCustomCultureSettingsOnClass.MapTimeOnly), "TimeOnly", "string", SampleTimeOnly, mapper.MapTimeOnly(SampleTimeOnly));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCustomCultureSettingsOnClass.MapTimeSpan), "TimeSpan", "string", SampleTimeSpan, mapper.MapTimeSpan(SampleTimeSpan));
        report.RecordInvocation(nameof(InvokeToStringMapperWithCustomCultureSettingsOnClass.MapGuid), "Guid", "string", SampleGuid, mapper.MapGuid(SampleGuid));
    }

    private static void RunInvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethodSupersedingTheOnesOnClass(AotReport report)
    {
        RunWithInvariantCulture(() =>
        {
            report.BeginMapper(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethodSupersedingTheOnesOnClass));
            var mapper = new InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethodSupersedingTheOnesOnClass();

            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethodSupersedingTheOnesOnClass.MapDateTime), "DateTime", "string", SampleDateTime, mapper.MapDateTime(SampleDateTime));
            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethodSupersedingTheOnesOnClass.MapDateTimeOffset), "DateTimeOffset", "string", SampleDateTimeOffset, mapper.MapDateTimeOffset(SampleDateTimeOffset));
            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethodSupersedingTheOnesOnClass.MapDateOnly), "DateOnly", "string", SampleDateOnly, mapper.MapDateOnly(SampleDateOnly));
            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethodSupersedingTheOnesOnClass.MapTimeOnly), "TimeOnly", "string", SampleTimeOnly, mapper.MapTimeOnly(SampleTimeOnly));
            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethodSupersedingTheOnesOnClass.MapTimeSpan), "TimeSpan", "string", SampleTimeSpan, mapper.MapTimeSpan(SampleTimeSpan));
            report.RecordInvocation(nameof(InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethodSupersedingTheOnesOnClass.MapGuid), "Guid", "string", SampleGuid, mapper.MapGuid(SampleGuid));
        });
    }
}