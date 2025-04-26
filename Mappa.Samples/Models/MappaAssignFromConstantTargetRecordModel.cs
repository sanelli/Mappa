// <copyright file="MappaAssignFromConstantTargetRecordModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples.Models;

#pragma warning disable CA1819

/// <summary>
/// Model with constructor used as target to test <see cref="MappaAssignFromConstantAttribute"/>.
/// </summary>
public sealed record MappaAssignFromConstantTargetRecordModel(
    sbyte SbyteProperty,
    byte ByteProperty,
    short ShortProperty,
    ushort UshortProperty,
    int IntProperty,
    uint UintProperty,
    long LongProperty,
    ulong UlongProperty,
    float FloatProperty,
    double DoubleProperty,
    char CharProperty,
    string StringProperty,
    object? ObjectProperty,
    StringComparison EnumProperty,
    Type TypeProperty,
    sbyte[] SbytePropertyArray,
    byte[] BytePropertyArray,
    short[] ShortPropertyArray,
    ushort[] UshortPropertyArray,
    int[] IntPropertyArray,
    uint[] UintPropertyArray,
    long[] LongPropertyArray,
    ulong[] UlongPropertyArray,
    float[] FloatPropertyArray,
    double[] DoublePropertyArray,
    char[] CharPropertyArray,
    string[] StringPropertyArray,
    object?[] ObjectPropertyArray,
    StringComparison[] EnumPropertyArray,
    Type[] TypePropertyArray);

#pragma warning restore CA1819