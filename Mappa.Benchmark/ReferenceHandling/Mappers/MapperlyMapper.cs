// <copyright file="MapperlyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Benchmark.ReferenceHandling.Models;

using Riok.Mapperly.Abstractions;

namespace Mappa.Benchmark.ReferenceHandling.Mappers;

/// <summary>
/// Mapperly mapper for reference-handling benchmarks.
/// </summary>
[Mapper(UseReferenceHandling = true)]
internal sealed partial class MapperlyMapper
{
    /// <summary>
    /// Maps a person with reference handling.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    public partial PersonTarget MapPerson(PersonSource source);

    /// <summary>
    /// Maps an address with reference handling.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    public partial AddressTarget MapAddress(AddressSource source);

    /// <summary>
    /// Maps a DAG root with reference handling.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    public partial RootTarget MapRoot(RootSource source);

    /// <summary>
    /// Maps a shared node with reference handling.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    public partial NodeTarget MapNode(NodeSource source);
}