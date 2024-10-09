// <copyright file="DateAndTimeMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples;

/// <summary>
/// Mapper for date and time related mappings
/// not covered by the <see cref="StringToSystemEntitiesMapper"/>.
/// </summary>
[Mappa]
public sealed partial class DateAndTimeMapper
{
    /// <summary>
    /// Map <see cref="DateTime"/> to <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="input">The input <see cref="DateTime"/> value.</param>
    /// <returns>The mapped <see cref="DateOnly"/> value.</returns>
    public partial DateOnly MapDateTimeToDateOnly(DateTime input);

    /// <summary>
    /// Map <see cref="DateTime"/> to <see cref="TimeOnly"/>.
    /// </summary>
    /// <param name="input">The input <see cref="DateTime"/> value.</param>
    /// <returns>The mapped <see cref="TimeOnly"/> value.</returns>
    public partial TimeOnly MapDateTimeToTimeOnly(DateTime input);

    /// <summary>
    /// Map <see cref="DateTime"/> to <see cref="long"/>.
    /// </summary>
    /// <param name="input">The input <see cref="DateTime"/> value.</param>
    /// <returns>The mapped <see cref="long"/> value.</returns>
    public partial long MapDateTimeToLong(DateTime input);

    /// <summary>
    /// Map <see cref="DateOnly"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input <see cref="DateOnly"/> value.</param>
    /// <returns>The mapped <see cref="DateTime"/> value.</returns>
    public partial DateTime MapDateOnlyToDateTime(DateOnly input);
}