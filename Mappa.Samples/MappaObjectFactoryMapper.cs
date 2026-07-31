// <copyright file="MappaObjectFactoryMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating an object factory with no parameters (empty-constructor-like).
/// </summary>
[Mappa]
public sealed partial class MappaObjectFactoryEmptyParameterMapper
{
    /// <summary>
    /// Map using a parameterless factory; matching properties are filled from the source.
    /// </summary>
    /// <param name="input">The source model.</param>
    /// <returns>The mapped target.</returns>
    [MappaObjectFactory(typeof(ObjectFactoryTargetModel), nameof(CreateEmpty))]
    [MappaIgnoreTargetProperty(nameof(ObjectFactoryTargetModel.FactoryTag))]
    public partial ObjectFactoryTargetModel Map(ObjectFactorySourceModel input);

    private static ObjectFactoryTargetModel CreateEmpty()
    {
        return new ObjectFactoryTargetModel
        {
            FactoryTag = "empty-parameter",
        };
    }
}

/// <summary>
/// Mapper demonstrating an object factory with a single <see cref="MappaContext"/> parameter.
/// </summary>
[Mappa]
public sealed partial class MappaObjectFactoryContextParameterMapper
{
    /// <summary>
    /// Map using a context-only factory; matching properties are filled from the source.
    /// </summary>
    /// <param name="input">The source model.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The mapped target.</returns>
    [MappaObjectFactory(typeof(ObjectFactoryTargetModel), nameof(CreateWithContext))]
    [MappaIgnoreTargetProperty(nameof(ObjectFactoryTargetModel.FactoryTag))]
    public partial ObjectFactoryTargetModel Map(ObjectFactorySourceModel input, MappaContext context);

    private static ObjectFactoryTargetModel CreateWithContext(MappaContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var tag = context.TryGetValue<string>("factory-tag", out var text) ? text : "context-parameter";
        return new ObjectFactoryTargetModel
        {
            FactoryTag = tag,
        };
    }
}

/// <summary>
/// Mapper demonstrating an object factory with source and <see cref="MappaContext"/> parameters.
/// </summary>
[Mappa]
public sealed partial class MappaObjectFactorySourceAndContextMapper
{
    private readonly ObjectFactoryDependency dependency = new();

    /// <summary>
    /// Map using a fully produced factory that accepts the source and context (no property assignment).
    /// </summary>
    /// <param name="input">The source model.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The mapped target.</returns>
    [MappaObjectFactory(typeof(ObjectFactoryTargetModel), nameof(dependency), nameof(ObjectFactoryDependency.CreateFromSourceAndContext))]
    public partial ObjectFactoryTargetModel Map(ObjectFactorySourceModel input, MappaContext context);
}

/// <summary>
/// Mapper demonstrating an object factory with a single source-type parameter.
/// </summary>
[Mappa]
public sealed partial class MappaObjectFactorySourceParameterMapper
{
    /// <summary>
    /// Map using a fully produced factory that accepts only the source (no property assignment).
    /// </summary>
    /// <param name="input">The source model.</param>
    /// <returns>The mapped target.</returns>
    [MappaObjectFactory(typeof(ObjectFactoryTargetModel), typeof(ObjectFactoryHelpers), nameof(ObjectFactoryHelpers.CreateFromSource))]
    public partial ObjectFactoryTargetModel Map(ObjectFactorySourceModel input);
}

/// <summary>
/// Mapper demonstrating an object factory with multiple parameters (parameterized-constructor-like).
/// </summary>
[Mappa]
public sealed partial class MappaObjectFactoryParameterizedMapper
{
    /// <summary>
    /// Map using a factory whose parameters are mapped from source properties like a non-empty constructor.
    /// </summary>
    /// <param name="input">The source model.</param>
    /// <returns>The mapped target.</returns>
    [MappaObjectFactory(typeof(ObjectFactoryTargetModel), nameof(CreateParameterized))]
    public partial ObjectFactoryTargetModel Map(ObjectFactorySourceModel input);

    private static ObjectFactoryTargetModel CreateParameterized(string name, int value)
    {
        return new ObjectFactoryTargetModel
        {
            Name = $"{name}-parameterized",
            Value = value + 50,
            FactoryTag = "parameterized",
        };
    }
}