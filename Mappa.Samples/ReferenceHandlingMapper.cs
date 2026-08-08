// <copyright file="ReferenceHandlingMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating <see cref="MappaSettingsAttribute.ReferenceReusing"/> with a
/// nullable A↔B cycle and a dedicated map method per nested type.
/// </summary>
[Mappa]
[MappaSettings(ReferenceReusing = BooleanSetting.Enable)]
public sealed partial class ReferenceReusingCycleMapper
{
    /// <summary>
    /// Maps a person, reusing already-mapped references via <paramref name="context"/>.
    /// </summary>
    /// <param name="input">The source person.</param>
    /// <param name="context">The mapping context that owns the reference manager.</param>
    /// <returns>The mapped person.</returns>
    public partial ReferenceHandlingPersonTarget MapPerson(ReferenceHandlingPersonSource input, MappaContext context);

    /// <summary>
    /// Maps an address, reusing already-mapped references via <paramref name="context"/>.
    /// </summary>
    /// <param name="input">The source address.</param>
    /// <param name="context">The mapping context that owns the reference manager.</param>
    /// <returns>The mapped address.</returns>
    public partial ReferenceHandlingAddressTarget MapAddress(ReferenceHandlingAddressSource input, MappaContext context);
}

/// <summary>
/// Mapper demonstrating <see cref="MappaSettingsAttribute.MaxRuntimeDepth"/> on a nested object graph.
/// </summary>
[Mappa]
[MappaSettings(MaxRuntimeDepth = 2)]
public sealed partial class MaxRuntimeDepthMapper
{
    /// <summary>
    /// Maps a three-level graph while enforcing a runtime nesting depth of <c>2</c>.
    /// </summary>
    /// <param name="input">The root source.</param>
    /// <param name="context">The mapping context that owns the reference manager.</param>
    /// <returns>The mapped root.</returns>
    public partial ReferenceHandlingLevel0Target Map(ReferenceHandlingLevel0Source input, MappaContext context);
}

/// <summary>
/// Mapper demonstrating that exceeding <see cref="MappaSettingsAttribute.MaxRuntimeDepth"/> throws
/// <see cref="MappaException"/>.
/// </summary>
[Mappa]
[MappaSettings(MaxRuntimeDepth = 1)]
public sealed partial class MaxRuntimeDepthOverflowMapper
{
    /// <summary>
    /// Maps a three-level graph with a runtime nesting depth of <c>1</c> (overflows on the second nest).
    /// </summary>
    /// <param name="input">The root source.</param>
    /// <param name="context">The mapping context that owns the reference manager.</param>
    /// <returns>The mapped root.</returns>
    public partial ReferenceHandlingLevel0Target Map(ReferenceHandlingLevel0Source input, MappaContext context);
}