// <copyright file="MappaAssignFromConstantAttributeMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using <see cref="MappaAssignFromConstantAttribute"/>.
/// </summary>
[Mappa]
public sealed partial class MappaAssignFromConstantAttributeMapper
{
    /// <summary>
    /// Tests that a mapping can happen where properties are mapped using <see cref="MappaAssignFromConstantAttribute"/>.
    /// Target model is a class.
    /// </summary>
    /// <param name="o">The input unused object.</param>
    /// <returns>The mapped object.</returns>
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.SbyteProperty), 1)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.ByteProperty), 2)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.ShortProperty), 3)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.UshortProperty), 4)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.IntProperty), 5)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.UintProperty), 6)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.LongProperty), 7)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.UlongProperty), 8)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.FloatProperty), 9.00f)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.DoubleProperty), 10.00)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.CharProperty), 'c')]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.StringProperty), "hello")]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.TypeProperty), typeof(float))]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.EnumProperty), StringComparison.CurrentCultureIgnoreCase)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.ObjectProperty), null)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.SbytePropertyArray), new sbyte[] { 1, 2, 3 })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.BytePropertyArray), new byte[] { 4, 5, 6 })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.ShortPropertyArray), new short[] { 7, 8, 9 })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.UshortPropertyArray), new ushort[] { 10, 11, 12 })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.IntPropertyArray), new[] { 13, 14, 15 })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.UintPropertyArray), new[] { 16u, 17u, 18u })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.LongPropertyArray), new[] { 19L, 20L, 21L })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.UlongPropertyArray), new[] { 22ul, 23ul, 24ul })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.FloatPropertyArray), new[] { 25.00f, 26.00f, 27.00f })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.DoublePropertyArray), new[] { 28.00, 29.00, 30.00 })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.CharPropertyArray), new[] { 'i', 'j', 'k' })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.StringPropertyArray), new[] { "hello", "world", "!" })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.TypePropertyArray), new[] { typeof(float), typeof(double), typeof(decimal) })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.EnumPropertyArray), new[] { StringComparison.CurrentCultureIgnoreCase, StringComparison.CurrentCulture, StringComparison.InvariantCulture })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.ObjectPropertyArray), new object?[] { "c", 'd', null, 36 })]
    #pragma warning disable CA1822
    public partial MappaAssignFromConstantTargetClassModel MapToClassModel(object o);
    #pragma warning restore CA1822

    /// <summary>
    /// Tests that a mapping can happen where properties are mapped using <see cref="MappaAssignFromConstantAttribute"/>.
    /// Target model is a class.
    /// </summary>
    /// <param name="o">The input unused object.</param>
    /// <returns>The mapped object.</returns>
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.SbyteProperty), 1)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.ByteProperty), 2)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.ShortProperty), 3)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.UshortProperty), 4)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.IntProperty), 5)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.UintProperty), 6)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.LongProperty), 7)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.UlongProperty), 8)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.FloatProperty), 9.00f)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.DoubleProperty), 10.00)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.CharProperty), 'c')]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.StringProperty), "hello")]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.TypeProperty), typeof(float))]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.EnumProperty), StringComparison.CurrentCultureIgnoreCase)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.ObjectProperty), null)]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.SbytePropertyArray), new sbyte[] { 1, 2, 3 })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.BytePropertyArray), new byte[] { 4, 5, 6 })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.ShortPropertyArray), new short[] { 7, 8, 9 })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.UshortPropertyArray), new ushort[] { 10, 11, 12 })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.IntPropertyArray), new[] { 13, 14, 15 })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.UintPropertyArray), new[] { 16u, 17u, 18u })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.LongPropertyArray), new[] { 19L, 20L, 21L })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.UlongPropertyArray), new[] { 22ul, 23ul, 24ul })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.FloatPropertyArray), new[] { 25.00f, 26.00f, 27.00f })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.DoublePropertyArray), new[] { 28.00, 29.00, 30.00 })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.CharPropertyArray), new[] { 'i', 'j', 'k' })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.StringPropertyArray), new[] { "hello", "world", "!" })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.TypePropertyArray), new[] { typeof(float), typeof(double), typeof(decimal) })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.EnumPropertyArray), new[] { StringComparison.CurrentCultureIgnoreCase, StringComparison.CurrentCulture, StringComparison.InvariantCulture })]
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.ObjectPropertyArray), new object?[] { "c", 'd', null, 36 })]
    #pragma warning disable CA1822
    public partial MappaAssignFromConstantTargetRecordModel MapToRecordModel(object o);
    #pragma warning restore CA1822
}