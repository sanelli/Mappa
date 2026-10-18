// <copyright file="ExplicitInterfacePublicSourceModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Source model with public properties.
/// </summary>
public sealed class ExplicitInterfacePublicSourceModel
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public int Value { get; set; }
}