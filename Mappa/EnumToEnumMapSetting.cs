// <copyright file="EnumToEnumMapSetting.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa;

/// <summary>
/// Defines how enum-to-enum mappings pair source and target members.
/// </summary>
public enum EnumToEnumMapSetting
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
    /// Match enum members by underlying numeric value.
    /// </summary>
    NumericValue,
}