// <copyright file="SourceUnmappedClass.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.Polymorphism.Two;

/// <summary>
/// A base class.
/// </summary>
public class SourceUnmappedClass : ISourceBaseClass
{
    /// <summary>
    /// Gets or sets a <see cref="string"/> property.
    /// </summary>
    public required string UnmappedProperty { get; set; }

    /// <inheritdoc />
    public int NumericProperty { get; set; }
}