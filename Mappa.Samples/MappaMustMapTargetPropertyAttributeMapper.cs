// <copyright file="MappaMustMapTargetPropertyAttributeMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper used to demonstrate <see cref="MappaMustMapTargetPropertyAttribute"/>.
/// </summary>
[Mappa]
public sealed partial class MappaMustMapTargetPropertyAttributeMapper
{
    /// <summary>
    /// Map from <see cref="MappaMustMapTargetPropertySourceModel"/> to
    /// <see cref="MappaMustMapTargetPropertyTargetModel"/> requiring the listed
    /// target properties to be mapped.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaMustMapTargetProperty(
        nameof(MappaMustMapTargetPropertyTargetModel.PropertyA),
        nameof(MappaMustMapTargetPropertyTargetModel.PropertyB))]
    public partial MappaMustMapTargetPropertyTargetModel Map(MappaMustMapTargetPropertySourceModel source);
}