// <copyright file="CustomQueue.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Custom Queue.
/// </summary>
/// <typeparam name="T">The type in the Queue.</typeparam>
public class CustomQueue<T> : Queue<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomQueue{T}"/> class.
    /// </summary>
    /// <param name="items">The items to place on the Queue.</param>
    public CustomQueue(IEnumerable<T> items)
        : base(items)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomQueue{T}"/> class.
    /// </summary>
    public CustomQueue()
        : this([])
    {
    }
}