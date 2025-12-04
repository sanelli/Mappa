// <copyright file="MapperHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.Polymorphism.One;

/// <summary>
/// A helper for mapping.
/// </summary>
#pragma warning disable S1118
#pragma warning disable CA1052
public class MapperHelper
#pragma warning restore CA1052
#pragma warning restore S1118
{
    /// <summary>
    /// Default method to generate a new <see cref="Models.Polymorphism.One.TargetBaseClass"/>.
    /// </summary>
    /// <param name="source">The source of the mapping.</param>
    /// <returns>The target.</returns>
    public static TargetBaseClass InvokeMe(SourceBaseClass source)
    {
        return new TargetBaseClass { NumericProperty = 1984, };
    }
}