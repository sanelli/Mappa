// <copyright file="MappaMapAlgorithmContextSettings.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;

namespace Mappa.Generator.Models;

/// <summary>
/// Settings that can applied to a <see cref="MappaMapAlgorithmContext"/>.
/// </summary>
internal sealed class MappaMapAlgorithmContextSettings
{
    /// <summary>
    /// Gets the stack settings that enable or disable the algorithm
    /// in making sure if a constructor map strategy can be
    /// applied or not.
    /// </summary>
    /// <remarks>
    /// Typically the constructor strategy won't be applied if
    /// we are looking for a strategy to match a constructor
    /// with single parameter.
    /// </remarks>
    internal StackSettings<bool> UseConstructorMapStrategyDetector { get; } = new(true);
}