// <copyright file="InvokeParseMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Samples;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for invoke-parse strategy sample mappers.
/// </summary>
internal static class InvokeParseMapperRunner
{
    private const string NumericInput = "100";
    private const string DateTimeInput = "2025-02-01 22:17:34";
    private const string DateTimeOffsetInput = "2025-02-01 22:17:34";
    private const string DateTimeOffsetFormattedInput = "01-02-2025 34:17:22";
    private const string DateOnlyInput = "2025-02-01";
    private const string DateOnlyFormattedInput = "2025+02+01";
    private const string TimeOnlyInput = "22:20:05";
    private const string TimeOnlyFormattedInput = "22+20+05";
    private const string TimeSpanInput = "22:20:05";
    private const string TimeSpanGeneralInput = "0:18:30:00.0000000";
    private const string CustomClassInput = "this-is-a-string";

    /// <summary>
    /// Runs all map methods on invoke-parse strategy sample mappers.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        RunParseNumericMapper(report);
        RunParseUriMapper(report);
        RunParseMapperWithoutAnySettings(report);
        RunParseMapperWithFormatSettingsOnMethod(report);
        RunWithInvariantCulture(() => RunParseMapperWithFormatAndInvariantCultureSettingsOnMethod(report));
        RunWithInvariantCulture(() => RunParseMapperWithInvariantCultureSettingsOnMethod(report));
        RunParseMapperWithCurrentCultureSettingsOnMethod(report);
        RunParseMapperWithCustomCultureSettingsOnMethod(report);
        RunParseMapperWithDateTimeStyleSettingsOnMethod(report);
        RunWithInvariantCulture(() => RunParseMapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod(report));
        RunParseMapperWithFormatSettingsOnClass(report);
        RunWithInvariantCulture(() => RunParseMapperWithFormatAndInvariantCultureSettingsOnClass(report));
        RunWithInvariantCulture(() => RunParseMapperWithInvariantCultureSettingsOnClass(report));
        RunParseMapperWithCurrentCultureSettingsOnClass(report);
        RunParseMapperWithCustomCultureSettingsOnClass(report);
        RunParseMapperWithDateTimeStyleSettingsOnClass(report);
        RunWithInvariantCulture(() => RunParseMapperWithSettingsOnClassSupersededBySettingsOnMethod(report));
        RunWithInvariantCulture(() => RunParseNumericMapperWithInvariantCultureSettingsOnMethod(report));
        RunParseNumericMapperWithCustomCultureSettingsOnMethod(report);
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

    private static void RunParseNumericMapper(AotReport report)
    {
        report.BeginMapper(nameof(ParseNumericMapper));
        var mapper = new ParseNumericMapper();

        report.RecordInvocation(nameof(ParseNumericMapper.MapToSignedByte), "string", "sbyte", NumericInput, mapper.MapToSignedByte(NumericInput));
        report.RecordInvocation(nameof(ParseNumericMapper.MapToShort), "string", "short", NumericInput, mapper.MapToShort(NumericInput));
        report.RecordInvocation(nameof(ParseNumericMapper.MapToInteger), "string", "int", NumericInput, mapper.MapToInteger(NumericInput));
        report.RecordInvocation(nameof(ParseNumericMapper.MapToLong), "string", "long", NumericInput, mapper.MapToLong(NumericInput));
        report.RecordInvocation(nameof(ParseNumericMapper.MapToByte), "string", "byte", NumericInput, mapper.MapToByte(NumericInput));
        report.RecordInvocation(nameof(ParseNumericMapper.MapToUnsignedShort), "string", "ushort", NumericInput, mapper.MapToUnsignedShort(NumericInput));
        report.RecordInvocation(nameof(ParseNumericMapper.MapToUnsignedInteger), "string", "uint", NumericInput, mapper.MapToUnsignedInteger(NumericInput));
        report.RecordInvocation(nameof(ParseNumericMapper.MapToUnsignedLong), "string", "ulong", NumericInput, mapper.MapToUnsignedLong(NumericInput));
        report.RecordInvocation(nameof(ParseNumericMapper.MapToDecimal), "string", "decimal", NumericInput, mapper.MapToDecimal(NumericInput));
        report.RecordInvocation(nameof(ParseNumericMapper.MapToFloat), "string", "float", NumericInput, mapper.MapToFloat(NumericInput));
        report.RecordInvocation(nameof(ParseNumericMapper.MapToDouble), "string", "double", NumericInput, mapper.MapToDouble(NumericInput));
    }

