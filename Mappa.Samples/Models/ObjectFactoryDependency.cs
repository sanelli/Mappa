// <copyright file="ObjectFactoryDependency.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Dependency type exposing an instance factory used by samples.
/// </summary>
public sealed class ObjectFactoryDependency
{
    private readonly string factoryTag = "source-and-context";

    /// <summary>
    /// Creates a fully produced target from the source and <see cref="MappaContext"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The created target.</returns>
    public ObjectFactoryTargetModel CreateFromSourceAndContext(ObjectFactorySourceModel source, MappaContext context)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(context);
        var suffix = context.TryGetValue<string>("suffix", out var text) ? text : "none";
        return new ObjectFactoryTargetModel
        {
            Name = $"{source.Name}-{suffix}",
            Value = source.Value + 200,
            FactoryTag = this.factoryTag,
        };
    }
}