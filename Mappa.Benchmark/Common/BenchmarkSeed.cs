// <copyright file="BenchmarkSeed.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Bogus;

namespace Mappa.Benchmark.Common;

/// <summary>
/// Applies the shared fixed Bogus seed for reproducible benchmark inputs.
/// </summary>
internal static class BenchmarkSeed
{
    /// <summary>
    /// Sets <see cref="Randomizer.Seed"/> from <see cref="BenchmarkConstants.RandomSeed"/>.
    /// </summary>
    public static void Apply()
    {
        // Deterministic Bogus seed for reproducible benchmarks; not used for security.
#pragma warning disable S2245, S3010
        Randomizer.Seed = new Random(BenchmarkConstants.RandomSeed);
#pragma warning restore S2245, S3010
    }
}