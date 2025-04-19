// <copyright file="SourceClassWithMultipleFieldsForDependencyModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A class with an inner class as property.
/// </summary>
public sealed class SourceClassWithMultipleFieldsForDependencyModel
{
    /// <summary>
    /// Gets or sets the inner model.
    /// </summary>
    public SourceClassModel InnerModel { get; set; } = new();

    /// <summary>
    /// Gets or sets an integer value.
    /// </summary>
    public int Property1 { get; set; }

    /// <summary>
    /// Gets or sets a long value.
    /// </summary>
    public long Property2 { get; set; }

    /// <summary>
    /// Gets or sets an unsigned long value.
    /// </summary>
    public ulong Property3 { get; set; }

    /// <summary>
    /// Gets or sets a byte value.
    /// </summary>
    public byte Property4 { get; set; }

    /// <summary>
    /// Gets or sets a sbyte value.
    /// </summary>
    public sbyte Property5 { get; set; }

    /// <summary>
    /// Gets or sets a float value.
    /// </summary>
    public float Property6 { get; set; }

    /// <summary>
    /// Gets or sets a double value.
    /// </summary>
    public double Property7 { get; set; }
}