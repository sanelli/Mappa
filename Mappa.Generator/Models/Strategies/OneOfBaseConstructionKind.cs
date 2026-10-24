// <copyright file="OneOfBaseConstructionKind.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Describes how to construct a <c>OneOfBase</c> (or derived) target from a wrapped arm value.
/// </summary>
internal enum OneOfBaseConstructionKind
{
    /// <summary>
    /// Use a public implicit conversion from the arm type to the target.
    /// </summary>
    ImplicitFromArm,

    /// <summary>
    /// Use <c>OneOf.FromTx</c> then a public constructor accepting <c>OneOf&lt;…&gt;</c>.
    /// </summary>
    ConstructorFromOneOf,
}