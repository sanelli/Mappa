// <copyright file="CustomCollectionImplementingICollectionOfIntegers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Custom non-generic class implementing <see cref="ICollection{T}"/>
/// of <see cref="int"/>.
/// </summary>
public sealed class CustomCollectionImplementingICollectionOfIntegers(int[] items)
    : CustomCollectionImplementingICollection<int>(items)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCollectionImplementingICollectionOfIntegers"/> class.
    /// </summary>
    public CustomCollectionImplementingICollectionOfIntegers()
        : this([])
    {
    }
}