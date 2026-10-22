// <copyright file="ProtobufOneofPolymorphismMappersRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;
using Mappa.Samples.Models.ProtobufOneof;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for the protobuf oneof polymorphism mapper sample classes.
/// </summary>
internal static class ProtobufOneofPolymorphismMappersRunner
{
    private const string ResultMessage = "ProtobufOneofResult";
    private const string ReplyMessage = "ProtobufOneofReply";
    private const string ResultModel = "ProtobufOneofResultModel";

    /// <summary>
    /// Runs all map methods on the protobuf oneof polymorphism mapper sample classes.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var ok = AotSampleData.ProtobufOneofResultOk;
        var error = AotSampleData.ProtobufOneofResultError;
        var unset = AotSampleData.ProtobufOneofResultUnset;
        var okModel = AotSampleData.ProtobufOneofOkResultModel;
        var errorModel = AotSampleData.ProtobufOneofErrorResultModel;
        var unknownModel = AotSampleData.ProtobufOneofUnknownResultModel;

        report.BeginMapper(nameof(ProtobufOneofToPocoMapper));
        var toPocoMapper = new ProtobufOneofToPocoMapper();
        RecordToPoco(report, nameof(ProtobufOneofToPocoMapper.Map), ok, toPocoMapper.Map(ok));
        RecordToPoco(report, nameof(ProtobufOneofToPocoMapper.Map), error, toPocoMapper.Map(error));

        report.BeginMapper(nameof(ProtobufOneofToPocoWithCustomExceptionMapper));
        var toPocoCustomMapper = new ProtobufOneofToPocoWithCustomExceptionMapper();
        RecordToPoco(report, nameof(ProtobufOneofToPocoWithCustomExceptionMapper.Map), ok, toPocoCustomMapper.Map(ok));
        RecordToPoco(report, nameof(ProtobufOneofToPocoWithCustomExceptionMapper.Map), error, toPocoCustomMapper.Map(error));

        report.BeginMapper(nameof(ProtobufOneofFromPocoMapper));
        var fromPocoMapper = new ProtobufOneofFromPocoMapper();
        RecordFromPoco(report, nameof(ProtobufOneofFromPocoMapper.Map), ResultMessage, okModel, fromPocoMapper.Map(okModel));
        RecordFromPoco(report, nameof(ProtobufOneofFromPocoMapper.Map), ResultMessage, errorModel, fromPocoMapper.Map(errorModel));
        RecordFromPoco(report, nameof(ProtobufOneofFromPocoMapper.Map), ResultMessage, unknownModel, fromPocoMapper.Map(unknownModel));

        report.BeginMapper(nameof(ProtobufOneofFromPocoWithFallbackMapper));
        var fromPocoFallbackMapper = new ProtobufOneofFromPocoWithFallbackMapper();
        RecordFromPoco(report, nameof(ProtobufOneofFromPocoWithFallbackMapper.Map), ReplyMessage, okModel, fromPocoFallbackMapper.Map(okModel));
        RecordFromPoco(report, nameof(ProtobufOneofFromPocoWithFallbackMapper.Map), ReplyMessage, errorModel, fromPocoFallbackMapper.Map(errorModel));
        RecordFromPoco(report, nameof(ProtobufOneofFromPocoWithFallbackMapper.Map), ReplyMessage, unknownModel, fromPocoFallbackMapper.Map(unknownModel));

        report.BeginMapper(nameof(ProtobufOneofToProtobufMapper));
        var toProtobufMapper = new ProtobufOneofToProtobufMapper();
        RecordToProtobuf(report, nameof(ProtobufOneofToProtobufMapper.Map), ok, toProtobufMapper.Map(ok));
        RecordToProtobuf(report, nameof(ProtobufOneofToProtobufMapper.Map), error, toProtobufMapper.Map(error));
        RecordToProtobuf(report, nameof(ProtobufOneofToProtobufMapper.Map), unset, toProtobufMapper.Map(unset));

        report.BeginMapper(nameof(ProtobufOneofToProtobufIgnoringUnsetMapper));
        var toProtobufIgnoringUnsetMapper = new ProtobufOneofToProtobufIgnoringUnsetMapper();
        RecordToProtobuf(report, nameof(ProtobufOneofToProtobufIgnoringUnsetMapper.Map), ok, toProtobufIgnoringUnsetMapper.Map(ok));
        RecordToProtobuf(report, nameof(ProtobufOneofToProtobufIgnoringUnsetMapper.Map), error, toProtobufIgnoringUnsetMapper.Map(error));
        RecordToProtobuf(report, nameof(ProtobufOneofToProtobufIgnoringUnsetMapper.Map), unset, toProtobufIgnoringUnsetMapper.Map(unset));
    }

    private static void RecordToPoco(AotReport report, string method, ProtobufOneofResult source, ProtobufOneofResultModel target)
        => report.RecordInvocation(method, ResultMessage, ResultModel, source, target.GetType().Name + ": " + target.Description);

    private static void RecordFromPoco(AotReport report, string method, string returnType, ProtobufOneofResultModel source, object target)
        => report.RecordInvocation(method, ResultModel, returnType, source.GetType().Name + ": " + source.Description, target);

    private static void RecordToProtobuf(AotReport report, string method, ProtobufOneofResult source, ProtobufOneofReply target)
        => report.RecordInvocation(method, ResultMessage, ReplyMessage, source, target);
}