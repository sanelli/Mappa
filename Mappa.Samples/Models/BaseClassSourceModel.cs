// <copyright file="BaseClassSourceModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A base class exposing some properties.
/// </summary>
public abstract class BaseClassSourceModel
{
    /// <summary>
    /// Gets or sets a string property.
    /// </summary>
    public virtual string StringProperty { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an integer property.
    /// </summary>
    public int IntegerProperty { get; set; }

    /// <summary>
    /// Gets or sets a character property.
    /// </summary>
    public char CharProperty { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the value is true or false.
    /// </summary>
    public bool BooleanProperty { get; set; }
}