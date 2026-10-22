// <copyright file="ProtobufPolymorphismArm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// One arm of a protobuf <c>oneof</c> polymorphism mapping.
/// </summary>
/// <param name="strategy">The strategy mapping the arm source value to the arm target value.</param>
/// <param name="sourceCaseMemberName">The source <c>*OneofCase</c> enum member name (proto source only).</param>
/// <param name="sourceArmPropertyName">The source arm property name (proto source only).</param>
/// <param name="targetArmPropertyName">The target arm property name (proto target only).</param>
internal sealed class ProtobufPolymorphismArm(
    MapStrategy strategy,
    string? sourceCaseMemberName,
    string? sourceArmPropertyName,
    string? targetArmPropertyName)
{
    /// <summary>
    /// Gets the strategy mapping the arm source value to the arm target value.
    /// </summary>
    internal MapStrategy Strategy { get; } = strategy;

    /// <summary>
    /// Gets the source <c>*OneofCase</c> enum member name, or <c>null</c> when the source is not a protobuf message.
    /// </summary>
    internal string? SourceCaseMemberName { get; } = sourceCaseMemberName;

    /// <summary>
    /// Gets the source arm property name, or <c>null</c> when the source is not a protobuf message.
    /// </summary>
    internal string? SourceArmPropertyName { get; } = sourceArmPropertyName;

    /// <summary>
    /// Gets the target arm property name, or <c>null</c> when the target is not a protobuf message.
    /// </summary>
    internal string? TargetArmPropertyName { get; } = targetArmPropertyName;
}