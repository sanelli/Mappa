// <copyright file="ProtobufOneofPolymorphismMappersUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models.Polymorphism;
using Mappa.Samples.Models.ProtobufOneof;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the protobuf <c>oneof</c> polymorphism mappers.
/// </summary>
public sealed class ProtobufOneofPolymorphismMappersUnitTests
{
    private static readonly ProtobufOneofToPocoMapper ToPocoMapper = new();
    private static readonly ProtobufOneofToPocoWithCustomExceptionMapper ToPocoWithCustomExceptionMapper = new();
    private static readonly ProtobufOneofFromPocoMapper FromPocoMapper = new();
    private static readonly ProtobufOneofFromPocoWithFallbackMapper FromPocoWithFallbackMapper = new();
    private static readonly ProtobufOneofToProtobufMapper ToProtobufMapper = new();
    private static readonly ProtobufOneofToProtobufIgnoringUnsetMapper ToProtobufIgnoringUnsetMapper = new();

    /// <summary>
    /// Tests <see cref="ProtobufOneofToPocoMapper.Map"/> with the <c>Ok</c> arm.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapProtobufOkToPoco()
    {
        // Arrange.
        var source = CreateOkResult();

        // Act.
        var target = ToPocoMapper.Map(source);

        // Assert.
        var typedTarget = target.Should().BeOfType<ProtobufOneofOkResultModel>().Subject;
        typedTarget.Value.Should().Be("value");
        typedTarget.Description.Should().Be("ok description");
    }

    /// <summary>
    /// Tests <see cref="ProtobufOneofToPocoMapper.Map"/> with the <c>Error</c> arm.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapProtobufErrorToPoco()
    {
        // Arrange.
        var source = CreateErrorResult();

        // Act.
        var target = ToPocoMapper.Map(source);

        // Assert.
        var typedTarget = target.Should().BeOfType<ProtobufOneofErrorResultModel>().Subject;
        typedTarget.Code.Should().Be(42);
        typedTarget.Description.Should().Be("error description");
    }

