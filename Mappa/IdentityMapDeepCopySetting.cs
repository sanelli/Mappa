// <copyright file="IdentityMapDeepCopySetting.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa;

/// <summary>
/// Defines how identity mappings copy a type to itself.
/// </summary>
public enum IdentityMapDeepCopySetting
{
    /// <summary>
    /// Ignore the setting from the application of this attribute
    /// and use the value from a parent scope or global configuration.
    /// </summary>
    Undefined,

    /// <summary>
    /// Return the original reference without cloning.
    /// </summary>
    ShallowCopy,

    /// <summary>
    /// Create a new instance via <see cref="object.MemberwiseClone"/> without recursively copying nested references.
    /// </summary>
    DeepCopy,

    /// <summary>
    /// Create a new instance and recursively copy nested fields.
    /// </summary>
    NestedDeepCopy,
}