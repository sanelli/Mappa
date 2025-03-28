// <copyright file="ReferenceNullableToReferenceNullableMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using nullable-to-nullable strategy.
/// </summary>
[Mappa]
public sealed partial class ReferenceNullableToReferenceNullableMapper
{
    /// <summary>
    /// Map a nullable reference type to a nullable reference type.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The integer mapped from the value.</returns>
    public partial TargetClassModel? MapReferenceNullableToReferenceNullable(SourceClassModel? input);

    /// <summary>
    /// Map a non-nullable reference type to a nullable reference type.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The integer mapped from the value.</returns>
    public partial TargetClassModel? MapToReferenceNullable(SourceClassModel input);

    /// <summary>
    /// Map a nullable reference type to a non-nullable reference type.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The integer mapped from the value.</returns>
    public partial TargetClassModel MapFromReferenceNullable(SourceClassModel? input);

    // TODO [#106] string? -> int
    // TODO [#106] string? -> int?
    // TODO [#106] string -> int
    // TODO [#106] string -> int?
}