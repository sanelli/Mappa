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

/// <summary>
/// Common settings used for the invoke parse strategy.
/// </summary>
 #pragma warning disable SA1204
public static class InvokeParseStrategySettings
 #pragma warning restore SA1204
{
    /// <summary>
    /// The <see cref="DateTimeOffset"/> format applied.
    /// </summary>
    public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

    /// <summary>
    /// The <see cref="DateTimeOffset"/> format applied.
    /// </summary>
    public const string DateTimeOffsetFormat = "dd-MM-yyyy ss:mm:HH";

    /// <summary>
    /// The <see cref="DateOnly"/> format applied.
    /// </summary>
    public const string DateOnlyFormat = "yyyy+MM+dd";

    /// <summary>
    /// The <see cref="TimeOnly"/> format applied.
    /// </summary>
    public const string TimeOnlyFormat = "HH+mm+ss";

    /// <summary>
    /// The <see cref="TimeSpan"/> format applied.
    /// </summary>
    public const string TimeSpanFormat = "G";

    /// <summary>
    /// The <see cref="Guid"/> format applied.
    /// </summary>
    public const string GuidFormat = "N";

    /// <summary>
    /// The culture name to be applied.
    /// </summary>
    public const string CultureName = "it-IT";
}

/// <summary>
/// Mapper mapping string for specific classes with format.
/// </summary>
[Mappa]
public sealed partial class ParseMapperWithFormatSettingsOnMethod
{
    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(DateTimeFormat = InvokeParseStrategySettings.DateTimeFormat)]
    public partial DateTime MapDateTime(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(DateTimeOffsetFormat = InvokeParseStrategySettings.DateTimeOffsetFormat)]
    public partial DateTimeOffset MapDateTimeOffset(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(DateOnlyFormat = InvokeParseStrategySettings.DateOnlyFormat)]
    public partial DateOnly MapDateOnly(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="TimeOnly"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(TimeOnlyFormat = InvokeParseStrategySettings.TimeOnlyFormat)]
    public partial TimeOnly MapTimeOnly(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(TimeSpanFormat = InvokeParseStrategySettings.TimeSpanFormat)]
    public partial TimeSpan MapTimeSpan(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="Guid"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(GuidFormat = InvokeParseStrategySettings.GuidFormat)]
    public partial Guid MapGuid(string input);
}

/// <summary>
/// Mapper mapping string for specific classes with format and invariant culture.
/// </summary>
[Mappa]
public sealed partial class ParseMapperWithFormatAndInvariantCultureSettingsOnMethod
{
    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(DateTimeFormat = InvokeParseStrategySettings.DateTimeFormat, CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial DateTime MapDateTime(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(DateTimeOffsetFormat = InvokeParseStrategySettings.DateTimeOffsetFormat, CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial DateTimeOffset MapDateTimeOffset(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(DateOnlyFormat = InvokeParseStrategySettings.DateOnlyFormat, CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial DateOnly MapDateOnly(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="TimeOnly"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(TimeOnlyFormat = InvokeParseStrategySettings.TimeOnlyFormat, CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial TimeOnly MapTimeOnly(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(TimeSpanFormat = InvokeParseStrategySettings.TimeSpanFormat, CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial TimeSpan MapTimeSpan(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="Guid"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(GuidFormat = InvokeParseStrategySettings.GuidFormat, CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial Guid MapGuid(string input);
}

/// <summary>
/// Mapper mapping string for specific classes with invariant culture.
/// </summary>
[Mappa]
public sealed partial class ParseMapperWithInvariantCultureSettingsOnMethod
{
    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial DateTime MapDateTime(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial DateTimeOffset MapDateTimeOffset(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial DateOnly MapDateOnly(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="TimeOnly"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial TimeOnly MapTimeOnly(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial TimeSpan MapTimeSpan(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="Guid"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial Guid MapGuid(string input);
}

/// <summary>
/// Mapper mapping string for specific classes with current culture.
/// </summary>
[Mappa]
public sealed partial class ParseMapperWithCurrentCultureSettingsOnMethod
{
    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial DateTime MapDateTime(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial DateTimeOffset MapDateTimeOffset(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial DateOnly MapDateOnly(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="TimeOnly"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial TimeOnly MapTimeOnly(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial TimeSpan MapTimeSpan(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="Guid"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial Guid MapGuid(string input);
}

/// <summary>
/// Mapper mapping string for specific classes with custom culture.
/// </summary>
[Mappa]
public sealed partial class ParseMapperWithCustomCultureSettingsOnMethod
{
    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = InvokeParseStrategySettings.CultureName)]
    public partial DateTime MapDateTime(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = InvokeParseStrategySettings.CultureName)]
    public partial DateTimeOffset MapDateTimeOffset(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = InvokeParseStrategySettings.CultureName)]
    public partial DateOnly MapDateOnly(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="TimeOnly"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = InvokeParseStrategySettings.CultureName)]
    public partial TimeOnly MapTimeOnly(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = InvokeParseStrategySettings.CultureName)]
    public partial TimeSpan MapTimeSpan(string input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="Guid"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input mapped to the target type.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = InvokeParseStrategySettings.CultureName)]
    public partial Guid MapGuid(string input);
}

// TODO String -> * : On Class : Format
// TODO String -> * : On Class : Format + Invariant culture
// TODO String -> * : On Class : Invariant culture
// TODO String -> * : On Class : Current culture
// TODO String -> * : On Class : Custom culture
// TODO String -> * : On Method override On Class