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
    /// Gets or sets a value indicating whether the protobuf optional feature is enabled when performing mapping.
    /// It is not enabled by default.
    /// </summary>
    public BooleanSetting ProtobufOptional { get; set; } = BooleanSetting.Undefined;

    /// <summary>
    /// Gets or sets a value indicating whether the mapping method should be surrounded
    /// by <c>#pragma warning disable</c>.
    /// </summary>
    public PragmaWarningSetting PragmaWarning { get; set; } = PragmaWarningSetting.Undefined;

    /// <summary>
    /// Gets or sets a value indicating whether to use <c>Span{T}</c> for fast iterations
    /// over arrays and <see cref="List{T}"/>.
    /// </summary>
    public BooleanSetting FastCollections { get; set; } = BooleanSetting.Undefined;

    /// <summary>
    /// Gets or sets a value indicating whether the source generator is allowed to use
    /// a constructor with a single integer parameters when mapping collections. The
    /// single integer parameter represents the initial collection capacity,
    /// similar to <see cref="List{T}(int)"/>.
    /// </summary>
    public BooleanSetting ContainerCapacityConstructors { get; set; } = BooleanSetting.Undefined;

    /// <summary>
    /// Gets or sets a value indicating whether to allow the source generator to
    /// use the <see cref="MappaTypeMappingDefaultAttribute"/> when looking for a
    /// polymorphic method to support a mapping between two types and the behavior
    /// of the attribute is <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/>
    /// and the target type is explicitly defined.By default, this behavior is disabled
    /// because it can be dangerous due to the nature of the polymorphic method
    /// where the method can accept any type derived from the input parameter and
    /// there is no guarantee that the method will return the exact type specified by
    /// the <see cref="MappaTypeMappingDefaultAttribute"/>.
    /// </summary>
    public BooleanSetting PolymorphicMapMethodWithMatchingDefaultAttribute { get; set; } = BooleanSetting.Undefined;
}