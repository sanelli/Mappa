// <copyright file="EnumStringMapSetting.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa;

/// <summary>
/// Defines how enum and string mappings pair enum members with string values.
/// </summary>
public enum EnumStringMapSetting
{
    /// <summary>
    /// Ignore the setting from the application of this attribute
    /// and use the value from a parent scope or global configuration.
    /// </summary>
    Undefined,

    /// <summary>
    /// Match enum members by name.
    /// </summary>
    MemberName,

    /// <summary>
    /// Match enum members by <see cref="System.ComponentModel.DescriptionAttribute"/> value.
    /// </summary>
    Description,
}