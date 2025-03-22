// <copyright file="CustomCollectionImplementingIReadOnlyCollection.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;

namespace Mappa.Samples.Models;

/// <summary>
/// Custom class implementing <see cref="IReadOnlyCollection{T}"/>.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public class CustomCollectionImplementingIReadOnlyCollection<T>
    : IReadOnlyCollection<T>
{
    private readonly List<T> items;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingIReadOnlyCollection{T}"/> class.
    /// </summary>
    /// <param name="items">Items in the custom collection.</param>
    public CustomCollectionImplementingIReadOnlyCollection(T[] items)
    {
        this.items = new(items);
    }

    /// <inheritdoc />
    public int Count => this.items.Count;

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => this.items.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.items.GetEnumerator();
}