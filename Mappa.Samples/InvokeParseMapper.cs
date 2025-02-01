// <copyright file="InvokeParseMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Attributes;

namespace Mappa.Samples;

#pragma warning disable SA1402 // File may only contain a single type

/// <summary>
/// Mapper using the string-to-number strategy.
/// </summary>
[Mappa]
public sealed partial class ParseNumericMapper
{
    /// <summary>
    /// Map a <see cref="string"/> to <see cref="sbyte"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial sbyte MapToSignedByte(string input);

    /// <summary>
    /// Map a <see cref="string"/> to <see cref="short"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial short MapToShort(string input);

    /// <summary>
    /// Map a <see cref="string"/> to <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial int MapToInteger(string input);

    /// <summary>
    /// Map a <see cref="string"/> to <see cref="long"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial long MapToLong(string input);

    /// <summary>
    /// Map a <see cref="string"/> to <see cref="byte"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial byte MapToByte(string input);

    /// <summary>
    /// Map a <see cref="string"/> to <see cref="ushort"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial ushort MapToUnsignedShort(string input);

    /// <summary>
    /// Map a <see cref="string"/> to <see cref="uint"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial uint MapToUnsignedInteger(string input);

    /// <summary>
    /// Map a <see cref="string"/> to <see cref="ulong"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial ulong MapToUnsignedLong(string input);

    /// <summary>
    /// Map a <see cref="string"/> to <see cref="decimal"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial decimal MapToDecimal(string input);

    /// <summary>
    /// Map a <see cref="string"/> to <see cref="float"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial float MapToFloat(string input);

    /// <summary>
    /// Map a <see cref="string"/> to <see cref="double"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial double MapToDouble(string input);
}

/// <summary>
/// Mapper mapping string to URI.
/// </summary>
[Mappa]
public sealed partial class ParseUriMapper
{
    /// <summary>
    /// Map <see cref="string"/> to <see cref="Uri"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    public partial Uri Map(string input);
}

/// <summary>
/// Mapper mapping string to some specific classes.
/// </summary>
[Mappa]
public sealed partial class ParseMapperWithoutAnySettings
{
    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    public partial DateTime MapDateTime(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    public partial DateTimeOffset MapDateTimeOffset(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    public partial DateOnly MapDateOnly(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="TimeOnly"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    public partial TimeOnly MapTimeOnly(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    public partial TimeSpan MapTimeSpan(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="Guid"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    public partial Guid MapGuid(string input);
}

// TODO String -> * : On Method : Format
// TODO String -> * : On Method : Format + Invariant culture
// TODO String -> * : On Method : Invariant culture
// TODO String -> * : On Method : Current culture
// TODO String -> * : On Method : Custom culture
// TODO String -> * : On Class : Format
// TODO String -> * : On Class : Format + Invariant culture
// TODO String -> * : On Class : Invariant culture
// TODO String -> * : On Class : Current culture
// TODO String -> * : On Class : Custom culture
// TODO String -> * : On Method override On Class