// <copyright file="MappaAssignFromConstantTargetClassModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples.Models;

#pragma warning disable CA1819

/// <summary>
/// Model used as target to test <see cref="MappaAssignFromConstantAttribute"/>.
/// </summary>
public sealed class MappaAssignFromConstantTargetClassModel
{
    /// <summary>
    /// Gets or sets an <see cref="sbyte"/> property.
    /// </summary>
    public sbyte SbyteProperty { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="byte"/> property.
    /// </summary>
    public byte ByteProperty { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="short"/> property.
    /// </summary>
    public short ShortProperty { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="ushort"/> property.
    /// </summary>
    public ushort UshortProperty { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="int"/> property.
    /// </summary>
    public int IntProperty { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="uint"/> property.
    /// </summary>
    public uint UintProperty { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="long"/> property.
    /// </summary>
    public long LongProperty { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="ulong"/> property.
    /// </summary>
    public ulong UlongProperty { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="float"/> property.
    /// </summary>
    public float FloatProperty { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="double"/> property.
    /// </summary>
    public double DoubleProperty { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="char"/> property.
    /// </summary>
    public char CharProperty { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="string"/> property.
    /// </summary>
    public string StringProperty { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an <see cref="object"/> property.
    /// </summary>
    public object? ObjectProperty { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="Enum"/> property.
    /// </summary>
    public StringComparison EnumProperty { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="Type"/> property.
    /// </summary>
    public Type TypeProperty { get; set; } = typeof(object);

    /// <summary>
    /// Gets or sets an <see cref="sbyte"/> property array.
    /// </summary>
    public sbyte[] SbytePropertyArray { get; set; } = [];

    /// <summary>
    /// Gets or sets an <see cref="byte"/> property array.
    /// </summary>
    public byte[] BytePropertyArray { get; set; } = [];

    /// <summary>
    /// Gets or sets an <see cref="short"/> property array.
    /// </summary>
    public short[] ShortPropertyArray { get; set; } = [];

    /// <summary>
    /// Gets or sets an <see cref="ushort"/> property array.
    /// </summary>
    public ushort[] UshortPropertyArray { get; set; } = [];

    /// <summary>
    /// Gets or sets an <see cref="int"/> property array.
    /// </summary>
    public int[] IntPropertyArray { get; set; } = [];

    /// <summary>
    /// Gets or sets an <see cref="uint"/> property array.
    /// </summary>
    public uint[] UintPropertyArray { get; set; } = [];

    /// <summary>
    /// Gets or sets an <see cref="long"/> property array.
    /// </summary>
    public long[] LongPropertyArray { get; set; } = [];

    /// <summary>
    /// Gets or sets an <see cref="ulong"/> property array.
    /// </summary>
    public ulong[] UlongPropertyArray { get; set; } = [];

    /// <summary>
    /// Gets or sets an <see cref="float"/> property array.
    /// </summary>
    public float[] FloatPropertyArray { get; set; } = [];

    /// <summary>
    /// Gets or sets an <see cref="double"/> property array.
    /// </summary>
    public double[] DoublePropertyArray { get; set; } = [];

    /// <summary>
    /// Gets or sets an <see cref="char"/> property array.
    /// </summary>
    public char[] CharPropertyArray { get; set; } = [];

    /// <summary>
    /// Gets or sets an <see cref="string"/> property array.
    /// </summary>
    public string[] StringPropertyArray { get; set; } = [];

    /// <summary>
    /// Gets or sets an <see cref="object"/> property array.
    /// </summary>
    public object?[] ObjectPropertyArray { get; set; } = [];

    /// <summary>
    /// Gets or sets an <see cref="Enum"/> property array.
    /// </summary>
    public StringComparison[] EnumPropertyArray { get; set; } = [];

    /// <summary>
    /// Gets or sets an <see cref="Type"/> property array.
    /// </summary>
    public Type[] TypePropertyArray { get; set; } = [];
}

#pragma warning restore CA1819