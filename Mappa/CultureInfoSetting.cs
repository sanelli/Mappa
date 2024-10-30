// <copyright file="CultureInfoSetting.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Attributes;

namespace Mappa;

/// <summary>
/// The type of <see cref="CultureInfo"/> to apply.
/// </summary>
public enum CultureInfoSetting
{
    /// <summary>
    /// Ignore the setting from the application of this
    /// (similar to applying a <c>null</c> to a format).
    /// This mean this setting will be ignored and previous
    /// settings will be accepted.
    /// </summary>
    Undefined,

    /// <summary>
    /// Do not apply any culture.
    /// (Similar to applying an empty string to a format).
    /// </summary>
    None,

    /// <summary>
    /// Use the <see cref="CultureInfo.CurrentCulture"/>.
    /// </summary>
    CurrentCulture,

    /// <summary>
    /// Use the <see cref="CultureInfo.CurrentCulture"/>.
    /// </summary>
    InvariantCulture,

    /// <summary>
    /// Allow to user to specify a culture setting via <see cref="MappaSettingsAttribute.CultureName"/>.
    /// </summary>
    UserDefined,
}