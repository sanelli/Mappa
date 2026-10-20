// <copyright file="ConstructorType.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa;

/// <summary>
/// Selects which kind of constructor the source generator should use when constructing a target type.
/// </summary>
public enum ConstructorType
{
    /// <summary>
    /// Use an accessible parameterless constructor (empty constructor), then map settable properties when applicable.
    /// </summary>
    NoParameters,

    /// <summary>
    /// Use a constructor that takes one or more parameters. When no parameter types are specified on
    /// <see cref="Attributes.MappaUseConstructorAttribute"/>, exactly one such constructor must exist;
    /// otherwise the parameter types identify the exact signature.
    /// </summary>
    WithParameters,
}