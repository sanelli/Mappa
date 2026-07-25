// <copyright file="EnumConfigMultiDefaultTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target model demonstrating per-enum default behaviour on class maps.
/// </summary>
public sealed class EnumConfigMultiDefaultTargetModel
{
    /// <summary>
    /// Gets or sets the mapped status code.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the mapped priority code.
    /// </summary>
    public int Priority { get; set; }
}