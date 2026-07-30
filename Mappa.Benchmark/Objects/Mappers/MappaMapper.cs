// <copyright file="MappaMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Benchmark.Objects.Models;

namespace Mappa.Benchmark.Objects.Mappers;

/// <summary>
/// Mappa mapper for class/record/struct person models.
/// </summary>
[Mappa]
internal sealed partial class MappaMapper
{
    /// <summary>
    /// Maps a class person DTO.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    public partial PersonClass Map(PersonClassDto source);

    /// <summary>
    /// Maps a record person DTO.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    public partial PersonRecord Map(PersonRecordDto source);

    /// <summary>
    /// Maps a struct person DTO.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    public partial PersonStruct Map(PersonStructDto source);
}