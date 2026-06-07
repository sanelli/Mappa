// <copyright file="ReadonlyAddCollectionPropertyMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ReadonlyAddCollectionPropertyMapStrategy"/>.
/// </summary>
internal sealed class ReadonlyAddCollectionPropertyMapStrategyBuilder(ReadonlyAddCollectionPropertyMapStrategy strategy)
    : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
        => ReadonlyCollectionPropertyLoopBuilder.BuildSource(
            strategy.SourceType,
            strategy.TargetType,
            strategy.TargetProperty,
            strategy.ElementStrategy,
            ReadonlyCollectionPropertyLoopBuilder.InsertionMethod.Add,
            source,
            context,
            mappaGlobalOptions);
}