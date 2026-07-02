// <copyright file="MappaGlobalOptions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Generator.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mappa.Generator.Models;

/// <summary>
/// Global options of the mapper as read from the .editorconfig.
/// Values used are:
/// <list type="bullet">
///     <item>
///         <term><c>mappa.debug</c></term>
///         <description>Enable the report of debugging messages when value is equal to <c>true</c>.</description>
///     </item>
///     <item>
///         <term><c>mappa.debugcomments</c></term>
///         <description>Enable the report of debugging comments in the generated code when value is equal to <c>true</c>.</description>
///     </item>
///     <item>
///         <term><c>mappa.datetimeformat</c></term>
///         <description>Default format to be used for parsing strings and converting to string <see cref="DateTime"/> <c>struct</c>s.</description>
///     </item>
///     <item>
///         <term><c>mappa.datetimeoffsetformat</c></term>
///         <description>Default format to be used for parsing strings and converting to string <see cref="DateTimeOffset"/> <c>struct</c>s.</description>
///     </item>
///     <item>
///         <term><c>mappa.dateonlyformat</c></term>
///         <description>Default format to be used for parsing strings and converting to string <c>DateOnly</c> <c>struct</c>s.</description>
///     </item>
    ///     <item>
    ///         <term><c>mappa.timeonlyformat</c></term>
    ///         <description>Default format to be used for parsing strings and converting to string <c>TimeOnly</c> <c>struct</c>s.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.datetimestyle</c></term>
    ///         <description>Default <see cref="DateTimeStyles"/> to be used when parsing strings to <see cref="DateTime"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.datetimeoffsetstyle</c></term>
    ///         <description>Default <see cref="DateTimeStyles"/> to be used when parsing strings to <see cref="DateTimeOffset"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.dateonlystyle</c></term>
    ///         <description>Default <see cref="DateTimeStyles"/> to be used when parsing strings to <c>DateOnly</c>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.timeonlystyle</c></term>
    ///         <description>Default <see cref="DateTimeStyles"/> to be used when parsing strings to <c>TimeOnly</c>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.globaldatetimestyle</c></term>
    ///         <description>Default <see cref="DateTimeStyles"/> for all date/time types when the type-specific style is unset.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.timespanformat</c></term>
