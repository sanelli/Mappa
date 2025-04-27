// <copyright file="DerivedClassTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Derived model class.
/// </summary>
public sealed class DerivedClassTargetModel
: BaseClassTargetModel, IInterfaceModel
{
    /// <summary>
    /// Gets or sets a string property.
    /// </summary>
    public string StringProperty { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an integer property.
    /// </summary>
    public override int IntegerProperty { get; set; }

    /// <summary>
    /// Gets or sets a character property.
    /// </summary>
    public new char CharProperty { get; set; }

    /// <summary>
    /// Gets or sets a byte property.
    /// </summary>
 #pragma warning disable CS0108, CS0114
    public byte ByteProperty { get; set; }
 #pragma warning restore CS0108, CS0114

    /// <inheritdoc />
    public long LongProperty { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the value is true or false.
    /// </summary>
    public bool BooleanProperty { get; set; }
}