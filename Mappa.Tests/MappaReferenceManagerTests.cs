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
    /// when no pair was registered for the requested target type (composite-key miss).
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetReferenceReturnsFalseWhenTargetTypeDoesNotMatch()
    {
        // Arrange
        var manager = new MappaReferenceManager();
        var source = new object();
        var mapped = new MappedTarget { Name = "mapped" };
        manager.AddReferencePair(mapped, source);

        // Act
        var found = manager.TryGetReference<string>(source, out _);

        // Assert
        found.Should().BeFalse();
        manager.TryGetReference<MappedTarget>(source, out var stillStored).Should().BeTrue();
        stillStored.Should().BeSameAs(mapped);
    }

    /// <summary>
    /// Tests that the same source can store distinct targets for different declared target types.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AddReferencePairAllowsSameSourceForDifferentTargetTypes()
    {
        // Arrange
        var manager = new MappaReferenceManager();
        var source = new object();
        var mappedTarget = new MappedTarget { Name = "mapped" };
        const string stringTarget = "other";

        // Act
        manager.AddReferencePair(mappedTarget, source);
        manager.AddReferencePair(stringTarget, source);
        var foundMapped = manager.TryGetReference<MappedTarget>(source, out var retrievedMapped);
        var foundString = manager.TryGetReference<string>(source, out var retrievedString);

        // Assert
        foundMapped.Should().BeTrue();
        retrievedMapped.Should().BeSameAs(mappedTarget);
        foundString.Should().BeTrue();
        retrievedString.Should().BeSameAs(stringTarget);
    }

    /// <summary>
    /// Tests that registering again for the same source and declared target type overwrites the stored target.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AddReferencePairOverwritesSameSourceAndTargetType()
    {
        // Arrange
        var manager = new MappaReferenceManager();
        var source = new object();
        var first = new MappedTarget { Name = "first" };
        var second = new MappedTarget { Name = "second" };
        manager.AddReferencePair(first, source);

        // Act
        manager.AddReferencePair(second, source);
        var found = manager.TryGetReference<MappedTarget>(source, out var retrieved);

        // Assert
        found.Should().BeTrue();
        retrieved.Should().BeSameAs(second);
    }

    /// <summary>
    /// Tests that <see cref="MappaReferenceManager.AddReferencePair{TTarget,TSource}"/> throws when source is <see langword="null"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AddReferencePairThrowsWhenSourceIsNull()
    {
        // Arrange
        var manager = new MappaReferenceManager();

        // Act
#pragma warning disable CS8625 // Intentional null argument
        var act = () => manager.AddReferencePair<object, object>(new object(), null);
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
    /// Tests that the private composite reference key uses reference equality for sources
    /// and distinguishes different declared target types.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ReferenceKeyEqualsAndGetHashCodeUseSourceIdentityAndTargetType()
    {
        // Arrange
        var keyType = typeof(MappaReferenceManager).GetNestedType(
            "ReferenceKey",
            BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("ReferenceKey was not found.");
        var sourceA = new object();
        var sourceB = new object();
        var keyAMapped = Activator.CreateInstance(keyType, sourceA, typeof(MappedTarget))
            ?? throw new InvalidOperationException("Failed to create ReferenceKey.");
        var keyAMappedAgain = Activator.CreateInstance(keyType, sourceA, typeof(MappedTarget))
            ?? throw new InvalidOperationException("Failed to create ReferenceKey.");
        var keyAString = Activator.CreateInstance(keyType, sourceA, typeof(string))
            ?? throw new InvalidOperationException("Failed to create ReferenceKey.");
        var keyBMapped = Activator.CreateInstance(keyType, sourceB, typeof(MappedTarget))
            ?? throw new InvalidOperationException("Failed to create ReferenceKey.");

        // Act
        var equalsSame = keyAMapped.Equals(keyAMappedAgain);
        var equalsDifferentTargetType = keyAMapped.Equals(keyAString);
        var equalsDifferentSource = keyAMapped.Equals(keyBMapped);
        var equalsObjectWrongType = keyAMapped.Equals("not-a-key");
        var hashSame = keyAMapped.GetHashCode() == keyAMappedAgain.GetHashCode();

        // Assert
        equalsSame.Should().BeTrue();
        equalsDifferentTargetType.Should().BeFalse();
        equalsDifferentSource.Should().BeFalse();
        equalsObjectWrongType.Should().BeFalse();
        hashSame.Should().BeTrue();
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