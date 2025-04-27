// <copyright file="DerivedClassTargetModelWithConstructor.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Derived model class.
/// </summary>
/// <param name="stringProperty">A string property.</param>
/// <param name="integerProperty">An integer property.</param>
/// <param name="charProperty">A char property.</param>
/// <param name="byteProperty">A byte property.</param>
/// <param name="longProperty">A long property.</param>
/// <param name="booleanProperty">A boolean property.</param>
public sealed class DerivedClassTargetModelWithConstructor(string stringProperty, int integerProperty, char charProperty, byte byteProperty, long longProperty, bool booleanProperty)
: BaseClassTargetModel, IInterfaceModel
{
    /// <summary>
    /// Gets or sets a string property.
    /// </summary>
    public string StringProperty { get; set; } = stringProperty;

    /// <summary>
    /// Gets or sets an integer property.
    /// </summary>
    public override int IntegerProperty { get; set; } = integerProperty;

    /// <summary>
    /// Gets or sets a character property.
    /// </summary>
    public new char CharProperty { get; set; } = charProperty;

    /// <summary>
    /// Gets or sets a byte property.
    /// </summary>
 #pragma warning disable CS0108, CS0114
    public byte ByteProperty { get; set; } = byteProperty;
 #pragma warning restore CS0108, CS0114

    /// <inheritdoc />
    public long LongProperty { get; set; } = longProperty;

    /// <summary>
    /// Gets or sets a value indicating whether the value is true or false.
    /// </summary>
    public bool BooleanProperty { get; set; } = booleanProperty;
}