// <copyright file="MappaDependencyProtobufMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper relying on <see cref="Dependency.Protobuf.MappaProtobufMapper"/>.
/// </summary>
[Mappa]
public partial class MappaDependencyProtobufMapper
{
    [MappaDependency]
    private Mappa.Dependency.Protobuf.MappaProtobufMapper dependency = new();

    /// <summary>
    /// Map from <see cref="MappaDependencySourceRecord"/> to <see cref="MappaDependencyTargetModel"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    public partial MappaDependencyTargetModel MapWithDependencies(MappaDependencySourceRecord source);
}