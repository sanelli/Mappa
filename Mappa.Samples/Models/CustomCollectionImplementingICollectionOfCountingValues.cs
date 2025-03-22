// <copyright file="CustomCollectionImplementingICollectionOfCountingValues.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Custom non-generic class implementing <see cref="ICollection{T}"/>
/// or <see cref="CountingValues"/>.
/// </summary>
public sealed class CustomCollectionImplementingICollectionOfCountingValues(CountingValues[] items)
    : CustomCollectionImplementingICollection<CountingValues>(items);