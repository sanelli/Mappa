// <copyright file="CustomQueueWithCapacityConstructor.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Custom stack derived from <see cref="Queue{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the items in teh stack.</typeparam>
#pragma warning disable CA1710
public sealed class CustomQueueWithCapacityConstructor<T>
#pragma warning restore CA1710
    : Queue<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomQueueWithCapacityConstructor{T}"/> class.
    /// </summary>
    /// <param name="capacity">The initial capacity of the stack.</param>
    public CustomQueueWithCapacityConstructor(int capacity)
        : base(capacity)
    {
    }
}