    private static void RunParseUriMapper(AotReport report)
    {
        report.BeginMapper(nameof(ParseUriMapper));
        var mapper = new ParseUriMapper();
        report.RecordInvocation(nameof(ParseUriMapper.Map), "string", "Uri", AotSampleData.StringToSystemEntitiesUriInput, mapper.Map(AotSampleData.StringToSystemEntitiesUriInput));
    }

    private static void RunParseMapperWithoutAnySettings(AotReport report)
    {
        report.BeginMapper(nameof(ParseMapperWithoutAnySettings));
        var mapper = new ParseMapperWithoutAnySettings();
        var guidInput = AotSampleData.SampleGuid.ToString("N");

        report.RecordInvocation(nameof(ParseMapperWithoutAnySettings.MapDateTime), "string", "DateTime", DateTimeInput, mapper.MapDateTime(DateTimeInput));
        report.RecordInvocation(nameof(ParseMapperWithoutAnySettings.MapDateTimeOffset), "string", "DateTimeOffset", DateTimeOffsetInput, mapper.MapDateTimeOffset(DateTimeOffsetInput));
        report.RecordInvocation(nameof(ParseMapperWithoutAnySettings.MapDateOnly), "string", "DateOnly", DateOnlyInput, mapper.MapDateOnly(DateOnlyInput));
        report.RecordInvocation(nameof(ParseMapperWithoutAnySettings.MapTimeOnly), "string", "TimeOnly", TimeOnlyInput, mapper.MapTimeOnly(TimeOnlyInput));
        report.RecordInvocation(nameof(ParseMapperWithoutAnySettings.MapTimeSpan), "string", "TimeSpan", TimeSpanInput, mapper.MapTimeSpan(TimeSpanInput));
        report.RecordInvocation(nameof(ParseMapperWithoutAnySettings.MapGuid), "string", "Guid", guidInput, mapper.MapGuid(guidInput));
        report.RecordInvocation(nameof(ParseMapperWithoutAnySettings.MapCustomClassWithStaticParse), "string", "CustomClassWithStaticParse", CustomClassInput, mapper.MapCustomClassWithStaticParse(CustomClassInput));
    }

    private static void RunParseMapperWithFormatSettingsOnMethod(AotReport report)
    {
        report.BeginMapper(nameof(ParseMapperWithFormatSettingsOnMethod));
        var mapper = new ParseMapperWithFormatSettingsOnMethod();
        var guidInput = AotSampleData.SampleGuid.ToString(InvokeParseStrategySettings.GuidFormat);

        report.RecordInvocation(nameof(ParseMapperWithFormatSettingsOnMethod.MapDateTime), "string", "DateTime", DateTimeInput, mapper.MapDateTime(DateTimeInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatSettingsOnMethod.MapDateTimeOffset), "string", "DateTimeOffset", DateTimeOffsetInput, mapper.MapDateTimeOffset(DateTimeOffsetInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatSettingsOnMethod.MapDateOnly), "string", "DateOnly", DateOnlyFormattedInput, mapper.MapDateOnly(DateOnlyFormattedInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatSettingsOnMethod.MapTimeOnly), "string", "TimeOnly", TimeOnlyFormattedInput, mapper.MapTimeOnly(TimeOnlyFormattedInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatSettingsOnMethod.MapTimeSpan), "string", "TimeSpan", TimeSpanGeneralInput, mapper.MapTimeSpan(TimeSpanGeneralInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatSettingsOnMethod.MapGuid), "string", "Guid", guidInput, mapper.MapGuid(guidInput));
    }

