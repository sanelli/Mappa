// <copyright file="MappaMustMapTargetPropertyAttributeMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type. Multiple sample mappers share this file by design.

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper used to demonstrate <see cref="MappaMustMapTargetPropertyAttribute"/>
/// with an explicit list of target properties that must be mapped.
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

/// <summary>
/// Mapper used to demonstrate <see cref="MappaMustMapTargetPropertyAttribute"/>
/// requiring all non-required target properties to be mapped.
/// </summary>
[Mappa]
public sealed partial class MappaMustMapAllTargetPropertiesAttributeMapper
{
    /// <summary>
    /// Map from <see cref="MappaMustMapTargetPropertySourceModel"/> to
    /// <see cref="MappaMustMapTargetPropertyTargetModel"/> requiring every
    /// non-required target property to be mapped.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaMustMapTargetProperty]
    public partial MappaMustMapTargetPropertyTargetModel Map(MappaMustMapTargetPropertySourceModel source);
}