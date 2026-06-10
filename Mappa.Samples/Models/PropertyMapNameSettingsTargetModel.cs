// <copyright file="PropertyMapNameSettingsTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target model for <see cref="PropertyMapNameSettingsMapper"/> samples.
/// </summary>
public sealed class PropertyMapNameSettingsTargetModel
{
    /// <summary>
    /// Gets or sets the mapped user name.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the mapped property B value.
    /// </summary>
    public long PropertyB { get; set; }
}