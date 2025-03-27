// <copyright file="CustomCollectionImplementingExplicitlyISet.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;

namespace Mappa.Samples.Models;

/// <summary>
/// Custom class implementing <see cref="ISet{T}"/>.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public sealed class CustomCollectionImplementingExplicitlyISet<T>
    : ISet<T>
{
    private readonly HashSet<T> items;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingExplicitlyISet{T}"/> class.
    /// </summary>
    /// <param name="items">Items in the custom collection.</param>
    public CustomCollectionImplementingExplicitlyISet(T[] items)
    {
        this.items = new(items);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingExplicitlyISet{T}"/> class.
    /// </summary>
    public CustomCollectionImplementingExplicitlyISet()
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
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <inheritdoc />
    void ICollection<T>.Add(T item) => this.items.Add(item);

    /// <inheritdoc />
    void ISet<T>.ExceptWith(IEnumerable<T> other) => this.items.ExceptWith(other);

    /// <inheritdoc />
    void ISet<T>.IntersectWith(IEnumerable<T> other) => this.items.IntersectWith(other);

    /// <inheritdoc />
    bool ISet<T>.IsProperSubsetOf(IEnumerable<T> other) => this.items.IsProperSubsetOf(other);

    /// <inheritdoc />
    bool ISet<T>.IsProperSupersetOf(IEnumerable<T> other) => this.items.IsProperSupersetOf(other);

    /// <inheritdoc />
    bool ISet<T>.IsSubsetOf(IEnumerable<T> other) => this.items.IsSubsetOf(other);

    /// <inheritdoc />
    bool ISet<T>.IsSupersetOf(IEnumerable<T> other) => this.items.IsSupersetOf(other);

    /// <inheritdoc />
    bool ISet<T>.Overlaps(IEnumerable<T> other) => this.items.Overlaps(other);

    /// <inheritdoc />
    bool ISet<T>.SetEquals(IEnumerable<T> other) => this.items.SetEquals(other);

    /// <inheritdoc />
    void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) => this.items.SymmetricExceptWith(other);

    /// <inheritdoc />
    void ISet<T>.UnionWith(IEnumerable<T> other) => this.items.UnionWith(other);

    /// <inheritdoc />
    bool ISet<T>.Add(T item) => this.items.Add(item);

    /// <inheritdoc />
    void ICollection<T>.Clear() => this.items.Clear();

    /// <inheritdoc />
    bool ICollection<T>.Contains(T item) => this.items.Contains(item);

    /// <inheritdoc />
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => this.items.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    bool ICollection<T>.Remove(T item) => this.items.Remove(item);
}