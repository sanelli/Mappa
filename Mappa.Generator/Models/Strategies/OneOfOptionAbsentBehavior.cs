// <copyright file="OneOfOptionAbsentBehavior.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Describes how to map a <c>OneOf&lt;T, None&gt;</c> (Option) when the value is <c>None</c>.
/// </summary>
internal enum OneOfOptionAbsentBehavior
{
    /// <summary>
    /// Throw <c>MappaException</c> at runtime.
    /// </summary>
    Throw,

    /// <summary>
    /// Return <c>null</c> (nullable reference or <c>Nullable&lt;T&gt;</c> target).
    /// </summary>
    ReturnNull,

    /// <summary>
    /// Return <c>default</c> for the target value type, or <c>null</c> for a reference type when NRT is disabled.
    /// </summary>
    ReturnDefault,
}