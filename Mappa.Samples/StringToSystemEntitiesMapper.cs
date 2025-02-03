// <copyright file="StringToSystemEntitiesMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Attributes;

namespace Mappa.Samples;

// TODO [#56] Add tests for DateTimeOffset
// TODO [#56] Add tests using various combinations of MappaSettings.

/// <summary>
/// Mapper using the strategies from string to other system entities.
/// </summary>
[Mappa]
public sealed partial class StringToSystemEntitiesMapper
{
    /// <summary>
    /// Map a string to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped <see cref="DateTimeKind"/> value.</returns>
    public partial DateTime MapToDateTime(string input);

    /// <summary>
    /// Map a string to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped <see cref="TimeSpan"/> value.</returns>
    public partial TimeSpan MapToTimeSpan(string input);

    /// <summary>
    /// Map a string to <see cref="TimeOnly"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped <see cref="TimeOnly"/> value.</returns>
    public partial TimeOnly MapToTimeOnly(string input);

    /// <summary>
    /// Map a string to <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped <see cref="DateOnly"/> value.</returns>
    public partial DateOnly MapToDateOnly(string input);

    /// <summary>
    /// Map a string to <see cref="Guid"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped <see cref="Guid"/> value.</returns>
    public partial Guid MapToGuid(string input);

    /// <summary>
    /// Map a string to <see cref="Uri"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped <see cref="Uri"/> value.</returns>
    public partial Uri MapToUri(string input);
}