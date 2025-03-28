// <copyright file="NullableStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="NullableStrategy"/>.
/// </summary>
internal sealed class NullableStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly NullableStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public NullableStrategyBuilder(NullableStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        if (this.strategy is null)
        {
            throw new MappaGeneratorException("whatever");
        }

        throw new NotImplementedException();
    }
}