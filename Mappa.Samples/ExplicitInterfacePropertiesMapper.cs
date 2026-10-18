// <copyright file="ExplicitInterfacePropertiesMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating <see cref="MappaSettingsAttribute.MapSourceExplicitInterfaceProperties"/>
/// and <see cref="MappaSettingsAttribute.MapTargetExplicitInterfaceProperties"/>.
/// </summary>
[Mappa]
[MappaSettings(
    MapSourceExplicitInterfaceProperties = BooleanSetting.Enable,
    MapTargetExplicitInterfaceProperties = BooleanSetting.Enable)]
public sealed partial class ExplicitInterfacePropertiesMapper
{
    /// <summary>
    /// Maps from an explicitly implemented source property via empty constructor.
    /// </summary>
    /// <param name="input">The input model.</param>
    /// <returns>The mapped model.</returns>
    public partial ExplicitInterfacePublicTargetModel MapFromExplicitSource(ExplicitInterfaceSourceModel input);

    /// <summary>
    /// Maps to an explicitly implemented target property via empty constructor.
    /// </summary>
    /// <param name="input">The input model.</param>
    /// <returns>The mapped model.</returns>
    public partial ExplicitInterfaceTargetModel MapToExplicitTarget(ExplicitInterfacePublicSourceModel input);

    /// <summary>
    /// Maps constructor parameter <c>id</c> and leftover explicit interface <c>Value</c> after construction.
    /// </summary>
    /// <param name="input">The input model.</param>
    /// <returns>The mapped model.</returns>
    public partial ExplicitInterfaceParameterizedTargetModel MapParameterizedWithLeftoverExplicitTarget(ExplicitInterfacePublicSourceModel input);
}