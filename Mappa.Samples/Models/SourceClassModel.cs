// <copyright file="SourceClassModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A source model with a few parameters.
/// </summary>
public sealed class SourceClassModel
{
    /// <summary>
    /// Gets or sets an integer value.
    /// </summary>
    public int ParamA { get; set; } = 10;

    /// <summary>
    /// Gets or sets an enumeration value.
    /// </summary>
    public CountingValues ParamB { get; set; } = CountingValues.Two;
}