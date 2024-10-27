// <copyright file="MappaGlobalOptions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

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
///         <description>Default format to be used for parsing strings and converting to string <see cref="DateTime"/> structs.</description>
///     </item>
///     <item>
///         <term><c>mappa.datetimeoffsetformat</c></term>
///         <description>Default format to be used for parsing strings and converting to string <see cref="DateTimeOffset"/> structs.</description>
///     </item>
///     <item>
///         <term><c>mappa.dateonlyformat</c></term>
///         <description>Default format to be used for parsing strings and converting to string DateOnly structs.</description>
///     </item>
///     <item>
///         <term><c>mappa.timeonlyformat</c></term>
///         <description>Default format to be used for parsing strings and converting to string TimeOnly structs.</description>
///     </item>
///     <item>
///         <term><c>mappa.timespanformat</c></term>
///         <description>Default format to be used for parsing strings and converting to string <see cref="TimeSpan"/> structs.</description>
///     </item>
///     <item>
///         <term><c>mappa.guidformat</c></term>
///         <description>Default format to be used for parsing strings and converting to string <see cref="Guid"/> structs.</description>
///     </item>
///     <item>
///         <term><c>mappa.cultureinfosettings</c></term>
///         <description>Set the default culture info settings. Valid values are the values of the <see cref="MappaSettingsAttribute.CultureInfoSettings"/>.</description>
///     </item>
///     <item>
///         <term><c>mappa.culturename</c></term>
///         <description>The name of the default culture to be applied.</description>
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
            ? FromString(cultureInfoSettings)
            : MappaSettingsAttribute.CultureInfoSettings.CurrentCulture;

        static MappaSettingsAttribute.CultureInfoSettings FromString(string cultureInfoSettings)
        {
            if (cultureInfoSettings.Equals(nameof(MappaSettingsAttribute.CultureInfoSettings.CurrentCulture), StringComparison.OrdinalIgnoreCase))
            {
                return MappaSettingsAttribute.CultureInfoSettings.CurrentCulture;
            }

            if (cultureInfoSettings.Equals(nameof(MappaSettingsAttribute.CultureInfoSettings.InvariantCulture), StringComparison.OrdinalIgnoreCase))
            {
                return MappaSettingsAttribute.CultureInfoSettings.InvariantCulture;
            }

            if (cultureInfoSettings.Equals(nameof(MappaSettingsAttribute.CultureInfoSettings.UserDefined), StringComparison.OrdinalIgnoreCase))
            {
                return MappaSettingsAttribute.CultureInfoSettings.UserDefined;
            }

            return MappaSettingsAttribute.CultureInfoSettings.UserDefined;
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
    public MappaSettingsAttribute.CultureInfoSettings CultureInfoSetting { get; }

    /// <inheritdoc />
    public string? CultureName { get; }

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