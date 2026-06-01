// <copyright file="DictionaryToDictionaryMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="DictionaryToDictionaryMapper"/>.
/// </summary>
internal static class DictionaryToDictionaryMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="DictionaryToDictionaryMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(DictionaryToDictionaryMapper));
        var mapper = new DictionaryToDictionaryMapper();
        var dictionary = AotSampleData.IntCountingValuesDictionary;

        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapDictionaryToDictionary),
            "Dictionary<int, CountingValues>",
            "Dictionary<string, string>",
            dictionary,
            mapper.MapDictionaryToDictionary(dictionary));

        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapDictionaryToIDictionary),
            "Dictionary<int, CountingValues>",
            "IDictionary<string, string>",
            dictionary,
            mapper.MapDictionaryToIDictionary(dictionary));

        var asIDictionary = AotSampleData.IntCountingValuesAsIDictionary;
        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapIDictionaryToDictionary),
            "IDictionary<int, CountingValues>",
            "Dictionary<string, string>",
            asIDictionary,
            mapper.MapIDictionaryToDictionary(asIDictionary));

        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapIDictionaryToIDictionary),
            "IDictionary<int, CountingValues>",
            "IDictionary<string, string>",
            asIDictionary,
            mapper.MapIDictionaryToIDictionary(asIDictionary));

        var customGeneric = AotSampleData.CustomGenericIntCountingDictionary;
        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapCustomDictionaryWithGenerics),
            "CustomDictionaryWithGeneric<int, CountingValues>",
            "CustomDictionaryWithGeneric<string, string>",
            customGeneric,
            mapper.MapCustomDictionaryWithGenerics(customGeneric));

        var customInt = AotSampleData.CustomIntCountingDictionary;
        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapCustomDictionaryWithoutGenerics),
            "CustomDictionaryIntToCountingValues",
            "CustomDictionaryStringToString",
            customInt,
            mapper.MapCustomDictionaryWithoutGenerics(customInt));

        var keyValuePairs = AotSampleData.IntCountingValuesAsKeyValuePairs;
        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapIEnumerableOfKeyValuePairsToDictionary),
            "IEnumerable<KeyValuePair<int, CountingValues>>",
            "Dictionary<string, string>",
            keyValuePairs,
            mapper.MapIEnumerableOfKeyValuePairsToDictionary(keyValuePairs));

        var readOnly = AotSampleData.IntCountingValuesAsIReadOnlyDictionary;
        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapIReadOnlyDictionaryToDictionary),
            "IReadOnlyDictionary<int, CountingValues>",
            "Dictionary<string, string>",
            readOnly,
            mapper.MapIReadOnlyDictionaryToDictionary(readOnly));

        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapDictionaryToIEnumerableOfKeyValuePair),
            "Dictionary<int, CountingValues>",
            "IEnumerable<KeyValuePair<string, string>>",
            dictionary,
            mapper.MapDictionaryToIEnumerableOfKeyValuePair(dictionary));

        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapDictionaryToIReadOnlyDictionary),
            "Dictionary<int, CountingValues>",
            "IReadOnlyDictionary<string, string>",
            dictionary,
            mapper.MapDictionaryToIReadOnlyDictionary(dictionary));

        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapDictionaryToReadOnlyDictionary),
            "Dictionary<int, CountingValues>",
            "ReadOnlyDictionary<string, string>",
            dictionary,
            mapper.MapDictionaryToReadOnlyDictionary(dictionary));

        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapDictionaryToIImmutableDictionary),
            "Dictionary<int, CountingValues>",
            "IImmutableDictionary<string, string>",
            dictionary,
            mapper.MapDictionaryToIImmutableDictionary(dictionary));

        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapDictionaryToImmutableDictionary),
            "Dictionary<int, CountingValues>",
            "ImmutableDictionary<string, string>",
            dictionary,
            mapper.MapDictionaryToImmutableDictionary(dictionary));

        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapDictionaryToImmutableSortedDictionary),
            "Dictionary<int, CountingValues>",
            "ImmutableSortedDictionary<string, string>",
            dictionary,
            mapper.MapDictionaryToImmutableSortedDictionary(dictionary));

        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapDictionaryToFrozenDictionary),
            "Dictionary<int, CountingValues>",
            "FrozenDictionary<string, string>",
            dictionary,
            mapper.MapDictionaryToFrozenDictionary(dictionary));

        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapDictionaryToSortedDictionary),
            "Dictionary<int, CountingValues>",
            "SortedDictionary<string, string>",
            dictionary,
            mapper.MapDictionaryToSortedDictionary(dictionary));

        var sorted = AotSampleData.IntCountingValuesSortedDictionary;
        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapSortedDictionaryToDictionary),
            "SortedDictionary<int, CountingValues>",
            "Dictionary<string, string>",
            sorted,
            mapper.MapSortedDictionaryToDictionary(sorted));

        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapDictionaryToCustomDictionaryWithGenericAndExplicitImplementation),
            "Dictionary<int, CountingValues>",
            "CustomDictionaryWithGenericAndExplicitImplementation<string, string>",
            dictionary,
            mapper.MapDictionaryToCustomDictionaryWithGenericAndExplicitImplementation(dictionary));

        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapDictionaryToCustomDictionaryWithNonGenericAndExplicitImplementation),
            "Dictionary<int, CountingValues>",
            "CustomDictionaryWithNonGenericAndExplicitImplementation",
            dictionary,
            mapper.MapDictionaryToCustomDictionaryWithNonGenericAndExplicitImplementation(dictionary));

        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapDictionaryToConcurrentDictionary),
            "Dictionary<int, CountingValues>",
            "ConcurrentDictionary<string, string>",
            dictionary,
            mapper.MapDictionaryToConcurrentDictionary(dictionary));

        var concurrent = AotSampleData.IntCountingValuesConcurrentDictionary;
        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapConcurrentDictionaryToDictionary),
            "ConcurrentDictionary<int, CountingValues>",
            "Dictionary<string, string>",
            concurrent,
            mapper.MapConcurrentDictionaryToDictionary(concurrent));

        report.RecordInvocation(
            nameof(DictionaryToDictionaryMapper.MapDictionaryToCustomConcurrentDictionary),
            "Dictionary<int, CountingValues>",
            "CustomConcurrentDictionary<string, string>",
            dictionary,
            mapper.MapDictionaryToCustomConcurrentDictionary(dictionary));
    }
}