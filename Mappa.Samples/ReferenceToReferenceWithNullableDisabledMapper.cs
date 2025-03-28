// <copyright file="ReferenceToReferenceWithNullableDisabledMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

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
    #nullable  disable
    public partial TargetClassModel Map(SourceClassModel input);
    #nullable restore

    // TODO [#106] string -> int
    // TODO [#106] string -> int?
}