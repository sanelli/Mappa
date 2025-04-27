// <copyright file="DerivedClassSourceModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Derived model class.
/// </summary>
public sealed class DerivedClassSourceModel
: BaseClassSourceModel, IInterfaceModel
{
    /// <inheritdoc />
    public override string StringProperty { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an integer property.
    /// </summary>
    public new int IntegerProperty { get; set; }

    /// <summary>
    /// Gets or sets a character property.
    /// </summary>
 #pragma warning disable CS0108, CS0114
    public char CharProperty { get; set; }
 #pragma warning restore CS0108, CS0114

    /// <summary>
    /// Gets or sets a byte property.
    /// </summary>
    public byte ByteProperty { get; set; }

    /// <inheritdoc />
    public long LongProperty { get; set; }
}