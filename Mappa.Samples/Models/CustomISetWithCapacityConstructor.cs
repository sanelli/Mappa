// <copyright file="CustomISetWithCapacityConstructor.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;

namespace Mappa.Samples.Models;

/// <summary>
/// The custom implementation of <see cref="ISet{T}"/>
/// with capacity constructor.
/// </summary>
/// <typeparam name="T">The type of the items in the set.</typeparam>
#pragma warning disable CA1710
public sealed class CustomISetWithCapacityConstructor<T>
#pragma warning restore CA1710
    : ISet<T>
{
    private readonly ISet<T> set;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomISetWithCapacityConstructor{T}"/> class.
    /// </summary>
    /// <param name="capacity">The initial capacity.</param>
    public CustomISetWithCapacityConstructor(int capacity)
    {
        this.set = new HashSet<T>(capacity);
    }

    /// <inheritdoc />
    public int Count => this.set.Count;

    /// <inheritdoc />
    public bool IsReadOnly => this.set.IsReadOnly;

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        return this.set.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)this.set).GetEnumerator();
    }

    /// <inheritdoc/>
    void ICollection<T>.Add(T item)
    {
        this.set.Add(item);
    }

    /// <inheritdoc />
    public void ExceptWith(IEnumerable<T> other)
    {
        this.set.ExceptWith(other);
    }

    /// <inheritdoc />
    public void IntersectWith(IEnumerable<T> other)
    {
        this.set.IntersectWith(other);
    }

    /// <inheritdoc />
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return this.set.IsProperSubsetOf(other);
    }

    /// <inheritdoc />
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return this.set.IsProperSupersetOf(other);
    }

    /// <inheritdoc />
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return this.set.IsSubsetOf(other);
    }

    /// <inheritdoc />
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return this.set.IsSupersetOf(other);
    }

    /// <inheritdoc />
    public bool Overlaps(IEnumerable<T> other)
    {
        return this.set.Overlaps(other);
    }

    /// <inheritdoc />
    public bool SetEquals(IEnumerable<T> other)
    {
        return this.set.SetEquals(other);
    }

    /// <inheritdoc />
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        this.set.SymmetricExceptWith(other);
    }

    /// <inheritdoc />
    public void UnionWith(IEnumerable<T> other)
    {
        this.set.UnionWith(other);
    }

    /// <inheritdoc />
    bool ISet<T>.Add(T item)
    {
        return this.set.Add(item);
    }

    /// <inheritdoc />
    public void Clear()
    {
        this.set.Clear();
    }

    /// <inheritdoc />
    public bool Contains(T item)
    {
        return this.set.Contains(item);
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
        this.set.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        return this.set.Remove(item);
    }
}