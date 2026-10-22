// <copyright file="ProtobufPolymorphismScenario.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models;

/// <summary>
/// Describes which side of a mapping is a protobuf message driven by a <c>oneof</c>.
/// </summary>
internal enum ProtobufPolymorphismScenario
{
    /// <summary>
    /// The source is a protobuf message and the target is a CLR type hierarchy.
    /// </summary>
    ProtoToPoco,

    /// <summary>
    /// The source is a CLR type hierarchy and the target is a protobuf message.
    /// </summary>
    PocoToProto,

    /// <summary>
    /// Both the source and the target are protobuf messages.
    /// </summary>
    ProtoToProto,
}