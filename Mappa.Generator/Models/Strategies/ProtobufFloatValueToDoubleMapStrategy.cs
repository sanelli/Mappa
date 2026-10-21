// <copyright file="ProtobufFloatValueToDoubleMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map Google.Protobuf.WellKnownTypes.FloatValue to <c>double</c>.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
internal sealed class ProtobufFloatValueToDoubleMapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType)
        : MapStrategy(targetType, sourceType)
{
    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new ProtobufFloatValueToDoubleMapStrategyBuilder();
}