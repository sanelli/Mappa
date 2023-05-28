// <copyright file="MapMethodStrategyMapperWithDependency.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using the map method strategy.
/// </summary>
[Mappa]
public sealed partial class MapMethodStrategyMapperWithDependency
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapMethodStrategyMapperWithDependency"/> class.
    /// </summary>
    /// <param name="dependency">The mapping dependency.</param>
    public MapMethodStrategyMapperWithDependency(MapMethodStrategyMapperDependency dependency)
    {
        this.Dependency = dependency;
    }

    [MappaDependency]
    private MapMethodStrategyMapperDependency Dependency { get; }

    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetClassWithInnerClassModel Map(SourceClassWithInnerClassModel sourceClassModel);
}