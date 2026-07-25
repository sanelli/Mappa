// <copyright file="MappaMapEnumDefaultBehavior.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa;

/// <summary>
/// Describes the fallback behaviour when an enum value cannot be mapped
/// (for example because it was excluded via <see cref="MappaMapEnumIgnoreAttribute{TEnum}"/>
/// or because no pairing exists for that member).
/// </summary>
public enum MappaMapEnumDefaultBehavior
{
    /// <summary>
    /// Throw an exception when the value cannot be mapped.
    /// This is the default behaviour and preserves existing generated code.
    /// </summary>
    Throw,

    /// <summary>
    /// Use the default value provided via <see cref="MappaMapEnumDefaultAttribute{TEnum}"/>
    /// when the value cannot be mapped.
    /// </summary>
    UseDefaultValue,
}