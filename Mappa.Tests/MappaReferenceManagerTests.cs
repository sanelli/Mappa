// <copyright file="MappaReferenceManagerTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Reflection;

using AwesomeAssertions;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaReferenceManager"/> and <see cref="MappaException"/>.
/// </summary>
public sealed class MappaReferenceManagerTests
{
    /// <summary>
    /// Tests that <see cref="MappaReferenceManager.TryGetReference{TTarget}"/> returns a stored target.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetReferenceReturnsStoredTarget()
    {
        // Arrange
        var manager = new MappaReferenceManager();
        var source = new object();
        var target = new MappedTarget { Name = "mapped" };

        // Act
        manager.AddReferencePair(target, source);
        var found = manager.TryGetReference<MappedTarget>(source, out var retrieved);

        // Assert
        found.Should().BeTrue();
        retrieved.Should().BeSameAs(target);
    }

    /// <summary>
    /// Tests that <see cref="MappaReferenceManager.TryGetReference{TTarget}"/> returns <c>false</c>
    /// when the source has not been registered.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetReferenceReturnsFalseForUnknownSource()
    {
        // Arrange
        var manager = new MappaReferenceManager();

        // Act
        var found = manager.TryGetReference<object>(new object(), out var target);

        // Assert
        found.Should().BeFalse();
        target.Should().BeNull();
    }

    /// <summary>
    /// Tests that <see cref="MappaReferenceManager.TryGetReference{TTarget}"/> returns <c>false</c>
    /// when the source is <see langword="null"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetReferenceReturnsFalseForNullSource()
    {
        // Arrange
        var manager = new MappaReferenceManager();

        // Act
#pragma warning disable CS8600 // Intentional null source
#pragma warning disable CS8625
        var found = manager.TryGetReference<object>(null, out _);
#pragma warning restore CS8625
#pragma warning restore CS8600

        // Assert
        found.Should().BeFalse();
    }

    /// <summary>
    /// Tests that <see cref="MappaReferenceManager.TryGetReference{TTarget}"/> returns <c>false</c>
    /// when the stored target is not of the requested type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetReferenceReturnsFalseWhenTargetTypeDoesNotMatch()
    {
        // Arrange
        var manager = new MappaReferenceManager();
        var source = new object();
        manager.AddReferencePair(new MappedTarget { Name = "mapped" }, source);

        // Act
        var found = manager.TryGetReference<string>(source, out _);

        // Assert
        found.Should().BeFalse();
    }

    /// <summary>
    /// Tests that <see cref="MappaReferenceManager.AddReferencePair"/> throws when source is <see langword="null"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AddReferencePairThrowsWhenSourceIsNull()
    {
        // Arrange
        var manager = new MappaReferenceManager();

        // Act
#pragma warning disable CS8625 // Intentional null argument
        var act = () => manager.AddReferencePair(new object(), null);
#pragma warning restore CS8625

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("source");
    }

    /// <summary>
    /// Tests that reference equality is used so equal but distinct instances are not shared.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AddReferencePairUsesReferenceEquality()
    {
        // Arrange
        var manager = new MappaReferenceManager();
        var sourceA = new EqualByValue("same");
        var sourceB = new EqualByValue("same");
        var target = new MappedTarget { Name = "mapped" };
        manager.AddReferencePair(target, sourceA);

        // Act
        var foundForA = manager.TryGetReference<MappedTarget>(sourceA, out var retrievedA);
        var foundForB = manager.TryGetReference<MappedTarget>(sourceB, out _);

        // Assert
        foundForA.Should().BeTrue();
        retrievedA.Should().BeSameAs(target);
        foundForB.Should().BeFalse();
    }

