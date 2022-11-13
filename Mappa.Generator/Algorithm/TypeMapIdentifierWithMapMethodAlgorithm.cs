// <copyright file="TypeMapIdentifierWithMapMethodAlgorithm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.TypeMapStrategy;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm;

/// <summary>
/// Algorithm to identify a suitable map strategy from <see cref="TypeMapIdentifierAlgorithm.SourceType"/>
/// to <see cref="TypeMapIdentifierAlgorithm.TargetType"/>. This is similar to <see cref="TypeMapIdentifierAlgorithm"/>
/// but it first check if a suitable map already exists in <see cref="TypeMapIdentifierAlgorithm.MethodContext"/>.
/// </summary>
internal sealed class TypeMapIdentifierWithMapMethodAlgorithm
    : TypeMapIdentifierAlgorithm
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeMapIdentifierWithMapMethodAlgorithm"/> class.
    /// </summary>
    /// <param name="methodContext">The mappa method generator context.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="source">The mapping source.</param>
    public TypeMapIdentifierWithMapMethodAlgorithm(
        MappaMethodGeneratorContext methodContext,
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        string source)
        : base(methodContext, targetType, sourceType, source)
    {
    }

    /// <inheritdoc/>
    internal override ITypeMapStrategy GetStrategy()
    {
        if (this.MethodContext.ClassContext.TryGetMethod(this.TargetType, this.SourceType, out var mapMethod))
        {
            return new MapMethodTypeMapStrategy(mapMethod, this.Source);
        }

        return base.GetStrategy();
    }
}