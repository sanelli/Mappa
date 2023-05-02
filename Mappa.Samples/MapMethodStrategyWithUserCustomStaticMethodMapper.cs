// <copyright file="MapMethodStrategyWithUserCustomStaticMethodMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using the map method strategy.
/// </summary>
[Mappa]
public sealed partial class MapMethodStrategyWithUserCustomStaticMethodMapper
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The target model.</returns>
    public static TargetClassModel Map(SourceClassModel sourceClassModel)
    {
        ArgumentNullException.ThrowIfNull(sourceClassModel);

        return new()
        {
            ParamA = $"{sourceClassModel.ParamA + 100}",
            ParamB = 17,
        };
    }

    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetClassWithInnerClassModel Map(SourceClassWithInnerClassModel sourceClassModel);
}