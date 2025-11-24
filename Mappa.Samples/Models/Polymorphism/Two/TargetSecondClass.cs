// <copyright file="TargetSecondClass.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.Polymorphism.Two;

/// <summary>
/// A base class.
/// </summary>
public class TargetSecondClass : ITargetBaseClass
{
    /// <summary>
    /// Gets or sets a guid property represented as string.
    /// </summary>
    public required string GuidProperty { get; set; }

    /// <inheritdoc />
    public long NumericProperty { get; set; }
}