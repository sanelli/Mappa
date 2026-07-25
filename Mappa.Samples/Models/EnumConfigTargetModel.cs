// <copyright file="EnumConfigTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target model with integral properties mapped from nested enum configuration samples.
/// </summary>
public sealed class EnumConfigTargetModel
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