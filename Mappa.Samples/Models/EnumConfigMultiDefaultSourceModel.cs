// <copyright file="EnumConfigMultiDefaultSourceModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Source model demonstrating per-enum default behaviour on class maps.
/// </summary>
public sealed class EnumConfigMultiDefaultSourceModel
{
    /// <summary>
    /// Gets or sets the service status.
    /// </summary>
    public ConfigStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the service priority.
    /// </summary>
    public ConfigPriority Priority { get; set; }
}