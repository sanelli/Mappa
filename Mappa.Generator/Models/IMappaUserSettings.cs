// <copyright file="IMappaUserSettings.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Attributes;

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
    /// Gets the <see cref="DateTimeStyles"/> when parsing a <see cref="DateTime"/>.
    /// </summary>
    DateTimeStyles? DateTimeStyle { get; }

    /// <summary>
    /// Gets the <see cref="DateTimeStyles"/> when parsing a <see cref="DateTimeOffset"/>.
    /// </summary>
    DateTimeStyles? DateTimeOffsetStyle { get; }

    /// <summary>
    /// Gets the <see cref="DateTimeStyles"/> when parsing a <c>DateOnly</c>.
    /// </summary>
    DateTimeStyles? DateOnlyStyle { get; }

    /// <summary>
    /// Gets the <see cref="DateTimeStyles"/> when parsing a <c>TimeOnly</c>.
    /// </summary>
    DateTimeStyles? TimeOnlyStyle { get; }

    /// <summary>
    /// Gets the default <see cref="DateTimeStyles"/> for parsing date/time types when the type-specific style is unset.
    /// </summary>
    DateTimeStyles? GlobalDateTimeStyle { get; }

    /// <summary>
    /// Gets the format when using <see cref="TimeSpan.ToString(string,System.IFormatProvider)"/> or <see cref="TimeSpan.ParseExact(string,string,System.IFormatProvider)"/>.
    /// </summary>
    string? TimeSpanFormat { get; }

    /// <summary>
    /// Gets the format when using <see cref="Guid.ToString(string)"/> or <see cref="Guid.ParseExact(string,string)"/>.
    /// </summary>
    string? GuidFormat { get; }

    /// <summary>
    /// Gets the format when using <see cref="byte.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    string? ByteFormat { get; }

    /// <summary>
    /// Gets the format when using <see cref="sbyte.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    string? SByteFormat { get; }

    /// <summary>
    /// Gets the format when using <see cref="short.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    string? ShortFormat { get; }

    /// <summary>
    /// Gets the format when using <see cref="ushort.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    string? UShortFormat { get; }

    /// <summary>
    /// Gets the format when using <see cref="int.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    string? IntFormat { get; }

    /// <summary>
    /// Gets the format when using <see cref="uint.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    string? UIntFormat { get; }

    /// <summary>
    /// Gets the format when using <see cref="long.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    string? LongFormat { get; }

    /// <summary>
    /// Gets the format when using <see cref="ulong.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    string? ULongFormat { get; }

    /// <summary>
    /// Gets the format when using <see cref="decimal.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    string? DecimalFormat { get; }

    /// <summary>
    /// Gets the format when using <see cref="float.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    string? FloatFormat { get; }

    /// <summary>
    /// Gets the format when using <see cref="double.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    string? DoubleFormat { get; }

    /// <summary>
    /// Gets the <see cref="NumberStyles"/> when parsing a <see cref="byte"/>.
    /// </summary>
    NumberStyles? ByteStyle { get; }

    /// <summary>
    /// Gets the <see cref="NumberStyles"/> when parsing a <see cref="sbyte"/>.
    /// </summary>
    NumberStyles? SByteStyle { get; }

    /// <summary>
    /// Gets the <see cref="NumberStyles"/> when parsing a <see cref="short"/>.
    /// </summary>
    NumberStyles? ShortStyle { get; }

    /// <summary>
    /// Gets the <see cref="NumberStyles"/> when parsing a <see cref="ushort"/>.
    /// </summary>
    NumberStyles? UShortStyle { get; }

    /// <summary>
    /// Gets the <see cref="NumberStyles"/> when parsing an <see cref="int"/>.
    /// </summary>
    NumberStyles? IntStyle { get; }

    /// <summary>
    /// Gets the <see cref="NumberStyles"/> when parsing a <see cref="uint"/>.
    /// </summary>
    NumberStyles? UIntStyle { get; }

    /// <summary>
    /// Gets the <see cref="NumberStyles"/> when parsing a <see cref="long"/>.
    /// </summary>
    NumberStyles? LongStyle { get; }

    /// <summary>
    /// Gets the <see cref="NumberStyles"/> when parsing a <see cref="ulong"/>.
    /// </summary>
    NumberStyles? ULongStyle { get; }

    /// <summary>
    /// Gets the <see cref="NumberStyles"/> when parsing a <see cref="decimal"/>.
    /// </summary>
    NumberStyles? DecimalStyle { get; }

    /// <summary>
    /// Gets the <see cref="NumberStyles"/> when parsing a <see cref="float"/>.
    /// </summary>
    NumberStyles? FloatStyle { get; }

    /// <summary>
    /// Gets the <see cref="NumberStyles"/> when parsing a <see cref="double"/>.
    /// </summary>
    NumberStyles? DoubleStyle { get; }

    /// <summary>
    /// Gets the default <see cref="NumberStyles"/> for parsing numeric types when the type-specific style is unset.
    /// </summary>
    NumberStyles? GlobalNumberStyle { get; }

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
    /// Gets a value indicating whether the mapping method should be surrounded
    /// by <c>#pragma warning disable</c>.
    /// </summary>
    PragmaWarningSetting PragmaWarning { get; }

    /// <summary>
    /// Gets a value indicating whether to use <c>Span{T}</c> for fast iterations
    /// over arrays and <see cref="List{T}"/>.
    /// </summary>
    BooleanSetting FastCollections { get; }

    /// <summary>
    /// Gets a value indicating whether the source generator is allowed to use
    /// a constructor with a single integer parameters when mapping collections. The
    /// single integer parameter represents the initial collection capacity,
    /// similar to <see cref="List{T}(int)"/>.
    /// </summary>
    BooleanSetting ContainerCapacityConstructors { get; }

    /// <summary>
    /// Gets a value indicating whether to allow the source generator to
    /// use the <see cref="MappaTypeMappingDefaultAttribute"/> when looking for a
    /// polymorphic method to support a mapping between two types and the behavior
    /// of the attribute is <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/>
    /// and the target type is explicitly defined.By default, this behavior is disabled
    /// because it can be dangerous due to the nature of the polymorphic method
    /// where the method can accept any type derived from the input parameter and
    /// there is no guarantee that the method will return the exact type specified by
    /// the <see cref="MappaTypeMappingDefaultAttribute"/>.
    /// </summary>
    BooleanSetting PolymorphicMapMethodWithMatchingDefaultAttribute { get; }

    /// <summary>
    /// Gets a value indicating whether source property names are matched
    /// case-insensitively when pairing a target property or constructor parameter
    /// with a source property by name.
    /// </summary>
    BooleanSetting CaseInsensitivePropertyMap { get; }

    /// <summary>
    /// Gets a value indicating whether underscore characters are ignored
    /// when pairing a target property or constructor parameter with a source property
    /// by name.
    /// </summary>
    BooleanSetting IgnoreUnderscoreForPropertyMap { get; }

    /// <summary>
    /// Gets a value indicating whether enum member names or Description attribute values
    /// are matched case-insensitively when mapping between enums or from string to an enum.
    /// </summary>
    BooleanSetting CaseInsensitiveEnumMap { get; }

    /// <summary>
    /// Gets how enum members are paired with string values when mapping between an enum and string.
    /// </summary>
    EnumStringMapSetting EnumStringMapSetting { get; }

    /// <summary>
    /// Gets how enum members are paired when mapping from one enum to another enum.
    /// </summary>
    EnumToEnumMapSetting EnumToEnumMapSetting { get; }
}