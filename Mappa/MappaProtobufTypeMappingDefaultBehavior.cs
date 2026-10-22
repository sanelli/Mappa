// <copyright file="MappaProtobufTypeMappingDefaultBehavior.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa;

/// <summary>
/// Describe the default mapping behaviour when protobuf <c>oneof</c> polymorphism
/// is applied via <see cref="MappaProtobufTypeMappingAttribute"/> and none of the
/// defined arms matches the actual input (including when the source oneof case is <c>None</c>).
/// </summary>
public enum MappaProtobufTypeMappingDefaultBehavior
{
    /// <summary>
    /// Undefined setting.
    /// </summary>
    Undefined,

    /// <summary>
    /// If no arm matches, throw an exception. The default exception thrown is
    /// <see cref="ArgumentOutOfRangeException"/> but the exception type can be defined
    /// using <see cref="MappaProtobufTypeMappingDefaultAttribute"/>.
    /// </summary>
    Throw,

    /// <summary>
    /// If no arm matches, leave the target oneof unset (<c>None</c>).
    /// Only valid when the target is a protobuf message.
    /// </summary>
    NoMap,

    /// <summary>
    /// If no arm matches, map the unmatched source into a specific target oneof arm's
    /// message type and assign that property. Only valid when the target is a protobuf message.
    /// The arm is selected via <see cref="MappaProtobufTypeMappingDefaultAttribute"/>.
    /// </summary>
    MapTo,
}