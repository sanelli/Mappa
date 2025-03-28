// <copyright file="CustomCollectionImplementingExplicitlyICollectionOfStrings.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;

namespace Mappa.Samples.Models;

/// <summary>
/// Custom class implementing <see cref="ICollection{T}"/>.
/// </summary>
 #pragma warning disable CA1710
public sealed class CustomCollectionImplementingExplicitlyICollectionOfStrings
 #pragma warning restore CA1710
    : ICollection<string>
{
    private readonly List<string> items;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingExplicitlyICollectionOfStrings"/> class.
    /// </summary>
    /// <param name="items">Items in the custom collection.</param>
    public CustomCollectionImplementingExplicitlyICollectionOfStrings(string[] items)
    {
        this.items = new(items);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingExplicitlyICollectionOfStrings"/> class.
    /// </summary>
    public CustomCollectionImplementingExplicitlyICollectionOfStrings()
        : this([])
    {
    }

    /// <inheritdoc />
    int ICollection<string>.Count => this.items.Count;

    /// <inheritdoc />
    bool ICollection<string>.IsReadOnly => false;

    /// <inheritdoc />
    public IEnumerator<string> GetEnumerator() => this.items.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.items.GetEnumerator();

    /// <inheritdoc />
    void ICollection<string>.Add(string item) => this.items.Add(item);

    /// <inheritdoc />
    void ICollection<string>.Clear() => this.items.Clear();

    /// <inheritdoc />
    bool ICollection<string>.Contains(string item) => this.items.Contains(item);

    /// <inheritdoc />
    void ICollection<string>.CopyTo(string[] array, int arrayIndex) => this.items.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    bool ICollection<string>.Remove(string item) => this.items.Remove(item);
}