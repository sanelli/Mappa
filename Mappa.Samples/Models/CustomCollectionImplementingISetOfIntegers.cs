// <copyright file="CustomCollectionImplementingISetOfIntegers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Implementation of <see cref="CustomCollectionImplementingISet{T}"/>
/// for <see cref="int"/>.
/// </summary>
/// <param name="items">The items to initialize the set.</param>
public sealed class CustomCollectionImplementingISetOfIntegers(int[] items)
    : CustomCollectionImplementingISet<int>(items)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingISetOfIntegers"/> class.
    /// </summary>
    public CustomCollectionImplementingISetOfIntegers()
        : this([])
    {
    }
}