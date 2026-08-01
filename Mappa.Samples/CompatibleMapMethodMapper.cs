// <copyright file="CompatibleMapMethodMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating <see cref="MappaSettingsAttribute.CompatibleMapMethod"/>:
/// nested mapping from a derived source to a base target reuses a hand-written method
/// that accepts the base source and returns a derived target.
/// </summary>
[Mappa]
[MappaSettings(CompatibleMapMethod = BooleanSetting.Enable)]
public sealed partial class CompatibleMapMethodMapper
{
    /// <summary>
    /// Compatible hand-written map from <see cref="CompatibleMapMethodBaseSource"/>
    /// to <see cref="CompatibleMapMethodDerivedTarget"/>. The nested property mapping
    /// from <see cref="CompatibleMapMethodDerivedSource"/> to <see cref="CompatibleMapMethodBaseTarget"/>
    /// reuses this method when <see cref="MappaSettingsAttribute.CompatibleMapMethod"/> is enabled.
    /// </summary>
    /// <param name="source">The base source.</param>
    /// <returns>The derived target with an offset applied so reuse is observable at runtime.</returns>
    public static CompatibleMapMethodDerivedTarget MapInner(CompatibleMapMethodBaseSource source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new CompatibleMapMethodDerivedTarget
        {
            Value = source.Value + 100,
            Label = "mapped",
        };
    }

    /// <summary>
    /// Map <see cref="CompatibleMapMethodSource"/> to <see cref="CompatibleMapMethodTarget"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    public partial CompatibleMapMethodTarget Map(CompatibleMapMethodSource source);
}