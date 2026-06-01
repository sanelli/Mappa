// <copyright file="DateAndTimeMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="DateAndTimeMapper"/>.
/// </summary>
internal static class DateAndTimeMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="DateAndTimeMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(DateAndTimeMapper));
        var mapper = new DateAndTimeMapper();
        var utcDateTime = AotSampleData.UtcDateTime;
        var dateOnly = AotSampleData.SampleDateOnly;
        var long100 = AotSampleData.Long100;
        var uint100 = AotSampleData.Uint100;
        var int100 = AotSampleData.Int100;
        var ushort100 = AotSampleData.UShort100;
        var short100 = AotSampleData.Short100;
        var sbyte100 = AotSampleData.SByte100;
        var byte100 = AotSampleData.Byte100;
        var sampleTimeSpan = AotSampleData.SampleTimeSpan;
        var double100 = AotSampleData.Double100;
        var float100 = AotSampleData.Float100;
        var ulong100 = AotSampleData.ULong100;
        var utcDateTimeOffset = AotSampleData.UtcDateTimeOffset;

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapDateTimeToDateOnly),
            "DateTime",
            "DateOnly",
            utcDateTime,
            mapper.MapDateTimeToDateOnly(utcDateTime));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapDateTimeToTimeOnly),
            "DateTime",
            "TimeOnly",
            utcDateTime,
            mapper.MapDateTimeToTimeOnly(utcDateTime));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapDateTimeToLong),
            "DateTime",
            "long",
            utcDateTime,
            mapper.MapDateTimeToLong(utcDateTime));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapDateOnlyToDateTime),
            "DateOnly",
            "DateTime",
            dateOnly,
            mapper.MapDateOnlyToDateTime(dateOnly));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapLongToDateTime),
            "long",
            "DateTime",
            long100,
            mapper.MapLongToDateTime(long100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapUintToDateTime),
            "uint",
            "DateTime",
            uint100,
            mapper.MapUintToDateTime(uint100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapIntToDateTime),
            "int",
            "DateTime",
            int100,
            mapper.MapIntToDateTime(int100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapUShortToDateTime),
            "ushort",
            "DateTime",
            ushort100,
            mapper.MapUShortToDateTime(ushort100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapShortToDateTime),
            "short",
            "DateTime",
            short100,
            mapper.MapShortToDateTime(short100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapSByteToDateTime),
            "sbyte",
            "DateTime",
            sbyte100,
            mapper.MapSByteToDateTime(sbyte100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapByteToDateTime),
            "byte",
            "DateTime",
            byte100,
            mapper.MapByteToDateTime(byte100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapDateOnlyToLong),
            "DateOnly",
            "long",
            dateOnly,
            mapper.MapDateOnlyToLong(dateOnly));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapTimeSpanToDouble),
            "TimeSpan",
            "double",
            sampleTimeSpan,
            mapper.MapTimeSpanToDouble(sampleTimeSpan));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapDoubleToTimeSpan),
            "double",
            "TimeSpan",
            double100,
            mapper.MapDoubleToTimeSpan(double100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapFloatToTimeSpan),
            "float",
            "TimeSpan",
            float100,
            mapper.MapFloatToTimeSpan(float100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapULongToTimeSpan),
            "ulong",
            "TimeSpan",
            ulong100,
            mapper.MapULongToTimeSpan(ulong100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapLongToTimeSpan),
            "long",
            "TimeSpan",
            long100,
            mapper.MapLongToTimeSpan(long100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapUintToTimeSpan),
            "uint",
            "TimeSpan",
            uint100,
            mapper.MapUintToTimeSpan(uint100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapIntToTimeSpan),
            "int",
            "TimeSpan",
            int100,
            mapper.MapIntToTimeSpan(int100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapUShortToTimeSpan),
            "ushort",
            "TimeSpan",
            ushort100,
            mapper.MapUShortToTimeSpan(ushort100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapShortToTimeSpan),
            "short",
            "TimeSpan",
            short100,
            mapper.MapShortToTimeSpan(short100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapSByteToTimeSpan),
            "sbyte",
            "TimeSpan",
            sbyte100,
            mapper.MapSByteToTimeSpan(sbyte100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapByteToTimeSpan),
            "byte",
            "TimeSpan",
            byte100,
            mapper.MapByteToTimeSpan(byte100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapDateTimeOffsetToDateOnly),
            "DateTimeOffset",
            "DateOnly",
            utcDateTimeOffset,
            mapper.MapDateTimeOffsetToDateOnly(utcDateTimeOffset));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapDateTimeOffsetToTimeOnly),
            "DateTimeOffset",
            "TimeOnly",
            utcDateTimeOffset,
            mapper.MapDateTimeOffsetToTimeOnly(utcDateTimeOffset));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapDateTimeOffsetToLong),
            "DateTimeOffset",
            "long",
            utcDateTimeOffset,
            mapper.MapDateTimeOffsetToLong(utcDateTimeOffset));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapLongToDateTimeOffset),
            "long",
            "DateTimeOffset",
            long100,
            mapper.MapLongToDateTimeOffset(long100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapUintToDateTimeOffset),
            "uint",
            "DateTimeOffset",
            uint100,
            mapper.MapUintToDateTimeOffset(uint100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapIntToDateTimeOffset),
            "int",
            "DateTimeOffset",
            int100,
            mapper.MapIntToDateTimeOffset(int100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapUShortToDateTimeOffset),
            "ushort",
            "DateTimeOffset",
            ushort100,
            mapper.MapUShortToDateTimeOffset(ushort100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapShortToDateTimeOffset),
            "short",
            "DateTimeOffset",
            short100,
            mapper.MapShortToDateTimeOffset(short100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapSByteToDateTimeOffset),
            "sbyte",
            "DateTimeOffset",
            sbyte100,
            mapper.MapSByteToDateTimeOffset(sbyte100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapByteToDateTimeOffset),
            "byte",
            "DateTimeOffset",
            byte100,
            mapper.MapByteToDateTimeOffset(byte100));

        report.RecordInvocation(
            nameof(DateAndTimeMapper.MapDateTimeOffsetToDateTime),
            "DateTimeOffset",
            "DateTime",
            utcDateTimeOffset,
            mapper.MapDateTimeOffsetToDateTime(utcDateTimeOffset));
    }
}