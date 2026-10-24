// <copyright file="EmptyToEmptyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating empty-to-empty mapping via the automatic empty-constructor strategy (#175).
/// </summary>
[Mappa]
public sealed partial class EmptyToEmptyMapper
{
    /// <summary>
    /// Map from <see cref="EmptySourceModel"/> to <see cref="EmptyTargetModel"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>A new empty target instance.</returns>
    public partial EmptyTargetModel Map(EmptySourceModel source);

    /// <summary>
    /// Map from <see cref="SourceWithEmptyNestedModel"/> to <see cref="TargetWithEmptyNestedModel"/>,
    /// including a nested empty type.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model with a nested empty instance.</returns>
    public partial TargetWithEmptyNestedModel Map(SourceWithEmptyNestedModel source);
}