// <copyright file="CustomDictionaryWithNonGenericAndExplicitImplementation.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Mappa.Samples.Models;

/// <summary>
/// Custom dictionary implementation without generics.
/// Interface is implemented explicitly.
/// </summary>
 #pragma warning disable CA1710
public sealed class CustomDictionaryWithNonGenericAndExplicitImplementation : IDictionary<string, string>
 #pragma warning restore CA1710
{
    private readonly Dictionary<string, string> dictionaryImplementation = new();

    /// <inheritdoc/>
    int ICollection<KeyValuePair<string, string>>.Count => this.dictionaryImplementation.Count;

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<string, string>>.IsReadOnly => false;

    /// <inheritdoc/>
    ICollection<string> IDictionary<string, string>.Keys => this.dictionaryImplementation.Keys;

    /// <inheritdoc/>
    ICollection<string> IDictionary<string, string>.Values => this.dictionaryImplementation.Values;

    /// <inheritdoc/>
    string IDictionary<string, string>.this[string key]
    {
        get => this.dictionaryImplementation[key];
        set => this.dictionaryImplementation[key] = value;
    }

    /// <inheritdoc/>
    IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
    {
        return this.dictionaryImplementation.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)this.dictionaryImplementation).GetEnumerator();
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
    {
        this.dictionaryImplementation.Add(item.Key, item.Value);
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<string, string>>.Clear()
    {
        this.dictionaryImplementation.Clear();
    }

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
    {
        return this.dictionaryImplementation.Contains(item);
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
    {
        // Ignore.
    }

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
    {
        return this.dictionaryImplementation.Remove(item.Key);
    }

    /// <inheritdoc/>
    void IDictionary<string, string>.Add(string key, string value)
    {
        this.dictionaryImplementation.Add(key, value);
    }

    /// <inheritdoc/>
    bool IDictionary<string, string>.ContainsKey(string key)
    {
        return this.dictionaryImplementation.ContainsKey(key);
    }

    /// <inheritdoc/>
    bool IDictionary<string, string>.Remove(string key)
    {
        return this.dictionaryImplementation.Remove(key);
    }

    /// <inheritdoc/>
    bool IDictionary<string, string>.TryGetValue(string key, [MaybeNullWhen(false)] out string value)
    {
        return this.dictionaryImplementation.TryGetValue(key, out value);
    }
}