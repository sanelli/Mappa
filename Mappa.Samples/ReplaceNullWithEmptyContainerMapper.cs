// <copyright file="ReplaceNullWithEmptyContainerMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating <see cref="MappaSettingsAttribute.ReplaceNullWithEmptyContainer"/>.
/// </summary>
[Mappa]
[MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable)]
public sealed partial class ReplaceNullWithEmptyContainerMapper
{
    /// <summary>
    /// Map a nullable list to a non-nullable list, replacing a null source with an empty list.
    /// </summary>
    /// <param name="input">The source list.</param>
    /// <returns>The mapped list.</returns>
    public partial List<string> Map(List<int>? input);
}