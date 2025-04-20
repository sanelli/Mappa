// <copyright file="DictionaryToDictionaryMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Concurrent;

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="DictionaryToDictionaryMapper"/>.
/// </summary>
public sealed class DictionaryToDictionaryMapperUnitTests
{
    private readonly DictionaryToDictionaryMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapDictionaryToDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDictionaryToDictionary()
    {
        // Arrange
        Dictionary<int, CountingValues> input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapDictionaryToDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapDictionaryToIDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDictionaryToIDictionary()
    {
        // Arrange
        Dictionary<int, CountingValues> input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapDictionaryToIDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapIDictionaryToDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIDictionaryToDictionary()
    {
        // Arrange
        IDictionary<int, CountingValues> input = new Dictionary<int, CountingValues>
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapIDictionaryToDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapIDictionaryToIDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIDictionaryToIDictionary()
    {
        // Arrange
        IDictionary<int, CountingValues> input = new Dictionary<int, CountingValues>
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapIDictionaryToIDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapCustomDictionaryWithGenerics"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapCustomDictionaryWithGenerics()
    {
        // Arrange
        CustomDictionaryWithGeneric<int, CountingValues> input = new CustomDictionaryWithGeneric<int, CountingValues>
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapCustomDictionaryWithGenerics(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapCustomDictionaryWithoutGenerics"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapCustomDictionaryWithoutGenerics()
    {
        // Arrange
        CustomDictionaryIntToCountingValues input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapCustomDictionaryWithoutGenerics(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapIEnumerableOfKeyValuePairsToDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIEnumerableOfKeyValuePairsToDictionary()
    {
        // Arrange
        IEnumerable<KeyValuePair<int, CountingValues>> input = new Dictionary<int, CountingValues>
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapIEnumerableOfKeyValuePairsToDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapIReadOnlyDictionaryToDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIReadOnlyDictionaryToDictionary()
    {
        // Arrange
        IReadOnlyDictionary<int, CountingValues> input = new Dictionary<int, CountingValues>
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapIReadOnlyDictionaryToDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapDictionaryToIEnumerableOfKeyValuePair"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDictionaryToIEnumerableOfKeyValuePair()
    {
        // Arrange
        Dictionary<int, CountingValues> input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapDictionaryToIEnumerableOfKeyValuePair(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapDictionaryToIReadOnlyDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDictionaryToIReadOnlyDictionary()
    {
        // Arrange
        Dictionary<int, CountingValues> input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapDictionaryToIReadOnlyDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapDictionaryToReadOnlyDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDictionaryToReadOnlyDictionary()
    {
        // Arrange
        Dictionary<int, CountingValues> input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapDictionaryToReadOnlyDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapDictionaryToIImmutableDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDictionaryToIImmutableDictionary()
    {
        // Arrange
        Dictionary<int, CountingValues> input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapDictionaryToIImmutableDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapDictionaryToImmutableDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDictionaryToImmutableDictionary()
    {
        // Arrange
        Dictionary<int, CountingValues> input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapDictionaryToImmutableDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapDictionaryToImmutableSortedDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDictionaryToImmutableSortedDictionary()
    {
        // Arrange
        Dictionary<int, CountingValues> input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapDictionaryToImmutableSortedDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapDictionaryToFrozenDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDictionaryToFrozenDictionary()
    {
        // Arrange
        Dictionary<int, CountingValues> input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapDictionaryToFrozenDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapDictionaryToSortedDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDictionaryToSortedDictionary()
    {
        // Arrange
        Dictionary<int, CountingValues> input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapDictionaryToSortedDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new SortedDictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapSortedDictionaryToDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapSortedDictionaryToDictionary()
    {
        // Arrange
        SortedDictionary<int, CountingValues> input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapSortedDictionaryToDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapDictionaryToCustomDictionaryWithGenericAndExplicitImplementation"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDictionaryToCustomDictionaryWithGenericAndExplicitImplementation()
    {
        // Arrange
        Dictionary<int, CountingValues> input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapDictionaryToCustomDictionaryWithGenericAndExplicitImplementation(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapDictionaryToCustomDictionaryWithNonGenericAndExplicitImplementation"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDictionaryToCustomDictionaryWithNonGenericAndExplicitImplementation()
    {
        // Arrange
        Dictionary<int, CountingValues> input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapDictionaryToCustomDictionaryWithNonGenericAndExplicitImplementation(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapDictionaryToConcurrentDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDictionaryToConcurrentDictionary()
    {
        // Arrange
        Dictionary<int, CountingValues> input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapDictionaryToConcurrentDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapConcurrentDictionaryToDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapConcurrentDictionaryToDictionary()
    {
        // Arrange
        ConcurrentDictionary<int, CountingValues> input = new()
        {
            [1] = CountingValues.One,
            [2] = CountingValues.Two,
            [3] = CountingValues.Three,
        };

        // Act
        var actual = this.mapper.MapConcurrentDictionaryToDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }

    /// <summary>
    /// Unit test for <see cref="DictionaryToDictionaryMapper.MapDictionaryToCustomConcurrentDictionary"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDictionaryToCustomConcurrentDictionary()
    {
        // Arrange
        Dictionary<int, CountingValues> input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        // Act
        var actual = this.mapper.MapDictionaryToCustomConcurrentDictionary(input);

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        });
    }
}