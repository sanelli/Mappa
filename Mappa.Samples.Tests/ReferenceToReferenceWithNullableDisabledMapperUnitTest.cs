// <copyright file="ReferenceToReferenceWithNullableDisabledMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="ReferenceToReferenceWithNullableDisabledMapper"/>.
/// </summary>
public sealed class ReferenceToReferenceWithNullableDisabledMapperUnitTest
{
    private readonly ReferenceToReferenceWithNullableDisabledMapper mapper = new();
    private readonly ReferenceToValueTypeWithNullableDisabledMapper valueTypeMapper = new();

    /// <summary>
    /// Unit test for <see cref="ReferenceToReferenceWithNullableDisabledMapper.Map"/>
    /// when input is not null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapWhenNotNull()
    {
        // Arrange
        SourceClassModel source = new() { ParamA = 123, ParamB = CountingValues.Three };

        // Act
        var target = this.mapper.Map(source);

        // Arrange
        target.Should().NotBeNull();
        target.ParamA.Should().Be($"{source.ParamA}");
        target.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Unit test for <see cref="ReferenceToReferenceWithNullableDisabledMapper.Map"/>
    /// when input is null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapWhenNull()
    {
        // Act
        var target = this.mapper.Map(null);

        // Arrange
        target.Should().BeNull();
    }

    /// <summary>
    /// Test <see cref="ReferenceToValueTypeWithNullableDisabledMapper.MapToInteger"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToInteger()
    {
        // Act
        var target = this.valueTypeMapper.MapToInteger("30");

        // Arrange
        target.Should().Be(30);
    }

    /// <summary>
    /// Test <see cref="ReferenceToValueTypeWithNullableDisabledMapper.MapToInteger"/> when input is null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToIntegerWithInputIsNull()
    {
        // Act
        var target = () => this.valueTypeMapper.MapToInteger(null);

        // Arrange
        target.Should().Throw<NullReferenceException>();
    }

    /// <summary>
    /// Test <see cref="ReferenceToValueTypeWithNullableDisabledMapper.MapToNullableInteger"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToNullableInteger()
    {
        // Act
        var target = this.valueTypeMapper.MapToNullableInteger("30");

        // Arrange
        target.Should().Be(30);
    }

    /// <summary>
    /// Test <see cref="ReferenceToValueTypeWithNullableDisabledMapper.MapToNullableInteger"/> when input is null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToNullableIntegerWhenInputIsNull()
    {
        // Act
        var target = this.valueTypeMapper.MapToNullableInteger(null);

        // Arrange
        target.Should().Be(null);
    }
}