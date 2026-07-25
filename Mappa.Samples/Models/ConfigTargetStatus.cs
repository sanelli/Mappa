// <copyright file="ConfigTargetStatus.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target-side deployment status values for enum-to-enum configuration samples.
/// </summary>
public enum ConfigTargetStatus
{
    /// <summary>
    /// The deployment is online.
    /// </summary>
    Online,

    /// <summary>
    /// The deployment is offline.
    /// </summary>
    Offline,

    /// <summary>
    /// The deployment is on standby.
    /// </summary>
    Standby,
}