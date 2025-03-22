// <copyright file="CustomCollectionImplementingICollection.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;

namespace Mappa.Samples.Models;

/// <summary>
/// Custom class implementing <see cref="ICollection{T}"/>.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public class CustomCollectionImplementingICollection<T>
    : ICollection<T>
{
    private readonly List<T> items;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingICollection{T}"/> class.
    /// </summary>
    /// <param name="items">Items in the custom collection.</param>
    public CustomCollectionImplementingICollection(T[] items)
    {
        this.items = new(items);
    }

    /// <inheritdoc />
    public int Count => this.items.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => this.items.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.items.GetEnumerator();

    /// <inheritdoc />
    public void Add(T item) => this.items.Add(item);

    /// <inheritdoc />
    public void Clear() => this.items.Clear();

    /// <inheritdoc />
    public bool Contains(T item) => this.items.Contains(item);

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex) => this.items.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool Remove(T item) => this.items.Remove(item);
}