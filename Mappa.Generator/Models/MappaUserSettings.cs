// <copyright file="MappaUserSettings.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

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
    private readonly StackSetting<string?> timeSpanFormat;
    private readonly StackSetting<string?> guidFormat;
    private readonly StackSetting<CultureInfoSetting> cultureInfoSetting;
    private readonly StackSetting<string?> cultureName;
    private readonly StackSetting<BooleanSetting> protobufOptional;
    private readonly StackSetting<PragmaWarningSetting> pragmaWarning;

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
            otherSettings.TimeSpanFormat,
            otherSettings.GuidFormat,
            otherSettings.CultureInfoSetting,
            otherSettings.CultureName,
            otherSettings.ProtobufOptional,
            otherSettings.PragmaWarning)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaUserSettings"/> class.
    /// </summary>
    /// <param name="dateTimeFormat">The default format for <see cref="DateTime"/>.</param>
    /// <param name="dateTimeOffsetFormat">The default format for <see cref="DateTimeOffset"/>.</param>
    /// <param name="dateOnlyFormat">The default format for DateOnly.</param>
    /// <param name="timeOnlyFormat">The default format for TimeOnly.</param>
    /// <param name="timeSpanFormat">The default format for <see cref="TimeSpan"/>.</param>
    /// <param name="guidFormat">The default format for <see cref="Guid"/>.</param>
    /// <param name="cultureInfoSetting">The type of culture info settings to be provided.</param>
    /// <param name="cultureName">The default culture info to use to generate a format provider.</param>
    /// <param name="protobufOptional">Enable or disable (protobuf) optional feature.</param>
    /// <param name="pragmaWarningSetting">Allow to surround the code generated with a <c>#pragma warning disable</c> block.</param>
    private MappaUserSettings(
        string? dateTimeFormat,
        string? dateTimeOffsetFormat,
        string? dateOnlyFormat,
        string? timeOnlyFormat,
        string? timeSpanFormat,
        string? guidFormat,
        CultureInfoSetting cultureInfoSetting,
        string? cultureName,
        BooleanSetting protobufOptional,
        PragmaWarningSetting pragmaWarningSetting)
    {
        this.dateTimeFormat = new(dateTimeFormat);
        this.dateTimeOffsetFormat = new(dateTimeOffsetFormat);
        this.dateOnlyFormat = new(dateOnlyFormat);
        this.timeOnlyFormat = new(timeOnlyFormat);
        this.timeSpanFormat = new(timeSpanFormat);
        this.guidFormat = new(guidFormat);
        this.cultureInfoSetting = new(cultureInfoSetting);
        this.cultureName = new(cultureName);
        this.protobufOptional = new(protobufOptional);
        this.pragmaWarning = new(pragmaWarningSetting);
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
    public string? TimeSpanFormat => this.timeSpanFormat;

    /// <inheritdoc />
    public string? GuidFormat => this.guidFormat;

    /// <inheritdoc />
    public CultureInfoSetting CultureInfoSetting => this.cultureInfoSetting;

    /// <inheritdoc />
    public string? CultureName => this.cultureName;

    /// <inheritdoc/>
    public BooleanSetting ProtobufOptional => this.protobufOptional;

    /// <inheritdoc/>
    public PragmaWarningSetting PragmaWarning => this.pragmaWarning;

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
            this.timeSpanFormat.Apply(mappaSettingsAttribute.TimeSpanFormat ?? this.timeSpanFormat),
            this.guidFormat.Apply(mappaSettingsAttribute.GuidFormat ?? this.guidFormat),
            this.cultureInfoSetting.Apply(mappaSettingsAttribute.CultureInfoSetting is not CultureInfoSetting.Undefined ? mappaSettingsAttribute.CultureInfoSetting : this.cultureInfoSetting),
            this.cultureName.Apply(mappaSettingsAttribute.CultureName ?? this.cultureName),
            this.protobufOptional.Apply(mappaSettingsAttribute.ProtobufOptional is not BooleanSetting.Undefined ? mappaSettingsAttribute.ProtobufOptional : this.protobufOptional),
            this.pragmaWarning.Apply(mappaSettingsAttribute.PragmaWarning is not PragmaWarningSetting.Undefined ? mappaSettingsAttribute.PragmaWarning : this.pragmaWarning),
 #pragma warning restore CA2000
        ]);
    }

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