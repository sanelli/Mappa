// <copyright file="ExplicitInterfaceParameterizedTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target model constructed with <see cref="Id"/> and an explicit leftover <see cref="IHasExplicitValue.Value"/>.
/// </summary>
public sealed class ExplicitInterfaceParameterizedTargetModel : IHasExplicitValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExplicitInterfaceParameterizedTargetModel"/> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public ExplicitInterfaceParameterizedTargetModel(int id)
    {
        this.Id = id;
    }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public int Id { get; }

    /// <inheritdoc/>
    int IHasExplicitValue.Value { get; set; }
}