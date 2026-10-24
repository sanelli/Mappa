// <copyright file="OneOfOptionWrapBehavior.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Describes how to wrap a value into a <c>OneOf&lt;T, None&gt;</c> (Option).
/// </summary>
internal enum OneOfOptionWrapBehavior
{
    /// <summary>
    /// Always emit <c>FromT0</c> / Some.
    /// </summary>
    AlwaysSome,

    /// <summary>
    /// Emit Some when the reference source is not null; otherwise <c>None</c>.
    /// </summary>
    SomeWhenNotNull,

    /// <summary>
    /// Emit Some when a <c>Nullable&lt;T&gt;</c> source has a value; otherwise <c>None</c>.
    /// </summary>
    SomeWhenHasValue,

    /// <summary>
    /// Emit Some when a non-nullable value-type source is not <c>default</c>; otherwise <c>None</c>.
    /// </summary>
    SomeWhenNotDefault,
}