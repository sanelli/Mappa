// <copyright file="MapMethodStrategyWithUserCustomInstanceMethodMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using the map method strategy.
/// </summary>
[Mappa]
public sealed partial class MapMethodStrategyWithUserCustomInstanceMethodMapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapMethodStrategyWithUserCustomInstanceMethodMapper"/> class.
    /// </summary>
    /// <param name="aValue">A value used in the mapping.</param>
    public MapMethodStrategyWithUserCustomInstanceMethodMapper(int aValue)
    {
        this.AValue = aValue;
    }

    /// <summary>
    /// Gets the value to be used in the instance method.
    /// </summary>
    public int AValue { get; }

    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The target model.</returns>
    public TargetClassModel Map(SourceClassModel sourceClassModel)
    {
        ArgumentNullException.ThrowIfNull(sourceClassModel);

        return new()
        {
            ParamA = $"{sourceClassModel.ParamA + this.AValue}",
            ParamB = this.AValue,
        };
    }

    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetClassWithInnerClassModel Map(SourceClassWithInnerClassModel sourceClassModel);
}