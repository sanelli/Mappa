// <copyright file="DictionaryAssignmentMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the dictionary assignment sample mappers.
/// </summary>
public sealed class DictionaryAssignmentMapperUnitTest
{
    private readonly DictionaryAssignmentIndexerMapper indexerMapper = new();
    private readonly DictionaryAssignmentAddMapper addMapper = new();

    /// <summary>
    /// Test indexer and <see cref="IDictionary{TKey,TValue}.Add"/> mappers produce equivalent results for unique keys.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IndexerAndAddMappersProduceEquivalentResultsForUniqueKeys()
    {
        // Arrange
        Dictionary<int, CountingValues> input = new()
        {
            { 1, CountingValues.One },
            { 2, CountingValues.Two },
            { 3, CountingValues.Three },
        };

        var expected = new Dictionary<string, string>
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
        };

        // Act
        var indexerResult = this.indexerMapper.Map(input);
        var addResult = this.addMapper.Map(input);

        // Assert
        indexerResult.Should().BeEquivalentTo(expected);
        addResult.Should().BeEquivalentTo(expected);
        addResult.Should().BeEquivalentTo(indexerResult);
    }
}