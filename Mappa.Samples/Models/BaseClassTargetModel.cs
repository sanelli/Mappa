// <copyright file="BaseClassTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A base class exposing some properties.
/// </summary>
public abstract class BaseClassTargetModel
{
    /// <summary>
    /// Gets or sets an integer property.
    /// </summary>
    public virtual int IntegerProperty { get; set; }

    /// <summary>
    /// Gets or sets a character property.
    /// </summary>
    public char CharProperty { get; set; }

    /// <summary>
    /// Gets or sets a byte property.
    /// </summary>
    public byte ByteProperty { get; set; }
}