    /// <summary>
    /// Tests <see cref="ProtobufOneofToPocoMapper.Map"/> throws when the oneof is not set.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapProtobufWithoutOneofToPocoThrows()
    {
        // Arrange.
        var source = new ProtobufOneofResult { Description = "nothing" };

        // Act.
        var act = () => ToPocoMapper.Map(source);

        // Assert.
        source.ResultCase.Should().Be(ProtobufOneofResult.ResultOneofCase.None);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Tests <see cref="ProtobufOneofToPocoWithCustomExceptionMapper.Map"/> with the <c>Ok</c> arm.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapProtobufOkToPocoWithCustomExceptionMapper()
    {
        // Arrange.
        var source = CreateOkResult();

        // Act.
        var target = ToPocoWithCustomExceptionMapper.Map(source);

        // Assert.
        var typedTarget = target.Should().BeOfType<ProtobufOneofOkResultModel>().Subject;
        typedTarget.Value.Should().Be("value");
        typedTarget.Description.Should().Be("ok description");
    }

    /// <summary>
    /// Tests <see cref="ProtobufOneofToPocoWithCustomExceptionMapper.Map"/> throws the custom exception
    /// when the oneof is not set.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapProtobufWithoutOneofToPocoThrowsCustomException()
    {
        // Arrange.
        var source = new ProtobufOneofResult { Description = "nothing" };

        // Act.
        var act = () => ToPocoWithCustomExceptionMapper.Map(source);

        // Assert.
        act.Should().Throw<PolymorphismCustomException>();
    }

    /// <summary>
    /// Tests <see cref="ProtobufOneofFromPocoMapper.Map"/> with an <see cref="ProtobufOneofOkResultModel"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapPocoOkToProtobuf()
    {
        // Arrange.
        var source = new ProtobufOneofOkResultModel { Value = "value", Description = "ok description" };

        // Act.
        var target = FromPocoMapper.Map(source);

        // Assert.
        target.ResultCase.Should().Be(ProtobufOneofResult.ResultOneofCase.Ok);
        target.Ok.Value.Should().Be("value");
        target.Ok.Description.Should().Be("ok description");
    }

    /// <summary>
    /// Tests <see cref="ProtobufOneofFromPocoMapper.Map"/> with an <see cref="ProtobufOneofErrorResultModel"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapPocoErrorToProtobuf()
    {
        // Arrange.
        var source = new ProtobufOneofErrorResultModel { Code = 42, Description = "error description" };

        // Act.
        var target = FromPocoMapper.Map(source);

        // Assert.
        target.ResultCase.Should().Be(ProtobufOneofResult.ResultOneofCase.Error);
        target.Error.Code.Should().Be(42);
        target.Error.Description.Should().Be("error description");
    }

    /// <summary>
    /// Tests <see cref="ProtobufOneofFromPocoMapper.Map"/> ignores POCOs that are not covered by an arm.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapUnknownPocoToProtobufLeavesTheOneofUnset()
    {
        // Arrange.
        var source = new ProtobufOneofUnknownResultModel { Description = "unknown" };

        // Act.
        var target = FromPocoMapper.Map(source);

        // Assert.
        target.ResultCase.Should().Be(ProtobufOneofResult.ResultOneofCase.None);
    }

    /// <summary>
    /// Tests <see cref="ProtobufOneofFromPocoWithFallbackMapper.Map"/> with an <see cref="ProtobufOneofOkResultModel"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapPocoOkToProtobufWithFallbackMapper()
    {
        // Arrange.
        var source = new ProtobufOneofOkResultModel { Value = "value", Description = "ok description" };

        // Act.
        var target = FromPocoWithFallbackMapper.Map(source);

        // Assert.
        target.ReplyCase.Should().Be(ProtobufOneofReply.ReplyOneofCase.Ok);
        target.Ok.Value.Should().Be("value");
        target.Ok.Description.Should().Be("ok description");
    }

    /// <summary>
    /// Tests <see cref="ProtobufOneofFromPocoWithFallbackMapper.Map"/> with an <see cref="ProtobufOneofErrorResultModel"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapPocoErrorToProtobufWithFallbackMapper()
    {
        // Arrange.
        var source = new ProtobufOneofErrorResultModel { Code = 42, Description = "error description" };

        // Act.
        var target = FromPocoWithFallbackMapper.Map(source);

        // Assert.
        target.ReplyCase.Should().Be(ProtobufOneofReply.ReplyOneofCase.Error);
        target.Error.Code.Should().Be(42);
        target.Error.Description.Should().Be("error description");
    }

    /// <summary>
    /// Tests <see cref="ProtobufOneofFromPocoWithFallbackMapper.Map"/> maps POCOs that are not
    /// covered by an arm into the <c>Fallback</c> arm.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapUnknownPocoToProtobufFallsBackToTheFallbackArm()
    {
        // Arrange.
        var source = new ProtobufOneofUnknownResultModel { Description = "unknown" };

        // Act.
        var target = FromPocoWithFallbackMapper.Map(source);

        // Assert.
        target.ReplyCase.Should().Be(ProtobufOneofReply.ReplyOneofCase.Fallback);
        target.Fallback.Description.Should().Be("unknown");
    }

    /// <summary>
    /// Tests <see cref="ProtobufOneofToProtobufMapper.Map"/> with the <c>Ok</c> arm.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapProtobufOkToProtobuf()
    {
        // Arrange.
        var source = CreateOkResult();

        // Act.
        var target = ToProtobufMapper.Map(source);

        // Assert.
        target.ReplyCase.Should().Be(ProtobufOneofReply.ReplyOneofCase.Ok);
        target.Ok.Value.Should().Be("value");
        target.Ok.Description.Should().Be("ok description");
    }

    /// <summary>
    /// Tests <see cref="ProtobufOneofToProtobufMapper.Map"/> with the <c>Error</c> arm.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapProtobufErrorToProtobuf()
    {
        // Arrange.
        var source = CreateErrorResult();

        // Act.
        var target = ToProtobufMapper.Map(source);

        // Assert.
        target.ReplyCase.Should().Be(ProtobufOneofReply.ReplyOneofCase.Error);
        target.Error.Code.Should().Be(42);
        target.Error.Description.Should().Be("error description");
    }

    /// <summary>
    /// Tests <see cref="ProtobufOneofToProtobufMapper.Map"/> maps a source without oneof
    /// into the <c>Fallback</c> arm.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapProtobufWithoutOneofToProtobufFallsBackToTheFallbackArm()
    {
        // Arrange.
        var source = new ProtobufOneofResult { Description = "nothing" };

        // Act.
        var target = ToProtobufMapper.Map(source);

        // Assert.
        target.ReplyCase.Should().Be(ProtobufOneofReply.ReplyOneofCase.Fallback);
        target.Fallback.Description.Should().Be("nothing");
    }

    /// <summary>
    /// Tests <see cref="ProtobufOneofToProtobufIgnoringUnsetMapper.Map"/> with the <c>Ok</c> arm.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapProtobufOkToProtobufIgnoringUnset()
    {
        // Arrange.
        var source = CreateOkResult();

        // Act.
        var target = ToProtobufIgnoringUnsetMapper.Map(source);

        // Assert.
        target.ReplyCase.Should().Be(ProtobufOneofReply.ReplyOneofCase.Ok);
        target.Ok.Value.Should().Be("value");
        target.Ok.Description.Should().Be("ok description");
    }

    /// <summary>
    /// Tests <see cref="ProtobufOneofToProtobufIgnoringUnsetMapper.Map"/> leaves the target oneof
    /// unset when the source oneof is not set.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapProtobufWithoutOneofToProtobufIgnoringUnsetLeavesTheOneofUnset()
    {
        // Arrange.
        var source = new ProtobufOneofResult { Description = "nothing" };

        // Act.
        var target = ToProtobufIgnoringUnsetMapper.Map(source);

        // Assert.
        target.ReplyCase.Should().Be(ProtobufOneofReply.ReplyOneofCase.None);
    }

    private static ProtobufOneofResult CreateOkResult()
        => new()
        {
            Description = "result",
            Ok = new ProtobufOneofOkPayload { Value = "value", Description = "ok description" },
        };

    private static ProtobufOneofResult CreateErrorResult()
        => new()
        {
            Description = "result",
            Error = new ProtobufOneofErrorPayload { Code = 42, Description = "error description" },
        };
}