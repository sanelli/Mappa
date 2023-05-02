// <copyright file="TypeMapIdentifierWithMapMethodAlgorithm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm;

/// <summary>
/// Algorithm to identify a suitable map strategy from <see cref="MappaMapAlgorithmContext.SourceType"/>
/// to <see cref="MappaMapAlgorithmContext.TargetType"/>. This is similar to <see cref="TypeMapIdentifierAlgorithm"/>
/// but it first check if a suitable map already exists in <see cref="TypeMapIdentifierAlgorithm.Context"/>.
/// </summary>
internal sealed class TypeMapIdentifierWithMapMethodAlgorithm
    : TypeMapIdentifierAlgorithm
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeMapIdentifierWithMapMethodAlgorithm"/> class.
    /// </summary>
    /// <param name="methodContext">The mappa method generator context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public TypeMapIdentifierWithMapMethodAlgorithm(
        MappaMapAlgorithmContext methodContext,
        Compilation compilation,
        CancellationToken cancellationToken)
        : base(methodContext, compilation, cancellationToken)
    {
    }

    /// <inheritdoc/>
    internal override IMapStrategy GetStrategy()
    {
        this.CancellationToken.ThrowIfCancellationRequested();

        // TODO: Allow to get method compatible with expected types:
        // - Input can be any of the base types/interfaces
        // - Return type can be a derived type
        // This will require identifying a mapping between input & target parameter.
        if (this.Context.TryGetMethod(this.Context.TargetType, this.Context.SourceType, out var mapMethod))
        {
            return new MethodMapStrategy(MappaAlgorithmRule.MapUsingExistingMethod, mapMethod);
        }

        return base.GetStrategy();
    }
}