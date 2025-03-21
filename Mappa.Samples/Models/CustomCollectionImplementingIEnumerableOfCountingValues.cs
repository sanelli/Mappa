// <copyright file="CustomCollectionImplementingIEnumerableOfCountingValues.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Custom non-generic class implementing <see cref="IEnumerable{T}"/>
/// or <see cref="CountingValues"/>.
/// </summary>
public sealed class CustomCollectionImplementingIEnumerableOfCountingValues(CountingValues[] items)
    : CustomCollectionImplementingIEnumerable<CountingValues>(items);