// <copyright file="MappaBeforeAfterMapHooksAttributeMapperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for <see cref="MappaBeforeAfterMapHooksAttributeMapper"/>.
/// </summary>
public sealed class MappaBeforeAfterMapHooksAttributeMapperTests
{
    /// <summary>
    /// Test before hooks mutate the source, after hooks mutate the returned target,
    /// context is observed, and hooks run in the phase-specific class/method order.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapPersonWithBeforeAndAfterHooks()
    {
        // Arrange
        var mapper = new MappaBeforeAfterMapHooksAttributeMapper();
        var input = new BeforeAfterMapHookPersonModel
        {
            Name = "Ada",
            Score = 0,
        };
        MappaContext context = new Dictionary<string, object>
        {
            ["suffix"] = "ctx",
        };

        // Act
        var actual = mapper.MapPerson(input, context);

        // Assert
        actual.Score.Should().Be(11);
        actual.Name.Should().Be("Ada-ctx-method-class");
        mapper.HookCalls.Should().Equal(
            "class-before",
            "method-before",
            "method-after",
            "class-after");
    }

    /// <summary>
    /// Test class-level hooks resolve independently for a second mapping method type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapCounterWithClassLevelHooksResolvedForCounterType()
    {
        // Arrange
        var mapper = new MappaBeforeAfterMapHooksAttributeMapper();
        var input = new BeforeAfterMapHookCounterModel
        {
            Value = 7,
        };

        // Act
        var actual = mapper.MapCounter(input);

        // Assert
        actual.Value.Should().Be(108);
        mapper.HookCalls.Should().Equal(
            "class-before",
            "class-after");
    }
}