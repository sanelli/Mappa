// <copyright file="CustomStackWithCapacityConstructor.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Custom stack derived from <see cref="Stack{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the items in teh stack.</typeparam>
#pragma warning disable CA1710
public sealed class CustomStackWithCapacityConstructor<T>
#pragma warning restore CA1710
    : Stack<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomStackWithCapacityConstructor{T}"/> class.
    /// </summary>
    /// <param name="capacity">The initial capacity of the stack.</param>
    public CustomStackWithCapacityConstructor(int capacity)
        : base(capacity)
    {
    }
}