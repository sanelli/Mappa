// <copyright file="BooleanSetting.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa;

/// <summary>
/// Enable or disable a specific setting.
/// </summary>
public enum BooleanSetting
{
    /// <summary>
    /// Ignore the setting from the application of this.
    /// </summary>
    Undefined,

    /// <summary>
    /// Enable the feature.
    /// </summary>
    Enable,

    /// <summary>
    /// Disable the feature.
    /// </summary>
    Disable,
}