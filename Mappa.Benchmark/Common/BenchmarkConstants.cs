// <copyright file="BenchmarkConstants.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Benchmark.Common;

/// <summary>
/// Shared sizes and seeds for reproducible benchmarks.
/// </summary>
internal static class BenchmarkConstants
{
    /// <summary>
    /// Number of entries used when building collection inputs (a few hundred).
    /// </summary>
    public const int CollectionSize = 300;

    /// <summary>
    /// Number of dictionary entries attached to each collection element.
    /// </summary>
    public const int AttributesPerItem = 5;

    /// <summary>
    /// Fixed seed for AutoBogus / Bogus randomization.
    /// </summary>
    public const int RandomSeed = 134;
}