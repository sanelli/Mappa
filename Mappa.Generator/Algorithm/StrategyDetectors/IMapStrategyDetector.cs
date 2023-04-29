// <copyright file="IMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Generic interface for the strategy detector.
/// </summary>
internal interface IMapStrategyDetector
{
    /// <summary>
    /// Attempt to detect the strategy.
    /// </summary>
    /// <param name="mapStrategy">The map strategy.</param>
    /// <returns><c>true</c> if a strategy has been found.</returns>
    bool TryDetect(out IMapStrategy mapStrategy);
}