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
        // -- ArrayOrListToArrayMapper --
        ArrayOrListToArrayMapper arrayOrListToArrayMapper = new();
        Title(nameof(ArrayOrListToArrayMapper), true);
        Printout(arrayOrListToArrayMapper.Map(new[] { CountingValues.One, CountingValues.Two }));
        Printout(arrayOrListToArrayMapper.Map([CountingValues.One, null, CountingValues.Two]));
        Printout(arrayOrListToArrayMapper.Map(new List<CountingValues> { CountingValues.One, CountingValues.Two }));
        Printout(arrayOrListToArrayMapper.Map((IList<CountingValues>)new List<CountingValues> { CountingValues.One, CountingValues.Two }));

        // -- ArrayOrListToCollectionMapper --
        ArrayOrListToCollectionMapper arrayOrListToCollectionMapper = new();
        Title(nameof(ArrayOrListToCollectionMapper));
        Printout(arrayOrListToCollectionMapper.MapArrayToIList([CountingValues.One, CountingValues.Two]));
        Printout(arrayOrListToCollectionMapper.MapArrayToList([CountingValues.One, CountingValues.Two]));
        Printout(arrayOrListToCollectionMapper.MapArrayToICollection([CountingValues.One, CountingValues.Two]));
        Printout(arrayOrListToCollectionMapper.MapArrayToIReadOnlyCollection([CountingValues.One, CountingValues.Two]));
        Printout(arrayOrListToCollectionMapper.MapArrayToIEnumerable([CountingValues.One, CountingValues.Two]));
        Printout(arrayOrListToCollectionMapper.MapIListToIList(new List<CountingValues> { CountingValues.One, CountingValues.Two }));
        Printout(arrayOrListToCollectionMapper.MapIListToList(new List<CountingValues> { CountingValues.One, CountingValues.Two }));
        Printout(arrayOrListToCollectionMapper.MapIListToICollection(new List<CountingValues> { CountingValues.One, CountingValues.Two }));
        Printout(arrayOrListToCollectionMapper.MapIListToIReadOnlyCollection(new List<CountingValues> { CountingValues.One, CountingValues.Two }));
        Printout(arrayOrListToCollectionMapper.MapIListToIEnumerable(new List<CountingValues> { CountingValues.One, CountingValues.Two }));
        Printout(arrayOrListToCollectionMapper.MapListToIList([CountingValues.One, CountingValues.Two]));
        Printout(arrayOrListToCollectionMapper.MapListToList([CountingValues.One, CountingValues.Two]));
        Printout(arrayOrListToCollectionMapper.MapListToICollection([CountingValues.One, CountingValues.Two]));
        Printout(arrayOrListToCollectionMapper.MapListToIReadOnlyCollection([CountingValues.One, CountingValues.Two]));
        Printout(arrayOrListToCollectionMapper.MapListToIEnumerable([CountingValues.One, CountingValues.Two]));

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

        // -- EnumerableOrCollectionToArrayMapper --
        EnumerableOrCollectionToArrayMapper enumerableOrCollectionToArrayMapper = new();
        Title(nameof(EnumerableOrCollectionToArrayMapper));
        Printout(enumerableOrCollectionToArrayMapper.Map((IEnumerable<CountingValues>)new[] { CountingValues.One, CountingValues.Two }));
        Printout(enumerableOrCollectionToArrayMapper.Map((ICollection<CountingValues>)new[] { CountingValues.One, CountingValues.Two }));
        Printout(enumerableOrCollectionToArrayMapper.Map((IReadOnlyCollection<CountingValues>)new[] { CountingValues.One, CountingValues.Two }));

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

    private static void Printout<TItem>(IEnumerable<TItem> enumerable)
        => Console.WriteLine(JoinToString(enumerable));

    private static void Printout<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        => Console.WriteLine(JoinToString(dictionary));

    private static string JoinToString<TItem>(IEnumerable<TItem> enumerable)
        => $"[ {string.Join(", ", enumerable.Select(item => item is null ? (object)"<null>" : item))} ]";

    private static string JoinToString<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        => JoinToString(dictionary.Select(kvp => $"{{ {kvp.Key}: {kvp.Value} }}"));
}