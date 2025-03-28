// <copyright file="CustomCollectionImplementingExplicitlyISetOfIntegers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;

namespace Mappa.Samples.Models;

/// <summary>
/// Custom class implementing <see cref="ISet{T}"/>.
/// </summary>
#pragma warning disable CA1710
public sealed class CustomCollectionImplementingExplicitlyISetOfIntegers
#pragma warning restore CA1710
    : ISet<int>
{
    private readonly HashSet<int> items;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingExplicitlyISetOfIntegers"/> class.
    /// </summary>
    /// <param name="items">Items in the custom collection.</param>
    public CustomCollectionImplementingExplicitlyISetOfIntegers(int[] items)
    {
        this.items = new(items);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingExplicitlyISetOfIntegers"/> class.
    /// </summary>
    public CustomCollectionImplementingExplicitlyISetOfIntegers()
    : this([])
    {
    }

    /// <inheritdoc />
    int ICollection<int>.Count => this.items.Count;

    /// <inheritdoc />
    bool ICollection<int>.IsReadOnly => false;

    /// <inheritdoc />
    public IEnumerator<int> GetEnumerator() => this.items.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <inheritdoc />
    void ICollection<int>.Add(int item) => this.items.Add(item);

    /// <inheritdoc />
    void ISet<int>.ExceptWith(IEnumerable<int> other) => this.items.ExceptWith(other);

    /// <inheritdoc />
    void ISet<int>.IntersectWith(IEnumerable<int> other) => this.items.IntersectWith(other);

    /// <inheritdoc />
    bool ISet<int>.IsProperSubsetOf(IEnumerable<int> other) => this.items.IsProperSubsetOf(other);

    /// <inheritdoc />
    bool ISet<int>.IsProperSupersetOf(IEnumerable<int> other) => this.items.IsProperSupersetOf(other);

    /// <inheritdoc />
    bool ISet<int>.IsSubsetOf(IEnumerable<int> other) => this.items.IsSubsetOf(other);

    /// <inheritdoc />
    bool ISet<int>.IsSupersetOf(IEnumerable<int> other) => this.items.IsSupersetOf(other);

    /// <inheritdoc />
    bool ISet<int>.Overlaps(IEnumerable<int> other) => this.items.Overlaps(other);

    /// <inheritdoc />
    bool ISet<int>.SetEquals(IEnumerable<int> other) => this.items.SetEquals(other);

    /// <inheritdoc />
    void ISet<int>.SymmetricExceptWith(IEnumerable<int> other) => this.items.SymmetricExceptWith(other);

    /// <inheritdoc />
    void ISet<int>.UnionWith(IEnumerable<int> other) => this.items.UnionWith(other);

    /// <inheritdoc />
    bool ISet<int>.Add(int item) => this.items.Add(item);

    /// <inheritdoc />
    void ICollection<int>.Clear() => this.items.Clear();

    /// <inheritdoc />
    bool ICollection<int>.Contains(int item) => this.items.Contains(item);

    /// <inheritdoc />
    void ICollection<int>.CopyTo(int[] array, int arrayIndex) => this.items.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    bool ICollection<int>.Remove(int item) => this.items.Remove(item);
}