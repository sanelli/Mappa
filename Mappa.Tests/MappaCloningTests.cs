// <copyright file="MappaCloningTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaCloning"/>.
/// </summary>
public sealed class MappaCloningTests
{
    /// <summary>
    /// Tests <see cref="MappaCloning.MemberwiseClone{T}"/> creates a shallow clone.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MemberwiseCloneCreatesShallowCopy()
    {
        // Arrange
        var nested = new NestedType { Value = 42 };
        var source = new CloneableType { Name = "alpha", Nested = nested };

        // Act
        var clone = MappaCloning.MemberwiseClone(source);

        // Assert
        ReferenceEquals(clone, source).Should().BeFalse();
        clone.Name.Should().Be("alpha");
        clone.Nested.Should().BeSameAs(nested);
        clone.Nested.Value.Should().Be(42);
    }

    /// <summary>
    /// Tests <see cref="MappaCloning.MemberwiseClone{T}"/> throws when source is <see langword="null"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MemberwiseCloneThrowsWhenSourceIsNull()
    {
        // Act
#pragma warning disable CS8625 // Intentional null argument to exercise ArgumentNullException path
        var act = () => MappaCloning.MemberwiseClone<CloneableType>(null);
#pragma warning restore CS8625

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("source");
    }

    private sealed class CloneableType
    {
        public string Name { get; set; } = string.Empty;

        public NestedType Nested { get; set; } = new();
    }

    private sealed class NestedType
    {
        public int Value { get; set; }
    }
}