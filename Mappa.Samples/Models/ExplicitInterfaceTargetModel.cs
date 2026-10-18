// <copyright file="ExplicitInterfaceTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target model with an explicitly implemented <see cref="IHasExplicitValue.Value"/>.
/// </summary>
public sealed class ExplicitInterfaceTargetModel : IHasExplicitValue
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public int Id { get; set; }

    /// <inheritdoc/>
    int IHasExplicitValue.Value { get; set; }
}