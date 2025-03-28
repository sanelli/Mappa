// <copyright file="ReferenceNullableToReferenceNullableMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

#pragma warning disable SA1402

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
}

/// <summary>
/// Mapper for non-nullable reference type to value type.
/// </summary>
[Mappa]
public sealed partial class ReferenceToValueTypeNullableMapper
{
    /// <summary>
    /// Map from <see cref="string"/> to <see cref="int"/>.
    /// </summary>
    /// <param name="input">The source value.</param>
    /// <returns>the mapper value.</returns>
    public partial int MapToInteger(string input);

    /// <summary>
    /// Map from <see cref="string"/> to <see cref="Nullable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The source value.</param>
    /// <returns>the mapper value.</returns>
    public partial int? MapToNullableInteger(string input);
}

/// <summary>
/// Mapper for nullable reference type to value type.
/// </summary>
[Mappa]
public sealed partial class NullableReferenceToValueTypeNullableMapper
{
    /// <summary>
    /// Map from <see cref="string"/> to <see cref="int"/>.
    /// </summary>
    /// <param name="input">The source value.</param>
    /// <returns>the mapper value</returns>
    public partial int MapToInteger(string? input);

    /// <summary>
    /// Map from <see cref="string"/> to <see cref="Nullable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The source value.</param>
    /// <returns>the mapper value.</returns>
    public partial int? MapToNullableInteger(string? input);
}