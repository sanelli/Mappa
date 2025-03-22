// <copyright file="CustomCollectionImplementingIEnumerable.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;

namespace Mappa.Samples.Models;

/// <summary>
/// Custom class implementing <see cref="IEnumerable{T}"/>.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public class CustomCollectionImplementingIEnumerable<T>
    : IEnumerable<T>
{
    private readonly List<T> items;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingIEnumerable{T}"/> class.
    /// </summary>
    /// <param name="items">Items in the custom collection.</param>
    public CustomCollectionImplementingIEnumerable(T[] items)
    {
        this.items = new(items);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => this.items.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}