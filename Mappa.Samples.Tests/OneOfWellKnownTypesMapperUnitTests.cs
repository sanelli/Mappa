// <copyright file="OneOfWellKnownTypesMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using OneOf;
using OneOf.Types;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="OneOfWellKnownTypesMapper"/>.
/// </summary>
public sealed class OneOfWellKnownTypesMapperUnitTests
{
    private readonly OneOfWellKnownTypesMapper mapper = new();

    /// <summary>
    /// Unwraps a Some Option to <see cref="string"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanUnwrapOptionSomeToString()
    {
        var source = OneOf<string, None>.FromT0("hello");

        var result = this.mapper.MapOptionToString(source);

        result.Should().Be("hello");
    }

    /// <summary>
    /// Throws <see cref="MappaException"/> when unwrapping None to non-nullable <see cref="string"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UnwrapOptionNoneToStringThrowsMappaException()
    {
        var source = OneOf<string, None>.FromT1(default(None));

        var act = () => this.mapper.MapOptionToString(source);

        act.Should().Throw<MappaException>();
    }

    /// <summary>
    /// Unwraps None to nullable <see cref="string"/> as <see langword="null"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanUnwrapOptionNoneToNullableString()
    {
        var source = OneOf<string, None>.FromT1(default(None));

        var result = this.mapper.MapOptionToNullableString(source);

        result.Should().BeNull();
    }

    /// <summary>
    /// Wraps a non-null <see cref="string"/> into Option Some.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanWrapStringToOptionSome()
    {
        var result = this.mapper.MapStringToOption("hello");

        result.IsT0.Should().BeTrue();
        result.AsT0.Should().Be("hello");
    }

    /// <summary>
    /// Wraps <see langword="null"/> into Option None.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanWrapNullStringToOptionNone()
    {
        var result = this.mapper.MapStringToOption(null);

        result.IsT1.Should().BeTrue();
    }

    /// <summary>
    /// Unwraps the exact <see cref="string"/> arm from <c>OneOf&lt;string, int&gt;</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanUnwrapStringOrIntToString()
    {
        var source = OneOf<string, int>.FromT0("exact");

        var result = this.mapper.MapStringOrIntToString(source);

        result.Should().Be("exact");
    }

    /// <summary>
    /// Throws <see cref="MappaException"/> when the wrong OneOf arm is selected.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UnwrapWrongStringOrIntArmThrowsMappaException()
    {
        var source = OneOf<string, int>.FromT1(42);

        var act = () => this.mapper.MapStringOrIntToString(source);

        act.Should().Throw<MappaException>();
    }

    /// <summary>
    /// Wraps <see cref="string"/> into <c>OneOf&lt;string, int&gt;</c> via identity.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanWrapStringToStringOrIntViaIdentity()
    {
        var result = this.mapper.MapStringToStringOrInt("identity");

        result.IsT0.Should().BeTrue();
        result.AsT0.Should().Be("identity");
    }

    /// <summary>
    /// Unwraps a <c>OneOfBase</c>-derived source to the <see cref="string"/> arm.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanUnwrapStringOrIntBaseToString()
    {
        var source = new StringOrInt("base");

        var result = this.mapper.MapStringOrIntBaseToString(source);

        result.Should().Be("base");
    }

    /// <summary>
    /// Wraps <see cref="string"/> into a constructible <c>OneOfBase</c> subclass.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanWrapStringToStringOrIntBase()
    {
        var result = this.mapper.MapStringToStringOrIntBase("wrapped");

        result.IsT0.Should().BeTrue();
        result.AsT0.Should().Be("wrapped");
    }
}