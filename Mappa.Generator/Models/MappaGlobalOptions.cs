// <copyright file="MappaGlobalOptions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

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
///         <term><c>mappa.timespanformat</c></term>
///         <description>Default format to be used for parsing strings and converting to string <see cref="TimeSpan"/> <c>struct</c>s.</description>
///     </item>
///     <item>
///         <term><c>mappa.guidformat</c></term>
///         <description>Default format to be used for parsing strings and converting to string <see cref="Guid"/>  <c>struct</c>s.</description>
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
    private const string MappaSettingsTimeSpanFormat = "timespanformat";
    private const string MappaSettingsGuidFormat = "guidformat";
    private const string MappaSettingsCultureInfoSettings = "cultureinfosettings";
    private const string MappaSettingsCultureName = "culturename";
    private const string MappaSettingsProtobufOptional = "protobufoptional";
    private const string MappaSettingsPragmaWarning = "pragmawarning";
    private const string MappaSettingsFastCollections = "fastcollections";

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

        this.TimeSpanFormat = options.TryGetValue(GetOptionName(MappaSettingsTimeSpanFormat), out var timeSpanFormat)
                              && !string.IsNullOrWhiteSpace(timeSpanFormat)
            ? timeSpanFormat
            : null;

        this.GuidFormat = options.TryGetValue(GetOptionName(MappaSettingsGuidFormat), out var guidFormat)
                          && !string.IsNullOrWhiteSpace(guidFormat)
            ? guidFormat
            : null;

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

        static CultureInfoSetting GetCultureInfoSettingsFromString(string cultureInfoSettings)
        {
            if (cultureInfoSettings.Equals(nameof(CultureInfoSetting.CurrentCulture), StringComparison.OrdinalIgnoreCase))
            {
                return CultureInfoSetting.None;
            }

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
    public string? TimeSpanFormat { get; }

    /// <inheritdoc />
    public string? GuidFormat { get; }

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