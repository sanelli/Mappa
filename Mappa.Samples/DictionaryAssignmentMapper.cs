// <copyright file="DictionaryAssignmentMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type

using Mappa;
using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating default <see cref="DictionaryAssignmentSetting.Indexer"/> for dictionary-to-dictionary mapping.
/// </summary>
[Mappa]
public sealed partial class DictionaryAssignmentIndexerMapper
{
    /// <summary>
    /// Map a <see cref="Dictionary{TKey,TValue}"/> to <see cref="Dictionary{TKey,TValue}"/> using the default indexer assignment.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapped dictionary.</returns>
    public partial Dictionary<string, string> Map(Dictionary<int, CountingValues> input);
}

/// <summary>
/// Mapper demonstrating <see cref="DictionaryAssignmentSetting.Add"/> for dictionary-to-dictionary mapping.
/// </summary>
[Mappa]
[MappaSettings(DictionaryAssignment = DictionaryAssignmentSetting.Add)]
public sealed partial class DictionaryAssignmentAddMapper
{
    /// <summary>
    /// Map a <see cref="Dictionary{TKey,TValue}"/> to <see cref="Dictionary{TKey,TValue}"/> using <see cref="IDictionary{TKey,TValue}.Add"/>.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapped dictionary.</returns>
    public partial Dictionary<string, string> Map(Dictionary<int, CountingValues> input);
}