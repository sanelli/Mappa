// <copyright file="InvokeToStringMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Attributes;

namespace Mappa.Samples;

#pragma warning disable SA1402 // File may only contain a single type

/// <summary>
/// Mapper using the invoke-to-string strategy with no other settings.
/// No other setting is applied.
/// </summary>
[Mappa]
public sealed partial class InvokeToStringMapper
{
    /// <summary>
    /// Map <see cref="int"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    public partial string MapInt(int input);

    /// <summary>
    /// Map <see cref="DateTime"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    public partial string MapDateTime(DateTime input);

    /// <summary>
    /// Map <see cref="DateTimeOffset"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    public partial string MapDateTimeOffset(DateTimeOffset input);

    /// <summary>
    /// Map <see cref="DateOnly"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    public partial string MapDateOnly(DateOnly input);

    /// <summary>
    /// Map <see cref="TimeOnly"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    public partial string MapTimeOnly(TimeOnly input);

    /// <summary>
    /// Map <see cref="TimeSpan"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    public partial string MapTimeSpan(TimeSpan input);

    /// <summary>
    /// Map <see cref="Guid"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    public partial string MapGuid(Guid input);
}

/// <summary>
/// Common settings used for the invoke to string strategy.
/// </summary>
 #pragma warning disable SA1204
public static class InvokeToStringStrategySettings
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
/// Mapper using the invoke-to-string strategy with string format.
/// Settings are applied on the method.
/// </summary>
[Mappa]
public sealed partial class InvokeToStringMapperWithFormatSettingsOnMethod
{
    /// <summary>
    /// Map <see cref="DateTime"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(DateTimeFormat = InvokeToStringStrategySettings.DateTimeFormat)]
    public partial string MapDateTime(DateTime input);

    /// <summary>
    /// Map <see cref="DateTimeOffset"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(DateTimeOffsetFormat = InvokeToStringStrategySettings.DateTimeOffsetFormat)]
    public partial string MapDateTimeOffset(DateTimeOffset input);

    /// <summary>
    /// Map <see cref="DateOnly"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(DateOnlyFormat = InvokeToStringStrategySettings.DateOnlyFormat)]
    public partial string MapDateOnly(DateOnly input);

    /// <summary>
    /// Map <see cref="TimeOnly"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(TimeOnlyFormat = InvokeToStringStrategySettings.TimeOnlyFormat)]
    public partial string MapTimeOnly(TimeOnly input);

    /// <summary>
    /// Map <see cref="TimeSpan"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(TimeSpanFormat = InvokeToStringStrategySettings.TimeSpanFormat)]
    public partial string MapTimeSpan(TimeSpan input);

    /// <summary>
    /// Map <see cref="Guid"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(GuidFormat = InvokeToStringStrategySettings.GuidFormat)]
    public partial string MapGuid(Guid input);
}

/// <summary>
/// Mapper using the invoke-to-string strategy with string format and invariant culture.
/// Settings are applied on the method.
/// </summary>
[Mappa]
public sealed partial class InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod
{
    /// <summary>
    /// Map <see cref="DateTime"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(DateTimeFormat = InvokeToStringStrategySettings.DateTimeFormat, CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial string MapDateTime(DateTime input);

    /// <summary>
    /// Map <see cref="DateTimeOffset"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(DateTimeOffsetFormat = InvokeToStringStrategySettings.DateTimeOffsetFormat, CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial string MapDateTimeOffset(DateTimeOffset input);

    /// <summary>
    /// Map <see cref="DateOnly"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(DateOnlyFormat = InvokeToStringStrategySettings.DateOnlyFormat, CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial string MapDateOnly(DateOnly input);

    /// <summary>
    /// Map <see cref="TimeOnly"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(TimeOnlyFormat = InvokeToStringStrategySettings.TimeOnlyFormat, CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial string MapTimeOnly(TimeOnly input);

