// <copyright file="CollectionToCollectionMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="CollectionToCollectionMapStrategy"/>.
/// </summary>
internal sealed class CollectionToCollectionMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly CollectionToCollectionMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionToCollectionMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public CollectionToCollectionMapStrategyBuilder(CollectionToCollectionMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        if (this.strategy is null)
        {
            throw new MappaGeneratorException(nameof(this.strategy));
        }

        throw new NotImplementedException();
    }
}