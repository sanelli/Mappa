// <copyright file="StringToSystemEntitiesMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Attributes;

namespace Mappa.Samples;

#pragma warning disable SA1402 // File may only contain a single type

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
    /// <returns>The mapped <see cref="DateTime"/> value.</returns>
    public partial DateTime MapToDateTime(string input);

    /// <summary>
    /// Map a string to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped <see cref="DateTimeOffset"/> value.</returns>
    public partial DateTimeOffset MapToDateTimeOffset(string input);

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

/// <summary>
/// Common settings used for the invoke the parse methods.
/// </summary>
#pragma warning disable SA1204
public static class StringToSystemEntitiesSettings
#pragma warning restore SA1204
{
    /// <summary>
    /// The <see cref="DateTimeOffset"/> format applied.
    /// </summary>
    public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.ffffff";

    /// <summary>
    /// The <see cref="DateTimeOffset"/> format applied.
    /// </summary>
    public const string DateTimeOffsetFormat = "yyyy-MM-dd HH:mm:ss.fffffff zzz";

    /// <summary>
    /// The <see cref="DateOnly"/> format applied.
    /// </summary>
    public const string DateOnlyFormat = "yyyy+MM+dd";

    /// <summary>
    /// The <see cref="TimeOnly"/> format applied.
    /// </summary>
    public const string TimeOnlyFormat = "HH+mm+ss.fffffff";

    /// <summary>
    /// The <see cref="TimeSpan"/> format applied.
    /// </summary>
    public const string TimeSpanFormat = "G";

    /// <summary>
    /// The <see cref="Guid"/> format applied.
    /// </summary>
    public const string GuidFormat = "N";
}

/// <summary>
/// Mapper from string to system entities that also applies.
/// </summary>
[Mappa]
public sealed partial class StringToSystemEntitiesWithSettingsMapper
{
    /// <summary>
    /// Map a string to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped <see cref="DateTime"/> value.</returns>
    [MappaSettings(DateTimeFormat = StringToSystemEntitiesSettings.DateTimeFormat, CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial DateTime MapToDateTime(string input);

    /// <summary>
    /// Map a string to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped <see cref="DateTimeOffset"/> value.</returns>
    [MappaSettings(DateTimeOffsetFormat = StringToSystemEntitiesSettings.DateTimeOffsetFormat, CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial DateTimeOffset MapToDateTimeOffset(string input);

    /// <summary>
    /// Map a string to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped <see cref="TimeSpan"/> value.</returns>
    [MappaSettings(TimeSpanFormat = StringToSystemEntitiesSettings.TimeSpanFormat, CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial TimeSpan MapToTimeSpan(string input);

    /// <summary>
    /// Map a string to <see cref="TimeOnly"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped <see cref="TimeOnly"/> value.</returns>
    [MappaSettings(TimeOnlyFormat = StringToSystemEntitiesSettings.TimeOnlyFormat, CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial TimeOnly MapToTimeOnly(string input);

    /// <summary>
    /// Map a string to <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped <see cref="DateOnly"/> value.</returns>
    [MappaSettings(DateOnlyFormat = StringToSystemEntitiesSettings.DateOnlyFormat, CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
    public partial DateOnly MapToDateOnly(string input);

    /// <summary>
    /// Map a string to <see cref="Guid"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped <see cref="Guid"/> value.</returns>
    [MappaSettings(GuidFormat = StringToSystemEntitiesSettings.GuidFormat)]
    public partial Guid MapToGuid(string input);
}