    /// <summary>
    /// Tests that <see cref="MappaReferenceManager.MaxDepth"/> of <c>0</c> allows unlimited nesting.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IncreaseDepthAllowsUnlimitedNestingWhenMaxDepthIsZero()
    {
        // Arrange
        var manager = new MappaReferenceManager
        {
            MaxDepth = 0,
        };

        // Act
        var act = () =>
        {
            using var depth1 = manager.IncreaseDepth();
            using var depth2 = manager.IncreaseDepth();
            using var depth3 = manager.IncreaseDepth();
            using var depth4 = manager.IncreaseDepth();
            _ = depth1;
            _ = depth2;
            _ = depth3;
            _ = depth4;
        };

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that nested <see cref="MappaReferenceManager.IncreaseDepth"/> throws
    /// <see cref="MappaException"/> when depth exceeds <see cref="MappaReferenceManager.MaxDepth"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IncreaseDepthThrowsWhenExceedingMaxDepth()
    {
        // Arrange
        var manager = new MappaReferenceManager
        {
            MaxDepth = 1,
        };

        // Act
        using (manager.IncreaseDepth())
        {
            var act = () => manager.IncreaseDepth();

            // Assert
            act.Should().Throw<MappaException>()
                .Which.Message.Should().Contain("1");
        }
    }

    /// <summary>
    /// Tests that disposing an increased depth restores capacity for further nesting.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IncreaseDepthDisposeRestoresDepth()
    {
        // Arrange
        var manager = new MappaReferenceManager
        {
            MaxDepth = 1,
        };

        // Act
        using (var first = manager.IncreaseDepth())
        {
            _ = first;
        }

        var act = () =>
        {
            using var second = manager.IncreaseDepth();
            _ = second;
        };

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that disposing an <see cref="MappaReferenceManager.IncreaseDepth"/> scope twice is safe.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IncreaseDepthDisposeIsIdempotent()
    {
        // Arrange
        var manager = new MappaReferenceManager
        {
            MaxDepth = 1,
        };
        var scope = manager.IncreaseDepth();

        // Act
        scope.Dispose();
        scope.Dispose();
        var act = () =>
        {
            using var restored = manager.IncreaseDepth();
            _ = restored;
        };

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that the private reference-equality comparer rejects <see langword="null"/> hash codes.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ReferenceEqualityComparerGetHashCodeThrowsForNull()
    {
        // Arrange
        var comparerType = typeof(MappaReferenceManager).GetNestedType(
            "ReferenceEqualityComparer",
            BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("ReferenceEqualityComparer was not found.");
        var instance = comparerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null)
            ?? throw new InvalidOperationException("ReferenceEqualityComparer.Instance was not found.");
        var interfaceMap = comparerType.GetInterfaceMap(typeof(IEqualityComparer<object>));
        var getHashCodeInterface = typeof(IEqualityComparer<object>).GetMethod(nameof(IEqualityComparer<object>.GetHashCode))
            ?? throw new InvalidOperationException("IEqualityComparer<object>.GetHashCode was not found.");
        var getHashCodeIndex = Array.IndexOf(interfaceMap.InterfaceMethods, getHashCodeInterface);
        getHashCodeIndex.Should().BeGreaterThanOrEqualTo(0);
        var getHashCode = interfaceMap.TargetMethods[getHashCodeIndex];

        // Act
#pragma warning disable CS8625 // Intentional null argument
        var act = () => getHashCode.Invoke(instance, [null]);
#pragma warning restore CS8625

        // Assert
        act.Should().Throw<TargetInvocationException>()
            .WithInnerException<ArgumentNullException>()
            .Which.ParamName.Should().Be("obj");
    }

    /// <summary>
    /// Tests <see cref="MappaException"/> constructors.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MappaExceptionConstructorsPreserveMessageAndInnerException()
    {
        // Arrange
        var inner = new InvalidOperationException("inner");

        // Act
        var empty = new MappaException();
        var withMessage = new MappaException("depth exceeded");
        var withInner = new MappaException("depth exceeded", inner);

        // Assert
        empty.Message.Should().NotBeNullOrEmpty();
        withMessage.Message.Should().Be("depth exceeded");
        withInner.Message.Should().Be("depth exceeded");
        withInner.InnerException.Should().BeSameAs(inner);
    }

    private sealed class MappedTarget
    {
        public string Name { get; set; } = string.Empty;
    }

    private sealed class EqualByValue(string value)
    {
        private readonly string value = value;

        public override bool Equals(object? obj)
            => obj is EqualByValue other && this.value == other.value;

        public override int GetHashCode() => this.value.GetHashCode(StringComparison.Ordinal);
    }
}