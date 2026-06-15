// <copyright file="MappaBsonMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using MongoDB.Bson;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Dependency.Bson.Tests;

/// <summary>
/// Unit tests for <see cref="MappaBsonMapper"/>.
/// </summary>
public sealed class MappaBsonMapperUnitTests
{
    private readonly MappaBsonMapper mapper = new();

    /// <summary>
    /// Tests <see cref="MappaBsonMapper.MapToObjectId(string)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToObjectIdFromString()
    {
        // Arrange
        var source = ObjectId.GenerateNewId();
        var input = source.ToString();

        // Act
        var actual = this.mapper.MapToObjectId(input);

        // Assert
        actual.Should().Be(source);
    }

    /// <summary>
    /// Tests <see cref="MappaBsonMapper.MapToObjectId(string)"/>
    /// throws when input is <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapToObjectIdFromStringThrowsWhenInputIsNull()
    {
        // Arrange
        var action = () => this.mapper.MapToObjectId((string)null!);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Tests <see cref="MappaBsonMapper.MapToNullableObjectId"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToNullableObjectId()
    {
        // Arrange
        var source = ObjectId.GenerateNewId();
        string? input = source.ToString();

        // Act
        var actual = this.mapper.MapToNullableObjectId(input);

        // Assert
        actual.Should().Be(source);
    }

    /// <summary>
    /// Tests <see cref="MappaBsonMapper.MapToNullableObjectId"/> when input is null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToNullableObjectIdWhenInputIsNull()
    {
        // Act
        var actual = this.mapper.MapToNullableObjectId(null);

        // Assert
        actual.Should().Be(null);
    }

    /// <summary>
    /// Tests <see cref="MappaBsonMapper.MapToObjectId(byte[])"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToObjectIdFromBytes()
    {
        // Arrange
        var source = ObjectId.GenerateNewId();
        var input = source.ToByteArray();

        // Act
        var actual = this.mapper.MapToObjectId(input);

        // Assert
        actual.Should().Be(source);
    }

    /// <summary>
    /// Tests <see cref="MappaBsonMapper.MapToString(ObjectId)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToStringFromObjectId()
    {
        // Arrange
        var input = ObjectId.GenerateNewId();

        // Act
        var actual = this.mapper.MapToString(input);

        // Assert
        actual.Should().Be(input.ToString());
    }

    /// <summary>
    /// Tests <see cref="MappaBsonMapper.MapToNullableString"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToNullableStringFromNullableObjectId()
    {
        // Arrange
        ObjectId? input = ObjectId.GenerateNewId();

        // Act
        var actual = this.mapper.MapToNullableString(input);

        // Assert
        actual.Should().Be(input.ToString());
    }

    /// <summary>
    /// Tests <see cref="MappaBsonMapper.MapToNullableString"/> when input is null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToNullableStringFromNullableObjectIdWhenInputIsNull()
    {
        // Act
        var actual = this.mapper.MapToNullableString(null);

        // Assert
        actual.Should().Be(null);
    }

    /// <summary>
    /// Tests <see cref="MappaBsonMapper.MapToBytes"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToBytes()
    {
        // Arrange
        var input = ObjectId.GenerateNewId();

        // Act
        var actual = this.mapper.MapToBytes(input);

        // Assert
        var expected = input.ToByteArray();
        actual.Should().HaveCount(expected.Length);
        for (int index = 0; index < expected.Length; index++)
        {
            actual[index].Should().Be(expected[index]);
        }
    }
}