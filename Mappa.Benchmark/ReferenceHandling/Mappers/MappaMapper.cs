// <copyright file="MappaMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Benchmark.ReferenceHandling.Models;

namespace Mappa.Benchmark.ReferenceHandling.Mappers;

/// <summary>
/// Mappa mapper for reference-handling benchmarks.
/// </summary>
[Mappa]
[MappaSettings(ReferenceReusing = BooleanSetting.Enable)]
internal sealed partial class MappaMapper
{
    /// <summary>
    /// Maps a person with reference reuse.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The target.</returns>
    public partial PersonTarget MapPerson(PersonSource source, MappaContext context);

    /// <summary>
    /// Maps an address with reference reuse.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The target.</returns>
    public partial AddressTarget MapAddress(AddressSource source, MappaContext context);

    /// <summary>
    /// Maps a DAG root with reference reuse.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The target.</returns>
    public partial RootTarget MapRoot(RootSource source, MappaContext context);

    /// <summary>
    /// Maps a shared node with reference reuse.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The target.</returns>
    public partial NodeTarget MapNode(NodeSource source, MappaContext context);
}