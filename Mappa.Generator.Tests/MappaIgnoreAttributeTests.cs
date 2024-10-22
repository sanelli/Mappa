// <copyright file="MappaIgnoreAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests for the <see cref="MappaIgnoreAttribute"/>.
/// </summary>
public sealed class MappaIgnoreAttributeTests
{
    /// <summary>
    /// Tests that the <see cref="MappaIgnoreAttribute"/> allows
    /// to ignore an existing potential mapping method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [UnitTest]
    public Task MappaIgnoreAttributeAllowsToIgnorePotentialMapMethod()
    {
        // TODO [#37] Implement me.
        false.Should().BeTrue();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Tests that the <see cref="MappaIgnoreAttribute"/> allows
    /// to ignore an partial potential mapping method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [UnitTest]
    public Task MappaIgnoreAttributeAllowsToIgnorePartialMapMethods()
    {
        // TODO [#37] Implement me.
        true.Should().BeFalse();
        return Task.CompletedTask;
    }
}