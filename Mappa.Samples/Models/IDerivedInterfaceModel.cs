// <copyright file="IDerivedInterfaceModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// An interface exposing properties.
/// </summary>
public interface IDerivedInterfaceModel
    : IInterfaceModel
{
    /// <summary>
    /// Gets or sets a double property.
    /// </summary>
    public double DoubleProperty { get; set; }
}