// <copyright file="Program.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;

namespace Mappa.Samples.Aot;

/// <summary>
/// Program class.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Entrypoint.
    /// </summary>
    public static void Main()
    {
        // -- DictionaryToDictionaryMapper --
        DictionaryToDictionaryMapper dictionaryToDictionaryMapper = new();
        Title(nameof(DictionaryToDictionaryMapper));
        Printout(dictionaryToDictionaryMapper.MapDictionaryToDictionary(new()
        {
            { 10, CountingValues.One },
            { 20, CountingValues.Two },
            { 30, CountingValues.Three },
        }));
        Printout(dictionaryToDictionaryMapper.MapDictionaryToIDictionary(new()
        {
            { 10, CountingValues.One },
            { 20, CountingValues.Two },
            { 30, CountingValues.Three },
        }));
        Printout(dictionaryToDictionaryMapper.MapIDictionaryToDictionary(new Dictionary<int, CountingValues>
        {
            { 10, CountingValues.One },
            { 20, CountingValues.Two },
            { 30, CountingValues.Three },
        }));
        Printout(dictionaryToDictionaryMapper.MapIDictionaryToIDictionary(new Dictionary<int, CountingValues>
        {
            { 10, CountingValues.One },
            { 20, CountingValues.Two },
            { 30, CountingValues.Three },
        }));

        // TODO [#41] Add all remaining classes from Mappa.Samples (next is EnumerableOrCollectionToCollectionMapper).
    }

    private static void Title(string s, bool first = false)
    {
        var lines = new string('-', s.Length + 6);
        if (!first)
        {
            Console.WriteLine();
        }

        Console.WriteLine(lines);
        Console.WriteLine($"-- {s} --");
        Console.WriteLine(lines);
    }

    private static void Printout<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        => Console.WriteLine(JoinToString(dictionary));

    private static string JoinToString<TItem>(IEnumerable<TItem> enumerable)
        => $"[ {string.Join(", ", enumerable.Select(item => item is null ? (object)"<null>" : item))} ]";

    private static string JoinToString<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        => JoinToString(dictionary.Select(kvp => $"{{ {kvp.Key}: {kvp.Value} }}"));
}