///         <description>Default format to be used for parsing strings and converting to string <see cref="TimeSpan"/> <c>struct</c>s.</description>
///     </item>
    ///     <item>
    ///         <term><c>mappa.guidformat</c></term>
    ///         <description>Default format to be used for parsing strings and converting to string <see cref="Guid"/>  <c>struct</c>s.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.byteformat</c></term>
    ///         <description>Default format to be used when converting <see cref="byte"/> values to <see cref="string"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.sbyteformat</c></term>
    ///         <description>Default format to be used when converting <see cref="sbyte"/> values to <see cref="string"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.shortformat</c></term>
    ///         <description>Default format to be used when converting <see cref="short"/> values to <see cref="string"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.ushortformat</c></term>
    ///         <description>Default format to be used when converting <see cref="ushort"/> values to <see cref="string"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.intformat</c></term>
    ///         <description>Default format to be used when converting <see cref="int"/> values to <see cref="string"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.uintformat</c></term>
    ///         <description>Default format to be used when converting <see cref="uint"/> values to <see cref="string"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.longformat</c></term>
    ///         <description>Default format to be used when converting <see cref="long"/> values to <see cref="string"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.ulongformat</c></term>
    ///         <description>Default format to be used when converting <see cref="ulong"/> values to <see cref="string"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.decimalformat</c></term>
    ///         <description>Default format to be used when converting <see cref="decimal"/> values to <see cref="string"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.floatformat</c></term>
    ///         <description>Default format to be used when converting <see cref="float"/> values to <see cref="string"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.doubleformat</c></term>
    ///         <description>Default format to be used when converting <see cref="double"/> values to <see cref="string"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.bytestyle</c></term>
    ///         <description>Default <see cref="NumberStyles"/> to be used when parsing strings to <see cref="byte"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.sbytestyle</c></term>
    ///         <description>Default <see cref="NumberStyles"/> to be used when parsing strings to <see cref="sbyte"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.shortstyle</c></term>
    ///         <description>Default <see cref="NumberStyles"/> to be used when parsing strings to <see cref="short"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.ushortstyle</c></term>
    ///         <description>Default <see cref="NumberStyles"/> to be used when parsing strings to <see cref="ushort"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.intstyle</c></term>
    ///         <description>Default <see cref="NumberStyles"/> to be used when parsing strings to <see cref="int"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.uintstyle</c></term>
    ///         <description>Default <see cref="NumberStyles"/> to be used when parsing strings to <see cref="uint"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.longstyle</c></term>
    ///         <description>Default <see cref="NumberStyles"/> to be used when parsing strings to <see cref="long"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.ulongstyle</c></term>
    ///         <description>Default <see cref="NumberStyles"/> to be used when parsing strings to <see cref="ulong"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.decimalstyle</c></term>
    ///         <description>Default <see cref="NumberStyles"/> to be used when parsing strings to <see cref="decimal"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.floatstyle</c></term>
    ///         <description>Default <see cref="NumberStyles"/> to be used when parsing strings to <see cref="float"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.doublestyle</c></term>
    ///         <description>Default <see cref="NumberStyles"/> to be used when parsing strings to <see cref="double"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.globalnumberstyle</c></term>
    ///         <description>Default <see cref="NumberStyles"/> for all numeric types when the type-specific style is unset.</description>
    ///     </item>
    ///     <item>
    ///         <term><c>mappa.cultureinfosettings</c></term>
