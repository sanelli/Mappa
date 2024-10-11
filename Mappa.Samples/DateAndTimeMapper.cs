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

    /// <summary>
    /// Map <see cref="long"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input <see cref="long"/> value.</param>
    /// <returns>The mapped <see cref="DateTime"/> value.</returns>
    public partial DateTime MapLongToDateTime(long input);

    /// <summary>
    /// Map <see cref="uint"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input <see cref="uint"/> value.</param>
    /// <returns>The mapped <see cref="DateTime"/> value.</returns>
    public partial DateTime MapUintToDateTime(uint input);

    /// <summary>
    /// Map <see cref="int"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input <see cref="int"/> value.</param>
    /// <returns>The mapped <see cref="DateTime"/> value.</returns>
    public partial DateTime MapIntToDateTime(int input);

    /// <summary>
    /// Map <see cref="ushort"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input <see cref="ushort"/> value.</param>
    /// <returns>The mapped <see cref="DateTime"/> value.</returns>
    public partial DateTime MapUShortToDateTime(ushort input);

    /// <summary>
    /// Map <see cref="short"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input <see cref="short"/> value.</param>
    /// <returns>The mapped <see cref="DateTime"/> value.</returns>
    public partial DateTime MapShortToDateTime(short input);

    /// <summary>
    /// Map <see cref="sbyte"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input <see cref="sbyte"/> value.</param>
    /// <returns>The mapped <see cref="DateTime"/> value.</returns>
    public partial DateTime MapSByteToDateTime(sbyte input);

    /// <summary>
    /// Map <see cref="byte"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input <see cref="byte"/> value.</param>
    /// <returns>The mapped <see cref="DateTime"/> value.</returns>
    public partial DateTime MapByteToDateTime(byte input);

    /// <summary>
    /// Map <see cref="DateOnly"/> to <see cref="long"/>.
    /// </summary>
    /// <param name="input">The input <see cref="DateOnly"/> value.</param>
    /// <returns>The mapped <see cref="long"/> value.</returns>
    public partial long MapDateOnlyToLong(DateOnly input);

    /// <summary>
    /// Map <see cref="TimeSpan"/> to <see cref="double"/>.
    /// </summary>
    /// <param name="input">The input <see cref="TimeSpan"/> value.</param>
    /// <returns>The mapped <see cref="double"/> value.</returns>
    public partial double MapTimeSpanToDouble(TimeSpan input);

    /// <summary>
    /// Map <see cref="double"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input <see cref="double"/> value.</param>
    /// <returns>The mapped <see cref="TimeSpan"/> value.</returns>
    public partial TimeSpan MapDoubleToTimeSpan(double input);

    /// <summary>
    /// Map <see cref="float"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input <see cref="float"/> value.</param>
    /// <returns>The mapped <see cref="TimeSpan"/> value.</returns>
    public partial TimeSpan MapFloatToTimeSpan(float input);

    /// <summary>
    /// Map <see cref="ulong"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input <see cref="ulong"/> value.</param>
    /// <returns>The mapped <see cref="TimeSpan"/> value.</returns>
    public partial TimeSpan MapULongToTimeSpan(ulong input);

    /// <summary>
    /// Map <see cref="long"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input <see cref="long"/> value.</param>
    /// <returns>The mapped <see cref="TimeSpan"/> value.</returns>
    public partial TimeSpan MapLongToTimeSpan(long input);

    /// <summary>
    /// Map <see cref="uint"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input <see cref="uint"/> value.</param>
    /// <returns>The mapped <see cref="TimeSpan"/> value.</returns>
    public partial TimeSpan MapUintToTimeSpan(uint input);

    /// <summary>
    /// Map <see cref="int"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input <see cref="int"/> value.</param>
    /// <returns>The mapped <see cref="TimeSpan"/> value.</returns>
    public partial TimeSpan MapIntToTimeSpan(int input);

    /// <summary>
    /// Map <see cref="ushort"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input <see cref="ushort"/> value.</param>
    /// <returns>The mapped <see cref="TimeSpan"/> value.</returns>
    public partial TimeSpan MapUShortToTimeSpan(ushort input);

    /// <summary>
    /// Map <see cref="short"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input <see cref="short"/> value.</param>
    /// <returns>The mapped <see cref="TimeSpan"/> value.</returns>
    public partial TimeSpan MapShortToTimeSpan(short input);

    /// <summary>
    /// Map <see cref="sbyte"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input <see cref="sbyte"/> value.</param>
    /// <returns>The mapped <see cref="TimeSpan"/> value.</returns>
    public partial TimeSpan MapSByteToTimeSpan(sbyte input);

    /// <summary>
    /// Map <see cref="byte"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input <see cref="byte"/> value.</param>
    /// <returns>The mapped <see cref="TimeSpan"/> value.</returns>
    public partial TimeSpan MapByteToTimeSpan(byte input);

    /// <summary>
    /// Map <see cref="DateTimeOffset"/> to <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="input">The input <see cref="DateTime"/> value.</param>
    /// <returns>The mapped <see cref="DateOnly"/> value.</returns>
    public partial DateOnly MapDateTimeOffsetToDateOnly(DateTimeOffset input);

    /// <summary>
    /// Map <see cref="DateTimeOffset"/> to <see cref="TimeOnly"/>.
    /// </summary>
    /// <param name="input">The input <see cref="DateTime"/> value.</param>
    /// <returns>The mapped <see cref="TimeOnly"/> value.</returns>
    public partial TimeOnly MapDateTimeOffsetToTimeOnly(DateTimeOffset input);

    /// <summary>
    /// Map <see cref="DateTimeOffset"/> to <see cref="long"/>.
    /// </summary>
    /// <param name="input">The input <see cref="DateTime"/> value.</param>
    /// <returns>The mapped <see cref="long"/> value.</returns>
    public partial long MapDateTimeOffsetToLong(DateTimeOffset input);

     /// <summary>
    /// Map <see cref="long"/> to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="input">The input <see cref="long"/> value.</param>
    /// <returns>The mapped <see cref="DateTimeOffset"/> value.</returns>
    public partial DateTimeOffset MapLongToDateTimeOffset(long input);

    /// <summary>
    /// Map <see cref="uint"/> to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="input">The input <see cref="uint"/> value.</param>
    /// <returns>The mapped <see cref="DateTimeOffset"/> value.</returns>
    public partial DateTimeOffset MapUintToDateTimeOffset(uint input);

    /// <summary>
    /// Map <see cref="int"/> to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="input">The input <see cref="int"/> value.</param>
    /// <returns>The mapped <see cref="DateTimeOffset"/> value.</returns>
    public partial DateTimeOffset MapIntToDateTimeOffset(int input);

    /// <summary>
    /// Map <see cref="ushort"/> to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="input">The input <see cref="ushort"/> value.</param>
    /// <returns>The mapped <see cref="DateTimeOffset"/> value.</returns>
    public partial DateTimeOffset MapUShortToDateTimeOffset(ushort input);

    /// <summary>
    /// Map <see cref="short"/> to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="input">The input <see cref="short"/> value.</param>
    /// <returns>The mapped <see cref="DateTimeOffset"/> value.</returns>
    public partial DateTimeOffset MapShortToDateTimeOffset(short input);

    /// <summary>
    /// Map <see cref="sbyte"/> to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="input">The input <see cref="sbyte"/> value.</param>
    /// <returns>The mapped <see cref="DateTimeOffset"/> value.</returns>
    public partial DateTimeOffset MapSByteToDateTimeOffset(sbyte input);

    /// <summary>
    /// Map <see cref="byte"/> to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="input">The input <see cref="byte"/> value.</param>
    /// <returns>The mapped <see cref="DateTimeOffset"/> value.</returns>
    public partial DateTimeOffset MapByteToDateTimeOffset(byte input);

    /// <summary>
    /// Map <see cref="DateTimeOffset"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input <see cref="DateTimeOffset"/> value.</param>
    /// <returns>The mapped <see cref="DateTime"/> value.</returns>
    public partial DateTime MapDateTimeOffsetToDateTime(DateTimeOffset input);
}