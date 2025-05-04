// <copyright file="IMappaUserSettings.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

namespace Mappa.Generator.Models;

/// <summary>
/// Expose the properties to obtain the user settings.
/// </summary>
internal interface IMappaUserSettings
{
    /// <summary>
    /// Gets the format when using <see cref="DateTime.ToString(string,System.IFormatProvider)"/> or <see cref="DateTime.ParseExact(string,string,System.IFormatProvider)"/>.
    /// </summary>
    string? DateTimeFormat { get; }

    /// <summary>
    /// Gets the format when using <see cref="DateTimeOffset.ToString(string,System.IFormatProvider)"/> or <see cref="DateTimeOffset.ParseExact(string,string,System.IFormatProvider)"/>.
    /// </summary>
    string? DateTimeOffsetFormat { get; }

    /// <summary>
    /// Gets the format when using <c>DateOnly.ToString(string,System.IFormatProvider)</c> or <c>DateOnly.ParseExact(string,string,System.IFormatProvider)</c>.
    /// </summary>
    string? DateOnlyFormat { get; }

    /// <summary>
    /// Gets the format when using <c>TimeOnly.ToString(string,System.IFormatProvider)</c> or <c>TimeOnly.ParseExact(string,string,System.IFormatProvider)</c>.
    /// </summary>
    string? TimeOnlyFormat { get; }

    /// <summary>
    /// Gets the format when using <see cref="TimeSpan.ToString(string,System.IFormatProvider)"/> or <see cref="TimeSpan.ParseExact(string,string,System.IFormatProvider)"/>.
    /// </summary>
    string? TimeSpanFormat { get; }

    /// <summary>
    /// Gets the format when using <see cref="Guid.ToString(string)"/> or <see cref="Guid.ParseExact(string,string)"/>.
    /// </summary>
    string? GuidFormat { get; }

    /// <summary>
    /// Gets the <see cref="CultureInfo"/> to use when converting to string or parsing form string.
    /// </summary>
    CultureInfoSetting CultureInfoSetting { get; }

    /// <summary>
    /// Gets the culture name when <see cref="CultureInfoSetting"/> is <see cref="Mappa.CultureInfoSetting.UserDefined"/>.
    /// </summary>
    string? CultureName { get; }

    /// <summary>
    /// Gets a value indicating whether the protobuf optional feature is enabled when performing mapping.
    /// </summary>
    BooleanSetting ProtobufOptional { get; }

    /// <summary>
    /// Gets a value indicating weather the mapping method should be surrounded
    /// by <c>#pragma warning disable</c>.
    /// </summary>
    PragmaWarningSetting PragmaWarning { get; }

    /// <summary>
    /// Gets a value indicating weather to use <c>Span{T}</c> for fast iterations
    /// over arrays and <see cref="List{T}"/>.
    /// </summary>
    BooleanSetting FastCollections { get; }
}