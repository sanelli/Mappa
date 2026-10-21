// <copyright file="ProtobufWellKnownTypesMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="ProtobufWellKnownTypesMapper"/>.
/// </summary>
internal static class ProtobufWellKnownTypesMapperRunner
{
    /// <summary>
    /// Runs map methods on <see cref="ProtobufWellKnownTypesMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(ProtobufWellKnownTypesMapper));
        var mapper = new ProtobufWellKnownTypesMapper();
        var source = AotSampleData.MappaDependencySourceRecord;

        report.RecordInvocation(
            nameof(ProtobufWellKnownTypesMapper.MapTimestamp),
            "MappaDependencySourceRecord",
            "MappaDependencyTargetModel",
            source,
            mapper.MapTimestamp(source));
        report.RecordInvocation(
            nameof(ProtobufWellKnownTypesMapper.MapDateTimeToTimestamp),
            "DateTime",
            "Timestamp",
            source.TimeStamp,
            mapper.MapDateTimeToTimestamp(source.TimeStamp));
        report.RecordInvocation(
            nameof(ProtobufWellKnownTypesMapper.MapTimeSpanToDuration),
            "TimeSpan",
            "Duration",
            TimeSpan.FromMinutes(5),
            mapper.MapTimeSpanToDuration(TimeSpan.FromMinutes(5)));
        report.RecordInvocation(
            nameof(ProtobufWellKnownTypesMapper.MapStringToStringValue),
            "string",
            "StringValue",
            "aot",
            mapper.MapStringToStringValue("aot"));
        report.RecordInvocation(
            nameof(ProtobufWellKnownTypesMapper.MapBoolToBoolValue),
            "bool",
            "BoolValue",
            true,
            mapper.MapBoolToBoolValue(true));
        report.RecordInvocation(
            nameof(ProtobufWellKnownTypesMapper.MapIntToInt32Value),
            "int",
            "Int32Value",
            7,
            mapper.MapIntToInt32Value(7));
        report.RecordInvocation(
            nameof(ProtobufWellKnownTypesMapper.MapDoubleToDoubleValue),
            "double",
            "DoubleValue",
            2.5,
            mapper.MapDoubleToDoubleValue(2.5));
        report.RecordInvocation(
            nameof(ProtobufWellKnownTypesMapper.MapBytesToBytesValue),
            "byte[]",
            "BytesValue",
            new byte[] { 9 },
            mapper.MapBytesToBytesValue(new byte[] { 9 }));
    }
}