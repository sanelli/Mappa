// <copyright file="MapMethodStrategyWithDependencyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using the map method strategy.
/// </summary>
// TODO [#93] Map using static dependency on static class.
// TODO [#93] Map using static dependency on non-static class with static method.
// TODO [#93] Map using static property.
// TODO [#93] Map using field.
// TODO [#93] Map using static field.
// TODO [#93] Map using static method on property.
// TODO [#93] Map using static method on field.
[Mappa]
public sealed partial class MapMethodStrategyWithDependencyMapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapMethodStrategyWithDependencyMapper"/> class.
    /// </summary>
    /// <param name="dependency">The mapping dependency.</param>
    public MapMethodStrategyWithDependencyMapper(MapMethodStrategyMapperDependency dependency)
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