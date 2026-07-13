// <copyright file="DictionaryAssignmentMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for the dictionary assignment sample mappers.
/// </summary>
internal static class DictionaryAssignmentMapperRunner
{
    /// <summary>
    /// Runs all dictionary assignment sample mappers.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var input = AotSampleData.IntCountingValuesDictionary;

        report.BeginMapper(nameof(DictionaryAssignmentIndexerMapper));
        var indexerMapper = new DictionaryAssignmentIndexerMapper();
        var indexerResult = indexerMapper.Map(input);
        report.RecordInvocation(
            nameof(DictionaryAssignmentIndexerMapper.Map),
            "Dictionary<int, CountingValues>",
            "Dictionary<string, string>",
            input,
            indexerResult);

        report.BeginMapper(nameof(DictionaryAssignmentAddMapper));
        var addMapper = new DictionaryAssignmentAddMapper();
        var addResult = addMapper.Map(input);
        VerifyEquivalent(indexerResult, addResult);
        report.RecordInvocation(
            nameof(DictionaryAssignmentAddMapper.Map),
            "Dictionary<int, CountingValues>",
            "Dictionary<string, string>",
            input,
            addResult);
    }

    private static void VerifyEquivalent(
        Dictionary<string, string> indexerResult,
        Dictionary<string, string> addResult)
    {
        if (indexerResult.Count != addResult.Count)
        {
            throw new InvalidOperationException(
                $"Expected equivalent dictionary counts but got indexer={indexerResult.Count} and add={addResult.Count}.");
        }

        foreach (var pair in indexerResult)
        {
            if (!addResult.TryGetValue(pair.Key, out var addValue) || addValue != pair.Value)
            {
                throw new InvalidOperationException(
                    $"Expected equivalent dictionary entries but indexer and add results differ for key '{pair.Key}'.");
            }
        }
    }
}