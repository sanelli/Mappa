// <copyright file="ProtobufOptionalMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="ProtobufOptionalMapper"/>.
/// </summary>
internal static class ProtobufOptionalMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="ProtobufOptionalMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(ProtobufOptionalMapper));
        var mapper = new ProtobufOptionalMapper();
        var protobufSource = AotSampleData.SourceProtobufOptionalModelWithValues;

        report.RecordInvocation(
            nameof(ProtobufOptionalMapper.Map),
            "SourceProtobufOptionalModel",
            "TargetClassModel",
            protobufSource,
            mapper.Map(protobufSource));

        var classSource = AotSampleData.SourceClassModel33Three;
        report.RecordInvocation(
            nameof(ProtobufOptionalMapper.MapToOptionalProtobuf),
            "SourceClassModel",
            "TargetProtobufOptionalModel",
            classSource,
            mapper.MapToOptionalProtobuf(classSource));

        report.RecordInvocation(
            nameof(ProtobufOptionalMapper.MapToOptionalProtobuf),
            "SourceProtobufOptionalModel",
            "TargetProtobufOptionalModel",
            protobufSource,
            mapper.MapToOptionalProtobuf(protobufSource));
    }
}