    /// <summary>
    /// Map <see cref="TimeSpan"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(TimeSpanFormat = InvokeToStringStrategySettings.TimeSpanFormat, CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial string MapTimeSpan(TimeSpan input);

    /// <summary>
    /// Map <see cref="Guid"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(GuidFormat = InvokeToStringStrategySettings.GuidFormat, CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial string MapGuid(Guid input);
}

/// <summary>
/// Mapper using the invoke-to-string strategy with invariant culture.
/// Settings are applied on the method.
/// </summary>
[Mappa]
public sealed partial class InvokeToStringMapperWithInvariantCultureSettingsOnMethod
{
    /// <summary>
    /// Map <see cref="DateTime"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial string MapDateTime(DateTime input);

    /// <summary>
    /// Map <see cref="DateTimeOffset"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial string MapDateTimeOffset(DateTimeOffset input);

    /// <summary>
    /// Map <see cref="DateOnly"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial string MapDateOnly(DateOnly input);

    /// <summary>
    /// Map <see cref="TimeOnly"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial string MapTimeOnly(TimeOnly input);

    /// <summary>
    /// Map <see cref="TimeSpan"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial string MapTimeSpan(TimeSpan input);

    /// <summary>
    /// Map <see cref="Guid"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial string MapGuid(Guid input);
}

/// <summary>
/// Mapper using the invoke-to-string strategy with current culture.
/// Settings are applied on the method.
/// </summary>
[Mappa]
public sealed partial class InvokeToStringMapperWithCurrentCultureSettingsOnMethod
{
    /// <summary>
    /// Map <see cref="DateTime"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial string MapDateTime(DateTime input);

    /// <summary>
    /// Map <see cref="DateTimeOffset"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial string MapDateTimeOffset(DateTimeOffset input);

    /// <summary>
    /// Map <see cref="DateOnly"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial string MapDateOnly(DateOnly input);

    /// <summary>
    /// Map <see cref="TimeOnly"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial string MapTimeOnly(TimeOnly input);

    /// <summary>
    /// Map <see cref="TimeSpan"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial string MapTimeSpan(TimeSpan input);

    /// <summary>
    /// Map <see cref="Guid"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial string MapGuid(Guid input);
}

/// <summary>
/// Mapper using the invoke-to-string strategy with user defined culture.
/// Settings are applied on the method.
/// </summary>
[Mappa]
public sealed partial class InvokeToStringMapperWithCustomCultureSettingsOnMethod
{
    /// <summary>
    /// Map <see cref="DateTime"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = InvokeToStringStrategySettings.CultureName)]
    public partial string MapDateTime(DateTime input);

    /// <summary>
    /// Map <see cref="DateTimeOffset"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = InvokeToStringStrategySettings.CultureName)]
    public partial string MapDateTimeOffset(DateTimeOffset input);

    /// <summary>
    /// Map <see cref="DateOnly"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = InvokeToStringStrategySettings.CultureName)]
    public partial string MapDateOnly(DateOnly input);

    /// <summary>
    /// Map <see cref="TimeOnly"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = InvokeToStringStrategySettings.CultureName)]
    public partial string MapTimeOnly(TimeOnly input);

    /// <summary>
    /// Map <see cref="TimeSpan"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = InvokeToStringStrategySettings.CultureName)]
    public partial string MapTimeSpan(TimeSpan input);

    /// <summary>
    /// Map <see cref="Guid"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="input">The value to convert to string.</param>
    /// <returns>The string mapped from the input.</returns>
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = InvokeToStringStrategySettings.CultureName)]
    public partial string MapGuid(Guid input);
}

// TODO [#56] DateTime -> String format on class
// TODO [#56] DateTime -> String format and invariant culture on class
// TODO [#56] DateTime -> String invariant culture on class
// TODO [#56] DateTime -> String current culture on class
// TODO [#56] DateTime -> String custom culture on class
// TODO [#56] DateTime -> String format and invariant culture on method overriding class