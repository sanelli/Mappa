// <copyright file="SourceFirstClass.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.Polymorphism.Three;

/// <summary>
/// A base class.
/// </summary>
public class SourceFirstClass : SourceBaseClass
{
    /// <summary>
    /// Gets or sets a <see cref="DateTime"/> property.
    /// </summary>
    public DateTime DerivedProperty { get; set; }
}