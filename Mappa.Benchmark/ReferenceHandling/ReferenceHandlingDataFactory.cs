// <copyright file="ReferenceHandlingDataFactory.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Benchmark.Common;
using Mappa.Benchmark.ReferenceHandling.Models;

namespace Mappa.Benchmark.ReferenceHandling;

/// <summary>
/// Builds reproducible cyclic and shared-reference graphs for benchmarks.
/// </summary>
internal static class ReferenceHandlingDataFactory
{
    /// <summary>
    /// Creates a closed Person↔Address cycle.
    /// </summary>
    /// <returns>The person root of the cycle.</returns>
    public static PersonSource CreateClosedCycle()
    {
        var person = new PersonSource { Id = BenchmarkConstants.RandomSeed };
        var address = new AddressSource { Id = BenchmarkConstants.RandomSeed + 1, Owner = person };
        person.Address = address;
        return person;
    }

    /// <summary>
    /// Creates a root whose left and right children are the same node instance.
    /// </summary>
    /// <returns>The shared-reference DAG root.</returns>
    public static RootSource CreateSharedDag()
    {
        var shared = new NodeSource { Id = BenchmarkConstants.RandomSeed };
        return new RootSource
        {
            Left = shared,
            Right = shared,
        };
    }
}