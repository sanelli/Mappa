// <copyright file="CaseInsensitiveTargetValues.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target enum for case-insensitive enum-to-enum mapping samples.
/// </summary>
public enum CaseInsensitiveTargetValues
{
#pragma warning disable SA1300 // Intentional lowercase member name for case-insensitive mapping demo
    /// <summary>
    /// First value with different casing.
    /// </summary>
    one,
#pragma warning restore SA1300

    /// <summary>
    /// Second value.
    /// </summary>
    Two,
}