    private static void RunParseMapperWithFormatAndInvariantCultureSettingsOnMethod(AotReport report)
    {
        report.BeginMapper(nameof(ParseMapperWithFormatAndInvariantCultureSettingsOnMethod));
        var mapper = new ParseMapperWithFormatAndInvariantCultureSettingsOnMethod();
        var guidInput = AotSampleData.SampleGuid.ToString(InvokeParseStrategySettings.GuidFormat);

        report.RecordInvocation(nameof(ParseMapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateTime), "string", "DateTime", DateTimeInput, mapper.MapDateTime(DateTimeInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateTimeOffset), "string", "DateTimeOffset", DateTimeOffsetFormattedInput, mapper.MapDateTimeOffset(DateTimeOffsetFormattedInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateOnly), "string", "DateOnly", DateOnlyFormattedInput, mapper.MapDateOnly(DateOnlyFormattedInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatAndInvariantCultureSettingsOnMethod.MapTimeOnly), "string", "TimeOnly", TimeOnlyFormattedInput, mapper.MapTimeOnly(TimeOnlyFormattedInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatAndInvariantCultureSettingsOnMethod.MapTimeSpan), "string", "TimeSpan", TimeSpanGeneralInput, mapper.MapTimeSpan(TimeSpanGeneralInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatAndInvariantCultureSettingsOnMethod.MapGuid), "string", "Guid", guidInput, mapper.MapGuid(guidInput));
    }

    private static void RunParseMapperWithInvariantCultureSettingsOnMethod(AotReport report)
    {
        report.BeginMapper(nameof(ParseMapperWithInvariantCultureSettingsOnMethod));
        var mapper = new ParseMapperWithInvariantCultureSettingsOnMethod();
        var guidInput = AotSampleData.SampleGuid.ToString();

        report.RecordInvocation(nameof(ParseMapperWithInvariantCultureSettingsOnMethod.MapDateTime), "string", "DateTime", DateTimeInput, mapper.MapDateTime(DateTimeInput));
        report.RecordInvocation(nameof(ParseMapperWithInvariantCultureSettingsOnMethod.MapDateTimeOffset), "string", "DateTimeOffset", DateTimeOffsetInput, mapper.MapDateTimeOffset(DateTimeOffsetInput));
        report.RecordInvocation(nameof(ParseMapperWithInvariantCultureSettingsOnMethod.MapDateOnly), "string", "DateOnly", DateOnlyInput, mapper.MapDateOnly(DateOnlyInput));
        report.RecordInvocation(nameof(ParseMapperWithInvariantCultureSettingsOnMethod.MapTimeOnly), "string", "TimeOnly", TimeOnlyInput, mapper.MapTimeOnly(TimeOnlyInput));
        report.RecordInvocation(nameof(ParseMapperWithInvariantCultureSettingsOnMethod.MapTimeSpan), "string", "TimeSpan", TimeSpanGeneralInput, mapper.MapTimeSpan(TimeSpanGeneralInput));
        report.RecordInvocation(nameof(ParseMapperWithInvariantCultureSettingsOnMethod.MapGuid), "string", "Guid", guidInput, mapper.MapGuid(guidInput));
    }

