// <copyright file="ReferenceNullableToReferenceNullableMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="ReferenceNullableToReferenceNullableMapper"/>.
/// </summary>
public sealed class ReferenceNullableToReferenceNullableMapperUnitTest
{
    private readonly ReferenceNullableToReferenceNullableMapper mapper = new();

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
}