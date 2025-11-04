// <copyright file="CustomICollectionWithCapacityConstructor.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;

namespace Mappa.Samples.Models;

/// <summary>
/// Collection implementing the <see cref="ICollection{T}"/>\interface with
/// capacity constructor.
/// </summary>
/// <typeparam name="T">The type of the collection items.</typeparam>
#pragma warning disable CA1710
public sealed class CustomICollectionWithCapacityConstructor<T>
#pragma warning restore CA1710
    : ICollection<T>
{
    private readonly ICollection<T> list;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomICollectionWithCapacityConstructor{T}"/> class.
    /// </summary>
    /// <param name="capacity">The container initial capacity.</param>
    public CustomICollectionWithCapacityConstructor(int capacity)
    {
        this.list = new List<T>(capacity);
    }

    /// <inheritdoc />
    public int Count => this.list.Count;

    /// <inheritdoc />
    public bool IsReadOnly => this.list.IsReadOnly;

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => this.list.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => (this.list as IEnumerable).GetEnumerator();

    /// <inheritdoc />
    public void Add(T item) => this.list.Add(item);

    /// <inheritdoc />
    public void Clear() => this.list.Clear();

    /// <inheritdoc />
    public bool Contains(T item) => this.list.Contains(item);

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex) => this.list.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool Remove(T item) => this.list.Remove(item);
}