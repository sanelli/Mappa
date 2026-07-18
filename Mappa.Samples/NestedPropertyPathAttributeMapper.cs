// <copyright file="NestedPropertyPathAttributeMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

#pragma warning disable SA1402 // File may only contain a single type. Multiple sample mappers share this file by design.

/// <summary>
/// Mapper demonstrating two-segment nested paths with <see cref="MappaUsePropertyAttribute"/>.
/// </summary>
[Mappa]
public sealed partial class NestedPropertyPathAttributeMapper
{
    /// <summary>
    /// Maps a two-segment nested source and target path with <see cref="MappaUsePropertyAttribute"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaUseProperty("Address.City", "Address.City")]
    [MappaUseProperty("Address.ZipCode", "Address.ZipCode")]
    public partial NestedPropertyPathPersonTargetModel MapWithTwoSegmentUseProperty(
        NestedPropertyPathPersonSourceModel source);
}

/// <summary>
/// Mapper demonstrating three-segment nested source paths with <see cref="MappaUsePropertyAttribute"/>.
/// </summary>
[Mappa]
public sealed partial class NestedPropertyPathThreeSegmentUsePropertyAttributeMapper
{
    /// <summary>
    /// Maps a three-segment nested source path onto a two-segment nested target path.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaUseProperty("Address.City", "Location.Address.City")]
    [MappaUseProperty("Address.ZipCode", "Location.Address.ZipCode")]
    public partial NestedPropertyPathPersonTargetModel Map(NestedPropertyPathLocationSourceModel source);
}

/// <summary>
/// Mapper demonstrating a nested source path onto a flat target with <see cref="MappaUsePropertyAttribute"/>.
/// </summary>
[Mappa]
public sealed partial class NestedPropertyPathNestedSourceOnFlatTargetAttributeMapper
{
    /// <summary>
    /// Maps a nested source path onto a flat target property.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaUseProperty(nameof(NestedPropertyPathCityTargetModel.City), "Location.Address.City")]
    public partial NestedPropertyPathCityTargetModel Map(NestedPropertyPathLocationSourceModel source);
}

/// <summary>
/// Mapper demonstrating a nested target path with <see cref="MappaInvokeMethodAttribute"/>.
/// </summary>
[Mappa]
public sealed partial class NestedPropertyPathInvokeMethodAttributeMapper
{
    /// <summary>
    /// Maps a nested target property by invoking a custom method.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaInvokeMethod("Address.City", nameof(CustomMapCity))]
    public partial NestedPropertyPathPersonTargetModel Map(NestedPropertyPathPersonSourceModel source);

    /// <summary>
    /// Custom city mapping used by <see cref="Map"/>.
    /// </summary>
    /// <param name="city">The source city.</param>
    /// <returns>The mapped city.</returns>
    [MappaIgnore]
    private static string CustomMapCity(string city)
    {
        ArgumentNullException.ThrowIfNull(city);
        return city.ToUpperInvariant();
    }
}

/// <summary>
/// Mapper demonstrating a nested target path with <see cref="MappaAssignFromConstantAttribute"/>.
/// </summary>
[Mappa]
public sealed partial class NestedPropertyPathAssignFromConstantAttributeMapper
{
    /// <summary>
    /// Assigns a constant to a nested target property path.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaAssignFromConstant("Address.City", "London")]
    public partial NestedPropertyPathPersonTargetModel Map(NestedPropertyPathPersonSourceModel source);
}

/// <summary>
/// Mapper demonstrating a nested target path with <see cref="MappaAssignFromContextAttribute"/>.
/// </summary>
[Mappa]
public sealed partial class NestedPropertyPathAssignFromContextAttributeMapper
{
    /// <summary>
    /// Assigns a context value to a nested target property path.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The target model.</returns>
    [MappaAssignFromContext("Address.City", "city")]
    public partial NestedPropertyPathPersonTargetModel Map(
        NestedPropertyPathPersonSourceModel source,
        MappaContext context);
}

/// <summary>
/// Mapper demonstrating a nested target path with <see cref="MappaAssignToContextAttribute"/>.
/// </summary>
[Mappa]
public sealed partial class NestedPropertyPathAssignToContextAttributeMapper
{
    /// <summary>
    /// Stores a nested target property path value into the mapping context.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The target model.</returns>
    [MappaAssignToContext("MappedCity", "Address.City")]
    public partial NestedPropertyPathPersonTargetModel Map(
        NestedPropertyPathPersonSourceModel source,
        MappaContext context);
}

/// <summary>
/// Mapper demonstrating a nested target path with <see cref="MappaIgnoreTargetPropertyAttribute"/>.
/// </summary>
[Mappa]
public sealed partial class NestedPropertyPathIgnoreTargetPropertyAttributeMapper
{
    /// <summary>
    /// Ignores a nested target property path while mapping the remaining members.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaIgnoreTargetProperty("Address.ZipCode")]
    public partial NestedPropertyPathPersonTargetModel Map(NestedPropertyPathPersonSourceModel source);
}