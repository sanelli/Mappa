// <copyright file="CustomBlockingCollectionWithCapacityConstructor.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Concurrent;

namespace Mappa.Samples.Models;

/// <summary>
/// Custom implementation of <see cref="BlockingCollection{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the items inside the blocking collection.</typeparam>
public sealed class CustomBlockingCollectionWithCapacityConstructor<T>
    : BlockingCollection<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomBlockingCollectionWithCapacityConstructor{T}"/> class.
    /// </summary>
    /// <param name="capacity">The initial capacity of the container.</param>
    public CustomBlockingCollectionWithCapacityConstructor(int capacity)
        : base(capacity)
    {
    }
}