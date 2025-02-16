// <copyright file="MappaSettingsAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

namespace Mappa.Attributes;

/// <summary>
/// Allow to specify advanced settings for fine-tuning the mappings.
/// A <c>null</c> value means that the setting is ignored and to use previous
/// values (if any). An empty string value means do not use the setting.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class MappaSettingsAttribute
    : Attribute
{
    /// <summary>
    /// Gets or sets the format when using <see cref="DateTime.ToString(string,System.IFormatProvider)"/> or <see cref="DateTime.ParseExact(string,string,System.IFormatProvider)"/>.
    /// </summary>
    public string? DateTimeFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <see cref="DateTimeOffset.ToString(string,System.IFormatProvider)"/> or <see cref="DateTimeOffset.ParseExact(string,string,System.IFormatProvider)"/>.
    /// </summary>
    public string? DateTimeOffsetFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <c>DateOnly.ToString(string,System.IFormatProvider)</c> or <c>DateOnly.ParseExact(string,string,System.IFormatProvider)</c>.
    /// </summary>
    public string? DateOnlyFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <c>TimeOnly.ToString(string,System.IFormatProvider)</c> or <c>TimeOnly.ParseExact(string,string,System.IFormatProvider)</c>.
    /// </summary>
    public string? TimeOnlyFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <see cref="TimeSpan.ToString(string,System.IFormatProvider)"/> or <see cref="TimeSpan.ParseExact(string,string,System.IFormatProvider)"/>.
    /// </summary>
    public string? TimeSpanFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <see cref="Guid.ToString(string)"/> or <see cref="Guid.ParseExact(string,string)"/>.
    /// </summary>
    public string? GuidFormat { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="CultureInfo"/> to use when converting to string or parsing form string.
    /// </summary>
    public CultureInfoSetting CultureInfoSetting { get; set; } = CultureInfoSetting.Undefined;

    /// <summary>
    /// Gets or sets the culture name when <see cref="CultureInfoSetting"/> is <see cref="CultureInfoSetting.UserDefined"/>.
    /// </summary>
    public string? CultureName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the optional feature is enabled when performing mapping.
    /// It is not enabled by default.
    /// </summary>
    public BooleanSetting Optional { get; set; } = BooleanSetting.Undefined;
}