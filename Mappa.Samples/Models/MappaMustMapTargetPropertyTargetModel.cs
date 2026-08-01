// <copyright file="MappaMustMapTargetPropertyTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples.Models;

/// <summary>
/// Target model for <see cref="MappaMustMapTargetPropertyAttribute"/> samples.
/// </summary>
public sealed class MappaMustMapTargetPropertyTargetModel
{
    /// <summary>
    /// Gets or sets the first property that must be mapped from the source.
    /// </summary>
    public string PropertyA { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the second property that must be mapped from the source.
    /// </summary>
    public long PropertyB { get; set; }
}