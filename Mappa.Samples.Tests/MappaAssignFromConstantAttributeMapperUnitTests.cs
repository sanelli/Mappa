// <copyright file="MappaAssignFromConstantAttributeMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

#pragma warning disable CA1861
#pragma warning disable CA2263

/// <summary>
/// Unit tests for <see cref="MappaAssignFromConstantAttributeMapper"/>.
/// </summary>
public sealed class MappaAssignFromConstantAttributeMapperUnitTests
{
    private readonly MappaAssignFromConstantAttributeMapper mapper = new();

    /// <summary>
    /// Unit tets for <see cref="MappaAssignFromConstantAttributeMapper.MapToClassModel"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapToClassModel()
    {
        // Arrange
        var source = new object();

        // Act
        var target = this.mapper.MapToClassModel(source);

        // Assert
        target.SbyteProperty.Should().Be(1);
        target.ByteProperty.Should().Be(2);
        target.ShortProperty.Should().Be(3);
        target.UshortProperty.Should().Be(4);
        target.IntProperty.Should().Be(5);
        target.UintProperty.Should().Be(6);
        target.LongProperty.Should().Be(7);
        target.UlongProperty.Should().Be(8);
        target.FloatProperty.Should().Be(9.00f);
        target.DoubleProperty.Should().Be(10.00);
        target.CharProperty.Should().Be('c');
        target.StringProperty.Should().Be("hello");
        target.TypeProperty.Should().Be(typeof(float));
        target.EnumProperty.Should().Be(StringComparison.CurrentCultureIgnoreCase);
        target.ObjectProperty.Should().Be(null);

        target.SbytePropertyArray.Should().BeEquivalentTo(new sbyte[] { 1, 2, 3 });
        target.BytePropertyArray.Should().BeEquivalentTo(new byte[] { 4, 5, 6 });
        target.ShortPropertyArray.Should().BeEquivalentTo(new short[] { 7, 8, 9 });
        target.UshortPropertyArray.Should().BeEquivalentTo(new ushort[] { 10, 11, 12 });
        target.IntPropertyArray.Should().BeEquivalentTo([13, 14, 15]);
        target.UintPropertyArray.Should().BeEquivalentTo([16u, 17u, 18u]);
        target.LongPropertyArray.Should().BeEquivalentTo([19L, 20L, 21L]);
        target.UlongPropertyArray.Should().BeEquivalentTo([22ul, 23ul, 24ul]);
        target.FloatPropertyArray.Should().BeEquivalentTo([25.00f, 26.00f, 27.00f]);
        target.DoublePropertyArray.Should().BeEquivalentTo([28.00, 29.00, 30.00]);
        target.CharPropertyArray.Should().BeEquivalentTo(['i', 'j', 'k']);
        target.StringPropertyArray.Should().BeEquivalentTo("hello", "world", "!");
        target.TypePropertyArray.Should().BeEquivalentTo([typeof(float), typeof(double), typeof(decimal)]);
        target.EnumPropertyArray.Should().BeEquivalentTo([StringComparison.CurrentCultureIgnoreCase, StringComparison.CurrentCulture, StringComparison.InvariantCulture]);
        target.ObjectPropertyArray.Should().BeEquivalentTo(new object?[] { "c", 'd', null, 36 });
    }

    /// <summary>
    /// Unit tets for <see cref="MappaAssignFromConstantAttributeMapper.MapToRecordModel"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapToRecordModel()
    {
        // Arrange
        var source = new object();

        // Act
        var target = this.mapper.MapToRecordModel(source);

        // Assert
        target.SbyteProperty.Should().Be(1);
        target.ByteProperty.Should().Be(2);
        target.ShortProperty.Should().Be(3);
        target.UshortProperty.Should().Be(4);
        target.IntProperty.Should().Be(5);
        target.UintProperty.Should().Be(6);
        target.LongProperty.Should().Be(7);
        target.UlongProperty.Should().Be(8);
        target.FloatProperty.Should().Be(9.00f);
        target.DoubleProperty.Should().Be(10.00);
        target.CharProperty.Should().Be('c');
        target.StringProperty.Should().Be("hello");
        target.TypeProperty.Should().Be(typeof(float));
        target.EnumProperty.Should().Be(StringComparison.CurrentCultureIgnoreCase);
        target.ObjectProperty.Should().Be(null);

        target.SbytePropertyArray.Should().BeEquivalentTo(new sbyte[] { 1, 2, 3 });
        target.BytePropertyArray.Should().BeEquivalentTo(new byte[] { 4, 5, 6 });
        target.ShortPropertyArray.Should().BeEquivalentTo(new short[] { 7, 8, 9 });
        target.UshortPropertyArray.Should().BeEquivalentTo(new ushort[] { 10, 11, 12 });
        target.IntPropertyArray.Should().BeEquivalentTo([13, 14, 15]);
        target.UintPropertyArray.Should().BeEquivalentTo([16u, 17u, 18u]);
        target.LongPropertyArray.Should().BeEquivalentTo([19L, 20L, 21L]);
        target.UlongPropertyArray.Should().BeEquivalentTo([22ul, 23ul, 24ul]);
        target.FloatPropertyArray.Should().BeEquivalentTo([25.00f, 26.00f, 27.00f]);
        target.DoublePropertyArray.Should().BeEquivalentTo([28.00, 29.00, 30.00]);
        target.CharPropertyArray.Should().BeEquivalentTo(['i', 'j', 'k']);
        target.StringPropertyArray.Should().BeEquivalentTo("hello", "world", "!");
        target.TypePropertyArray.Should().BeEquivalentTo([typeof(float), typeof(double), typeof(decimal)]);
        target.EnumPropertyArray.Should().BeEquivalentTo([StringComparison.CurrentCultureIgnoreCase, StringComparison.CurrentCulture, StringComparison.InvariantCulture]);
        target.ObjectPropertyArray.Should().BeEquivalentTo(new object?[] { "c", 'd', null, 36 });
    }
}