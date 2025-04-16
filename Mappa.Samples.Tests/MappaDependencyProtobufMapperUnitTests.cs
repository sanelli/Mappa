// <copyright file="MappaDependencyProtobufMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="MappaDependencyProtobufMapper"/>.
/// </summary>
public sealed class MappaDependencyProtobufMapperUnitTests
{
    private readonly MappaDependencyProtobufMapper mapper = new();

    /// <summary>
    /// Tests <see cref="MappaDependencyProtobufMapper.MapWithDependencies"/>.
    /// </summary>
    [Fact]
    [Bug("#121")]
    [UnitTest]
    public void CanMapWithDependencies()
    {
        // Arrange
        var timestamp = new DateTime(1984, 06, 03, 14, 22, 00, DateTimeKind.Utc);
        var source = new MappaDependencySourceRecord(timestamp);

        // Act
        var target = this.mapper.MapWithDependencies(source);

        // Asset
        target.TimeStamp.ToDateTime().Should().Be(timestamp);
    }
}