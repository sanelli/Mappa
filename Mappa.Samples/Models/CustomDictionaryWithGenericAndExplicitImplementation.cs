// <copyright file="CustomDictionaryWithGenericAndExplicitImplementation.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Mappa.Samples.Models;

/// <summary>
/// Custom dictionary implementation with generics.
/// Interface is implemented explicitly.
/// </summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
 #pragma warning disable CA1710
public sealed class CustomDictionaryWithGenericAndExplicitImplementation<TKey, TValue> : IDictionary<TKey, TValue>
 #pragma warning restore CA1710
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> dictionaryImplementation = new();

    /// <inheritdoc/>
    int ICollection<KeyValuePair<TKey, TValue>>.Count => this.dictionaryImplementation.Count;

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

    /// <inheritdoc/>
    ICollection<TKey> IDictionary<TKey, TValue>.Keys => this.dictionaryImplementation.Keys;

    /// <inheritdoc/>
    ICollection<TValue> IDictionary<TKey, TValue>.Values => this.dictionaryImplementation.Values;

    /// <inheritdoc/>
    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
        get => this.dictionaryImplementation[key];
        set => this.dictionaryImplementation[key] = value;
    }

    /// <inheritdoc/>
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        return this.dictionaryImplementation.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)this.dictionaryImplementation).GetEnumerator();
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        this.dictionaryImplementation.Add(item.Key, item.Value);
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
        this.dictionaryImplementation.Clear();
    }

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        return this.dictionaryImplementation.Contains(item);
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        // Ignore.
    }

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        return this.dictionaryImplementation.Remove(item.Key);
    }

    /// <inheritdoc/>
    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
        this.dictionaryImplementation.Add(key, value);
    }

    /// <inheritdoc/>
    bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
    {
        return this.dictionaryImplementation.ContainsKey(key);
    }

    /// <inheritdoc/>
    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
        return this.dictionaryImplementation.Remove(key);
    }

    /// <inheritdoc/>
    bool IDictionary<TKey, TValue>.TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return this.dictionaryImplementation.TryGetValue(key, out value);
    }
}