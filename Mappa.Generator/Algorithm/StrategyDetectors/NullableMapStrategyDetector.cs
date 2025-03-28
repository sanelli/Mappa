// <copyright file="NullableMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for nullable related strategies.
/// </summary>
internal sealed class NullableMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;
    private readonly CancellationToken cancellationToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public NullableMapStrategyDetector(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        this.context = context;
        this.cancellationToken = cancellationToken;
        this.compilation = compilation;
    }

    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        var isSourceNullable = IsNullable(this.context.SourceType);
        var isTargetNullable = IsNullable(this.context.TargetType);

        if (isSourceNullable || isTargetNullable)
        {
            var sourceInnerType = GetTypeInsideNullable(this.context.SourceType);
            var targetInnerType = GetTypeInsideNullable(this.context.TargetType);
            var derivedContext = new DerivedMappaMapAlgorithmContext(
                this.context,
                targetInnerType,
                sourceInnerType);

            using (this.context.AlgorithmSettings.UseNullableMapStrategyDetector.Apply(false))
            {
                var algorithm = new TypeMapIdentifierAlgorithm(derivedContext, this.compilation, this.cancellationToken);
                var elementStrategy = algorithm.GetStrategy();
                if (elementStrategy is not NoMapStrategy)
                {
                    mapStrategy = new NullableStrategy(this.context.TargetType, this.context.SourceType, elementStrategy);
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsNullable(ITypeSymbol typeSymbol)
    {
        return typeSymbol.IsValueTypeNullable() || typeSymbol.IsReferenceNullable();
    }

    private static ITypeSymbol GetTypeInsideNullable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsReferenceType)
        {
            return typeSymbol;
        }

        return typeSymbol.GetElementType();
    }
}