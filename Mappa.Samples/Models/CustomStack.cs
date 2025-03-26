// <copyright file="CustomStack.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Custom stack.
/// </summary>
/// <typeparam name="T">The type in the stack.</typeparam>
public class CustomStack<T> : Stack<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomStack{T}"/> class.
    /// </summary>
    /// <param name="items">The items to place on the stack.</param>
    public CustomStack(IEnumerable<T> items)
        : base(items)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomStack{T}"/> class.
    /// </summary>
    public CustomStack()
        : this([])
    {
    }
}