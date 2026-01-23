// <copyright file="SourceFirstClass.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.Polymorphism.Two;

/// <summary>
/// A base class.
/// </summary>
public class SourceFirstClass : ISourceBaseClass
{
    /// <summary>
    /// Gets or sets a <see cref="DateTime"/> property.
    /// </summary>
    public DateTime DateTimeProperty { get; set; }

    /// <inheritdoc />
    public int NumericProperty { get; set; }
}