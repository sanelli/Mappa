// <copyright file="MappaGeneratorConsts.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System;

namespace Mappa.Generator;

/// <summary>
/// Mappa constants.
/// </summary>
internal static class MappaGeneratorConsts
{
    /// <summary>
    /// The mappa generator version.
    /// </summary>
    internal static readonly Version MappaGeneratorVersion = typeof(MappaGenerator).Assembly.GetName().Version;
}