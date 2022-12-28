// <copyright file="MappaMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Benchmark.Mappers;

/// <summary>
/// The Mappa mapper.
/// </summary>
[Attributes.Mappa]
public sealed partial class MappaMapper
{
    #nullable disable
    /// <summary>
    /// Map a string to an object.
    /// Nullable is disabled.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The mapped object.</returns>
    public partial object MapStringToObjectWithNullableDisabled(string input);
    #nullable restore
}