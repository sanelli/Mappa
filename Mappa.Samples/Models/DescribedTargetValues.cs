// <copyright file="DescribedTargetValues.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.ComponentModel;

namespace Mappa.Samples.Models;

/// <summary>
/// Target enum for Description-based enum-to-enum mapping samples.
/// </summary>
public enum DescribedTargetValues
{
    /// <summary>
    /// First value.
    /// </summary>
    [Description("Alpha")]
    First,

    /// <summary>
    /// Second value.
    /// </summary>
    [Description("Beta")]
    Second,
}