// <copyright file="MapMethodStrategyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

// TODO [#186] Map property invoking a method automatically identified in the base class of the mapper.
// TODO [#186] Map property invoking a method automatically identifier in the base class of the type of a dependency property.
// TODO [#186] Map property invoking a method automatically identifier in the base class of the type of a dependency field.
using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

#pragma warning disable SA1402

/// <summary>
/// Mapper using the map method strategy.
/// </summary>
///
[Mappa]
public sealed partial class MapMethodStrategyMapper
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetClassModel Map(SourceClassModel sourceClassModel);

    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetClassWithInnerClassModel Map(SourceClassWithInnerClassModel sourceClassModel);
}

/// <summary>
/// Same as <see cref="MapMethodStrategyMapper"/> but provide a static method.
/// </summary>
[Mappa]
public sealed partial class StaticMapMethodStrategyMapper
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The target model.</returns>
    public static partial TargetClassWithInnerClassModel Map(SourceClassWithInnerClassModel sourceClassModel);

    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetClassModel Map(SourceClassModel sourceClassModel);
}