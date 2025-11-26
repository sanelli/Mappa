// <copyright file="SourceSecondClass.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.Polymorphism.Three;

/// <summary>
/// A base class.
/// </summary>
public class SourceSecondClass : SourceBaseClass
{
    /// <summary>
    /// Gets or sets a <see cref="string"/> property.
    /// </summary>
    public required string DerivedProperty { get; set; }
}