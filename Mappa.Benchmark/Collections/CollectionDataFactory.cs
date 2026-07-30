// <copyright file="CollectionDataFactory.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Benchmark.Collections.Models;
using Mappa.Benchmark.Common;

namespace Mappa.Benchmark.Collections;

/// <summary>
/// Builds deterministic collection inputs with a few hundred entries.
/// </summary>
internal static class CollectionDataFactory
{
    /// <summary>
    /// Creates a list of <see cref="CollectionItemDto"/> with nested dictionaries.
    /// </summary>
    /// <returns>The list.</returns>
    public static List<CollectionItemDto> CreateList()
    {
        var list = new List<CollectionItemDto>(BenchmarkConstants.CollectionSize);
        for (var index = 0; index < BenchmarkConstants.CollectionSize; index++)
        {
            list.Add(CreateItem(index));
        }

        return list;
    }

    /// <summary>
    /// Creates an array of <see cref="CollectionItemDto"/> with nested dictionaries.
    /// </summary>
    /// <returns>The array.</returns>
    public static CollectionItemDto[] CreateArray()
    {
        var array = new CollectionItemDto[BenchmarkConstants.CollectionSize];
        for (var index = 0; index < BenchmarkConstants.CollectionSize; index++)
        {
            array[index] = CreateItem(index);
        }

        return array;
    }

    /// <summary>
    /// Creates a dictionary of <see cref="CollectionItemDto"/> with nested dictionaries.
    /// </summary>
    /// <returns>The dictionary.</returns>
    public static Dictionary<string, CollectionItemDto> CreateDictionary()
    {
        var dictionary = new Dictionary<string, CollectionItemDto>(BenchmarkConstants.CollectionSize);
        for (var index = 0; index < BenchmarkConstants.CollectionSize; index++)
        {
            dictionary[$"key-{index}"] = CreateItem(index);
        }

        return dictionary;
    }

    /// <summary>
    /// Creates a list of integers.
    /// </summary>
    /// <returns>The list.</returns>
    public static List<int> CreateIntList()
    {
        var list = new List<int>(BenchmarkConstants.CollectionSize);
        for (var index = 0; index < BenchmarkConstants.CollectionSize; index++)
        {
            list.Add(index);
        }

        return list;
    }

    /// <summary>
    /// Creates an integer array.
    /// </summary>
    /// <returns>The array.</returns>
    public static int[] CreateIntArray()
    {
        var array = new int[BenchmarkConstants.CollectionSize];
        for (var index = 0; index < BenchmarkConstants.CollectionSize; index++)
        {
            array[index] = index;
        }

        return array;
    }

    private static CollectionItemDto CreateItem(int index)
    {
        var attributes = new Dictionary<string, string>(BenchmarkConstants.AttributesPerItem);
        for (var attributeIndex = 0; attributeIndex < BenchmarkConstants.AttributesPerItem; attributeIndex++)
        {
            attributes[$"attr-{attributeIndex}"] = $"value-{index}-{attributeIndex}";
        }

        return new CollectionItemDto
        {
            Id = index,
            Name = $"item-{index}",
            Attributes = attributes,
        };
    }
}