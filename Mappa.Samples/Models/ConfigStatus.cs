// <copyright file="ConfigStatus.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Service status values used by enum mapping configuration samples.
/// </summary>
public enum ConfigStatus
{
    /// <summary>
    /// The service is active.
    /// </summary>
    Active,

    /// <summary>
    /// The service is inactive.
    /// </summary>
    Inactive,

    /// <summary>
    /// The service is deprecated and may be excluded from mapping.
    /// </summary>
    Deprecated,
}