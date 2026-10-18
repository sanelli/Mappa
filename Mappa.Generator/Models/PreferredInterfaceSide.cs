// <copyright file="PreferredInterfaceSide.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models;

/// <summary>
/// Identifies whether a preferred-interface resolution applies to the source or target side.
/// </summary>
internal enum PreferredInterfaceSide
{
    /// <summary>
    /// Source side of the mapping.
    /// </summary>
    Source,

    /// <summary>
    /// Target side of the mapping.
    /// </summary>
    Target,
}