// <copyright file="MappaAssignToContextAttributeMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using <see cref="MappaContext"/> and <see cref="MappaAssignToContextAttribute"/>.
/// </summary>
[Mappa]
public sealed partial class MappaAssignToContextAttributeMapper
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
    /// and store the mapped <see cref="TargetClassModel.ParamA"/> in <paramref name="context"/>.
    /// </summary>
    /// <param name="input">The input model.</param>
    /// <param name="context">The context.</param>
    /// <returns>The mapped model.</returns>
    [MappaAssignToContext("ParamA", nameof(TargetClassModel.ParamA))]
    public partial TargetClassModel Map(SourceClassModel input, MappaContext context);
}