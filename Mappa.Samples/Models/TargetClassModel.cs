// <copyright file="TargetClassModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A target model with a few parameters
/// similar to <see cref="SourceClassModel"/>.
/// </summary>
public sealed class TargetClassModel
{
    /// <summary>
    /// Gets or sets an integer value.
    /// </summary>
    public string ParamA { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an enumeration value.
    /// </summary>
    public int ParamB { get; set; } = -1;
}