// <copyright file="ReferenceNullableToReferenceNullableMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="ReferenceNullableToReferenceNullableMapper"/>.
/// </summary>
public sealed class ReferenceNullableToReferenceNullableMapperUnitTest
{
    private readonly ReferenceNullableToReferenceNullableMapper mapper = new();
    private readonly ReferenceToValueTypeNullableMapper referenceMapper = new();
    private readonly NullableReferenceToValueTypeNullableMapper nullableReferenceMapper = new();

    /// <summary>
    /// Unit test for <see cref="ReferenceNullableToReferenceNullableMapper.MapReferenceNullableToReferenceNullable"/>.
    /// </summary>
    [Fact]
    [IntegrationTest]
    public void CanMapReferenceNullableToReferenceNullable()
    {
        // Arrange
        SourceClassModel source = new() { ParamA = 123, ParamB = CountingValues.Three };

        // Act
        var target = this.mapper.MapReferenceNullableToReferenceNullable(source);

        // Arrange
        target.Should().NotBeNull();
        target!.ParamA.Should().Be($"{source.ParamA}");
        target.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Unit test for <see cref="ReferenceNullableToReferenceNullableMapper.MapReferenceNullableToReferenceNullable"/>
    /// when input is null.
    /// </summary>
    [Fact]
    [IntegrationTest]
    public void CanMapReferenceNullableToReferenceNullableWhenInputIsNull()
    {
        // Arrange
        SourceClassModel? source = null;

        // Act
        var target = this.mapper.MapReferenceNullableToReferenceNullable(source);

        // Arrange
        target.Should().BeNull();
    }

    /// <summary>
    /// Unit test for <see cref="ReferenceNullableToReferenceNullableMapper.MapToReferenceNullable"/>.
    /// </summary>
    [Fact]
    [IntegrationTest]
    public void CanMapToReferenceNullable()
    {
        // Arrange
        SourceClassModel source = new() { ParamA = 123, ParamB = CountingValues.Three };

        // Act
        var target = this.mapper.MapToReferenceNullable(source);

        // Arrange
        target.Should().NotBeNull();
        target!.ParamA.Should().Be($"{source.ParamA}");
        target.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Unit test for <see cref="ReferenceNullableToReferenceNullableMapper.MapFromReferenceNullable"/>.
    /// </summary>
    [Fact]
    [IntegrationTest]
    public void CanMapFromReferenceNullable()
    {
        // Arrange
        SourceClassModel source = new() { ParamA = 123, ParamB = CountingValues.Three };

        // Act
        var target = this.mapper.MapFromReferenceNullable(source);

        // Arrange
        target.Should().NotBeNull();
        target.ParamA.Should().Be($"{source.ParamA}");
        target.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Unit test for <see cref="ReferenceNullableToReferenceNullableMapper.MapFromReferenceNullable"/>.
    /// </summary>
    [Fact]
    [IntegrationTest]
    public void CannotMapFromReferenceNullableWhenInputIsNull()
    {
        // Arrange
        SourceClassModel source = null!;

        // Act
        var action = () => this.mapper.MapFromReferenceNullable(source);

        // Arrange
        action.Should().Throw<NullReferenceException>();
    }

    /// <summary>
    /// Test <see cref="ReferenceToValueTypeNullableMapper.MapToInteger"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToInteger()
    {
        // Act
        var target = this.referenceMapper.MapToInteger("30");

        // Arrange
        target.Should().Be(30);
    }

    /// <summary>
    /// Test <see cref="ReferenceToValueTypeNullableMapper.MapToNullableInteger"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToNullableInteger()
    {
        // Act
        var target = this.referenceMapper.MapToNullableInteger("30");

        // Arrange
        target.Should().Be(30);
    }

    /// <summary>
    /// Test <see cref="NullableReferenceToValueTypeNullableMapper.MapToInteger"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapNullableReferenceToInteger()
    {
        // Act
        var target = this.nullableReferenceMapper.MapToInteger("30");

        // Arrange
        target.Should().Be(30);
    }

    /// <summary>
    /// Test <see cref="NullableReferenceToValueTypeNullableMapper.MapToInteger"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapNullableReferenceToIntegerWhenInputIsNull()
    {
        // Act
        var target = () => this.nullableReferenceMapper.MapToInteger(null);

        // Arrange
        target.Should().Throw<NullReferenceException>();
    }

    /// <summary>
    /// Test <see cref="NullableReferenceToValueTypeNullableMapper.MapToNullableInteger"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapNullableReferenceToNullableInteger()
    {
        // Act
        var target = this.nullableReferenceMapper.MapToNullableInteger("30");

        // Arrange
        target.Should().Be(30);
    }

    /// <summary>
    /// Test <see cref="NullableReferenceToValueTypeNullableMapper.MapToNullableInteger"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapNullableReferenceToNullableIntegerWhenInputIsNull()
    {
        // Act
        var target = this.nullableReferenceMapper.MapToNullableInteger(null);

        // Arrange
        target.Should().Be(null);
    }
}