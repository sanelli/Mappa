// <copyright file="ReferenceToReferenceWithNullableDisabledMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for nullable-disabled reference mapping sample classes.
/// </summary>
internal static class ReferenceToReferenceWithNullableDisabledMapperRunner
{
    /// <summary>
    /// Runs all map methods on the nullable-disabled reference mapping sample classes.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var source = AotSampleData.SourceClassModel123Three;

        report.BeginMapper(nameof(ReferenceToReferenceWithNullableDisabledMapper));
        var referenceMapper = new ReferenceToReferenceWithNullableDisabledMapper();
        report.RecordInvocation(
            nameof(ReferenceToReferenceWithNullableDisabledMapper.Map),
            "SourceClassModel",
            "TargetClassModel",
            source,
            referenceMapper.Map(source));

        report.BeginMapper(nameof(ReferenceToValueTypeWithNullableDisabledMapper));
        var valueTypeMapper = new ReferenceToValueTypeWithNullableDisabledMapper();
        report.RecordInvocation(
            nameof(ReferenceToValueTypeWithNullableDisabledMapper.MapToInteger),
            "string",
            "int",
            AotSampleData.IntegerString,
            valueTypeMapper.MapToInteger(AotSampleData.IntegerString));

        report.RecordInvocation(
            nameof(ReferenceToValueTypeWithNullableDisabledMapper.MapToNullableInteger),
            "string",
            "int?",
            AotSampleData.IntegerString,
            valueTypeMapper.MapToNullableInteger(AotSampleData.IntegerString));
    }
}