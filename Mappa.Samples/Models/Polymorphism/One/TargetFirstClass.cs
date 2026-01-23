// <copyright file="TargetFirstClass.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.Polymorphism.One;

/// <summary>
/// A base class.
/// </summary>
public class TargetFirstClass : TargetBaseClass
{
    /// <summary>
    /// Gets or sets a date/time property represented by a string.
    /// </summary>
    public required string DateTimeProperty { get; set; }
}