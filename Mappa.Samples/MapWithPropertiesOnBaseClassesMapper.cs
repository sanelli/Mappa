// <copyright file="MapWithPropertiesOnBaseClassesMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper used to showcase mapping of mapping when properties
/// are on base classes and implemented interfaces.
/// </summary>
[Mappa]
public sealed partial class MapWithPropertiesOnBaseClassesMapper
{
    /// <summary>
    /// Map from <see cref="DerivedClassSourceModel"/> to <see cref="DerivedClassTargetModel"/>.
    /// </summary>
    /// <param name="input">The input model.</param>
    /// <returns>The target model.</returns>
    public partial DerivedClassTargetModel MapToClassWithProperties(DerivedClassSourceModel input);

    /// <summary>
    /// Map from <see cref="DerivedClassSourceModel"/> to <see cref="DerivedClassTargetModelWithConstructor"/>.
    /// </summary>
    /// <param name="input">The input model.</param>
    /// <returns>The target model.</returns>
    public partial DerivedClassTargetModelWithConstructor MapToClassWithConstructor(DerivedClassSourceModel input);
}