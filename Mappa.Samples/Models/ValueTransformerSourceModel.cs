// <copyright file="ValueTransformerSourceModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Source model used by the value transformer samples.
/// </summary>
public sealed class ValueTransformerSourceModel
{
    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the free-text note.
    /// </summary>
    public string Note { get; set; } = string.Empty;
}