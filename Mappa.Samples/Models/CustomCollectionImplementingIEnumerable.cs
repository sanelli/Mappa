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
    private readonly T[] items;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingIEnumerable{T}"/> class.
    /// </summary>
    /// <param name="items">Items in the custom collection.</param>
    public CustomCollectionImplementingIEnumerable(T[] items)
    {
        this.items = items;
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => new Enumerator(this);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    private sealed class Enumerator
        : IEnumerator<T>
    {
        private readonly CustomCollectionImplementingIEnumerable<T> parent;
        private int current = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator"/> class.
        /// </summary>
        /// <param name="parent">The parent class.</param>
        public Enumerator(CustomCollectionImplementingIEnumerable<T> parent)
        {
            this.parent = parent;
        }

        /// <inheritdoc/>
        T IEnumerator<T>.Current => this.parent.items[this.current];

        /// <inheritdoc/>
        object? IEnumerator.Current => this.parent.items[this.current];

        /// <inheritdoc/>
        public bool MoveNext()
        {
            this.current++;
            return this.current < this.parent.items.Length;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            this.current = -1;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}