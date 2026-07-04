// <copyright file="DescribedCountingValues.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.ComponentModel;

namespace Mappa.Samples.Models;

/// <summary>
/// An enum whose members are labeled with <see cref="DescriptionAttribute"/> values.
/// </summary>
public enum DescribedCountingValues
{
    /// <summary>
    /// First value.
    /// </summary>
    [Description("First")]
    One,

    /// <summary>
    /// Second value.
    /// </summary>
    [Description("Second")]
    Two,

    /// <summary>
    /// Third value.
    /// </summary>
    [Description("Third")]
    Three,
}