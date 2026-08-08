// <copyright file="BreakCompileTimeCyclesMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating <see cref="MappaSettingsAttribute.BreakCompileTimeCycles"/>:
/// a single root map over a Person↔Address cycle without dedicated <c>MapPerson</c>/<c>MapAddress</c>
/// methods. Paired with <see cref="MappaSettingsAttribute.ReferenceReusing"/> so closed cycles
/// terminate at runtime by reusing already-mapped instances.
/// </summary>
[Mappa]
[MappaSettings(
    BreakCompileTimeCycles = BooleanSetting.Enable,
    ReferenceReusing = BooleanSetting.Enable)]
public sealed partial class BreakCompileTimeCyclesMapper
{
    /// <summary>
    /// Maps a person graph. The generator synthesizes a private map method for the cycling
    /// nested type pair (see MP00078).
    /// </summary>
    /// <param name="input">The source person.</param>
    /// <param name="context">The mapping context that owns the reference manager.</param>
    /// <returns>The mapped person.</returns>
#pragma warning disable MP00078 // Intentional: BreakCompileTimeCycles synthesizes a private map method
    public partial ReferenceHandlingPersonTarget Map(ReferenceHandlingPersonSource input, MappaContext context);
#pragma warning restore MP00078
}