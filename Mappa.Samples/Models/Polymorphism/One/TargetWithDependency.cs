// <copyright file="TargetWithDependency.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.Polymorphism.One;

/// <summary>
/// Target class containing one of the derived classes as property.
/// </summary>
public sealed class TargetWithDependency
{
    /// <summary>
    /// Gets or sets a numeric property.
    /// </summary>
    public long NumericProperty { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="ThirdClass"/>.
    /// </summary>
    public required TargetThirdClass ThirdClass { get; set; }
}