    private static void RunParseMapperWithCurrentCultureSettingsOnMethod(AotReport report)
    {
        report.BeginMapper(nameof(ParseMapperWithCurrentCultureSettingsOnMethod));
        var mapper = new ParseMapperWithCurrentCultureSettingsOnMethod();
        var guidInput = AotSampleData.SampleGuid.ToString();

        report.RecordInvocation(nameof(ParseMapperWithCurrentCultureSettingsOnMethod.MapDateTime), "string", "DateTime", DateTimeInput, mapper.MapDateTime(DateTimeInput));
        report.RecordInvocation(nameof(ParseMapperWithCurrentCultureSettingsOnMethod.MapDateTimeOffset), "string", "DateTimeOffset", DateTimeOffsetInput, mapper.MapDateTimeOffset(DateTimeOffsetInput));
        report.RecordInvocation(nameof(ParseMapperWithCurrentCultureSettingsOnMethod.MapDateOnly), "string", "DateOnly", DateOnlyInput, mapper.MapDateOnly(DateOnlyInput));
        report.RecordInvocation(nameof(ParseMapperWithCurrentCultureSettingsOnMethod.MapTimeOnly), "string", "TimeOnly", TimeOnlyInput, mapper.MapTimeOnly(TimeOnlyInput));
        report.RecordInvocation(nameof(ParseMapperWithCurrentCultureSettingsOnMethod.MapTimeSpan), "string", "TimeSpan", TimeSpanGeneralInput, mapper.MapTimeSpan(TimeSpanGeneralInput));
        report.RecordInvocation(nameof(ParseMapperWithCurrentCultureSettingsOnMethod.MapGuid), "string", "Guid", guidInput, mapper.MapGuid(guidInput));
    }

    private static void RunParseMapperWithCustomCultureSettingsOnMethod(AotReport report)
    {
        report.BeginMapper(nameof(ParseMapperWithCustomCultureSettingsOnMethod));
        var mapper = new ParseMapperWithCustomCultureSettingsOnMethod();
        var guidInput = AotSampleData.SampleGuid.ToString();
        var customCultureTimeSpanInput = TimeSpan.FromDays(2)
            .Add(TimeSpan.FromHours(1))
            .Add(TimeSpan.FromMinutes(30))
            .ToString("G", CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName));

