// <copyright file="ExplicitInterfacePublicTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target model with a public <c>Value</c> property.
/// </summary>
public sealed class ExplicitInterfacePublicTargetModel
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