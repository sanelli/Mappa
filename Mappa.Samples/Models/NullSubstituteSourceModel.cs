// <copyright file="NullSubstituteSourceModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Source model for null-substitute mapping samples.
/// </summary>
public sealed class NullSubstituteSourceModel
{
    /// <summary>
    /// Gets or sets a nullable string that may be substituted when null.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets a nullable string used by InvokeMethod substitution.
    /// </summary>
    public string? Alias { get; set; }
}