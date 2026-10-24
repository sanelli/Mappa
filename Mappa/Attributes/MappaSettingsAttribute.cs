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
    /// Gets the sentinel value for unset depth settings.
    /// When used (or any negative value), the setting is ignored and previous values (if any) are used.
    /// </summary>
    public static short UndefinedDepth => -1;

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
    /// Gets or sets a value indicating whether well-known <c>MongoDB.Bson.ObjectId</c> type-pair
    /// mappings are enabled when performing mapping.
    /// It is not enabled by default.
    /// </summary>
    public BooleanSetting BsonWellKnownTypes { get; set; } = BooleanSetting.Undefined;

    /// <summary>
    /// Gets or sets a value indicating whether well-known <c>Google.Protobuf.WellKnownTypes</c>
    /// (Timestamp / Duration) type-pair mappings are enabled when performing mapping.
    /// It is not enabled by default.
    /// </summary>
    public BooleanSetting ProtobufWellKnownTypes { get; set; } = BooleanSetting.Undefined;

    /// <summary>
    /// Gets or sets a value indicating whether well-known <c>OneOf</c> / <c>OneOfBase</c>
    /// type-pair mappings (including the Option shape <c>OneOf&lt;T, None&gt;</c>) are enabled
    /// when performing mapping. It is not enabled by default; the consumer must reference
    /// the <c>OneOf</c> package. Exact-arm <c>T → OneOf&lt;…, T, …&gt;</c> via identity/implicit
    /// conversion remains available regardless of this setting.
    /// </summary>
    public BooleanSetting OneOfWellKnownTypes { get; set; } = BooleanSetting.Undefined;

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
    /// Gets or sets a value indicating whether a null source container is replaced with an empty
    /// target container instead of mapping to <c>null</c> or throwing. Disabled by default.
    /// When enabled, applies while mapping containers (types implementing <c>IEnumerable{T}</c>,
    /// excluding <c>Span</c>/<c>Memory</c> families): concrete types with an accessible
    /// parameterless constructor use <c>new T()</c>; arrays and allow-listed interfaces use
    /// <c>[]</c> or type-specific empty values (for example <c>ImmutableQueue{T}.Empty</c>).
    /// May be set at class or method level.
    /// </summary>
    public BooleanSetting ReplaceNullWithEmptyContainer { get; set; } = BooleanSetting.Undefined;

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
    /// Gets or sets a value indicating whether nested existing-method reuse may
    /// invoke a map method whose parameter type is a base type or interface of the
    /// required source type and whose return type is a derived type of the required
    /// target type. By default this behavior is disabled so that only exact (or
    /// nullability-relaxed) type matches are reused.
    /// </summary>
    public BooleanSetting CompatibleMapMethod { get; set; } = BooleanSetting.Undefined;

    /// <summary>
    /// Gets or sets a value indicating whether methods marked with
    /// <see cref="MappaTransformerAttribute"/> on types introduced via
    /// <see cref="MappaDependencyAttribute"/> or <see cref="MappaStaticDependencyAttribute"/>
    /// are discovered as value transformers. Disabled by default. When enabled, dependency
    /// transformers apply to every generated map in scope the same way as local
    /// <see cref="MappaTransformerAttribute"/> methods (subject to accessibility and
    /// <c>static</c>/instance rules). May be set at class or method level.
    /// </summary>
    public BooleanSetting MappaTransformerFromDependency { get; set; } = BooleanSetting.Undefined;

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
    /// Gets or sets a value indicating whether source properties that are explicitly
    /// implemented interface members may be read when mapping (via a cast to the
    /// declaring interface). Disabled by default.
    /// </summary>
    public BooleanSetting MapSourceExplicitInterfaceProperties { get; set; } = BooleanSetting.Undefined;

    /// <summary>
    /// Gets or sets a value indicating whether target properties that are explicitly
    /// implemented interface members may be written when mapping (via a cast to the
    /// declaring interface after construction). Disabled by default.
    /// </summary>
    public BooleanSetting MapTargetExplicitInterfaceProperties { get; set; } = BooleanSetting.Undefined;

    /// <summary>
    /// Gets or sets a value indicating whether accessible instance fields on the source type
    /// may be read when pairing with a target property, field, or constructor parameter by name
    /// (including nested paths and attribute-driven member names). Disabled by default.
    /// Private or protected source fields require <see cref="MappaAllowInaccessibleSourceMembersAttribute"/>.
    /// Compiler-generated backing fields and <c>static</c>/<c>const</c>/<c>ref</c>/<c>fixed</c> fields are never included.
    /// </summary>
    public BooleanSetting MapSourceFields { get; set; } = BooleanSetting.Undefined;

    /// <summary>
    /// Gets or sets a value indicating whether accessible instance fields on the target type
    /// may be written or initialized when pairing with a source property or field by name
    /// (including nested paths, post-constructor assignment, and attribute-driven member names).
    /// Disabled by default. Private or protected target fields require
    /// <see cref="MappaAllowInaccessibleTargetMembersAttribute"/> (subject to
    /// <see cref="MappaAllowInaccessibleTargetMembersAttribute.AllowFields"/>).
    /// Compiler-generated backing fields, <c>static</c>/<c>const</c>/<c>ref</c>/<c>fixed</c>,
    /// and <c>readonly</c> fields are never assigned.
    /// </summary>
    public BooleanSetting MapTargetFields { get; set; } = BooleanSetting.Undefined;

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

    /// <summary>
    /// Gets or sets a value indicating whether already-mapped reference-type instances
    /// are reused when the same source instance appears again in the object graph.
    /// Requires a <see cref="MappaContext"/> parameter on the map method.
    /// It is not enabled by default.
    /// </summary>
    public BooleanSetting ReferenceReusing { get; set; } = BooleanSetting.Undefined;

    /// <summary>
    /// Gets or sets the maximum nesting depth allowed while executing a mapping.
    /// Use <see cref="UndefinedDepth"/> (or any negative value) to ignore this setting and use previous values (if any).
    /// When fully unset, the effective default is <c>0</c> (no maximum depth).
    /// A positive value limits nested object mapping depth (<c>1</c> means root plus one nested level).
    /// Requires a <see cref="MappaContext"/> parameter on the map method.
    /// </summary>
    public short MaxRuntimeDepth { get; set; } = UndefinedDepth;

    /// <summary>
    /// Gets or sets the maximum nesting depth allowed while the source generator discovers mapping strategies.
    /// This setting is used only by the generator and is not emitted into generated mapping code.
    /// Use <see cref="UndefinedDepth"/> (or any negative value) to ignore this setting and use previous values (if any).
    /// When fully unset, the effective default is <c>50</c>.
    /// When the effective value is <c>0</c>, no compile-time depth limit is applied.
    /// </summary>
    public short MaxCompileTimeDepth { get; set; } = UndefinedDepth;

    /// <summary>
    /// Gets or sets a value indicating whether the source generator should break compile-time
    /// mapping cycles by synthesizing private map methods for the cycling type pair and invoking
    /// them at the cycle edge, instead of reporting a mapping-cycle error.
    /// This setting is used only by the generator and is not emitted into generated mapping code.
    /// It is not enabled by default.
    /// </summary>
    public BooleanSetting BreakCompileTimeCycles { get; set; } = BooleanSetting.Undefined;

    /// <summary>
    /// Gets or sets how enum members are paired with string values when mapping
    /// between an enum and <see cref="string"/> inside an <c>IQueryable</c> projection.
    /// When unset, <see cref="EnumStringMapSetting"/> is used.
    /// </summary>
    public EnumStringMapSetting ProjectionEnumStringMapSetting { get; set; } = EnumStringMapSetting.Undefined;

    /// <summary>
    /// Gets or sets how enum members are paired when mapping from one enum to another enum
    /// inside an <c>IQueryable</c> projection.
    /// When unset, <see cref="EnumToEnumMapSetting"/> is used.
    /// </summary>
    public EnumToEnumMapSetting ProjectionEnumToEnumMapSetting { get; set; } = EnumToEnumMapSetting.Undefined;

    /// <summary>
    /// Gets or sets a value indicating whether enum member names or
    /// <see cref="System.ComponentModel.DescriptionAttribute"/> values are matched
    /// case-insensitively when mapping between enums or from <see cref="string"/> to an enum
    /// inside an <c>IQueryable</c> projection.
    /// When unset, <see cref="CaseInsensitiveEnumMap"/> is used.
    /// </summary>
    public BooleanSetting ProjectionCaseInsensitiveEnumMap { get; set; } = BooleanSetting.Undefined;

    /// <summary>
    /// Gets or sets the format when using <see cref="Guid.ToString(string)"/> or
    /// <see cref="Guid.ParseExact(string,string)"/> inside an <c>IQueryable</c> projection.
    /// When unset, <see cref="GuidFormat"/> is used.
    /// </summary>
    public string? ProjectionGuidFormat { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="CultureInfo"/> to use when converting to string or parsing
    /// from string inside an <c>IQueryable</c> projection.
    /// When unset, <see cref="CultureInfoSetting"/> is used.
    /// </summary>
    public CultureInfoSetting ProjectionCultureInfoSetting { get; set; } = CultureInfoSetting.Undefined;

    /// <summary>
    /// Gets or sets the culture name when <see cref="ProjectionCultureInfoSetting"/> is
    /// <see cref="CultureInfoSetting.UserDefined"/> inside an <c>IQueryable</c> projection.
    /// When unset, <see cref="CultureName"/> is used.
    /// </summary>
    public string? ProjectionCultureName { get; set; }
}