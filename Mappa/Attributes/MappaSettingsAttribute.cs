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
    /// Gets the sentinel value for unset date/time style settings. When used, the style is ignored and previous values (if any) are used.
    /// </summary>
    public static DateTimeStyles UndefinedDateTimeStyle => (DateTimeStyles)(-1);

    /// <summary>
    /// Gets the sentinel value for unset numeric style settings. When used, the style is ignored and previous values (if any) are used.
    /// </summary>
    public static NumberStyles UndefinedNumberStyle => (NumberStyles)(-1);

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
    /// Gets or sets the <see cref="DateTimeStyles"/> when using <see cref="DateTime.Parse(string,System.IFormatProvider,System.Globalization.DateTimeStyles)"/> or <see cref="DateTime.ParseExact(string,string,System.IFormatProvider,System.Globalization.DateTimeStyles)"/>.
    /// Use <see cref="UndefinedDateTimeStyle"/> to ignore this setting and use previous values (if any).
    /// </summary>
    public DateTimeStyles DateTimeStyle { get; set; } = UndefinedDateTimeStyle;

    /// <summary>
    /// Gets or sets the <see cref="DateTimeStyles"/> when using <see cref="DateTimeOffset.Parse(string,System.IFormatProvider,System.Globalization.DateTimeStyles)"/> or <see cref="DateTimeOffset.ParseExact(string,string,System.IFormatProvider,System.Globalization.DateTimeStyles)"/>.
    /// Use <see cref="UndefinedDateTimeStyle"/> to ignore this setting and use previous values (if any).
    /// </summary>
    public DateTimeStyles DateTimeOffsetStyle { get; set; } = UndefinedDateTimeStyle;

    /// <summary>
    /// Gets or sets the <see cref="DateTimeStyles"/> when using <c>DateOnly.Parse(string,System.IFormatProvider,System.Globalization.DateTimeStyles)</c> or <c>DateOnly.ParseExact(string,string,System.IFormatProvider,System.Globalization.DateTimeStyles)</c>.
    /// Use <see cref="UndefinedDateTimeStyle"/> to ignore this setting and use previous values (if any).
    /// </summary>
    public DateTimeStyles DateOnlyStyle { get; set; } = UndefinedDateTimeStyle;

    /// <summary>
    /// Gets or sets the <see cref="DateTimeStyles"/> when using <c>TimeOnly.Parse(string,System.IFormatProvider,System.Globalization.DateTimeStyles)</c> or <c>TimeOnly.ParseExact(string,string,System.IFormatProvider,System.Globalization.DateTimeStyles)</c>.
    /// Use <see cref="UndefinedDateTimeStyle"/> to ignore this setting and use previous values (if any).
    /// </summary>
    public DateTimeStyles TimeOnlyStyle { get; set; } = UndefinedDateTimeStyle;

    /// <summary>
    /// Gets or sets the default <see cref="DateTimeStyles"/> for parsing <see cref="string"/>s to
    /// <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, <c>DateOnly</c>, and <c>TimeOnly</c>
    /// when the corresponding type-specific style is unset.
    /// Use <see cref="UndefinedDateTimeStyle"/> to ignore this setting and use previous values (if any).
    /// Type-specific style properties override this global default when both are set. Does not affect <c>ToString</c>.
    /// </summary>
    public DateTimeStyles GlobalDateTimeStyle { get; set; } = UndefinedDateTimeStyle;

    /// <summary>
    /// Gets or sets the format when using <see cref="TimeSpan.ToString(string,System.IFormatProvider)"/> or <see cref="TimeSpan.ParseExact(string,string,System.IFormatProvider)"/>.
    /// </summary>
    public string? TimeSpanFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <see cref="Guid.ToString(string)"/> or <see cref="Guid.ParseExact(string,string)"/>.
    /// </summary>
    public string? GuidFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <see cref="byte.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    public string? ByteFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <see cref="sbyte.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    public string? SByteFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <see cref="short.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    public string? ShortFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <see cref="ushort.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    public string? UShortFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <see cref="int.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    public string? IntFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <see cref="uint.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    public string? UIntFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <see cref="long.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    public string? LongFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <see cref="ulong.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    public string? ULongFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <see cref="decimal.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    public string? DecimalFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <see cref="float.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    public string? FloatFormat { get; set; }

    /// <summary>
    /// Gets or sets the format when using <see cref="double.ToString(string,System.IFormatProvider)"/> when converting to <see cref="string"/>.
    /// </summary>
    public string? DoubleFormat { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="NumberStyles"/> when using <see cref="byte.Parse(string,System.Globalization.NumberStyles)"/> or <see cref="byte.Parse(string,System.Globalization.NumberStyles,System.IFormatProvider)"/>.
    /// Use <see cref="UndefinedNumberStyle"/> to ignore this setting and use previous values (if any).
    /// </summary>
    public NumberStyles ByteStyle { get; set; } = UndefinedNumberStyle;

    /// <summary>
    /// Gets or sets the <see cref="NumberStyles"/> when using <see cref="sbyte.Parse(string,System.Globalization.NumberStyles)"/> or <see cref="sbyte.Parse(string,System.Globalization.NumberStyles,System.IFormatProvider)"/>.
    /// Use <see cref="UndefinedNumberStyle"/> to ignore this setting and use previous values (if any).
    /// </summary>
    public NumberStyles SByteStyle { get; set; } = UndefinedNumberStyle;

    /// <summary>
    /// Gets or sets the <see cref="NumberStyles"/> when using <see cref="short.Parse(string,System.Globalization.NumberStyles)"/> or <see cref="short.Parse(string,System.Globalization.NumberStyles,System.IFormatProvider)"/>.
    /// Use <see cref="UndefinedNumberStyle"/> to ignore this setting and use previous values (if any).
    /// </summary>
    public NumberStyles ShortStyle { get; set; } = UndefinedNumberStyle;

    /// <summary>
    /// Gets or sets the <see cref="NumberStyles"/> when using <see cref="ushort.Parse(string,System.Globalization.NumberStyles)"/> or <see cref="ushort.Parse(string,System.Globalization.NumberStyles,System.IFormatProvider)"/>.
    /// Use <see cref="UndefinedNumberStyle"/> to ignore this setting and use previous values (if any).
    /// </summary>
    public NumberStyles UShortStyle { get; set; } = UndefinedNumberStyle;

    /// <summary>
    /// Gets or sets the <see cref="NumberStyles"/> when using <see cref="int.Parse(string,System.Globalization.NumberStyles)"/> or <see cref="int.Parse(string,System.Globalization.NumberStyles,System.IFormatProvider)"/>.
    /// Use <see cref="UndefinedNumberStyle"/> to ignore this setting and use previous values (if any).
    /// </summary>
    public NumberStyles IntStyle { get; set; } = UndefinedNumberStyle;

    /// <summary>
    /// Gets or sets the <see cref="NumberStyles"/> when using <see cref="uint.Parse(string,System.Globalization.NumberStyles)"/> or <see cref="uint.Parse(string,System.Globalization.NumberStyles,System.IFormatProvider)"/>.
    /// Use <see cref="UndefinedNumberStyle"/> to ignore this setting and use previous values (if any).
    /// </summary>
    public NumberStyles UIntStyle { get; set; } = UndefinedNumberStyle;

    /// <summary>
    /// Gets or sets the <see cref="NumberStyles"/> when using <see cref="long.Parse(string,System.Globalization.NumberStyles)"/> or <see cref="long.Parse(string,System.Globalization.NumberStyles,System.IFormatProvider)"/>.
    /// Use <see cref="UndefinedNumberStyle"/> to ignore this setting and use previous values (if any).
    /// </summary>
    public NumberStyles LongStyle { get; set; } = UndefinedNumberStyle;

    /// <summary>
    /// Gets or sets the <see cref="NumberStyles"/> when using <see cref="ulong.Parse(string,System.Globalization.NumberStyles)"/> or <see cref="ulong.Parse(string,System.Globalization.NumberStyles,System.IFormatProvider)"/>.
    /// Use <see cref="UndefinedNumberStyle"/> to ignore this setting and use previous values (if any).
    /// </summary>
    public NumberStyles ULongStyle { get; set; } = UndefinedNumberStyle;

    /// <summary>
    /// Gets or sets the <see cref="NumberStyles"/> when using <see cref="decimal.Parse(string,System.Globalization.NumberStyles)"/> or <see cref="decimal.Parse(string,System.Globalization.NumberStyles,System.IFormatProvider)"/>.
    /// Use <see cref="UndefinedNumberStyle"/> to ignore this setting and use previous values (if any).
    /// </summary>
    public NumberStyles DecimalStyle { get; set; } = UndefinedNumberStyle;

    /// <summary>
    /// Gets or sets the <see cref="NumberStyles"/> when using <see cref="float.Parse(string,System.Globalization.NumberStyles)"/> or <see cref="float.Parse(string,System.Globalization.NumberStyles,System.IFormatProvider)"/>.
    /// Use <see cref="UndefinedNumberStyle"/> to ignore this setting and use previous values (if any).
    /// </summary>
    public NumberStyles FloatStyle { get; set; } = UndefinedNumberStyle;

    /// <summary>
    /// Gets or sets the <see cref="NumberStyles"/> when using <see cref="double.Parse(string,System.Globalization.NumberStyles)"/> or <see cref="double.Parse(string,System.Globalization.NumberStyles,System.IFormatProvider)"/>.
    /// Use <see cref="UndefinedNumberStyle"/> to ignore this setting and use previous values (if any).
    /// </summary>
    public NumberStyles DoubleStyle { get; set; } = UndefinedNumberStyle;

    /// <summary>
    /// Gets or sets the default <see cref="NumberStyles"/> for parsing <see cref="string"/>s to numeric types
    /// when the corresponding type-specific style is unset.
    /// Use <see cref="UndefinedNumberStyle"/> to ignore this setting and use previous values (if any).
    /// Type-specific style properties override this global default when both are set. Does not affect <c>ToString</c>.
    /// </summary>
    public NumberStyles GlobalNumberStyle { get; set; } = UndefinedNumberStyle;

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
    /// Gets or sets a value indicating whether the source generator should avoid invoking
    /// <c>System.Linq.Enumerable.Count{T}(IEnumerable{T})</c> when mapping from a source
    /// that does not expose <c>Count</c> or <c>Length</c> to a fixed-size target such as
    /// <c>T[]</c>, <c>Span{T}</c>, <c>ReadOnlySpan{T}</c>, <c>Memory{T}</c>, or
    /// <c>ReadOnlyMemory{T}</c>. When enabled, a growable buffer is used so the source is
    /// enumerated only once. When unset, the existing generator behavior is kept.
    /// </summary>
    public BooleanSetting PreventEnumerableCount { get; set; } = BooleanSetting.Undefined;

    /// <summary>
    /// Gets or sets the concrete type used when mapping to sequence-like collection interfaces
    /// such as <see cref="System.Collections.Generic.IEnumerable{T}"/>.
    /// When unset, <see cref="EnumerableConcreteTypeSetting.List"/> is used.
    /// </summary>
    public EnumerableConcreteTypeSetting EnumerableConcreteType { get; set; } = EnumerableConcreteTypeSetting.Undefined;

    /// <summary>
    /// Gets or sets how entries are inserted when mapping between dictionaries.
    /// When unset, <see cref="DictionaryAssignmentSetting.Indexer"/> is used.
    /// </summary>
    public DictionaryAssignmentSetting DictionaryAssignment { get; set; } = DictionaryAssignmentSetting.Undefined;

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

    /// <summary>
    /// Gets or sets a value indicating whether source property names are matched
    /// case-insensitively when pairing a target property or constructor parameter
    /// with a source property by name.
    /// </summary>
    public BooleanSetting CaseInsensitivePropertyMap { get; set; } = BooleanSetting.Undefined;

    /// <summary>
    /// Gets or sets a value indicating whether underscore characters are ignored
    /// when pairing a target property or constructor parameter with a source property
    /// by name.
    /// </summary>
    public BooleanSetting IgnoreUnderscoreForPropertyMap { get; set; } = BooleanSetting.Undefined;

    /// <summary>
    /// Gets or sets a value indicating whether enum member names or
    /// <see cref="System.ComponentModel.DescriptionAttribute"/> values are matched
    /// case-insensitively when mapping between enums or from <see cref="string"/> to an enum.
    /// </summary>
    public BooleanSetting CaseInsensitiveEnumMap { get; set; } = BooleanSetting.Undefined;

    /// <summary>
    /// Gets or sets how enum members are paired with string values when mapping
    /// between an enum and <see cref="string"/>. When unset, members are matched by name.
    /// </summary>
    public EnumStringMapSetting EnumStringMapSetting { get; set; } = EnumStringMapSetting.Undefined;

    /// <summary>
    /// Gets or sets how enum members are paired when mapping from one enum to another enum.
    /// When unset, members are matched by name.
    /// </summary>
    public EnumToEnumMapSetting EnumToEnumMapSetting { get; set; } = EnumToEnumMapSetting.Undefined;

    /// <summary>
    /// Gets or sets how identity mappings copy a type to itself.
    /// When unset, the original reference is returned.
    /// </summary>
    public IdentityMapDeepCopySetting IdentityMapDeepCopy { get; set; } = IdentityMapDeepCopySetting.Undefined;
}