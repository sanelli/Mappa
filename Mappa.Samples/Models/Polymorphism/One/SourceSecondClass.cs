// <copyright file="SourceSecondClass.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.Polymorphism.One;

/// <summary>
/// A base class.
/// </summary>
public class SourceSecondClass : SourceBaseClass
{
    /// <summary>
    /// Gets or sets a <see cref="Guid"/> property.
    /// </summary>
    public Guid GuidProperty { get; set; }
}