        report.RecordInvocation(nameof(ParseMapperWithCustomCultureSettingsOnMethod.MapDateTime), "string", "DateTime", DateTimeInput, mapper.MapDateTime(DateTimeInput));
        report.RecordInvocation(nameof(ParseMapperWithCustomCultureSettingsOnMethod.MapDateTimeOffset), "string", "DateTimeOffset", DateTimeOffsetInput, mapper.MapDateTimeOffset(DateTimeOffsetInput));
        report.RecordInvocation(nameof(ParseMapperWithCustomCultureSettingsOnMethod.MapDateOnly), "string", "DateOnly", DateOnlyInput, mapper.MapDateOnly(DateOnlyInput));
        report.RecordInvocation(nameof(ParseMapperWithCustomCultureSettingsOnMethod.MapTimeOnly), "string", "TimeOnly", TimeOnlyInput, mapper.MapTimeOnly(TimeOnlyInput));
        report.RecordInvocation(nameof(ParseMapperWithCustomCultureSettingsOnMethod.MapTimeSpan), "string", "TimeSpan", customCultureTimeSpanInput, mapper.MapTimeSpan(customCultureTimeSpanInput));
        report.RecordInvocation(nameof(ParseMapperWithCustomCultureSettingsOnMethod.MapGuid), "string", "Guid", guidInput, mapper.MapGuid(guidInput));
    }

    private static void RunParseMapperWithDateTimeStyleSettingsOnMethod(AotReport report)
    {
        report.BeginMapper(nameof(ParseMapperWithDateTimeStyleSettingsOnMethod));
        var mapper = new ParseMapperWithDateTimeStyleSettingsOnMethod();

        report.RecordInvocation(nameof(ParseMapperWithDateTimeStyleSettingsOnMethod.MapDateTime), "string", "DateTime", InvokeParseStrategySettings.DateTimeInputWithWhiteSpaces, mapper.MapDateTime(InvokeParseStrategySettings.DateTimeInputWithWhiteSpaces));
        report.RecordInvocation(nameof(ParseMapperWithDateTimeStyleSettingsOnMethod.MapDateTimeOffset), "string", "DateTimeOffset", InvokeParseStrategySettings.DateTimeOffsetInputWithWhiteSpaces, mapper.MapDateTimeOffset(InvokeParseStrategySettings.DateTimeOffsetInputWithWhiteSpaces));
        report.RecordInvocation(nameof(ParseMapperWithDateTimeStyleSettingsOnMethod.MapDateOnly), "string", "DateOnly", InvokeParseStrategySettings.DateOnlyInputWithWhiteSpaces, mapper.MapDateOnly(InvokeParseStrategySettings.DateOnlyInputWithWhiteSpaces));
        report.RecordInvocation(nameof(ParseMapperWithDateTimeStyleSettingsOnMethod.MapTimeOnly), "string", "TimeOnly", InvokeParseStrategySettings.TimeOnlyInputWithWhiteSpaces, mapper.MapTimeOnly(InvokeParseStrategySettings.TimeOnlyInputWithWhiteSpaces));
    }

    private static void RunParseMapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod(AotReport report)
    {
        report.BeginMapper(nameof(ParseMapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod));
        var mapper = new ParseMapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod();
        const string dateTimeOffsetFormattedInputWithWhiteSpaces = " 01-02-2025 34:17:22 ";
        const string dateOnlyFormattedInputWithWhiteSpaces = " 2025+02+01 ";
        const string timeOnlyFormattedInputWithWhiteSpaces = " 22+20+05 ";

        report.RecordInvocation(nameof(ParseMapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod.MapDateTime), "string", "DateTime", InvokeParseStrategySettings.DateTimeInputWithWhiteSpaces, mapper.MapDateTime(InvokeParseStrategySettings.DateTimeInputWithWhiteSpaces));
        report.RecordInvocation(nameof(ParseMapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod.MapDateTimeOffset), "string", "DateTimeOffset", dateTimeOffsetFormattedInputWithWhiteSpaces, mapper.MapDateTimeOffset(dateTimeOffsetFormattedInputWithWhiteSpaces));
        report.RecordInvocation(nameof(ParseMapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod.MapDateOnly), "string", "DateOnly", dateOnlyFormattedInputWithWhiteSpaces, mapper.MapDateOnly(dateOnlyFormattedInputWithWhiteSpaces));
        report.RecordInvocation(nameof(ParseMapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod.MapTimeOnly), "string", "TimeOnly", timeOnlyFormattedInputWithWhiteSpaces, mapper.MapTimeOnly(timeOnlyFormattedInputWithWhiteSpaces));
    }

    private static void RunParseMapperWithFormatSettingsOnClass(AotReport report)
    {
        report.BeginMapper(nameof(ParseMapperWithFormatSettingsOnClass));
        var mapper = new ParseMapperWithFormatSettingsOnClass();
        var guidInput = AotSampleData.SampleGuid.ToString(InvokeParseStrategySettings.GuidFormat);

        report.RecordInvocation(nameof(ParseMapperWithFormatSettingsOnClass.MapDateTime), "string", "DateTime", DateTimeInput, mapper.MapDateTime(DateTimeInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatSettingsOnClass.MapDateTimeOffset), "string", "DateTimeOffset", DateTimeOffsetInput, mapper.MapDateTimeOffset(DateTimeOffsetInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatSettingsOnClass.MapDateOnly), "string", "DateOnly", DateOnlyFormattedInput, mapper.MapDateOnly(DateOnlyFormattedInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatSettingsOnClass.MapTimeOnly), "string", "TimeOnly", TimeOnlyFormattedInput, mapper.MapTimeOnly(TimeOnlyFormattedInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatSettingsOnClass.MapTimeSpan), "string", "TimeSpan", TimeSpanGeneralInput, mapper.MapTimeSpan(TimeSpanGeneralInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatSettingsOnClass.MapGuid), "string", "Guid", guidInput, mapper.MapGuid(guidInput));
    }

    private static void RunParseMapperWithFormatAndInvariantCultureSettingsOnClass(AotReport report)
    {
        report.BeginMapper(nameof(ParseMapperWithFormatAndInvariantCultureSettingsOnClass));
        var mapper = new ParseMapperWithFormatAndInvariantCultureSettingsOnClass();
        var guidInput = AotSampleData.SampleGuid.ToString(InvokeParseStrategySettings.GuidFormat);

        report.RecordInvocation(nameof(ParseMapperWithFormatAndInvariantCultureSettingsOnClass.MapDateTime), "string", "DateTime", DateTimeInput, mapper.MapDateTime(DateTimeInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatAndInvariantCultureSettingsOnClass.MapDateTimeOffset), "string", "DateTimeOffset", DateTimeOffsetFormattedInput, mapper.MapDateTimeOffset(DateTimeOffsetFormattedInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatAndInvariantCultureSettingsOnClass.MapDateOnly), "string", "DateOnly", DateOnlyFormattedInput, mapper.MapDateOnly(DateOnlyFormattedInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatAndInvariantCultureSettingsOnClass.MapTimeOnly), "string", "TimeOnly", TimeOnlyFormattedInput, mapper.MapTimeOnly(TimeOnlyFormattedInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatAndInvariantCultureSettingsOnClass.MapTimeSpan), "string", "TimeSpan", TimeSpanGeneralInput, mapper.MapTimeSpan(TimeSpanGeneralInput));
        report.RecordInvocation(nameof(ParseMapperWithFormatAndInvariantCultureSettingsOnClass.MapGuid), "string", "Guid", guidInput, mapper.MapGuid(guidInput));
    }

    private static void RunParseMapperWithInvariantCultureSettingsOnClass(AotReport report)
    {
        report.BeginMapper(nameof(ParseMapperWithInvariantCultureSettingsOnClass));
        var mapper = new ParseMapperWithInvariantCultureSettingsOnClass();
        var guidInput = AotSampleData.SampleGuid.ToString();

        report.RecordInvocation(nameof(ParseMapperWithInvariantCultureSettingsOnClass.MapDateTime), "string", "DateTime", DateTimeInput, mapper.MapDateTime(DateTimeInput));
        report.RecordInvocation(nameof(ParseMapperWithInvariantCultureSettingsOnClass.MapDateTimeOffset), "string", "DateTimeOffset", DateTimeOffsetInput, mapper.MapDateTimeOffset(DateTimeOffsetInput));
        report.RecordInvocation(nameof(ParseMapperWithInvariantCultureSettingsOnClass.MapDateOnly), "string", "DateOnly", DateOnlyInput, mapper.MapDateOnly(DateOnlyInput));
        report.RecordInvocation(nameof(ParseMapperWithInvariantCultureSettingsOnClass.MapTimeOnly), "string", "TimeOnly", TimeOnlyInput, mapper.MapTimeOnly(TimeOnlyInput));
        report.RecordInvocation(nameof(ParseMapperWithInvariantCultureSettingsOnClass.MapTimeSpan), "string", "TimeSpan", TimeSpanGeneralInput, mapper.MapTimeSpan(TimeSpanGeneralInput));
        report.RecordInvocation(nameof(ParseMapperWithInvariantCultureSettingsOnClass.MapGuid), "string", "Guid", guidInput, mapper.MapGuid(guidInput));
    }

    private static void RunParseMapperWithCurrentCultureSettingsOnClass(AotReport report)
    {
        report.BeginMapper(nameof(ParseMapperWithCurrentCultureSettingsOnClass));
        var mapper = new ParseMapperWithCurrentCultureSettingsOnClass();
        var guidInput = AotSampleData.SampleGuid.ToString();

        report.RecordInvocation(nameof(ParseMapperWithCurrentCultureSettingsOnClass.MapDateTime), "string", "DateTime", DateTimeInput, mapper.MapDateTime(DateTimeInput));
        report.RecordInvocation(nameof(ParseMapperWithCurrentCultureSettingsOnClass.MapDateTimeOffset), "string", "DateTimeOffset", DateTimeOffsetInput, mapper.MapDateTimeOffset(DateTimeOffsetInput));
        report.RecordInvocation(nameof(ParseMapperWithCurrentCultureSettingsOnClass.MapDateOnly), "string", "DateOnly", DateOnlyInput, mapper.MapDateOnly(DateOnlyInput));
        report.RecordInvocation(nameof(ParseMapperWithCurrentCultureSettingsOnClass.MapTimeOnly), "string", "TimeOnly", TimeOnlyInput, mapper.MapTimeOnly(TimeOnlyInput));
        report.RecordInvocation(nameof(ParseMapperWithCurrentCultureSettingsOnClass.MapTimeSpan), "string", "TimeSpan", TimeSpanGeneralInput, mapper.MapTimeSpan(TimeSpanGeneralInput));
        report.RecordInvocation(nameof(ParseMapperWithCurrentCultureSettingsOnClass.MapGuid), "string", "Guid", guidInput, mapper.MapGuid(guidInput));
    }

    private static void RunParseMapperWithCustomCultureSettingsOnClass(AotReport report)
    {
        report.BeginMapper(nameof(ParseMapperWithCustomCultureSettingsOnClass));
        var mapper = new ParseMapperWithCustomCultureSettingsOnClass();
        var guidInput = AotSampleData.SampleGuid.ToString();
        var customCultureTimeSpanInput = TimeSpan.FromDays(2)
            .Add(TimeSpan.FromHours(1))
            .Add(TimeSpan.FromMinutes(30))
            .ToString("G", CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName));

        report.RecordInvocation(nameof(ParseMapperWithCustomCultureSettingsOnClass.MapDateTime), "string", "DateTime", DateTimeInput, mapper.MapDateTime(DateTimeInput));
        report.RecordInvocation(nameof(ParseMapperWithCustomCultureSettingsOnClass.MapDateTimeOffset), "string", "DateTimeOffset", DateTimeOffsetInput, mapper.MapDateTimeOffset(DateTimeOffsetInput));
        report.RecordInvocation(nameof(ParseMapperWithCustomCultureSettingsOnClass.MapDateOnly), "string", "DateOnly", DateOnlyInput, mapper.MapDateOnly(DateOnlyInput));
        report.RecordInvocation(nameof(ParseMapperWithCustomCultureSettingsOnClass.MapTimeOnly), "string", "TimeOnly", TimeOnlyInput, mapper.MapTimeOnly(TimeOnlyInput));
        report.RecordInvocation(nameof(ParseMapperWithCustomCultureSettingsOnClass.MapTimeSpan), "string", "TimeSpan", customCultureTimeSpanInput, mapper.MapTimeSpan(customCultureTimeSpanInput));
        report.RecordInvocation(nameof(ParseMapperWithCustomCultureSettingsOnClass.MapGuid), "string", "Guid", guidInput, mapper.MapGuid(guidInput));
    }

    private static void RunParseMapperWithDateTimeStyleSettingsOnClass(AotReport report)
    {
        report.BeginMapper(nameof(ParseMapperWithDateTimeStyleSettingsOnClass));
        var mapper = new ParseMapperWithDateTimeStyleSettingsOnClass();

        report.RecordInvocation(nameof(ParseMapperWithDateTimeStyleSettingsOnClass.MapDateTime), "string", "DateTime", InvokeParseStrategySettings.DateTimeInputWithWhiteSpaces, mapper.MapDateTime(InvokeParseStrategySettings.DateTimeInputWithWhiteSpaces));
        report.RecordInvocation(nameof(ParseMapperWithDateTimeStyleSettingsOnClass.MapDateTimeOffset), "string", "DateTimeOffset", InvokeParseStrategySettings.DateTimeOffsetInputWithWhiteSpaces, mapper.MapDateTimeOffset(InvokeParseStrategySettings.DateTimeOffsetInputWithWhiteSpaces));
        report.RecordInvocation(nameof(ParseMapperWithDateTimeStyleSettingsOnClass.MapDateOnly), "string", "DateOnly", InvokeParseStrategySettings.DateOnlyInputWithWhiteSpaces, mapper.MapDateOnly(InvokeParseStrategySettings.DateOnlyInputWithWhiteSpaces));
        report.RecordInvocation(nameof(ParseMapperWithDateTimeStyleSettingsOnClass.MapTimeOnly), "string", "TimeOnly", InvokeParseStrategySettings.TimeOnlyInputWithWhiteSpaces, mapper.MapTimeOnly(InvokeParseStrategySettings.TimeOnlyInputWithWhiteSpaces));
    }

    private static void RunParseMapperWithSettingsOnClassSupersededBySettingsOnMethod(AotReport report)
    {
        report.BeginMapper(nameof(ParseMapperWithSettingsOnClassSupersededBySettingsOnMethod));
        var mapper = new ParseMapperWithSettingsOnClassSupersededBySettingsOnMethod();
        var guidInput = AotSampleData.SampleGuid.ToString(InvokeParseStrategySettings.GuidFormat);

        report.RecordInvocation(nameof(ParseMapperWithSettingsOnClassSupersededBySettingsOnMethod.MapDateTime), "string", "DateTime", DateTimeInput, mapper.MapDateTime(DateTimeInput));
        report.RecordInvocation(nameof(ParseMapperWithSettingsOnClassSupersededBySettingsOnMethod.MapDateTimeOffset), "string", "DateTimeOffset", DateTimeOffsetFormattedInput, mapper.MapDateTimeOffset(DateTimeOffsetFormattedInput));
        report.RecordInvocation(nameof(ParseMapperWithSettingsOnClassSupersededBySettingsOnMethod.MapDateOnly), "string", "DateOnly", DateOnlyFormattedInput, mapper.MapDateOnly(DateOnlyFormattedInput));
        report.RecordInvocation(nameof(ParseMapperWithSettingsOnClassSupersededBySettingsOnMethod.MapTimeOnly), "string", "TimeOnly", TimeOnlyFormattedInput, mapper.MapTimeOnly(TimeOnlyFormattedInput));
        report.RecordInvocation(nameof(ParseMapperWithSettingsOnClassSupersededBySettingsOnMethod.MapTimeSpan), "string", "TimeSpan", TimeSpanGeneralInput, mapper.MapTimeSpan(TimeSpanGeneralInput));
        report.RecordInvocation(nameof(ParseMapperWithSettingsOnClassSupersededBySettingsOnMethod.MapGuid), "string", "Guid", guidInput, mapper.MapGuid(guidInput));
    }

    private static void RunParseNumericMapperWithInvariantCultureSettingsOnMethod(AotReport report)
    {
        report.BeginMapper(nameof(ParseNumericMapperWithInvariantCultureSettingsOnMethod));
        var mapper = new ParseNumericMapperWithInvariantCultureSettingsOnMethod();

        report.RecordInvocation(nameof(ParseNumericMapperWithInvariantCultureSettingsOnMethod.MapToInteger), "string", "int", NumericInput, mapper.MapToInteger(NumericInput));
        report.RecordInvocation(nameof(ParseNumericMapperWithInvariantCultureSettingsOnMethod.MapToDecimal), "string", "decimal", "100.50", mapper.MapToDecimal("100.50"));
    }

    private static void RunParseNumericMapperWithCustomCultureSettingsOnMethod(AotReport report)
    {
        report.BeginMapper(nameof(ParseNumericMapperWithCustomCultureSettingsOnMethod));
        var mapper = new ParseNumericMapperWithCustomCultureSettingsOnMethod();

        report.RecordInvocation(nameof(ParseNumericMapperWithCustomCultureSettingsOnMethod.MapToInteger), "string", "int", NumericInput, mapper.MapToInteger(NumericInput));
        report.RecordInvocation(nameof(ParseNumericMapperWithCustomCultureSettingsOnMethod.MapToDecimal), "string", "decimal", "100,50", mapper.MapToDecimal("100,50"));
    }
}