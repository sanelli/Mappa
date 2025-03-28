// <copyright file="ReferenceToReferenceWithNullableDisabledMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

#nullable disable
#pragma warning disable SA1402

/// <summary>
/// Mapper using nullable-to-nullable strategy.
/// </summary>
[Mappa]
public sealed partial class ReferenceToReferenceWithNullableDisabledMapper
{
    /// <summary>
    /// Map a nullable reference type to a nullable reference type.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The integer mapped from the value.</returns>
    public partial TargetClassModel Map(SourceClassModel input);
}

/// <summary>
/// Mapper from reference type to value type
/// when nullable is deisabled.
/// </summary>
[Mappa]
public sealed partial class ReferenceToValueTypeWithNullableDisabledMapper
{
    /// <summary>
    /// Map from <see cref="string"/> to <see cref="int"/>.
    /// </summary>
    /// <param name="input">The source value.</param>
    /// <returns>the mapper value</returns>
    public partial int MapToInteger(string input);

    /// <summary>
    /// Map from <see cref="string"/> to <see cref="Nullable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The source value.</param>
    /// <returns>the mapper value</returns>
    public partial int? MapToNullableInteger(string input);
}