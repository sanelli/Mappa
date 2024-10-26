// <copyright file="MappaAssignFromContextAttributeMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using <see cref="MappaContext"/> and <see cref="MappaAssignFromContextAttribute"/>.
/// </summary>
[Mappa]
public sealed partial class MappaAssignFromContextAttributeMapper
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
    /// using a custom value from <paramref name="context"/>.
    /// </summary>
    /// <param name="input">The input model.</param>
    /// <param name="context">The context.</param>
    /// <returns>The mapped model.</returns>
    [MappaAssignFromContext(nameof(TargetClassModel.ParamA), "CustomValue")]
    public partial TargetClassModel Map(SourceClassModel input, MappaContext context);

    /// <summary>
    /// Map from <see cref="SourceClassWithInnerClassModel"/> to <see cref="TargetClassWithInnerClassModel"/>
    /// using a <paramref name="context"/>.
    /// </summary>
    /// <param name="input">The input model.</param>
    /// <param name="context">The context.</param>
    /// <returns>The mapped model.</returns>
    public partial TargetClassWithInnerClassModel Map(SourceClassWithInnerClassModel input, MappaContext context);
}