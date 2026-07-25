// <copyright file="EnumMapLegKind.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models;

/// <summary>
/// Describes which enum mapping leg is being resolved.
/// </summary>
internal enum EnumMapLegKind
{
    /// <summary>
    /// An enum is mapped to another enum.
    /// </summary>
    EnumToEnum,

    /// <summary>
    /// An enum is mapped to a <see cref="string"/>.
    /// </summary>
    EnumToString,

    /// <summary>
    /// An enum is mapped to an integral type.
    /// </summary>
    EnumToIntegral,

    /// <summary>
    /// A <see cref="string"/> is mapped to an enum.
    /// </summary>
    StringToEnum,

    /// <summary>
    /// An integral type is mapped to an enum.
    /// </summary>
    IntegralToEnum,
}