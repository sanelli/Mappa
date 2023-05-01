// <copyright file="InvokeEmptyConstructorStrategyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Unit tests for the invoke-empty-constructor strategy.
/// </summary>
[Mappa]
public sealed partial class InvokeEmptyConstructorStrategyMapper
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/>
    /// to <see cref="TargetClassModel"/> using the
    /// empty constructor strategy.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetClassModel Map(SourceClassModel sourceClassModel);
}