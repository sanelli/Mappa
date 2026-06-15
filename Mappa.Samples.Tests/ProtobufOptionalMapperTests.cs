// <copyright file="ProtobufOptionalMapperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for <see cref="ProtobufOptionalMapper"/>.
/// </summary>
public sealed class ProtobufOptionalMapperTests
{
    private readonly ProtobufOptionalMapper mapper = new();

    /// <summary>
    /// Test <see cref="ProtobufOptionalMapper.Map"/>.
    /// </summary>
    [Fact]
    [IntegrationTest]
    public void CanMapOptionalProtobufToModelsWithOptionalValuesSet()
    {
        // Arrange
        var input = new SourceProtobufOptionalModel
        {
            ParamA = 33,
            ParamB = ProtobufCountingValues.Three,
        };

        // Act
        var actual = this.mapper.Map(input);

        // Assert
        actual.ParamA.Should().Be(input.ParamA.ToString(CultureInfo.CurrentCulture));
        actual.ParamB.Should().Be((int)input.ParamB);
    }

    /// <summary>
    /// Test <see cref="ProtobufOptionalMapper.Map"/>.
    /// </summary>
    [Fact]
    [IntegrationTest]
    public void CanMapOptionalProtobufToModelsWithOptionalValuesUnset()
    {
        // Arrange
        var input = new SourceProtobufOptionalModel();

        // Act
        var actual = this.mapper.Map(input);

        // Assert
        actual.ParamA.Should().Be(null);
        actual.ParamB.Should().Be(0);
    }

    /// <summary>
    /// Test <see cref="ProtobufOptionalMapper.MapToOptionalProtobuf(SourceClassModel)"/>.
    /// </summary>
    [Fact]
    [IntegrationTest]
    public void CanMapModelToOptionalProtobuf()
    {
        // Arrange
        var input = new SourceClassModel
        {
            ParamA = 33,
            ParamB = CountingValues.Three,
        };

        // Act
        var actual = this.mapper.MapToOptionalProtobuf(input);

        // Assert
        actual.ParamA.Should().Be(input.ParamA);
        actual.HasParamA.Should().BeTrue();
        actual.ParamB.Should().Be(ProtobufCountingValues.Three);
        actual.HasParamB.Should().BeTrue();
    }

    /// <summary>
    /// Test <see cref="ProtobufOptionalMapper.MapToOptionalProtobuf(SourceClassModel)"/>.
    /// </summary>
    [Fact]
    [IntegrationTest]
    public void CanMapModelToOptionalProtobufWithOptionalValuesUnset()
    {
        // Arrange
        var input = new SourceClassModel
        {
            ParamA = 0, ParamB = CountingValues.One,
        };

        // Act
        var actual = this.mapper.MapToOptionalProtobuf(input);

        // Assert
        actual.HasParamA.Should().BeFalse();
        actual.HasParamB.Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="ProtobufOptionalMapper.MapToOptionalProtobuf(SourceProtobufOptionalModel)"/>.
    /// </summary>
    [Fact]
    [IntegrationTest]
    public void CanMapOptionalModelToOptionalProtobuf()
    {
        // Arrange
        var input = new SourceProtobufOptionalModel
        {
            ParamA = 33,
            ParamB = ProtobufCountingValues.Three,
        };

        // Act
        var actual = this.mapper.MapToOptionalProtobuf(input);

        // Assert
        actual.ParamA.Should().Be(input.ParamA);
        actual.HasParamA.Should().BeTrue();
        actual.ParamB.Should().Be(ProtobufCountingValues.Three);
        actual.HasParamB.Should().BeTrue();
    }

    /// <summary>
    /// Test <see cref="ProtobufOptionalMapper.MapToOptionalProtobuf(SourceProtobufOptionalModel)"/>.
    /// </summary>
    [Fact]
    [IntegrationTest]
    public void CanMapOptionalModelToOptionalProtobufWithOptionalValuesUnset()
    {
        // Arrange
        var input = new SourceProtobufOptionalModel();

        // Act
        var actual = this.mapper.MapToOptionalProtobuf(input);

        // Assert
        actual.HasParamA.Should().BeFalse();
        actual.HasParamB.Should().BeFalse();
    }
}