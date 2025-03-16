// <copyright file="DictionaryToDictionaryMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

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
    /// <returns>The mapper dictionary.</returns>
    public partial Dictionary<string, string> MapDictionaryToDictionary(Dictionary<int, CountingValues> input);

    /// <summary>
    /// Map a <see cref="IDictionary{TKey,TValue}"/> to <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapper dictionary.</returns>
    public partial Dictionary<string, string> MapIDictionaryToDictionary(IDictionary<int, CountingValues> input);

    /// <summary>
    /// Map a <see cref="Dictionary{TKey,TValue}"/> to <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapper dictionary.</returns>
    public partial IDictionary<string, string> MapDictionaryToIDictionary(Dictionary<int, CountingValues> input);

    /// <summary>
    /// Map a <see cref="Dictionary{TKey,TValue}"/> to <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapper dictionary.</returns>
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
}