// <copyright file="EnumDataFactory.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Bogus;

using Mappa.Benchmark.Common;
using Mappa.Benchmark.Enums.Models;

namespace Mappa.Benchmark.Enums;

/// <summary>
/// Builds deterministic enum-related inputs with Bogus (fixed seed).
/// </summary>
internal static class EnumDataFactory
{
    /// <summary>
    /// Creates a <see cref="SourceStatus"/> value.
    /// </summary>
    /// <returns>The status.</returns>
    public static SourceStatus CreateSourceStatus()
    {
        BenchmarkSeed.Apply();
        return new Faker().PickRandom<SourceStatus>();
    }

    /// <summary>
    /// Creates a <see cref="StringComparison"/> value.
    /// </summary>
    /// <returns>The comparison.</returns>
    public static StringComparison CreateStringComparison()
    {
        BenchmarkSeed.Apply();
        return new Faker().PickRandom<StringComparison>();
    }

    /// <summary>
    /// Creates a <see cref="StringComparison"/> name string.
    /// </summary>
    /// <returns>The comparison name.</returns>
    public static string CreateStringComparisonName()
    {
        return CreateStringComparison().ToString();
    }

    /// <summary>
    /// Creates a <see cref="StringComparison"/> underlying integer.
    /// </summary>
    /// <returns>The comparison integer.</returns>
    public static int CreateStringComparisonValue()
    {
        return (int)CreateStringComparison();
    }
}