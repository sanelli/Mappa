// <copyright file="ValueTransformerLabelModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target model with a single label used by the value transformer samples.
/// </summary>
public sealed class ValueTransformerLabelModel
{
    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}