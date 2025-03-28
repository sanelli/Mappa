// <copyright file="CustomCollectionImplementingExplicitlyICollectionOfIntegers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;

namespace Mappa.Samples.Models;

/// <summary>
/// Custom class implementing <see cref="ICollection{T}"/>.
/// </summary>
 #pragma warning disable CA1710
public sealed class CustomCollectionImplementingExplicitlyICollectionOfIntegers
 #pragma warning restore CA1710
    : ICollection<int>
{
    private readonly List<int> items;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingExplicitlyICollectionOfIntegers"/> class.
    /// </summary>
    /// <param name="items">Items in the custom collection.</param>
    public CustomCollectionImplementingExplicitlyICollectionOfIntegers(int[] items)
    {
        this.items = new(items);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingExplicitlyICollectionOfIntegers"/> class.
    /// </summary>
    public CustomCollectionImplementingExplicitlyICollectionOfIntegers()
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
    IEnumerator IEnumerable.GetEnumerator() => this.items.GetEnumerator();

    /// <inheritdoc />
    void ICollection<int>.Add(int item) => this.items.Add(item);

    /// <inheritdoc />
    void ICollection<int>.Clear() => this.items.Clear();

    /// <inheritdoc />
    bool ICollection<int>.Contains(int item) => this.items.Contains(item);

    /// <inheritdoc />
    void ICollection<int>.CopyTo(int[] array, int arrayIndex) => this.items.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    bool ICollection<int>.Remove(int item) => this.items.Remove(item);
}