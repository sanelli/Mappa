// <copyright file="CustomCollectionImplementingISet.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;

namespace Mappa.Samples.Models;

/// <summary>
/// Custom class implementing <see cref="ISet{T}"/>.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public class CustomCollectionImplementingISet<T>
    : ISet<T>
{
    private readonly HashSet<T> items;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingISet{T}"/> class.
    /// </summary>
    /// <param name="items">Items in the custom collection.</param>
    public CustomCollectionImplementingISet(T[] items)
    {
        this.items = new(items);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingISet{T}"/> class.
    /// </summary>
    public CustomCollectionImplementingISet()
    : this([])
    {
    }

    /// <inheritdoc />
    public int Count => this.items.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => this.items.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <inheritdoc />
    void ICollection<T>.Add(T item) => this.items.Add(item);

    /// <inheritdoc />
    public void ExceptWith(IEnumerable<T> other) => this.items.ExceptWith(other);

    /// <inheritdoc />
    public void IntersectWith(IEnumerable<T> other) => this.items.IntersectWith(other);

    /// <inheritdoc />
    public bool IsProperSubsetOf(IEnumerable<T> other) => this.items.IsProperSubsetOf(other);

    /// <inheritdoc />
    public bool IsProperSupersetOf(IEnumerable<T> other) => this.items.IsProperSupersetOf(other);

    /// <inheritdoc />
    public bool IsSubsetOf(IEnumerable<T> other) => this.items.IsSubsetOf(other);

    /// <inheritdoc />
    public bool IsSupersetOf(IEnumerable<T> other) => this.items.IsSupersetOf(other);

    /// <inheritdoc />
    public bool Overlaps(IEnumerable<T> other) => this.items.Overlaps(other);

    /// <inheritdoc />
    public bool SetEquals(IEnumerable<T> other) => this.items.SetEquals(other);

    /// <inheritdoc />
    public void SymmetricExceptWith(IEnumerable<T> other) => this.items.SymmetricExceptWith(other);

    /// <inheritdoc />
    public void UnionWith(IEnumerable<T> other) => this.items.UnionWith(other);

    /// <inheritdoc />
    public bool Add(T item) => this.items.Add(item);

    /// <inheritdoc />
    public void Clear() => this.items.Clear();

    /// <inheritdoc />
    public bool Contains(T item) => this.items.Contains(item);

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex) => this.items.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool Remove(T item) => this.items.Remove(item);
}