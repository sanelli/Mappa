// <copyright file="SourceWithDependencyWithSourceBaseClass.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.Polymorphism.One;

/// <summary>
/// Source class containing one of the derived classes as property.
/// </summary>
public sealed class SourceWithDependencyWithSourceBaseClass
{
    /// <summary>
    /// Gets or sets a numeric property.
    /// </summary>
    public int NumericProperty { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="SourceBaseClass"/>.
    /// </summary>
    public required SourceBaseClass NestedProperty { get; set; }
}