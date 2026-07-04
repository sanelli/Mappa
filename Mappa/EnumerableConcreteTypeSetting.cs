// <copyright file="EnumerableConcreteTypeSetting.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa;

/// <summary>
/// Defines the concrete type used when mapping to sequence-like collection interfaces.
/// </summary>
public enum EnumerableConcreteTypeSetting
{
    /// <summary>
    /// Ignore the setting from the application of this attribute
    /// and use the value from a parent scope or global configuration.
    /// </summary>
    Undefined,

    /// <summary>
    /// Use <see cref="System.Collections.Generic.List{T}"/> as the concrete buffer.
    /// </summary>
    List,

    /// <summary>
    /// Use an array as the concrete buffer.
    /// </summary>
    Array,
}