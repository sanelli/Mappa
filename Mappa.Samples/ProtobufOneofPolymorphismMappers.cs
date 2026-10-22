// <copyright file="ProtobufOneofPolymorphismMappers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402

using Mappa.Attributes;
using Mappa.Samples.Models;
using Mappa.Samples.Models.Polymorphism;
using Mappa.Samples.Models.ProtobufOneof;

namespace Mappa.Samples;

/// <summary>
/// Maps a protobuf message with a <c>oneof</c> to a POCO hierarchy by dispatching on the
/// <c>ResultCase</c> of the source. No <see cref="MappaProtobufTypeMappingDefaultAttribute"/> is
/// defined, hence an unset oneof (<c>ResultOneofCase.None</c>) throws.
/// </summary>
[Mappa]
public sealed partial class ProtobufOneofToPocoMapper
{
    /// <summary>
    /// Map from <see cref="ProtobufOneofResult"/> to <see cref="ProtobufOneofResultModel"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaProtobufPolymorphismSetup("ResultCase")]
    [MappaProtobufTypeMapping(typeof(ProtobufOneofOkResultModel), "Ok")]
    [MappaProtobufTypeMapping(typeof(ProtobufOneofErrorResultModel), "Error")]
    public partial ProtobufOneofResultModel Map(ProtobufOneofResult source);
}

/// <summary>
/// Maps a protobuf message with a <c>oneof</c> to a POCO hierarchy and throws a custom
/// exception when the oneof is not set (<see cref="MappaProtobufTypeMappingDefaultBehavior.Throw"/>).
/// </summary>
[Mappa]
public sealed partial class ProtobufOneofToPocoWithCustomExceptionMapper
{
    /// <summary>
    /// Map from <see cref="ProtobufOneofResult"/> to <see cref="ProtobufOneofResultModel"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaProtobufPolymorphismSetup("ResultCase")]
    [MappaProtobufTypeMapping(typeof(ProtobufOneofOkResultModel), "Ok")]
    [MappaProtobufTypeMapping(typeof(ProtobufOneofErrorResultModel), "Error")]
    [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.Throw, typeof(PolymorphismCustomException))]
    public partial ProtobufOneofResultModel Map(ProtobufOneofResult source);
}

/// <summary>
/// Maps a POCO hierarchy to a protobuf message with a <c>oneof</c> by dispatching on the runtime
/// type of the source. POCOs that are not covered by an arm are ignored
/// (<see cref="MappaProtobufTypeMappingDefaultBehavior.NoMap"/>): the returned message has no oneof set.
/// </summary>
[Mappa]
public sealed partial class ProtobufOneofFromPocoMapper
{
    /// <summary>
    /// Map from <see cref="ProtobufOneofResultModel"/> to <see cref="ProtobufOneofResult"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaProtobufPolymorphismSetup("ResultCase")]
    [MappaProtobufTypeMapping("Ok", typeof(ProtobufOneofOkResultModel))]
    [MappaProtobufTypeMapping("Error", typeof(ProtobufOneofErrorResultModel))]
    [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.NoMap)]
    public partial ProtobufOneofResult Map(ProtobufOneofResultModel source);
}

/// <summary>
/// Maps a POCO hierarchy to a protobuf message with a <c>oneof</c>. POCOs that are not covered
/// by an arm are mapped into the <c>Fallback</c> arm
/// (<see cref="MappaProtobufTypeMappingDefaultBehavior.MapTo"/>).
/// </summary>
[Mappa]
public sealed partial class ProtobufOneofFromPocoWithFallbackMapper
{
    /// <summary>
    /// Map from <see cref="ProtobufOneofResultModel"/> to <see cref="ProtobufOneofReply"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaProtobufPolymorphismSetup("ReplyCase")]
    [MappaProtobufTypeMapping("Ok", typeof(ProtobufOneofOkResultModel))]
    [MappaProtobufTypeMapping("Error", typeof(ProtobufOneofErrorResultModel))]
    [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.MapTo, "Fallback")]
    public partial ProtobufOneofReply Map(ProtobufOneofResultModel source);
}

/// <summary>
/// Maps a protobuf message with a <c>oneof</c> to another protobuf message with a <c>oneof</c>.
/// The two-argument setup is (target, source). A source message whose oneof is not set is mapped
/// into the <c>Fallback</c> arm of the target (<see cref="MappaProtobufTypeMappingDefaultBehavior.MapTo"/>).
/// </summary>
[Mappa]
public sealed partial class ProtobufOneofToProtobufMapper
{
    /// <summary>
    /// Map from <see cref="ProtobufOneofResult"/> to <see cref="ProtobufOneofReply"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaProtobufPolymorphismSetup("ReplyCase", "ResultCase")]
    [MappaProtobufTypeMapping("Ok", "Ok")]
    [MappaProtobufTypeMapping("Error", "Error")]
    [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.MapTo, "Fallback")]
    public partial ProtobufOneofReply Map(ProtobufOneofResult source);
}

/// <summary>
/// Maps a protobuf message with a <c>oneof</c> to another protobuf message with a <c>oneof</c>.
/// A source message whose oneof is not set produces a target without oneof
/// (<see cref="MappaProtobufTypeMappingDefaultBehavior.NoMap"/>).
/// </summary>
[Mappa]
public sealed partial class ProtobufOneofToProtobufIgnoringUnsetMapper
{
    /// <summary>
    /// Map from <see cref="ProtobufOneofResult"/> to <see cref="ProtobufOneofReply"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaProtobufPolymorphismSetup("ReplyCase", "ResultCase")]
    [MappaProtobufTypeMapping("Ok", "Ok")]
    [MappaProtobufTypeMapping("Error", "Error")]
    [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.NoMap)]
    public partial ProtobufOneofReply Map(ProtobufOneofResult source);
}