// <copyright file="TypeMapIdentifierWithMapMethodAlgorithm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm;

/// <summary>
/// Algorithm to identify a suitable map strategy from <see cref="TypeMapIdentifierAlgorithm.SourceType"/>
/// to <see cref="TypeMapIdentifierAlgorithm.TargetType"/>. This is similar to <see cref="TypeMapIdentifierAlgorithm"/>
/// but it first check if a suitable map already exists in <see cref="TypeMapIdentifierAlgorithm.Context"/>.
/// </summary>
#pragma warning disable CA1812
internal sealed class TypeMapIdentifierWithMapMethodAlgorithm
#pragma warning restore CS1812
    : TypeMapIdentifierAlgorithm
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeMapIdentifierWithMapMethodAlgorithm"/> class.
    /// </summary>
    /// <param name="methodContext">The mappa method generator context.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="source">The mapping source.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public TypeMapIdentifierWithMapMethodAlgorithm(
        MappaMapAlgorithmContext methodContext,
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        string source,
        Compilation compilation,
        CancellationToken cancellationToken)
        : base(methodContext, targetType, sourceType, source, compilation, cancellationToken)
    {
    }

    /// <inheritdoc/>
    internal override IMapStrategy GetStrategy()
    {
        this.CancellationToken.ThrowIfCancellationRequested();

        if (this.Context.TryGetMethod(this.TargetType, this.SourceType, out var mapMethod))
        {
            return new MethodMapStrategy(MappaAlgorithmRule.MapUsingExistingMethod, mapMethod, this.Source);
        }

        return base.GetStrategy();
    }
}