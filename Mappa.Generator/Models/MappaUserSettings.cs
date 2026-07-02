// <copyright file="MappaUserSettings.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Attributes;
using Mappa.Generator.Helpers;

namespace Mappa.Generator.Models;

/// <summary>
/// Contains the settings specified by the user that can be used to perform a mapping.
/// </summary>
internal sealed class MappaUserSettings
 : IMappaUserSettings
{
    private readonly StackSetting<string?> dateTimeFormat;
    private readonly StackSetting<string?> dateTimeOffsetFormat;
    private readonly StackSetting<string?> dateOnlyFormat;
    private readonly StackSetting<string?> timeOnlyFormat;
    private readonly StackSetting<DateTimeStyles?> dateTimeStyle;
    private readonly StackSetting<DateTimeStyles?> dateTimeOffsetStyle;
    private readonly StackSetting<DateTimeStyles?> dateOnlyStyle;
    private readonly StackSetting<DateTimeStyles?> timeOnlyStyle;
    private readonly StackSetting<DateTimeStyles?> globalDateTimeStyle;
    private readonly StackSetting<string?> timeSpanFormat;
    private readonly StackSetting<string?> guidFormat;
    private readonly StackSetting<string?> byteFormat;
    private readonly StackSetting<string?> sByteFormat;
    private readonly StackSetting<string?> shortFormat;
    private readonly StackSetting<string?> uShortFormat;
    private readonly StackSetting<string?> intFormat;
    private readonly StackSetting<string?> uIntFormat;
    private readonly StackSetting<string?> longFormat;
    private readonly StackSetting<string?> uLongFormat;
    private readonly StackSetting<string?> decimalFormat;
    private readonly StackSetting<string?> floatFormat;
    private readonly StackSetting<string?> doubleFormat;
    private readonly StackSetting<NumberStyles?> byteStyle;
    private readonly StackSetting<NumberStyles?> sByteStyle;
    private readonly StackSetting<NumberStyles?> shortStyle;
    private readonly StackSetting<NumberStyles?> uShortStyle;
    private readonly StackSetting<NumberStyles?> intStyle;
    private readonly StackSetting<NumberStyles?> uIntStyle;
    private readonly StackSetting<NumberStyles?> longStyle;
    private readonly StackSetting<NumberStyles?> uLongStyle;
    private readonly StackSetting<NumberStyles?> decimalStyle;
    private readonly StackSetting<NumberStyles?> floatStyle;
    private readonly StackSetting<NumberStyles?> doubleStyle;
    private readonly StackSetting<NumberStyles?> globalNumberStyle;
    private readonly StackSetting<CultureInfoSetting> cultureInfoSetting;
    private readonly StackSetting<string?> cultureName;
    private readonly StackSetting<BooleanSetting> protobufOptional;
    private readonly StackSetting<PragmaWarningSetting> pragmaWarning;
    private readonly StackSetting<BooleanSetting> fastCollections;
    private readonly StackSetting<BooleanSetting> containerCapacityConstructors;
    private readonly StackSetting<BooleanSetting> polymorphicMapMethodWithMatchingDefaultAttribute;
    private readonly StackSetting<BooleanSetting> forceCaseInsensitivePropertyMap;
    private readonly StackSetting<BooleanSetting> ignoreUnderscoreForPropertyMap;
    private readonly StackSetting<BooleanSetting> caseInsensitiveStringToEnumMap;

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaUserSettings"/> class.
    /// </summary>
    /// <param name="otherSettings">The settings used to initialize the current settings.</param>
    internal MappaUserSettings(IMappaUserSettings otherSettings)
        : this(
            otherSettings.DateTimeFormat,
            otherSettings.DateTimeOffsetFormat,
            otherSettings.DateOnlyFormat,
            otherSettings.TimeOnlyFormat,
            otherSettings.DateTimeStyle,
            otherSettings.DateTimeOffsetStyle,
            otherSettings.DateOnlyStyle,
            otherSettings.TimeOnlyStyle,
            otherSettings.GlobalDateTimeStyle,
            otherSettings.TimeSpanFormat,
            otherSettings.GuidFormat,
            otherSettings.ByteFormat,
            otherSettings.SByteFormat,
            otherSettings.ShortFormat,
            otherSettings.UShortFormat,
            otherSettings.IntFormat,
            otherSettings.UIntFormat,
            otherSettings.LongFormat,
            otherSettings.ULongFormat,
            otherSettings.DecimalFormat,
            otherSettings.FloatFormat,
            otherSettings.DoubleFormat,
            otherSettings.ByteStyle,
            otherSettings.SByteStyle,
            otherSettings.ShortStyle,
            otherSettings.UShortStyle,
            otherSettings.IntStyle,
            otherSettings.UIntStyle,
            otherSettings.LongStyle,
            otherSettings.ULongStyle,
            otherSettings.DecimalStyle,
            otherSettings.FloatStyle,
            otherSettings.DoubleStyle,
            otherSettings.GlobalNumberStyle,
            otherSettings.CultureInfoSetting,
            otherSettings.CultureName,
            otherSettings.ProtobufOptional,
            otherSettings.PragmaWarning,
            otherSettings.FastCollections,
            otherSettings.ContainerCapacityConstructors,
            otherSettings.PolymorphicMapMethodWithMatchingDefaultAttribute,
            otherSettings.ForceCaseInsensitivePropertyMap,
            otherSettings.IgnoreUnderscoreForPropertyMap,
            otherSettings.CaseInsensitiveStringToEnumMap)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaUserSettings"/> class.
    /// </summary>
    /// <param name="dateTimeFormat">The default format for <see cref="DateTime"/>.</param>
    /// <param name="dateTimeOffsetFormat">The default format for <see cref="DateTimeOffset"/>.</param>
    /// <param name="dateOnlyFormat">The default format for DateOnly.</param>
    /// <param name="timeOnlyFormat">The default format for TimeOnly.</param>
    /// <param name="dateTimeStyle">The default <see cref="DateTimeStyles"/> for <see cref="DateTime"/>.</param>
    /// <param name="dateTimeOffsetStyle">The default <see cref="DateTimeStyles"/> for <see cref="DateTimeOffset"/>.</param>
    /// <param name="dateOnlyStyle">The default <see cref="DateTimeStyles"/> for DateOnly.</param>
    /// <param name="timeOnlyStyle">The default <see cref="DateTimeStyles"/> for TimeOnly.</param>
    /// <param name="globalDateTimeStyle">The default <see cref="DateTimeStyles"/> for all date/time types when type-specific style is unset.</param>
    /// <param name="timeSpanFormat">The default format for <see cref="TimeSpan"/>.</param>
    /// <param name="guidFormat">The default format for <see cref="Guid"/>.</param>
    /// <param name="byteFormat">The default format for <see cref="byte"/>.</param>
    /// <param name="sByteFormat">The default format for <see cref="sbyte"/>.</param>
    /// <param name="shortFormat">The default format for <see cref="short"/>.</param>
    /// <param name="uShortFormat">The default format for <see cref="ushort"/>.</param>
    /// <param name="intFormat">The default format for <see cref="int"/>.</param>
    /// <param name="uIntFormat">The default format for <see cref="uint"/>.</param>
    /// <param name="longFormat">The default format for <see cref="long"/>.</param>
    /// <param name="uLongFormat">The default format for <see cref="ulong"/>.</param>
    /// <param name="decimalFormat">The default format for <see cref="decimal"/>.</param>
    /// <param name="floatFormat">The default format for <see cref="float"/>.</param>
    /// <param name="doubleFormat">The default format for <see cref="double"/>.</param>
    /// <param name="byteStyle">The default <see cref="NumberStyles"/> for <see cref="byte"/>.</param>
    /// <param name="sByteStyle">The default <see cref="NumberStyles"/> for <see cref="sbyte"/>.</param>
    /// <param name="shortStyle">The default <see cref="NumberStyles"/> for <see cref="short"/>.</param>
    /// <param name="uShortStyle">The default <see cref="NumberStyles"/> for <see cref="ushort"/>.</param>
    /// <param name="intStyle">The default <see cref="NumberStyles"/> for <see cref="int"/>.</param>
    /// <param name="uIntStyle">The default <see cref="NumberStyles"/> for <see cref="uint"/>.</param>
    /// <param name="longStyle">The default <see cref="NumberStyles"/> for <see cref="long"/>.</param>
    /// <param name="uLongStyle">The default <see cref="NumberStyles"/> for <see cref="ulong"/>.</param>
    /// <param name="decimalStyle">The default <see cref="NumberStyles"/> for <see cref="decimal"/>.</param>
    /// <param name="floatStyle">The default <see cref="NumberStyles"/> for <see cref="float"/>.</param>
    /// <param name="doubleStyle">The default <see cref="NumberStyles"/> for <see cref="double"/>.</param>
    /// <param name="globalNumberStyle">The default <see cref="NumberStyles"/> for all numeric types when type-specific style is unset.</param>
    /// <param name="cultureInfoSetting">The type of culture info settings to be provided.</param>
    /// <param name="cultureName">The default culture info to use to generate a format provider.</param>
    /// <param name="protobufOptional">Enable or disable (protobuf) optional feature.</param>
    /// <param name="pragmaWarningSetting">Allow to surround the code generated with a <c>#pragma warning disable</c> block.</param>
    /// <param name="fastCollections">Enable or disable fast collection iterations for arrays and <see cref="List{T}"/> via <c>Span{T}</c>.</param>
    /// <param name="containerCapacityConstructors">Enable or disable the ability to support custom collection with capacity constructor.</param>
    /// <param name="polymorphicMapMethodWithMatchingDefaultAttribute">Enable or disable the support for <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/> when picking up a polymorphic method.</param>
    /// <param name="forceCaseInsensitivePropertyMap">Enable or disable case-insensitive matching when pairing a target member with a source property by name.</param>
    /// <param name="ignoreUnderscoreForPropertyMap">Enable or disable ignoring underscore characters when pairing a target member with a source property by name.</param>
    /// <param name="caseInsensitiveStringToEnumMap">Enable or disable case-insensitive matching when mapping from <see cref="string"/> to an enum.</param>
    private MappaUserSettings(
        string? dateTimeFormat,
        string? dateTimeOffsetFormat,
        string? dateOnlyFormat,
        string? timeOnlyFormat,
        DateTimeStyles? dateTimeStyle,
        DateTimeStyles? dateTimeOffsetStyle,
        DateTimeStyles? dateOnlyStyle,
        DateTimeStyles? timeOnlyStyle,
        DateTimeStyles? globalDateTimeStyle,
        string? timeSpanFormat,
        string? guidFormat,
        string? byteFormat,
        string? sByteFormat,
        string? shortFormat,
        string? uShortFormat,
        string? intFormat,
        string? uIntFormat,
        string? longFormat,
        string? uLongFormat,
        string? decimalFormat,
        string? floatFormat,
        string? doubleFormat,
        NumberStyles? byteStyle,
        NumberStyles? sByteStyle,
        NumberStyles? shortStyle,
        NumberStyles? uShortStyle,
        NumberStyles? intStyle,
        NumberStyles? uIntStyle,
        NumberStyles? longStyle,
        NumberStyles? uLongStyle,
        NumberStyles? decimalStyle,
        NumberStyles? floatStyle,
        NumberStyles? doubleStyle,
        NumberStyles? globalNumberStyle,
        CultureInfoSetting cultureInfoSetting,
        string? cultureName,
        BooleanSetting protobufOptional,
        PragmaWarningSetting pragmaWarningSetting,
        BooleanSetting fastCollections,
        BooleanSetting containerCapacityConstructors,
        BooleanSetting polymorphicMapMethodWithMatchingDefaultAttribute,
        BooleanSetting forceCaseInsensitivePropertyMap,
        BooleanSetting ignoreUnderscoreForPropertyMap,
        BooleanSetting caseInsensitiveStringToEnumMap)
    {
        this.dateTimeFormat = new(dateTimeFormat);
        this.dateTimeOffsetFormat = new(dateTimeOffsetFormat);
        this.dateOnlyFormat = new(dateOnlyFormat);
        this.timeOnlyFormat = new(timeOnlyFormat);
        this.dateTimeStyle = new(dateTimeStyle);
        this.dateTimeOffsetStyle = new(dateTimeOffsetStyle);
        this.dateOnlyStyle = new(dateOnlyStyle);
        this.timeOnlyStyle = new(timeOnlyStyle);
        this.globalDateTimeStyle = new(globalDateTimeStyle);
        this.timeSpanFormat = new(timeSpanFormat);
        this.guidFormat = new(guidFormat);
        this.byteFormat = new(byteFormat);
        this.sByteFormat = new(sByteFormat);
        this.shortFormat = new(shortFormat);
        this.uShortFormat = new(uShortFormat);
        this.intFormat = new(intFormat);
        this.uIntFormat = new(uIntFormat);
        this.longFormat = new(longFormat);
        this.uLongFormat = new(uLongFormat);
        this.decimalFormat = new(decimalFormat);
        this.floatFormat = new(floatFormat);
        this.doubleFormat = new(doubleFormat);
        this.byteStyle = new(byteStyle);
        this.sByteStyle = new(sByteStyle);
        this.shortStyle = new(shortStyle);
        this.uShortStyle = new(uShortStyle);
        this.intStyle = new(intStyle);
        this.uIntStyle = new(uIntStyle);
        this.longStyle = new(longStyle);
        this.uLongStyle = new(uLongStyle);
        this.decimalStyle = new(decimalStyle);
        this.floatStyle = new(floatStyle);
        this.doubleStyle = new(doubleStyle);
        this.globalNumberStyle = new(globalNumberStyle);
        this.cultureInfoSetting = new(cultureInfoSetting);
        this.cultureName = new(cultureName);
        this.protobufOptional = new(protobufOptional);
        this.pragmaWarning = new(pragmaWarningSetting);
        this.fastCollections = new(fastCollections);
        this.containerCapacityConstructors = new(containerCapacityConstructors);
        this.polymorphicMapMethodWithMatchingDefaultAttribute = new(polymorphicMapMethodWithMatchingDefaultAttribute);
        this.forceCaseInsensitivePropertyMap = new(forceCaseInsensitivePropertyMap);
        this.ignoreUnderscoreForPropertyMap = new(ignoreUnderscoreForPropertyMap);
        this.caseInsensitiveStringToEnumMap = new(caseInsensitiveStringToEnumMap);
    }

    /// <inheritdoc />
    public string? DateTimeFormat => this.dateTimeFormat;

    /// <inheritdoc />
    public string? DateTimeOffsetFormat => this.dateTimeOffsetFormat;

    /// <inheritdoc />
    public string? DateOnlyFormat => this.dateOnlyFormat;

    /// <inheritdoc />
    public string? TimeOnlyFormat => this.timeOnlyFormat;

    /// <inheritdoc />
    public DateTimeStyles? DateTimeStyle => this.dateTimeStyle;

    /// <inheritdoc />
    public DateTimeStyles? DateTimeOffsetStyle => this.dateTimeOffsetStyle;

    /// <inheritdoc />
    public DateTimeStyles? DateOnlyStyle => this.dateOnlyStyle;

    /// <inheritdoc />
    public DateTimeStyles? TimeOnlyStyle => this.timeOnlyStyle;

    /// <inheritdoc />
    public DateTimeStyles? GlobalDateTimeStyle => this.globalDateTimeStyle;

    /// <inheritdoc />
    public string? TimeSpanFormat => this.timeSpanFormat;

    /// <inheritdoc />
    public string? GuidFormat => this.guidFormat;

    /// <inheritdoc />
    public string? ByteFormat => this.byteFormat;

    /// <inheritdoc />
    public string? SByteFormat => this.sByteFormat;

    /// <inheritdoc />
    public string? ShortFormat => this.shortFormat;

    /// <inheritdoc />
    public string? UShortFormat => this.uShortFormat;

    /// <inheritdoc />
    public string? IntFormat => this.intFormat;

    /// <inheritdoc />
    public string? UIntFormat => this.uIntFormat;

    /// <inheritdoc />
    public string? LongFormat => this.longFormat;

    /// <inheritdoc />
    public string? ULongFormat => this.uLongFormat;

    /// <inheritdoc />
    public string? DecimalFormat => this.decimalFormat;

    /// <inheritdoc />
    public string? FloatFormat => this.floatFormat;

    /// <inheritdoc />
    public string? DoubleFormat => this.doubleFormat;

    /// <inheritdoc />
    public NumberStyles? ByteStyle => this.byteStyle;

    /// <inheritdoc />
    public NumberStyles? SByteStyle => this.sByteStyle;

    /// <inheritdoc />
    public NumberStyles? ShortStyle => this.shortStyle;

    /// <inheritdoc />
    public NumberStyles? UShortStyle => this.uShortStyle;

    /// <inheritdoc />
    public NumberStyles? IntStyle => this.intStyle;

    /// <inheritdoc />
    public NumberStyles? UIntStyle => this.uIntStyle;

    /// <inheritdoc />
    public NumberStyles? LongStyle => this.longStyle;

    /// <inheritdoc />
    public NumberStyles? ULongStyle => this.uLongStyle;

    /// <inheritdoc />
    public NumberStyles? DecimalStyle => this.decimalStyle;

    /// <inheritdoc />
    public NumberStyles? FloatStyle => this.floatStyle;

    /// <inheritdoc />
    public NumberStyles? DoubleStyle => this.doubleStyle;

    /// <inheritdoc />
    public NumberStyles? GlobalNumberStyle => this.globalNumberStyle;

    /// <inheritdoc />
    public CultureInfoSetting CultureInfoSetting => this.cultureInfoSetting;

    /// <inheritdoc />
    public string? CultureName => this.cultureName;

    /// <inheritdoc/>
    public BooleanSetting ProtobufOptional => this.protobufOptional;

    /// <inheritdoc/>
    public PragmaWarningSetting PragmaWarning => this.pragmaWarning;

    /// <inheritdoc/>
    public BooleanSetting FastCollections => this.fastCollections;

    /// <inheritdoc/>
    public BooleanSetting ContainerCapacityConstructors => this.containerCapacityConstructors;

    /// <inheritdoc/>
    public BooleanSetting PolymorphicMapMethodWithMatchingDefaultAttribute => this.polymorphicMapMethodWithMatchingDefaultAttribute;

    /// <inheritdoc/>
    public BooleanSetting ForceCaseInsensitivePropertyMap => this.forceCaseInsensitivePropertyMap;

    /// <inheritdoc/>
    public BooleanSetting IgnoreUnderscoreForPropertyMap => this.ignoreUnderscoreForPropertyMap;

    /// <inheritdoc/>
    public BooleanSetting CaseInsensitiveStringToEnumMap => this.caseInsensitiveStringToEnumMap;

    /// <summary>
    /// Push the changes required by the <paramref name="mappaSettingsAttribute"/> on the stack.
    /// If <paramref name="mappaSettingsAttribute"/> is <c>null</c>
    /// no setting is applied and disposable won't have any effect.
    /// </summary>
    /// <param name="mappaSettingsAttribute">The settings to apply.</param>
    /// <returns>A disposable object that will pop the values from the stack.</returns>
    internal IDisposable Apply(MappaSettingsAttribute? mappaSettingsAttribute)
    {
        if (mappaSettingsAttribute is null)
        {
            return new NoActionDisposable();
        }

        return new PopActionDisposable(
        [
#pragma warning disable CA2000 // Call System. IDisposable. Dispose on object created by '...' before all references to it are out of scope
            this.dateTimeFormat.Apply(mappaSettingsAttribute.DateTimeFormat ?? this.dateTimeFormat),
            this.dateTimeOffsetFormat.Apply(mappaSettingsAttribute.DateTimeOffsetFormat ?? this.dateTimeOffsetFormat),
            this.dateOnlyFormat.Apply(mappaSettingsAttribute.DateOnlyFormat ?? this.dateOnlyFormat),
            this.timeOnlyFormat.Apply(mappaSettingsAttribute.TimeOnlyFormat ?? this.timeOnlyFormat),
            this.dateTimeStyle.Apply(GetDateTimeStyle(mappaSettingsAttribute.DateTimeStyle, this.dateTimeStyle)),
            this.dateTimeOffsetStyle.Apply(GetDateTimeStyle(mappaSettingsAttribute.DateTimeOffsetStyle, this.dateTimeOffsetStyle)),
            this.dateOnlyStyle.Apply(GetDateTimeStyle(mappaSettingsAttribute.DateOnlyStyle, this.dateOnlyStyle)),
            this.timeOnlyStyle.Apply(GetDateTimeStyle(mappaSettingsAttribute.TimeOnlyStyle, this.timeOnlyStyle)),
            this.globalDateTimeStyle.Apply(GetDateTimeStyle(mappaSettingsAttribute.GlobalDateTimeStyle, this.globalDateTimeStyle)),
            this.timeSpanFormat.Apply(mappaSettingsAttribute.TimeSpanFormat ?? this.timeSpanFormat),
            this.guidFormat.Apply(mappaSettingsAttribute.GuidFormat ?? this.guidFormat),
            this.byteFormat.Apply(mappaSettingsAttribute.ByteFormat ?? this.byteFormat),
            this.sByteFormat.Apply(mappaSettingsAttribute.SByteFormat ?? this.sByteFormat),
            this.shortFormat.Apply(mappaSettingsAttribute.ShortFormat ?? this.shortFormat),
            this.uShortFormat.Apply(mappaSettingsAttribute.UShortFormat ?? this.uShortFormat),
            this.intFormat.Apply(mappaSettingsAttribute.IntFormat ?? this.intFormat),
            this.uIntFormat.Apply(mappaSettingsAttribute.UIntFormat ?? this.uIntFormat),
            this.longFormat.Apply(mappaSettingsAttribute.LongFormat ?? this.longFormat),
            this.uLongFormat.Apply(mappaSettingsAttribute.ULongFormat ?? this.uLongFormat),
            this.decimalFormat.Apply(mappaSettingsAttribute.DecimalFormat ?? this.decimalFormat),
            this.floatFormat.Apply(mappaSettingsAttribute.FloatFormat ?? this.floatFormat),
            this.doubleFormat.Apply(mappaSettingsAttribute.DoubleFormat ?? this.doubleFormat),
            this.byteStyle.Apply(GetNumberStyle(mappaSettingsAttribute.ByteStyle, this.byteStyle)),
            this.sByteStyle.Apply(GetNumberStyle(mappaSettingsAttribute.SByteStyle, this.sByteStyle)),
            this.shortStyle.Apply(GetNumberStyle(mappaSettingsAttribute.ShortStyle, this.shortStyle)),
            this.uShortStyle.Apply(GetNumberStyle(mappaSettingsAttribute.UShortStyle, this.uShortStyle)),
            this.intStyle.Apply(GetNumberStyle(mappaSettingsAttribute.IntStyle, this.intStyle)),
            this.uIntStyle.Apply(GetNumberStyle(mappaSettingsAttribute.UIntStyle, this.uIntStyle)),
            this.longStyle.Apply(GetNumberStyle(mappaSettingsAttribute.LongStyle, this.longStyle)),
            this.uLongStyle.Apply(GetNumberStyle(mappaSettingsAttribute.ULongStyle, this.uLongStyle)),
            this.decimalStyle.Apply(GetNumberStyle(mappaSettingsAttribute.DecimalStyle, this.decimalStyle)),
            this.floatStyle.Apply(GetNumberStyle(mappaSettingsAttribute.FloatStyle, this.floatStyle)),
            this.doubleStyle.Apply(GetNumberStyle(mappaSettingsAttribute.DoubleStyle, this.doubleStyle)),
            this.globalNumberStyle.Apply(GetNumberStyle(mappaSettingsAttribute.GlobalNumberStyle, this.globalNumberStyle)),
            this.cultureInfoSetting.Apply(mappaSettingsAttribute.CultureInfoSetting is not CultureInfoSetting.Undefined ? mappaSettingsAttribute.CultureInfoSetting : this.cultureInfoSetting),
            this.cultureName.Apply(mappaSettingsAttribute.CultureName ?? this.cultureName),
            this.protobufOptional.Apply(mappaSettingsAttribute.ProtobufOptional is not BooleanSetting.Undefined ? mappaSettingsAttribute.ProtobufOptional : this.protobufOptional),
            this.pragmaWarning.Apply(mappaSettingsAttribute.PragmaWarning is not PragmaWarningSetting.Undefined ? mappaSettingsAttribute.PragmaWarning : this.pragmaWarning),
            this.fastCollections.Apply(mappaSettingsAttribute.FastCollections is not BooleanSetting.Undefined ? mappaSettingsAttribute.FastCollections : this.fastCollections),
            this.containerCapacityConstructors.Apply(mappaSettingsAttribute.ContainerCapacityConstructors is not BooleanSetting.Undefined ? mappaSettingsAttribute.ContainerCapacityConstructors : this.containerCapacityConstructors),
            this.polymorphicMapMethodWithMatchingDefaultAttribute.Apply(mappaSettingsAttribute.PolymorphicMapMethodWithMatchingDefaultAttribute is not BooleanSetting.Undefined ? mappaSettingsAttribute.PolymorphicMapMethodWithMatchingDefaultAttribute : this.polymorphicMapMethodWithMatchingDefaultAttribute),
            this.forceCaseInsensitivePropertyMap.Apply(mappaSettingsAttribute.ForceCaseInsensitivePropertyMap is not BooleanSetting.Undefined ? mappaSettingsAttribute.ForceCaseInsensitivePropertyMap : this.forceCaseInsensitivePropertyMap),
            this.ignoreUnderscoreForPropertyMap.Apply(mappaSettingsAttribute.IgnoreUnderscoreForPropertyMap is not BooleanSetting.Undefined ? mappaSettingsAttribute.IgnoreUnderscoreForPropertyMap : this.ignoreUnderscoreForPropertyMap),
            this.caseInsensitiveStringToEnumMap.Apply(mappaSettingsAttribute.CaseInsensitiveStringToEnumMap is not BooleanSetting.Undefined ? mappaSettingsAttribute.CaseInsensitiveStringToEnumMap : this.caseInsensitiveStringToEnumMap),
 #pragma warning restore CA2000
        ]);
    }

    private static DateTimeStyles? GetDateTimeStyle(DateTimeStyles style, StackSetting<DateTimeStyles?> currentStyle)
        => style != MappaSettingsAttribute.UndefinedDateTimeStyle ? style : currentStyle;

    private static NumberStyles? GetNumberStyle(NumberStyles style, StackSetting<NumberStyles?> currentStyle)
        => style != MappaSettingsAttribute.UndefinedNumberStyle ? style : currentStyle;

    private sealed class PopActionDisposable
        : IDisposable
    {
        private readonly IEnumerable<IDisposable> disposables;

        internal PopActionDisposable(IEnumerable<IDisposable> disposables)
        {
            this.disposables = disposables;
        }

        public void Dispose()
        {
            foreach (var disposable in this.disposables)
            {
                disposable.Dispose();
            }
        }
    }

    private sealed class NoActionDisposable
        : IDisposable
    {
        public void Dispose()
        {
            // Nothing to dispose.
        }
    }
}