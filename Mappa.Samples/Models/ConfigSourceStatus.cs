// <copyright file="ConfigSourceStatus.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Source-side deployment status values for enum-to-enum configuration samples.
/// </summary>
public enum ConfigSourceStatus
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
    /// A legacy status with no matching target member name.
    /// </summary>
    Legacy,
}