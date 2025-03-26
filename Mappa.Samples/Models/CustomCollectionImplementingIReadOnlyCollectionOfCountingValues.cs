// <copyright file="CustomCollectionImplementingIReadOnlyCollectionOfCountingValues.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Custom non-generic class implementing <see cref="IReadOnlyCollection{T}"/>
/// or <see cref="CountingValues"/>.
/// </summary>
public sealed class CustomCollectionImplementingIReadOnlyCollectionOfCountingValues(CountingValues[] items)
    : CustomCollectionImplementingIReadOnlyCollection<CountingValues>(items);