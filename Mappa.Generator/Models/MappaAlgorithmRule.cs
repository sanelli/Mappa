// <copyright file="MappaAlgorithmRule.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models;

/// <summary>
/// Describe the rule used by the algorithm to generate a mappa.
/// </summary>
public enum MappaAlgorithmRule
{
    /// <summary>
    /// No rule is being applied.
    /// </summary>
    None,

    /// <summary>
    /// Source and target have the same type.
    /// </summary>
    MapToSameType,

    /// <summary>
    /// Target type is <see cref="object"/>.
    /// </summary>
    MapToObject,

    /// <summary>
    /// The mapping is performed using an existing method.
    /// </summary>
    MapUsingExistingMethod,

    /// <summary>
    /// There is an implicit conversion that can be used to perform the mapping.
    /// </summary>
    ImplicitConversion,

    /// <summary>
    /// Convert an enum to a string.
    /// </summary>
    EnumToString,

    /// <summary>
    /// Convert an enum to a integral numeric value.
    /// </summary>
    EnumToIntegral,

    /// <summary>
    /// Convert a string to enum value.
    /// </summary>
    StringToEnum,

    /// <summary>
    /// Convert an integral to an enum value.
    /// </summary>
    IntegralToEnum,

    /// <summary>
    /// Convert an enum to another enum.
    /// </summary>
    EnumToEnum,

    /// <summary>
    /// Map a string to a number value.
    /// </summary>
    StringToNumber,

    /// <summary>
    /// Map a string to a date-time.
    /// </summary>
    StringToDateTime,

    /// <summary>
    /// Map a string to a date-time.
    /// </summary>
    InvokeToString,

    /// <summary>
    /// Map a nullable struct to a nullable struct.
    /// </summary>
    NullableToNullable,

    /// <summary>
    /// Map an array to an array.
    /// </summary>
    ArrayToArray,

    /// <summary>
    /// Map an array, or <see cref="IList{T}"/> or <see cref="List{T}"/>
    /// to any collection (excluding array).
    /// </summary>
    ArrayOrListToCollection,

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/>, or <see cref="ICollection{T}"/> or <see cref="IReadOnlyCollection{T}"/>
    /// to any collection (excluding array).
    /// </summary>
    EnumerableOrCollectionToCollection,
}