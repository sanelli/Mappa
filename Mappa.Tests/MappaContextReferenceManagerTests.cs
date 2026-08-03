// <copyright file="MappaContextReferenceManagerTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

using AwesomeAssertions;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for the private <c>ReferenceManager</c> property on <see cref="MappaContext"/>.
/// </summary>
#pragma warning disable SA1402 // File-local accessor must share this file to remain visible
public sealed class MappaContextReferenceManagerTests
{
    /// <summary>
    /// Tests that each <see cref="MappaContext"/> constructor initializes a usable reference manager
    /// accessible via <see cref="UnsafeAccessorAttribute"/>.
    /// </summary>
    /// <param name="constructorKind">Which constructor to exercise.</param>
    [Theory]
    [UnitTest]
    [InlineData("default")]
    [InlineData("dictionary")]
    [InlineData("pairs")]
    public void ReferenceManagerIsAccessibleViaUnsafeAccessor(string constructorKind)
    {
        // Arrange
        var context = CreateContext(constructorKind);

        // Act
        var manager = MappaContextReferenceManagerAccessor.GetReferenceManager(context);

        // Assert
        manager.Should().NotBeNull();
        manager.MaxDepth.Should().Be(0);
    }

    /// <summary>
    /// Tests that the reference manager obtained via <see cref="UnsafeAccessorAttribute"/>
    /// can be manipulated (depth and reference reuse) for the owning context.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ReferenceManagerCanBeManipulatedViaUnsafeAccessor()
    {
        // Arrange
        var context = new MappaContext();
        var manager = MappaContextReferenceManagerAccessor.GetReferenceManager(context);
        var source = new object();
        var target = new object();

        // Act
        manager.MaxDepth = 2;
        manager.AddReferencePair(target, source);
        var found = manager.TryGetReference<object>(source, out var retrieved);
        using (manager.IncreaseDepth())
        using (manager.IncreaseDepth())
        {
            var act = () => manager.IncreaseDepth();
            act.Should().Throw<MappaException>();
        }

        // Assert
        found.Should().BeTrue();
        retrieved.Should().BeSameAs(target);
        manager.MaxDepth.Should().Be(2);
        MappaContextReferenceManagerAccessor.GetReferenceManager(context).Should().BeSameAs(manager);
    }

    private static MappaContext CreateContext(string constructorKind)
        => constructorKind switch
        {
            "default" => new MappaContext(),
            "dictionary" => new MappaContext(new Dictionary<string, object> { ["k"] = "v" }),
            "pairs" => new MappaContext([new KeyValuePair<string, object>("k", "v")]),
            _ => throw new ArgumentOutOfRangeException(nameof(constructorKind), constructorKind, null),
        };
}

/// <summary>
/// File-local accessor for the private <c>ReferenceManager</c> property on <see cref="MappaContext"/>.
/// </summary>
file static class MappaContextReferenceManagerAccessor
{
    /// <summary>
    /// Gets the private <c>ReferenceManager</c> property from <paramref name="context"/>.
    /// </summary>
    /// <param name="context">The context instance.</param>
    /// <returns>The reference manager instance.</returns>
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_ReferenceManager")]
    public static extern MappaReferenceManager GetReferenceManager(MappaContext context);
}
#pragma warning restore SA1402