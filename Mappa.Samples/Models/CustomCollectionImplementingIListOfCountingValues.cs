// <copyright file="CustomCollectionImplementingIListOfCountingValues.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Custom non-generic class implementing <see cref="IList{T}"/>
/// or <see cref="CountingValues"/>.
/// </summary>
public sealed class CustomCollectionImplementingIListOfCountingValues(CountingValues[] items)
    : CustomCollectionImplementingIList<CountingValues>(items);