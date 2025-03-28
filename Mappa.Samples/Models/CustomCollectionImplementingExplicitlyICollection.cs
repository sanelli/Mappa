// <copyright file="CustomCollectionImplementingExplicitlyICollection.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;

namespace Mappa.Samples.Models;

/// <summary>
/// Custom class implementing <see cref="ICollection{T}"/>.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public sealed class CustomCollectionImplementingExplicitlyICollection<T>
    : ICollection<T>
{
    private readonly List<T> items;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingExplicitlyICollection{T}"/> class.
    /// </summary>
    /// <param name="items">Items in the custom collection.</param>
    public CustomCollectionImplementingExplicitlyICollection(T[] items)
    {
        this.items = new(items);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingExplicitlyICollection{T}"/> class.
    /// </summary>
    public CustomCollectionImplementingExplicitlyICollection()
        : this([])
    {
    }

    /// <inheritdoc />
    int ICollection<T>.Count => this.items.Count;

    /// <inheritdoc />
    bool ICollection<T>.IsReadOnly => false;

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => this.items.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.items.GetEnumerator();

    /// <inheritdoc />
    void ICollection<T>.Add(T item) => this.items.Add(item);

    /// <inheritdoc />
    void ICollection<T>.Clear() => this.items.Clear();

    /// <inheritdoc />
    bool ICollection<T>.Contains(T item) => this.items.Contains(item);

    /// <inheritdoc />
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => this.items.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    bool ICollection<T>.Remove(T item) => this.items.Remove(item);
}