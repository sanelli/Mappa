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
    private readonly StackSetting<MappaSettingsAttribute.CultureInfoSettings> cultureInfoSetting;
    private readonly StackSetting<string?> cultureName;

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
            otherSettings.CultureName)
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
    internal MappaUserSettings(
        string? dateTimeFormat,
        string? dateTimeOffsetFormat,
        string? dateOnlyFormat,
        string? timeOnlyFormat,
        string? timeSpanFormat,
        string? guidFormat,
        MappaSettingsAttribute.CultureInfoSettings? cultureInfoSetting,
        string? cultureName)
    {
        this.dateTimeFormat = new(dateTimeFormat);
        this.dateTimeOffsetFormat = new(dateTimeOffsetFormat);
        this.dateOnlyFormat = new(dateOnlyFormat);
        this.timeOnlyFormat = new(timeOnlyFormat);
        this.timeSpanFormat = new(timeSpanFormat);
        this.guidFormat = new(guidFormat);
        this.cultureInfoSetting = new(cultureInfoSetting ?? MappaSettingsAttribute.CultureInfoSettings.None);
        this.cultureName = new(cultureName);
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
    public MappaSettingsAttribute.CultureInfoSettings? CultureInfoSetting => this.cultureInfoSetting;

    /// <inheritdoc />
    public string? CultureName => this.cultureName;

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
 #pragma warning disable CA2000
            this.dateTimeFormat.Apply(mappaSettingsAttribute.DateTimeFormat ?? this.dateTimeFormat),
            this.dateTimeOffsetFormat.Apply(mappaSettingsAttribute.DateTimeOffsetFormat ?? this.dateTimeOffsetFormat),
            this.dateOnlyFormat.Apply(mappaSettingsAttribute.DateOnlyFormat ?? this.dateOnlyFormat),
            this.timeOnlyFormat.Apply(mappaSettingsAttribute.TimeOnlyFormat ?? this.timeOnlyFormat),
            this.timeSpanFormat.Apply(mappaSettingsAttribute.TimeSpanFormat ?? this.timeSpanFormat),
            this.guidFormat.Apply(mappaSettingsAttribute.GuidFormat ?? this.guidFormat),
            this.cultureInfoSetting.Apply(mappaSettingsAttribute.CultureInfoSetting ?? this.cultureInfoSetting),
            this.cultureName.Apply(mappaSettingsAttribute.CultureName ?? this.cultureName),
 #pragma warning restore CA2000
        ]);
    }

    /// <summary>
    /// Gets a non-mutable version of these settings.
    /// </summary>
    /// <returns>A non-mutable version of these settings.</returns>
    internal IMappaUserSettings Freeze() => new FrozenMappaUserSettings(this);

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

    private sealed class FrozenMappaUserSettings
        : IMappaUserSettings
    {
        internal FrozenMappaUserSettings(IMappaUserSettings other)
        {
            this.DateTimeFormat = other.DateTimeFormat;
            this.DateTimeOffsetFormat = other.DateTimeOffsetFormat;
            this.DateOnlyFormat = other.DateOnlyFormat;
            this.TimeOnlyFormat = other.TimeOnlyFormat;
            this.TimeSpanFormat = other.TimeSpanFormat;
            this.GuidFormat = other.GuidFormat;
            this.CultureInfoSetting = other.CultureInfoSetting;
            this.CultureName = other.CultureName;
        }

        public string? DateTimeFormat { get; }

        public string? DateTimeOffsetFormat { get; }

        public string? DateOnlyFormat { get; }

        public string? TimeOnlyFormat { get; }

        public string? TimeSpanFormat { get; }

        public string? GuidFormat { get; }

        public MappaSettingsAttribute.CultureInfoSettings? CultureInfoSetting { get; }

        public string? CultureName { get; }
    }
}