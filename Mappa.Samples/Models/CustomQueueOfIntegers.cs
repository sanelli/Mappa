// <copyright file="CustomQueueOfIntegers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Custom Queue.
/// </summary>
public class CustomQueueOfIntegers : CustomQueue<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomQueueOfIntegers"/> class.
    /// </summary>
    /// <param name="items">The items to place on the Queue.</param>
    public CustomQueueOfIntegers(IEnumerable<int> items)
        : base(items)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomQueueOfIntegers"/> class.
    /// </summary>
    public CustomQueueOfIntegers()
        : this([])
    {
    }
}