// <copyright file="ProtobufUInt32ValueToUInt32MapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map Google.Protobuf.WellKnownTypes.UInt32Value to <c>uint</c>.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
internal sealed class ProtobufUInt32ValueToUInt32MapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType)
        : MapStrategy(targetType, sourceType)
{
    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new ProtobufUInt32ValueToUInt32MapStrategyBuilder();
}