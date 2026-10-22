// <copyright file="ProtobufPolymorphismMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to implement polymorphism driven by a protobuf <c>oneof</c>.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
/// <param name="scenario">The mapping scenario.</param>
/// <param name="sourceCaseProperty">The source oneof case property (proto source only).</param>
/// <param name="sourceCaseEnumType">The source <c>*OneofCase</c> enum type (proto source only).</param>
/// <param name="arms">The arms of the mapping.</param>
/// <param name="defaultBehavior">The default behavior when no arm matches.</param>
/// <param name="mapToArm">The arm used by <see cref="MappaProtobufTypeMappingDefaultBehavior.MapTo"/>, if any.</param>
internal sealed class ProtobufPolymorphismMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    ProtobufPolymorphismScenario scenario,
    IPropertySymbol? sourceCaseProperty,
    ITypeSymbol? sourceCaseEnumType,
    ProtobufPolymorphismArm[] arms,
    MappaProtobufTypeMappingDefaultAttribute defaultBehavior,
    ProtobufPolymorphismArm? mapToArm)
    : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the mapping scenario.
    /// </summary>
    internal ProtobufPolymorphismScenario Scenario { get; } = scenario;

    /// <summary>
    /// Gets a value indicating whether the generated code switches on the CLR type of the source
    /// (<c>true</c>) or on the source oneof case (<c>false</c>).
    /// </summary>
    internal bool SwitchesOnClrType => this.Scenario is ProtobufPolymorphismScenario.PocoToProto;

    /// <summary>
    /// Gets a value indicating whether the target is a protobuf message built via an empty constructor.
    /// </summary>
    internal bool TargetIsProtobuf => this.Scenario is not ProtobufPolymorphismScenario.ProtoToPoco;

    /// <summary>
    /// Gets the source oneof case property, or <c>null</c> when the source is not a protobuf message.
    /// </summary>
    internal IPropertySymbol? SourceCaseProperty { get; } = sourceCaseProperty;

    /// <summary>
    /// Gets the source <c>*OneofCase</c> enum type, or <c>null</c> when the source is not a protobuf message.
    /// </summary>
    internal ITypeSymbol? SourceCaseEnumType { get; } = sourceCaseEnumType;

    /// <summary>
    /// Gets the arms of the mapping.
    /// </summary>
    internal ProtobufPolymorphismArm[] Arms { get; } = arms;

    /// <summary>
    /// Gets the default behavior when no arm matches.
    /// </summary>
    internal MappaProtobufTypeMappingDefaultAttribute DefaultBehavior { get; } = defaultBehavior;

    /// <summary>
    /// Gets the arm used when the default behavior is <see cref="MappaProtobufTypeMappingDefaultBehavior.MapTo"/>.
    /// </summary>
    internal ProtobufPolymorphismArm? MapToArm { get; } = mapToArm;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new ProtobufPolymorphismMapStrategyBuilder(this);
}