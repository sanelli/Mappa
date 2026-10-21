// <copyright file="BsonWellKnownTypesMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using MongoDB.Bson;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="BsonWellKnownTypesMapper"/>.
/// </summary>
public sealed class BsonWellKnownTypesMapperUnitTests
{
    private readonly BsonWellKnownTypesMapper mapper = new();

    /// <summary>
    /// Tests <see cref="ObjectId"/> ↔ <see cref="string"/> mapping.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapObjectIdAndString()
    {
        // Arrange
        var objectId = ObjectId.GenerateNewId();

        // Act
        var asString = this.mapper.MapObjectIdToString(objectId);
        var roundTrip = this.mapper.MapStringToObjectId(asString);

        // Assert
        asString.Should().Be(objectId.ToString());
        roundTrip.Should().Be(objectId);
    }

    /// <summary>
    /// Tests <see cref="ObjectId"/> ↔ <see cref="byte"/> array mapping.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapObjectIdAndBytes()
    {
        // Arrange
        var objectId = ObjectId.GenerateNewId();

        // Act
        var asBytes = this.mapper.MapObjectIdToBytes(objectId);
        var roundTrip = this.mapper.MapBytesToObjectId(asBytes);

        // Assert
        asBytes.Should().Equal(objectId.ToByteArray());
        roundTrip.Should().Be(objectId);
    }

    /// <summary>
    /// Tests <see cref="Decimal128"/> → <see cref="decimal"/> mapping.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDecimal128ToDecimal()
    {
        // Arrange
        var source = new Decimal128(12.34m);

        // Act
        var target = this.mapper.MapDecimal128ToDecimal(source);

        // Assert
        target.Should().Be(12.34m);
    }
}