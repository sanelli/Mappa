// <copyright file="ProtobufDurationToTimeSpanMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map a Google.Protobuf.WellKnownTypes.Duration to
/// a <see cref="TimeSpan"/> value.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
internal sealed class ProtobufDurationToTimeSpanMapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType)
        : MapStrategy(targetType, sourceType)
{
    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new ProtobufDurationToTimeSpanMapStrategyBuilder();
}