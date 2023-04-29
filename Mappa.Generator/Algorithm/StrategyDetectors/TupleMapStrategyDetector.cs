// <copyright file="TupleMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for tuple strategies.
/// </summary>
internal sealed class TupleMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;
    private readonly CancellationToken cancellationToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="TupleMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public TupleMapStrategyDetector(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        this.context = context;
        this.cancellationToken = cancellationToken;
        this.compilation = compilation;
    }

    /// <inheritdoc/>
    public bool TryDetect(out IMapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // 01. (S1, ..., SN) -> (T1, ..., TN) : TupleStrategy( IMapStrategy(T1, S1), ..., IMapStrategy(TN, SN))
        if (this.CanMapTupleToTuple(out var tupleElementStrategies))
        {
            mapStrategy = new TupleToTupleMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                tupleElementStrategies.ToArray());
        }

        return mapStrategy is not NoMapStrategy;
    }

    private bool CanMapTupleToTuple(out IList<IMapStrategy> elementsStrategies)
    {
        var isSourceTuple = this.context.SourceType.IsTuple(this.compilation);
        var isTargetTuple = this.context.TargetType.IsTuple(this.compilation);

        var tmpTupleStrategies = elementsStrategies = new List<IMapStrategy>();

        return isSourceTuple
               && isTargetTuple
               && this.context.SourceType.TryGetTypeArguments(out var sourceTypeArguments)
               && this.context.TargetType.TryGetTypeArguments(out var targetTypeArguments)
               && sourceTypeArguments.Length == targetTypeArguments.Length
               && Enumerable.Range(0, sourceTypeArguments.Length)
                   .All(index =>
                   {
                       var elementContext = new DerivedMappaMapAlgorithmContext(
                       this.context,
                       targetTypeArguments[index],
                       sourceTypeArguments[index]);
                       var elementAlgorithm = new TypeMapIdentifierWithMapMethodAlgorithm(elementContext, this.compilation, this.cancellationToken);
                       var elementStrategy = elementAlgorithm.GetStrategy();

                       if (elementStrategy is NoMapStrategy)
                       {
                           return false;
                       }

                       tmpTupleStrategies.Add(elementStrategy);
                       return true;
                   });
    }
}