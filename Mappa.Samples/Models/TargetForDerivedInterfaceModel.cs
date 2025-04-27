// <copyright file="TargetForDerivedInterfaceModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A target class for <see cref="IDerivedInterfaceModel"/>.
/// </summary>
public sealed class TargetForDerivedInterfaceModel
{
    /// <summary>
    /// Gets or sets a long property.
    /// </summary>
    public long LongProperty { get; set; }

    /// <summary>
    /// Gets or sets a double property.
    /// </summary>
    public double DoubleProperty { get; set; }
}