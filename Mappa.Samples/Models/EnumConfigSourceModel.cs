// <copyright file="EnumConfigSourceModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Source model with enum properties for nested enum configuration samples.
/// </summary>
public sealed class EnumConfigSourceModel
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