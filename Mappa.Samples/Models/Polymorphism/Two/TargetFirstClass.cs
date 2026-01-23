// <copyright file="TargetFirstClass.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.Polymorphism.Two;

/// <summary>
/// A base class.
/// </summary>
public class TargetFirstClass : ITargetBaseClass
{
    /// <summary>
    /// Gets or sets a date/time property represented by a string.
    /// </summary>
    public required string DateTimeProperty { get; set; }

    /// <inheritdoc />
    public long NumericProperty { get; set; }
}