///         <description>Set the default culture info settings. Valid values are the values of the <see cref="CultureInfoSetting"/> <c>enum</c>.</description>
///     </item>
///     <item>
///         <term><c>mappa.culturename</c></term>
///         <description>The name of the default culture to be applied.</description>
///     </item>
///     <item>
///         <term><c>mappa.protobufoptional</c></term>
///         <description>Set the default value to enable or disable the (protobuf) optional setting. Valid values are the values from the <see cref="BooleanSetting"/> <c>enum</c>.</description>
///     </item>
///     <item>
///         <term><c>mappa.pragmawarning</c></term>
///         <description>Set the default value disable or not apply the <c>#pragma warning</c> around the mapping method. Valid values are the values from the <see cref="PragmaWarningSetting"/> <c>enum</c>.</description>
///     </item>
///     <item>
///         <term><c>mappa.fastcollections</c></term>
///         <description>Set the default value to enable or disable the fast iteration for arrays and list using <c>span{T}</c>.</description>
///     </item>
///     <item>
///         <term><c>mappa.containercapacityconstructors</c></term>
///         <description>Set the default value to enable or disable the support for mapping custom containers using (if available) a constructor accepting an integer value representing the initial capacity of the container.</description>
///     </item>
///     <item>
///         <term><c>mappa.polymorphicmapmethodwithmatchingdefaultattribute</c></term>
///         <description>Set the default value to enable or disable the support for <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/> when picking up a polymorphic method.</description>
///     </item>
///     <item>
///         <term><c>mappa.caseinsensitivepropertymap</c></term>
///         <description>Set the default value to enable or disable case-insensitive matching when pairing a target property or constructor parameter with a source property by name.</description>
///     </item>
///     <item>
///         <term><c>mappa.ignoreunderscoreforpropertymap</c></term>
///         <description>Set the default value to enable or disable ignoring underscore characters when pairing a target property or constructor parameter with a source property by name.</description>
///     </item>
///     <item>
///         <term><c>mappa.caseinsensitivestringtoenummap</c></term>
///         <description>Set the default value to enable or disable case-insensitive matching when mapping from <see cref="string"/> to an enum.</description>
///     </item>
/// </list>
/// </summary>
internal sealed class MappaGlobalOptions
    : IMappaUserSettings
{
    private const string MappaDebugFlagName = "debug";
    private const string MappaDebugCommentsFlagName = "debugcomments";

    private const string MappaSettingsDateTimeFormat = "datetimeformat";
    private const string MappaSettingsDateTimeOffsetFormat = "datetimeoffsetformat";
    private const string MappaSettingsDateOnlyFormat = "dateonlyformat";
    private const string MappaSettingsTimeOnlyFormat = "timeonlyformat";
    private const string MappaSettingsDateTimeStyle = "datetimestyle";
    private const string MappaSettingsDateTimeOffsetStyle = "datetimeoffsetstyle";
    private const string MappaSettingsDateOnlyStyle = "dateonlystyle";
    private const string MappaSettingsTimeOnlyStyle = "timeonlystyle";
    private const string MappaSettingsGlobalDateTimeStyle = "globaldatetimestyle";
    private const string MappaSettingsTimeSpanFormat = "timespanformat";
    private const string MappaSettingsGuidFormat = "guidformat";
    private const string MappaSettingsByteFormat = "byteformat";
    private const string MappaSettingsSByteFormat = "sbyteformat";
    private const string MappaSettingsShortFormat = "shortformat";
    private const string MappaSettingsUShortFormat = "ushortformat";
    private const string MappaSettingsIntFormat = "intformat";
    private const string MappaSettingsUIntFormat = "uintformat";
    private const string MappaSettingsLongFormat = "longformat";
    private const string MappaSettingsULongFormat = "ulongformat";
    private const string MappaSettingsDecimalFormat = "decimalformat";
    private const string MappaSettingsFloatFormat = "floatformat";
    private const string MappaSettingsDoubleFormat = "doubleformat";
    private const string MappaSettingsByteStyle = "bytestyle";
    private const string MappaSettingsSByteStyle = "sbytestyle";
    private const string MappaSettingsShortStyle = "shortstyle";
    private const string MappaSettingsUShortStyle = "ushortstyle";
    private const string MappaSettingsIntStyle = "intstyle";
    private const string MappaSettingsUIntStyle = "uintstyle";
    private const string MappaSettingsLongStyle = "longstyle";
    private const string MappaSettingsULongStyle = "ulongstyle";
    private const string MappaSettingsDecimalStyle = "decimalstyle";
    private const string MappaSettingsFloatStyle = "floatstyle";
    private const string MappaSettingsDoubleStyle = "doublestyle";
    private const string MappaSettingsGlobalNumberStyle = "globalnumberstyle";
    private const string MappaSettingsCultureInfoSettings = "cultureinfosettings";
    private const string MappaSettingsCultureName = "culturename";
    private const string MappaSettingsProtobufOptional = "protobufoptional";
    private const string MappaSettingsPragmaWarning = "pragmawarning";
    private const string MappaSettingsFastCollections = "fastcollections";
    private const string MappaSettingsContainerCapacityConstructors = "containercapacityconstructors";
    private const string MappaSettingsPolymorphicMapMethodWithMatchingDefaultAttribute = "polymorphicmapmethodwithmatchingdefaultattribute";
    private const string MappaSettingsCaseInsensitivePropertyMap = "caseinsensitivepropertymap";
    private const string MappaSettingsLegacyForceCaseInsensitivePropertyMap = "forcecaseinsensitivepropertymap";
    private const string MappaSettingsIgnoreUnderscoreForPropertyMap = "ignoreunderscoreforpropertymap";
    private const string MappaSettingsCaseInsensitiveStringToEnumMap = "caseinsensitivestringtoenummap";

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaGlobalOptions"/> class.
    /// </summary>
    /// <param name="analyzerConfigOptionsProvider">The analyzer configuration options.</param>
    /// <param name="syntaxTree">The syntax tree for which obtain the configuration.</param>
    public MappaGlobalOptions(AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider, SyntaxTree syntaxTree)
    {
        var options = analyzerConfigOptionsProvider.GetOptions(syntaxTree);

        this.MappaDebug = options.TryGetValue(GetOptionName(MappaDebugFlagName), out var mappaDebug)
                          && !string.IsNullOrWhiteSpace(mappaDebug)
                          && "true".Equals(mappaDebug, StringComparison.OrdinalIgnoreCase);

        this.MappaDebugComments =
            options.TryGetValue(GetOptionName(MappaDebugCommentsFlagName), out var mappaDebugComments)
            && !string.IsNullOrWhiteSpace(mappaDebugComments)
            && "true".Equals(mappaDebugComments, StringComparison.OrdinalIgnoreCase);

        this.DateTimeFormat = options.TryGetValue(GetOptionName(MappaSettingsDateTimeFormat), out var dateTimeFormat)
                              && !string.IsNullOrWhiteSpace(dateTimeFormat)
            ? dateTimeFormat
            : null;

        this.DateTimeOffsetFormat = options.TryGetValue(GetOptionName(MappaSettingsDateTimeOffsetFormat), out var dateTimeOffsetFormat)
                              && !string.IsNullOrWhiteSpace(dateTimeOffsetFormat)
            ? dateTimeOffsetFormat
            : null;

        this.DateOnlyFormat = options.TryGetValue(GetOptionName(MappaSettingsDateOnlyFormat), out var dateOnlyFormat)
                              && !string.IsNullOrWhiteSpace(dateOnlyFormat)
            ? dateOnlyFormat
            : null;

        this.TimeOnlyFormat = options.TryGetValue(GetOptionName(MappaSettingsTimeOnlyFormat), out var timeOnlyFormat)
                              && !string.IsNullOrWhiteSpace(timeOnlyFormat)
            ? timeOnlyFormat
            : null;

        this.DateTimeStyle = ReadDateTimeStylesOption(options, MappaSettingsDateTimeStyle);

        this.DateTimeOffsetStyle = ReadDateTimeStylesOption(options, MappaSettingsDateTimeOffsetStyle);

        this.DateOnlyStyle = ReadDateTimeStylesOption(options, MappaSettingsDateOnlyStyle);

        this.TimeOnlyStyle = ReadDateTimeStylesOption(options, MappaSettingsTimeOnlyStyle);

        this.GlobalDateTimeStyle = ReadDateTimeStylesOption(options, MappaSettingsGlobalDateTimeStyle);

        this.TimeSpanFormat = options.TryGetValue(GetOptionName(MappaSettingsTimeSpanFormat), out var timeSpanFormat)
                              && !string.IsNullOrWhiteSpace(timeSpanFormat)
            ? timeSpanFormat
            : null;

        this.GuidFormat = ReadFormatOption(options, MappaSettingsGuidFormat);

        this.ByteFormat = ReadFormatOption(options, MappaSettingsByteFormat);

        this.SByteFormat = ReadFormatOption(options, MappaSettingsSByteFormat);

        this.ShortFormat = ReadFormatOption(options, MappaSettingsShortFormat);

        this.UShortFormat = ReadFormatOption(options, MappaSettingsUShortFormat);

        this.IntFormat = ReadFormatOption(options, MappaSettingsIntFormat);

        this.UIntFormat = ReadFormatOption(options, MappaSettingsUIntFormat);

        this.LongFormat = ReadFormatOption(options, MappaSettingsLongFormat);

        this.ULongFormat = ReadFormatOption(options, MappaSettingsULongFormat);

        this.DecimalFormat = ReadFormatOption(options, MappaSettingsDecimalFormat);

        this.FloatFormat = ReadFormatOption(options, MappaSettingsFloatFormat);

        this.DoubleFormat = ReadFormatOption(options, MappaSettingsDoubleFormat);

        this.ByteStyle = ReadNumberStylesOption(options, MappaSettingsByteStyle);

        this.SByteStyle = ReadNumberStylesOption(options, MappaSettingsSByteStyle);

        this.ShortStyle = ReadNumberStylesOption(options, MappaSettingsShortStyle);

        this.UShortStyle = ReadNumberStylesOption(options, MappaSettingsUShortStyle);

        this.IntStyle = ReadNumberStylesOption(options, MappaSettingsIntStyle);

        this.UIntStyle = ReadNumberStylesOption(options, MappaSettingsUIntStyle);

        this.LongStyle = ReadNumberStylesOption(options, MappaSettingsLongStyle);

        this.ULongStyle = ReadNumberStylesOption(options, MappaSettingsULongStyle);

        this.DecimalStyle = ReadNumberStylesOption(options, MappaSettingsDecimalStyle);

        this.FloatStyle = ReadNumberStylesOption(options, MappaSettingsFloatStyle);

        this.DoubleStyle = ReadNumberStylesOption(options, MappaSettingsDoubleStyle);

        this.GlobalNumberStyle = ReadNumberStylesOption(options, MappaSettingsGlobalNumberStyle);

        this.CultureName = options.TryGetValue(GetOptionName(MappaSettingsCultureName), out var cultureName)
                           && !string.IsNullOrWhiteSpace(cultureName)
            ? cultureName
            : null;

        this.CultureInfoSetting = options.TryGetValue(GetOptionName(MappaSettingsCultureInfoSettings), out var cultureInfoSettings)
                                  && !string.IsNullOrWhiteSpace(cultureInfoSettings)
            ? GetCultureInfoSettingsFromString(cultureInfoSettings)
            : CultureInfoSetting.None;

        this.ProtobufOptional = options.TryGetValue(GetOptionName(MappaSettingsProtobufOptional), out var protobufOptional)
            ? GetBooleanSettingFromString(protobufOptional)
            : BooleanSetting.Undefined;

        this.PragmaWarning = options.TryGetValue(GetOptionName(MappaSettingsPragmaWarning), out var pragmaWarning)
            ? GetPragmaWarningSettingFromString(pragmaWarning)
            : PragmaWarningSetting.NoBlock;

        this.FastCollections = options.TryGetValue(GetOptionName(MappaSettingsFastCollections), out var fastCollections)
            ? GetBooleanSettingFromString(fastCollections)
            : BooleanSetting.Undefined;

        this.ContainerCapacityConstructors = options.TryGetValue(GetOptionName(MappaSettingsContainerCapacityConstructors), out var containerCapacityConstructors)
            ? GetBooleanSettingFromString(containerCapacityConstructors)
            : BooleanSetting.Undefined;

        this.PolymorphicMapMethodWithMatchingDefaultAttribute = options.TryGetValue(GetOptionName(MappaSettingsPolymorphicMapMethodWithMatchingDefaultAttribute), out var polymorphicMapMethodWithMatchingDefaultAttribute)
            ? GetBooleanSettingFromString(polymorphicMapMethodWithMatchingDefaultAttribute)
            : BooleanSetting.Undefined;

        this.CaseInsensitivePropertyMap = ReadCaseInsensitivePropertyMapOption(options);

        this.IgnoreUnderscoreForPropertyMap = options.TryGetValue(GetOptionName(MappaSettingsIgnoreUnderscoreForPropertyMap), out var ignoreUnderscoreForPropertyMap)
            ? GetBooleanSettingFromString(ignoreUnderscoreForPropertyMap)
            : BooleanSetting.Undefined;

        this.CaseInsensitiveStringToEnumMap = options.TryGetValue(GetOptionName(MappaSettingsCaseInsensitiveStringToEnumMap), out var caseInsensitiveStringToEnumMap)
            ? GetBooleanSettingFromString(caseInsensitiveStringToEnumMap)
            : BooleanSetting.Undefined;

        static CultureInfoSetting GetCultureInfoSettingsFromString(string cultureInfoSettings)
        {
            if (cultureInfoSettings.Equals(nameof(CultureInfoSetting.CurrentCulture), StringComparison.OrdinalIgnoreCase))
            {
                return CultureInfoSetting.CurrentCulture;
            }

            if (cultureInfoSettings.Equals(nameof(CultureInfoSetting.InvariantCulture), StringComparison.OrdinalIgnoreCase))
            {
                return CultureInfoSetting.InvariantCulture;
            }

            if (cultureInfoSettings.Equals(nameof(CultureInfoSetting.UserDefined), StringComparison.OrdinalIgnoreCase))
            {
                return CultureInfoSetting.UserDefined;
            }

            return CultureInfoSetting.None;
        }

        static BooleanSetting GetBooleanSettingFromString(string enableSettings)
        {
            if (enableSettings.Equals(nameof(BooleanSetting.Undefined), StringComparison.OrdinalIgnoreCase))
            {
                return BooleanSetting.Undefined;
            }

            if (enableSettings.Equals(nameof(BooleanSetting.Enable), StringComparison.OrdinalIgnoreCase))
            {
                return BooleanSetting.Enable;
            }

            if (enableSettings.Equals(nameof(BooleanSetting.Disable), StringComparison.OrdinalIgnoreCase))
            {
                return BooleanSetting.Disable;
            }

            return BooleanSetting.Undefined;
        }

        static PragmaWarningSetting GetPragmaWarningSettingFromString(string enableSettings)
        {
            if (enableSettings.Equals(nameof(PragmaWarningSetting.Undefined), StringComparison.OrdinalIgnoreCase))
            {
                return PragmaWarningSetting.Undefined;
            }

            if (enableSettings.Equals(nameof(PragmaWarningSetting.NoBlock), StringComparison.OrdinalIgnoreCase))
            {
                return PragmaWarningSetting.NoBlock;
            }

            if (enableSettings.Equals(nameof(BooleanSetting.Disable), StringComparison.OrdinalIgnoreCase))
            {
                return PragmaWarningSetting.Disable;
            }

            return PragmaWarningSetting.Undefined;
        }

        static string? ReadFormatOption(AnalyzerConfigOptions options, string optionName)
            => options.TryGetValue(GetOptionName(optionName), out var format)
               && !string.IsNullOrWhiteSpace(format)
                ? format
                : null;

        static DateTimeStyles? ReadDateTimeStylesOption(AnalyzerConfigOptions options, string optionName)
            => options.TryGetValue(GetOptionName(optionName), out var dateTimeStyles)
                ? ParseDateTimeStylesCodeHelper.TryParseFromString(dateTimeStyles)
                : null;

        static NumberStyles? ReadNumberStylesOption(AnalyzerConfigOptions options, string optionName)
            => options.TryGetValue(GetOptionName(optionName), out var numberStyles)
                ? ParseNumberStylesCodeHelper.TryParseFromString(numberStyles)
                : null;

        static BooleanSetting ReadCaseInsensitivePropertyMapOption(AnalyzerConfigOptions options)
        {
            if (options.TryGetValue(GetOptionName(MappaSettingsCaseInsensitivePropertyMap), out var caseInsensitivePropertyMap))
            {
                return GetBooleanSettingFromString(caseInsensitivePropertyMap);
            }

            if (options.TryGetValue(GetOptionName(MappaSettingsLegacyForceCaseInsensitivePropertyMap), out var legacyForceCaseInsensitivePropertyMap))
            {
                return GetBooleanSettingFromString(legacyForceCaseInsensitivePropertyMap);
            }

            return BooleanSetting.Undefined;
        }
    }

    /// <inheritdoc />
    public string? DateTimeFormat { get; }

    /// <inheritdoc />
    public string? DateTimeOffsetFormat { get; }

    /// <inheritdoc />
    public string? DateOnlyFormat { get; }

    /// <inheritdoc />
    public string? TimeOnlyFormat { get; }

    /// <inheritdoc />
    public DateTimeStyles? DateTimeStyle { get; }

    /// <inheritdoc />
    public DateTimeStyles? DateTimeOffsetStyle { get; }

    /// <inheritdoc />
    public DateTimeStyles? DateOnlyStyle { get; }

    /// <inheritdoc />
    public DateTimeStyles? TimeOnlyStyle { get; }

    /// <inheritdoc />
    public DateTimeStyles? GlobalDateTimeStyle { get; }

    /// <inheritdoc />
    public string? TimeSpanFormat { get; }

    /// <inheritdoc />
    public string? GuidFormat { get; }

    /// <inheritdoc />
    public string? ByteFormat { get; }

    /// <inheritdoc />
    public string? SByteFormat { get; }

    /// <inheritdoc />
    public string? ShortFormat { get; }

    /// <inheritdoc />
    public string? UShortFormat { get; }

    /// <inheritdoc />
    public string? IntFormat { get; }

    /// <inheritdoc />
    public string? UIntFormat { get; }

    /// <inheritdoc />
    public string? LongFormat { get; }

    /// <inheritdoc />
    public string? ULongFormat { get; }

    /// <inheritdoc />
    public string? DecimalFormat { get; }

    /// <inheritdoc />
    public string? FloatFormat { get; }

    /// <inheritdoc />
    public string? DoubleFormat { get; }

    /// <inheritdoc />
    public NumberStyles? ByteStyle { get; }

    /// <inheritdoc />
    public NumberStyles? SByteStyle { get; }

    /// <inheritdoc />
    public NumberStyles? ShortStyle { get; }

    /// <inheritdoc />
    public NumberStyles? UShortStyle { get; }

    /// <inheritdoc />
    public NumberStyles? IntStyle { get; }

    /// <inheritdoc />
    public NumberStyles? UIntStyle { get; }

    /// <inheritdoc />
    public NumberStyles? LongStyle { get; }

    /// <inheritdoc />
    public NumberStyles? ULongStyle { get; }

    /// <inheritdoc />
    public NumberStyles? DecimalStyle { get; }

    /// <inheritdoc />
    public NumberStyles? FloatStyle { get; }

    /// <inheritdoc />
    public NumberStyles? DoubleStyle { get; }

    /// <inheritdoc />
    public NumberStyles? GlobalNumberStyle { get; }

    /// <inheritdoc />
    public CultureInfoSetting CultureInfoSetting { get; }

    /// <inheritdoc />
    public string? CultureName { get; }

    /// <inheritdoc/>
    public BooleanSetting ProtobufOptional { get; }

    /// <inheritdoc/>
    public PragmaWarningSetting PragmaWarning { get; }

    /// <inheritdoc/>
    public BooleanSetting FastCollections { get; }

    /// <inheritdoc/>
    public BooleanSetting ContainerCapacityConstructors { get; }

    /// <inheritdoc/>
    public BooleanSetting PolymorphicMapMethodWithMatchingDefaultAttribute { get; }

    /// <inheritdoc/>
    public BooleanSetting CaseInsensitivePropertyMap { get; }

    /// <inheritdoc/>
    public BooleanSetting IgnoreUnderscoreForPropertyMap { get; }

    /// <inheritdoc/>
    public BooleanSetting CaseInsensitiveStringToEnumMap { get; }

    /// <summary>
    /// Gets a value indicating whether to report debug INFO diagnostics.
    /// </summary>
    internal bool MappaDebug { get; }

    /// <summary>
    /// Gets a value indicating whether to report debug comments in the generated code.
    /// </summary>
    internal bool MappaDebugComments { get; }

    private static string GetOptionName(string name)
#pragma warning disable CA1308 // Normalize strings to uppercase
        => $"{nameof(Mappa)}.{name}".ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
}