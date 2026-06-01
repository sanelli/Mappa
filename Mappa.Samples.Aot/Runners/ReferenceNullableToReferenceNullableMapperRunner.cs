// <copyright file="ReferenceNullableToReferenceNullableMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for nullable reference mapping sample classes.
/// </summary>
internal static class ReferenceNullableToReferenceNullableMapperRunner
{
    /// <summary>
    /// Runs all map methods on the nullable reference mapping sample classes.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var source = AotSampleData.SourceClassModel123Three;

        report.BeginMapper(nameof(ReferenceNullableToReferenceNullableMapper));
        var referenceMapper = new ReferenceNullableToReferenceNullableMapper();
        report.RecordInvocation(
            nameof(ReferenceNullableToReferenceNullableMapper.MapReferenceNullableToReferenceNullable),
            "SourceClassModel?",
            "TargetClassModel?",
            source,
            referenceMapper.MapReferenceNullableToReferenceNullable(source));

        report.RecordInvocation(
            nameof(ReferenceNullableToReferenceNullableMapper.MapToReferenceNullable),
            "SourceClassModel",
            "TargetClassModel?",
            source,
            referenceMapper.MapToReferenceNullable(source));

        report.RecordInvocation(
            nameof(ReferenceNullableToReferenceNullableMapper.MapFromReferenceNullable),
            "SourceClassModel?",
            "TargetClassModel",
            source,
            referenceMapper.MapFromReferenceNullable(source));

        report.BeginMapper(nameof(ReferenceToValueTypeNullableMapper));
        var valueTypeMapper = new ReferenceToValueTypeNullableMapper();
        report.RecordInvocation(
            nameof(ReferenceToValueTypeNullableMapper.MapToInteger),
            "string",
            "int",
            AotSampleData.IntegerString,
            valueTypeMapper.MapToInteger(AotSampleData.IntegerString));

        report.RecordInvocation(
            nameof(ReferenceToValueTypeNullableMapper.MapToNullableInteger),
            "string",
            "int?",
            AotSampleData.IntegerString,
            valueTypeMapper.MapToNullableInteger(AotSampleData.IntegerString));

        report.BeginMapper(nameof(NullableReferenceToValueTypeNullableMapper));
        var nullableReferenceMapper = new NullableReferenceToValueTypeNullableMapper();
        report.RecordInvocation(
            nameof(NullableReferenceToValueTypeNullableMapper.MapToInteger),
            "string?",
            "int",
            AotSampleData.IntegerString,
            nullableReferenceMapper.MapToInteger(AotSampleData.IntegerString));

        report.RecordInvocation(
            nameof(NullableReferenceToValueTypeNullableMapper.MapToNullableInteger),
            "string?",
            "int?",
            AotSampleData.IntegerString,
            nullableReferenceMapper.MapToNullableInteger(AotSampleData.IntegerString));
    }
}