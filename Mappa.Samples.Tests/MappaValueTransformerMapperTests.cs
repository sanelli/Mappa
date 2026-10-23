// <copyright file="MappaValueTransformerMapperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for <see cref="MappaValueTransformerMapper"/>.
/// </summary>
public sealed class MappaValueTransformerMapperTests
{
    /// <summary>
    /// Test the class-level trim transformer applies to every string while the
    /// member-scoped upper transformer applies only to the code.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanTrimEveryStringAndUpperCaseOnlyTheCode()
    {
        // Arrange
        var mapper = new MappaValueTransformerMapper();
        var input = new ValueTransformerSourceModel
        {
            Name = "  Ada  ",
            Code = "  ab-1  ",
            Note = "  hello  ",
        };

        // Act
        var actual = mapper.MapTrimmed(input);

        // Assert
        actual.Name.Should().Be("Ada");
        actual.Code.Should().Be("AB-1");
        actual.Note.Should().Be("hello");
    }

    /// <summary>
    /// Test dependency transformers are not applied unless explicitly enabled.
    /// </summary>
    [Fact]
    [UnitTest]
    public void DoesNotApplyDependencyTransformersByDefault()
    {
        // Arrange
        var mapper = new MappaValueTransformerMapper();
        var input = new ValueTransformerSourceModel
        {
            Name = "  Ada   Lovelace  ",
        };

        // Act
        var actual = mapper.MapTrimmed(input);

        // Assert
        actual.Name.Should().Be("Ada   Lovelace");
    }

    /// <summary>
    /// Test the context-aware transformer runs before the class-level trim, so the
    /// truncation is applied to the untrimmed value.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanTruncateNoteBeforeTrimmingUsingContext()
    {
        // Arrange
        var mapper = new MappaValueTransformerMapper();
        var input = new ValueTransformerSourceModel
        {
            Name = "  Ada  ",
            Note = "  hello  ",
        };
        MappaContext context = new Dictionary<string, object>
        {
            ["maxLength"] = 5,
        };

        // Act
        var actual = mapper.MapTruncated(input, context);

        // Assert
        actual.Name.Should().Be("Ada");
        actual.Note.Should().Be("hel");
    }

    /// <summary>
    /// Test the context-aware transformer leaves the value untouched when the context has no limit.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapNoteWithoutTruncationWhenContextHasNoLimit()
    {
        // Arrange
        var mapper = new MappaValueTransformerMapper();
        var input = new ValueTransformerSourceModel
        {
            Name = "Ada",
            Note = "  hello  ",
        };
        MappaContext context = new Dictionary<string, object>();

        // Act
        var actual = mapper.MapTruncated(input, context);

        // Assert
        actual.Name.Should().Be("Ada");
        actual.Note.Should().Be("hello");
    }

    /// <summary>
    /// Test the dependency transformer runs after the local trim when the setting is enabled on the method.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanApplyDependencyTransformersWhenEnabledOnTheMethod()
    {
        // Arrange
        var mapper = new MappaValueTransformerMapper();
        var input = new ValueTransformerSourceModel
        {
            Name = "  Ada   Lovelace  ",
        };

        // Act
        var actual = mapper.MapWithDependencyTransformers(input);

        // Assert
        actual.Name.Should().Be("Ada Lovelace");
    }
}