// <copyright file="ObjectFactoryHelpers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Static helper type used as an object factory location in samples.
/// </summary>
public static class ObjectFactoryHelpers
{
    /// <summary>
    /// Creates a fully produced target from the source only (no further property assignment).
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The created target.</returns>
    public static ObjectFactoryTargetModel CreateFromSource(ObjectFactorySourceModel source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new ObjectFactoryTargetModel
        {
            Name = $"{source.Name}-source",
            Value = source.Value + 100,
            FactoryTag = "source",
        };
    }
}