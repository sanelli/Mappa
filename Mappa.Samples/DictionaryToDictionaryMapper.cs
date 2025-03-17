// <copyright file="DictionaryToDictionaryMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using the dictionary-to-dictionary strategy.
/// </summary>
[Mappa]
public sealed partial class DictionaryToDictionaryMapper
{
    /// <summary>
    /// Map a <see cref="Dictionary{TKey,TValue}"/> to <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapped dictionary.</returns>
    public partial Dictionary<string, string> MapDictionaryToDictionary(Dictionary<int, CountingValues> input);

    /// <summary>
    /// Map a <see cref="IDictionary{TKey,TValue}"/> to <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapped dictionary.</returns>
    public partial Dictionary<string, string> MapIDictionaryToDictionary(IDictionary<int, CountingValues> input);

    /// <summary>
    /// Map a <see cref="Dictionary{TKey,TValue}"/> to <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapped dictionary.</returns>
    public partial IDictionary<string, string> MapDictionaryToIDictionary(Dictionary<int, CountingValues> input);

    /// <summary>
    /// Map a <see cref="Dictionary{TKey,TValue}"/> to <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapped dictionary.</returns>
    public partial IDictionary<string, string> MapIDictionaryToIDictionary(IDictionary<int, CountingValues> input);

    /// <summary>
    /// Map from <see cref="CustomDictionaryWithGeneric{TKey, TValue}"/> to <see cref="CustomDictionaryWithGeneric{TKey, TValue}"/>.
    /// </summary>
    /// <param name="input">The input model.</param>
    /// <returns>The target model.</returns>
    public partial CustomDictionaryWithGeneric<string, string> MapCustomDictionaryWithGenerics(CustomDictionaryWithGeneric<int, CountingValues> input);

    /// <summary>
    /// Map from <see cref="CustomDictionaryIntToCountingValues"/> to <see cref="CustomDictionaryStringToString"/>.
    /// </summary>
    /// <param name="input">The input model.</param>
    /// <returns>The target model.</returns>
    public partial CustomDictionaryStringToString MapCustomDictionaryWithoutGenerics(CustomDictionaryIntToCountingValues input);

    /// <summary>
    /// Map a <see cref="IEnumerable{T}"/> or <see cref="KeyValuePair{TKey,TValue}"/> to <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapped dictionary.</returns>
    public partial Dictionary<string, string> MapIEnumerableOfKeyValuePairsToDictionary(IEnumerable<KeyValuePair<int, CountingValues>> input);

    /// <summary>
    /// Map a <see cref="IReadOnlyDictionary{TKey,TValue}"/> to <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapped dictionary.</returns>
    public partial Dictionary<string, string> MapIReadOnlyDictionaryToDictionary(IReadOnlyDictionary<int, CountingValues> input);

    /// <summary>
    /// Map a <see cref="Dictionary{TKey,TValue}"/> to <see cref="IEnumerable{T}"/> or <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapped dictionary.</returns>
    public partial IEnumerable<KeyValuePair<string, string>> MapDictionaryToIEnumerableOfKeyValuePair(Dictionary<int, CountingValues> input);

    /// <summary>
    /// Map a <see cref="Dictionary{TKey,TValue}"/> to <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapped dictionary.</returns>
    public partial IReadOnlyDictionary<string, string> MapDictionaryToIReadOnlyDictionary(Dictionary<int, CountingValues> input);

    /// <summary>
    /// Map a <see cref="Dictionary{TKey,TValue}"/> to <see cref="ReadOnlyDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapped dictionary.</returns>
    public partial ReadOnlyDictionary<string, string> MapDictionaryToReadOnlyDictionary(Dictionary<int, CountingValues> input);

    /// <summary>
    /// Map a <see cref="Dictionary{TKey,TValue}"/> to <see cref="ImmutableDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapped dictionary.</returns>
    public partial ImmutableDictionary<string, string> MapDictionaryToImmutableDictionary(Dictionary<int, CountingValues> input);

    /// <summary>
    /// Map a <see cref="Dictionary{TKey,TValue}"/> to <see cref="FrozenDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapped dictionary.</returns>
    public partial FrozenDictionary<string, string> MapDictionaryToFrozenDictionary(Dictionary<int, CountingValues> input);
}