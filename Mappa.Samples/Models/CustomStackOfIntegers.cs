// <copyright file="CustomStackOfIntegers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Custom stack.
/// </summary>
public class CustomStackOfIntegers : CustomStack<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomStackOfIntegers"/> class.
    /// </summary>
    /// <param name="items">The items to place on the stack.</param>
    public CustomStackOfIntegers(IEnumerable<int> items)
        : base(items)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomStackOfIntegers"/> class.
    /// </summary>
    public CustomStackOfIntegers()
        : this([])
    {
    }
}