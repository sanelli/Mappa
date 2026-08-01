# Mappa features
Mappa is a compile-time source generator for `C#` that generates mapping code between types. This page is the complete catalog of features Mappa provides.

Use this page to discover what Mappa can do. For hands-on examples, see the [tutorial](./tutorial.md). For attribute and setting details, see [Mappa attributes](./mappa-attributes.md). For how the generator chooses a mapping strategy, see the [Mappa algorithm](./mappa-generator-algorithm.md). For runnable code, browse [Mappa.Samples](../Mappa.Samples).

## Core capabilities

| Feature | Description | Learn more |
|---------|-------------|------------|
| Compile-time code generation | Partial classes tagged with `[Mappa]` receive generated map method bodies at build time. | [Tutorial](./tutorial.md), [Algorithm — method eligibility](./mappa-generator-algorithm.md) |
| Instance mappers | Map methods on instance partial classes. | [Tutorial — Define the mapper](./tutorial.md) |
| Static mappers | Map methods on static partial classes. | [MapMethodStrategyMapper.cs](../Mappa.Samples/MapMethodStrategyMapper.cs) |
| Extension-method mappers | Map methods declared as extension methods. | [ExtensionMethodMapper.cs](../Mappa.Samples/ExtensionMethodMapper.cs) |
| `MappaContext` parameter | Optional second parameter on map methods for contextual input and output. | [Mappa attributes — MappaAssignFromContext](./mappa-attributes.md), [MappaAssignFromContextAttributeMapper.cs](../Mappa.Samples/MappaAssignFromContextAttributeMapper.cs) |
| `in` parameters and `params` | Map methods may use `in` ref modifiers and `params` arrays. | [ParamsAndInMapper.cs](../Mappa.Samples/ParamsAndInMapper.cs) |
| Automatic nested mapping | Missing type-pair mappings are generated on demand when mapping structured types, collections, and tuples. | [Documentation hub](./README.md) |
| IQueryable projection | Map methods with `IQueryable<TSource>` → `IQueryable<TTarget>` emit provider-translatable `Select` projections (no new attributes). Not compatible with Native AOT (`[RequiresDynamicCode]`). | [Tutorial — IQueryable projection](./tutorial.md#iqueryable-projection), [Algorithm](./mappa-generator-algorithm.md#7a-iqueryable-projection-strategy), [IQueryableProjectionMapper.cs](../Mappa.Samples/IQueryableProjectionMapper.cs) |
| Nested property paths on mapping attributes | Dot-separated `TargetPropertyName` / `SourcePropertyName` paths (for example `"Address.City"` ← `"Location.Address.City"`) on `MappaUseProperty`, invoke, constant, context, and ignore attributes. | [Nested property paths](./mappa-attributes.md#nested-property-paths), [Algorithm](./mappa-generator-algorithm.md#nested-property-paths), [Tutorial](./tutorial.md#nested-property-paths), [NestedPropertyPathAttributeMapper.cs](../Mappa.Samples/NestedPropertyPathAttributeMapper.cs) |
| Native AOT | Generated code works with [Native AOT](https://learn.microsoft.com/dotnet/core/deploying/native-aot) deployment. | [Mappa.Samples.Aot](../Mappa.Samples.Aot), [Mappa.Samples README — Native AOT](../Mappa.Samples/README.md) |
| Base-class method inheritance | Map methods and `[MappaDependency]` members on accessible mapper base classes are considered during resolution. | [Algorithm — existing-method pre-step](./mappa-generator-algorithm.md), [MapMethodStrategyWithInheritedMapMethodMapper.cs](../Mappa.Samples/MapMethodStrategyWithInheritedMapMethodMapper.cs) |
| Base-class and interface properties | Properties declared on source/target base classes and interfaces participate in structured-type mapping. | [MapWithPropertiesOnBaseClassesMapper.cs](../Mappa.Samples/MapWithPropertiesOnBaseClassesMapper.cs) |

## Automatic mapping strategies

The generator resolves mappings using a [detector chain](./mappa-generator-algorithm.md#detector-chain-order). Root partial methods run the chain directly; nested mappings may first reuse an existing map method (see [Existing-method pre-step](#existing-method-pre-step)).

| Strategy | Description | Algorithm | Sample |
|----------|-------------|-----------|--------|
| Identity | Same type, implicit conversions, and boxing to `object`. `IdentityMapDeepCopy` controls shallow, deep, and nested same-type copying. | [§1 Identity](./mappa-generator-algorithm.md#1-identity-strategy) | [IdentityStrategyMapper.cs](../Mappa.Samples/IdentityStrategyMapper.cs), [IdentityMapDeepCopyMapper.cs](../Mappa.Samples/IdentityMapDeepCopyMapper.cs) |
| Nullable | Nullable value-type and reference-type unwrapping and wrapping. | [§2 Nullable](./mappa-generator-algorithm.md#2-nullable-strategy) | [NullableToNullableMapper.cs](../Mappa.Samples/NullableToNullableMapper.cs), [ReferenceNullableToReferenceNullableMapper.cs](../Mappa.Samples/ReferenceNullableToReferenceNullableMapper.cs) |
| Polymorphic | Runtime source-type dispatch to a concrete target type. | [§3 Polymorphic](./mappa-generator-algorithm.md#3-polymorphic-mapping) | [PolymorphismMappers.cs](../Mappa.Samples/PolymorphismMappers.cs) |
| Enum | Enum-to-enum, enum-to-integral, enum-to-string, integral-to-enum, and string-to-enum. | [§4 Enum](./mappa-generator-algorithm.md#4-enum-strategy) | [EnumToEnumMapper.cs](../Mappa.Samples/EnumToEnumMapper.cs), [NumericValueEnumToEnumMapper.cs](../Mappa.Samples/NumericValueEnumToEnumMapper.cs), [DescriptionEnumToEnumMapper.cs](../Mappa.Samples/DescriptionEnumToEnumMapper.cs), [DescriptionEnumToStringMapper.cs](../Mappa.Samples/DescriptionEnumToStringMapper.cs), [DescriptionStringToEnumMapper.cs](../Mappa.Samples/DescriptionStringToEnumMapper.cs), [CaseInsensitiveEnumMapper.cs](../Mappa.Samples/CaseInsensitiveEnumMapper.cs), [CaseInsensitiveEnumToEnumMapper.cs](../Mappa.Samples/CaseInsensitiveEnumToEnumMapper.cs), [EnumToIntegralMapper.cs](../Mappa.Samples/EnumToIntegralMapper.cs), [EnumToStringMapper.cs](../Mappa.Samples/EnumToStringMapper.cs), [IntegralToEnumMapper.cs](../Mappa.Samples/IntegralToEnumMapper.cs), [StringToEnumMapper.cs](../Mappa.Samples/StringToEnumMapper.cs), [EnumMappingConfigurationMappers.cs](../Mappa.Samples/EnumMappingConfigurationMappers.cs) |
| String | `Parse` and `ToString` for date/time types, `Guid`, `Uri`, `TimeSpan`, and numeric types. | [§5 String](./mappa-generator-algorithm.md#5-string-strategy) | [InvokeParseMapper.cs](../Mappa.Samples/InvokeParseMapper.cs), [InvokeToStringMapper.cs](../Mappa.Samples/InvokeToStringMapper.cs), [StringToSystemEntitiesMapper.cs](../Mappa.Samples/StringToSystemEntitiesMapper.cs) |
| Date & time | Cross-conversions among `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly`, `TimeSpan`, and numeric tick representations. | [§6 Date & time](./mappa-generator-algorithm.md#6-date--time-strategy) | [DateAndTimeMapper.cs](../Mappa.Samples/DateAndTimeMapper.cs) |
| IQueryable projection | `IQueryable<TSource>` → `IQueryable<TTarget>` emits deferred `Select` expression trees for ORM providers (EF Core). Signature-driven; no new attributes. Not compatible with Native AOT (`[RequiresDynamicCode]`). | [§7a IQueryable projection](./mappa-generator-algorithm.md#7a-iqueryable-projection-strategy) | [IQueryableProjectionMapper.cs](../Mappa.Samples/IQueryableProjectionMapper.cs) |
| Container | Arrays, spans, lists, sets, queues, stacks, concurrent, immutable, frozen, and custom collection implementations. `EnumerableConcreteType` selects `List<T>` or `T[]` buffers for sequence-like interface targets. `PreventEnumerableCount` can avoid `Enumerable.Count` for fixed-size targets when the source length is unknown. | [§7 Container](./mappa-generator-algorithm.md#7-container-strategy) | [CollectionToCollectionMapper.cs](../Mappa.Samples/CollectionToCollectionMapper.cs), [EnumerableConcreteTypeMapper.cs](../Mappa.Samples/EnumerableConcreteTypeMapper.cs), [PreventEnumerableCountMapper.cs](../Mappa.Samples/PreventEnumerableCountMapper.cs), [ReadOnlyTargetCollectionMapper.cs](../Mappa.Samples/ReadOnlyTargetCollectionMapper.cs) |
| Dictionary | Dictionary-to-dictionary mapping across standard and custom dictionary types. `DictionaryAssignment` selects indexer or `Add` insertion. | [§7 Container](./mappa-generator-algorithm.md#7-container-strategy) | [DictionaryToDictionaryMapper.cs](../Mappa.Samples/DictionaryToDictionaryMapper.cs), [DictionaryAssignmentMapper.cs](../Mappa.Samples/DictionaryAssignmentMapper.cs) |
| Tuple | Reference tuples and value tuples with element-wise conversion. | [§8 Tuple](./mappa-generator-algorithm.md#8-tuple-strategy) | [TupleToTupleMapper.cs](../Mappa.Samples/TupleToTupleMapper.cs) |
| Guid | `Guid` to and from `byte[]`, `Span<byte>`, `ReadOnlySpan<byte>`, `Memory<byte>`, and `ReadOnlyMemory<byte>`. | [§9 Guid](./mappa-generator-algorithm.md#9-guid-strategy) | [GuidStrategyMapper.cs](../Mappa.Samples/GuidStrategyMapper.cs) |
| Constructor | Structured types via `[MappaObjectFactory]`, parameterized constructor, parameterless constructor with property initializers, or `[MapperConstructor]`. Factories are preferred over `new` when registered for the exact target type. | [§10 Constructor](./mappa-generator-algorithm.md#10-constructor-strategy) | [InvokeConstructorStrategyMapper.cs](../Mappa.Samples/InvokeConstructorStrategyMapper.cs), [InvokeEmptyConstructorStrategyMapper.cs](../Mappa.Samples/InvokeEmptyConstructorStrategyMapper.cs), [InvokeMappingConstructorStrategyMapper.cs](../Mappa.Samples/InvokeMappingConstructorStrategyMapper.cs), [InvokeEmptyConstructorOnPropertyMapper.cs](../Mappa.Samples/InvokeEmptyConstructorOnPropertyMapper.cs), [MappaObjectFactoryMapper.cs](../Mappa.Samples/MappaObjectFactoryMapper.cs) |

### Existing-method pre-step

Before the detector chain, nested mappings may invoke an existing map method on the mapper, a `[MappaDependency]` type, a `[MappaStaticDependency]` type, or a matching polymorphic method. See [Algorithm — existing-method pre-step](./mappa-generator-algorithm.md#existing-method-pre-step-nested-mappings-only).

| Feature | Sample |
|---------|--------|
| Hand-written map methods on the mapper | [MapMethodStrategyMapper.cs](../Mappa.Samples/MapMethodStrategyMapper.cs), [MapMethodStrategyWithUserCustomInstanceMethodMapper.cs](../Mappa.Samples/MapMethodStrategyWithUserCustomInstanceMethodMapper.cs), [MapMethodStrategyWithUserCustomStaticMethodMapper.cs](../Mappa.Samples/MapMethodStrategyWithUserCustomStaticMethodMapper.cs), [RelaxedNullabilityMethodMapMapper.cs](../Mappa.Samples/RelaxedNullabilityMethodMapMapper.cs), [CompatibleMapMethodMapper.cs](../Mappa.Samples/CompatibleMapMethodMapper.cs) |
| Polymorphic method resolution for nested properties | [PolymorphicMethodMapMapper.cs](../Mappa.Samples/PolymorphicMethodMapMapper.cs) |
| Compatible map-method reuse (base/interface source, derived return) | [CompatibleMapMethodMapper.cs](../Mappa.Samples/CompatibleMapMethodMapper.cs) |
| Relaxed nullability when reusing existing map methods | [Algorithm — existing-method pre-step](./mappa-generator-algorithm.md#existing-method-pre-step-nested-mappings-only), [RelaxedNullabilityMethodMapMapper.cs](../Mappa.Samples/RelaxedNullabilityMethodMapMapper.cs) |

## Attributes

| Attribute | Description | Reference | Tutorial | Sample |
|-----------|-------------|-----------|----------|--------|
| `Mappa` | Marks a partial class for source generation. | [Mappa attributes](./mappa-attributes.md) | [Tutorial — Define the mapper](./tutorial.md) | All samples |
| `MappaDependencyInjection` | Marks a partial registrar that emits an `IServiceCollection` registration method for all same-assembly `[Mappa]` mappers. | [MappaDependencyInjection](./mappa-attributes.md#mappadependencyinjection) | [MappaDependencyInjection attribute](./tutorial.md#mappadependencyinjection-attribute) | [MappaDependencyInjectionRegistrar.cs](../Mappa.Samples/MappaDependencyInjectionRegistrar.cs), [MappaDependencyInjectionMapper.cs](../Mappa.Samples/MappaDependencyInjectionMapper.cs) |
| `MappaIgnore` | Excludes a method from mapping resolution. | [Mappa attributes](./mappa-attributes.md) | [Ignoring methods via MappaIgnore](./tutorial.md#ignoring-methods-via-mappaignore) | [MappaIgnoreMappers.cs](../Mappa.Samples/MappaIgnoreMappers.cs) |
| `MappaDependency` | Field or property whose type provides mapping methods. | [MappaDependency and MappaStaticDependency](./mappa-attributes.md#mappadependency-and-mappastaticdependency) | [MappaDependency and MappaStaticDependency](./tutorial.md#mappadependency-and-mappastaticdependency) | [MapMethodStrategyWithDependencyMapper.cs](../Mappa.Samples/MapMethodStrategyWithDependencyMapper.cs), [MappaDependencyProtobufMapper.cs](../Mappa.Samples/MappaDependencyProtobufMapper.cs) |
| `MappaStaticDependency` | Static helper class whose methods are available as dependencies. | [MappaDependency and MappaStaticDependency](./mappa-attributes.md#mappadependency-and-mappastaticdependency) | [MappaDependency and MappaStaticDependency](./tutorial.md#mappadependency-and-mappastaticdependency) | [MapMethodStrategyWithDependencyMapper.cs](../Mappa.Samples/MapMethodStrategyWithDependencyMapper.cs) |
| `MappaSettings` | Class- or method-level mapping behaviour (culture, formats, styles, and feature toggles). | [MappaSettings](./mappa-attributes.md#mappasettings) | [MappaSettings attribute](./tutorial.md#mappasettings-attribute) | See [MappaSettings](#mappasettings) |
| `MappaUseProperty` | Overrides name-based source property pairing for a target member; supports [nested property paths](./mappa-attributes.md#nested-property-paths). | [MappaUseProperty](./mappa-attributes.md#mappauseproperty) | [MappaUseProperty attribute](./tutorial.md#mappauseproperty-attribute), [Nested property paths](./tutorial.md#nested-property-paths) | [MappaUsePropertyAttributeMapper.cs](../Mappa.Samples/MappaUsePropertyAttributeMapper.cs), [NestedPropertyPathAttributeMapper.cs](../Mappa.Samples/NestedPropertyPathAttributeMapper.cs) |
| `MappaIgnoreTargetProperty` | Excludes a target property from empty-constructor mapping; supports [nested property paths](./mappa-attributes.md#nested-property-paths). | [MappaIgnoreTargetProperty](./mappa-attributes.md#mappaignoretargetproperty) | [MappaIgnoreTargetProperty attribute](./tutorial.md#mappaignoretargetproperty-attribute) | [MappaIgnoreTargetPropertyAttributeMapper.cs](../Mappa.Samples/MappaIgnoreTargetPropertyAttributeMapper.cs), [NestedPropertyPathAttributeMapper.cs](../Mappa.Samples/NestedPropertyPathAttributeMapper.cs) |
| `MappaMustMapTargetProperty` | Requires listed (or all) non-required target properties to be mapped on the empty-constructor path; otherwise **MP00065**. | [MappaMustMapTargetProperty](./mappa-attributes.md#mappamustmaptargetproperty) | [MappaMustMapTargetProperty attribute](./tutorial.md#mappamustmaptargetproperty-attribute) | [MappaMustMapTargetPropertyAttributeMapper.cs](../Mappa.Samples/MappaMustMapTargetPropertyAttributeMapper.cs) |
| `MappaAllowInaccessibleSourceMembers` | Opt-in to read private/protected source properties via `UnsafeAccessor` (all or named flat list). | [MappaAllowInaccessibleSourceMembers](./mappa-attributes.md#mappaallowinaccessiblesourcemembers) | [MappaAllowInaccessibleSourceMembers / TargetMembers](./tutorial.md#mappaallowinaccessiblesourcemembers--targetmembers) | [InaccessibleMembersMapper.cs](../Mappa.Samples/InaccessibleMembersMapper.cs) |
| `MappaAllowInaccessibleTargetMembers` | Opt-in to write private/protected target properties and/or invoke inaccessible constructors via `UnsafeAccessor`. | [MappaAllowInaccessibleTargetMembers](./mappa-attributes.md#mappaallowinaccessibletargetmembers) | [MappaAllowInaccessibleSourceMembers / TargetMembers](./tutorial.md#mappaallowinaccessiblesourcemembers--targetmembers) | [InaccessibleMembersMapper.cs](../Mappa.Samples/InaccessibleMembersMapper.cs) |
| `MappaAssignFromContext` | Assigns a value from `MappaContext` to a target member; supports [nested property paths](./mappa-attributes.md#nested-property-paths). | [MappaAssignFromContext](./mappa-attributes.md#mappaassignfromcontext) | [MappaAssignFromContext attribute](./tutorial.md#mappaassignfromcontext-attribute) | [MappaAssignFromContextAttributeMapper.cs](../Mappa.Samples/MappaAssignFromContextAttributeMapper.cs), [NestedPropertyPathAttributeMapper.cs](../Mappa.Samples/NestedPropertyPathAttributeMapper.cs) |
| `MappaAssignToContext` | Stores a mapped target member value into `MappaContext`; supports [nested property paths](./mappa-attributes.md#nested-property-paths). | [MappaAssignToContext](./mappa-attributes.md#mappaassigntocontext) | [MappaAssignToContext attribute](./tutorial.md#mappaassigntocontext-attribute) | [MappaAssignToContextAttributeMapper.cs](../Mappa.Samples/MappaAssignToContextAttributeMapper.cs), [NestedPropertyPathAttributeMapper.cs](../Mappa.Samples/NestedPropertyPathAttributeMapper.cs) |
| `MappaInvokeMethod` | Forces a target member to be mapped by invoking a named method; supports [nested property paths](./mappa-attributes.md#nested-property-paths). | [MappaInvokeMethodAttribute](./mappa-attributes.md#mappainvokemethodattribute) | [MappaInvokeMethod attribute](./tutorial.md#mappainvokemethod-attribute) | [MappaInvokeMethodAttributeMappers.cs](../Mappa.Samples/MappaInvokeMethodAttributeMappers.cs), [NestedPropertyPathAttributeMapper.cs](../Mappa.Samples/NestedPropertyPathAttributeMapper.cs) |
| `MappaBeforeMap` | Invokes a named hook immediately before the generated root mapping body. | [MappaBeforeMap and MappaAfterMap](./mappa-attributes.md#mappabeforemap-and-mappaaftermap) | [MappaBeforeMap and MappaAfterMap attributes](./tutorial.md#mappabeforemap-and-mappaaftermap-attributes) | [MappaBeforeAfterMapHooksAttributeMapper.cs](../Mappa.Samples/MappaBeforeAfterMapHooksAttributeMapper.cs) |
| `MappaAfterMap` | Invokes a named hook immediately after the generated root mapping body and before returning the target. | [MappaBeforeMap and MappaAfterMap](./mappa-attributes.md#mappabeforemap-and-mappaaftermap) | [MappaBeforeMap and MappaAfterMap attributes](./tutorial.md#mappabeforemap-and-mappaaftermap-attributes) | [MappaBeforeAfterMapHooksAttributeMapper.cs](../Mappa.Samples/MappaBeforeAfterMapHooksAttributeMapper.cs) |
| `MappaObjectFactory` | Forces construction of a target type via a named factory method instead of `new`. | [MappaObjectFactory](./mappa-attributes.md#mappaobjectfactory) | [MappaObjectFactory attribute](./tutorial.md#mappaobjectfactory-attribute) | [MappaObjectFactoryMapper.cs](../Mappa.Samples/MappaObjectFactoryMapper.cs) |
| `MappaAssignFromConstant` | Assigns a compile-time constant to a target member; supports [nested property paths](./mappa-attributes.md#nested-property-paths). | [MappaAssignFromConstant](./mappa-attributes.md#mappaassignfromconstant) | [MappaAssignFromConstant attribute](./tutorial.md#mappaassignfromconstant-attribute) | [MappaAssignFromConstantAttributeMapper.cs](../Mappa.Samples/MappaAssignFromConstantAttributeMapper.cs), [NestedPropertyPathAttributeMapper.cs](../Mappa.Samples/NestedPropertyPathAttributeMapper.cs) |
| `MappaTypeMapping` | Maps a polymorphic source type to a concrete target type. | [MappaTypeMapping and MappaTypeMappingDefault](./mappa-attributes.md#mappatypemapping-and-mappatypemappingdefault) | [Polymorphism support](./tutorial.md#polymorphism-support) | [PolymorphismMappers.cs](../Mappa.Samples/PolymorphismMappers.cs) |
| `MappaTypeMappingDefault` | Default behaviour when no `[MappaTypeMapping]` matches the runtime source type. | [MappaTypeMapping and MappaTypeMappingDefault](./mappa-attributes.md#mappatypemapping-and-mappatypemappingdefault) | [Polymorphism support](./tutorial.md#polymorphism-support) | [PolymorphismMappers.cs](../Mappa.Samples/PolymorphismMappers.cs) |
| `MappaMapEnumMember` | Configures explicit enum↔integral, enum↔string, or enum↔enum member pairings. | [MappaMapEnumMember, MappaMapEnumIgnore, and MappaMapEnumDefault](./mappa-attributes.md#mappamapenummember-mappamapenumignore-and-mappamapenumdefault) | [Enum mapping configuration attributes](./tutorial.md#enum-mapping-configuration-attributes) | [EnumMappingConfigurationMappers.cs](../Mappa.Samples/EnumMappingConfigurationMappers.cs) |
| `MappaMapEnumIgnore` | Excludes a specific enum member from mapping. | [MappaMapEnumMember, MappaMapEnumIgnore, and MappaMapEnumDefault](./mappa-attributes.md#mappamapenummember-mappamapenumignore-and-mappamapenumdefault) | [Enum mapping configuration attributes](./tutorial.md#enum-mapping-configuration-attributes) | [EnumMappingConfigurationMappers.cs](../Mappa.Samples/EnumMappingConfigurationMappers.cs) |
| `MappaMapEnumDefault` | Configures fallback behaviour when an enum value cannot be mapped. | [MappaMapEnumMember, MappaMapEnumIgnore, and MappaMapEnumDefault](./mappa-attributes.md#mappamapenummember-mappamapenumignore-and-mappamapenumdefault) | [Enum mapping configuration attributes](./tutorial.md#enum-mapping-configuration-attributes) | [EnumMappingConfigurationMappers.cs](../Mappa.Samples/EnumMappingConfigurationMappers.cs) |
| `MappaContext` | Key/value bag passed to map methods for contextual input and output. | [Mappa attributes](./mappa-attributes.md) | [MappaAssignFromContext attribute](./tutorial.md#mappaassignfromcontext-attribute) | [MappaAssignFromContextAttributeMapper.cs](../Mappa.Samples/MappaAssignFromContextAttributeMapper.cs) |

## MappaSettings

Settings may be applied at the mapper class level or overridden on individual map methods. Method-level settings take precedence over class-level settings. See [MappaSettings attribute](./tutorial.md#mappasettings-attribute) and the full property list in [Mappa attributes — MappaSettings](./mappa-attributes.md#mappasettings).

| Group | Properties | Sample |
|-------|------------|--------|
| Culture | `CultureInfoSetting`, `CultureName` | [InvokeParseMapper.cs](../Mappa.Samples/InvokeParseMapper.cs), [InvokeToStringMapper.cs](../Mappa.Samples/InvokeToStringMapper.cs) |
| Date/time and Guid formatting | `DateTimeFormat`, `DateTimeOffsetFormat`, `DateOnlyFormat`, `TimeOnlyFormat`, `TimeSpanFormat`, `GuidFormat` | [InvokeParseMapper.cs](../Mappa.Samples/InvokeParseMapper.cs), [InvokeToStringMapper.cs](../Mappa.Samples/InvokeToStringMapper.cs), [StringToSystemEntitiesMapper.cs](../Mappa.Samples/StringToSystemEntitiesMapper.cs) |
| Numeric formatting | `ByteFormat`, `SByteFormat`, `ShortFormat`, `UShortFormat`, `IntFormat`, `UIntFormat`, `LongFormat`, `ULongFormat`, `DecimalFormat`, `FloatFormat`, `DoubleFormat` | [InvokeToStringMapper.cs](../Mappa.Samples/InvokeToStringMapper.cs) |
| Date/time parse styles | `DateTimeStyle`, `DateTimeOffsetStyle`, `DateOnlyStyle`, `TimeOnlyStyle`, `GlobalDateTimeStyle` | [InvokeParseMapper.cs](../Mappa.Samples/InvokeParseMapper.cs) |
| Numeric parse styles | `ByteStyle`, `SByteStyle`, `ShortStyle`, `UShortStyle`, `IntStyle`, `UIntStyle`, `LongStyle`, `ULongStyle`, `DecimalStyle`, `FloatStyle`, `DoubleStyle`, `GlobalNumberStyle` | [InvokeParseMapper.cs](../Mappa.Samples/InvokeParseMapper.cs) |
| Property name matching | `CaseInsensitivePropertyMap`, `IgnoreUnderscoreForPropertyMap` | [PropertyMapNameSettingsMapper.cs](../Mappa.Samples/PropertyMapNameSettingsMapper.cs) |
| Enum matching (case-insensitive) | `CaseInsensitiveEnumMap` | [CaseInsensitiveEnumMapper.cs](../Mappa.Samples/CaseInsensitiveEnumMapper.cs), [CaseInsensitiveEnumToEnumMapper.cs](../Mappa.Samples/CaseInsensitiveEnumToEnumMapper.cs) |
| Enum ↔ string matching (Description) | `EnumStringMapSetting` | [DescriptionEnumToStringMapper.cs](../Mappa.Samples/DescriptionEnumToStringMapper.cs), [DescriptionStringToEnumMapper.cs](../Mappa.Samples/DescriptionStringToEnumMapper.cs) |
| Enum-to-enum matching | `EnumToEnumMapSetting` | [NumericValueEnumToEnumMapper.cs](../Mappa.Samples/NumericValueEnumToEnumMapper.cs), [DescriptionEnumToEnumMapper.cs](../Mappa.Samples/DescriptionEnumToEnumMapper.cs) |
| Identity same-type copy | `IdentityMapDeepCopy` | [IdentityMapDeepCopyMapper.cs](../Mappa.Samples/IdentityMapDeepCopyMapper.cs) |
| Collection performance | `FastCollections` | [FastCollectionToCollectionMapper.cs](../Mappa.Samples/FastCollectionToCollectionMapper.cs) |
| Interface collection buffer | `EnumerableConcreteType` | [EnumerableConcreteTypeMapper.cs](../Mappa.Samples/EnumerableConcreteTypeMapper.cs) |
| Dictionary entry insertion | `DictionaryAssignment` | [DictionaryAssignmentMapper.cs](../Mappa.Samples/DictionaryAssignmentMapper.cs) |
| Custom container construction | `ContainerCapacityConstructors` | [ContainersWithCapacityConstructorMapper.cs](../Mappa.Samples/ContainersWithCapacityConstructorMapper.cs) |
| Fixed-size target without `Enumerable.Count` | `PreventEnumerableCount` | [PreventEnumerableCountMapper.cs](../Mappa.Samples/PreventEnumerableCountMapper.cs) |
| Polymorphic method resolution | `PolymorphicMapMethodWithMatchingDefaultAttribute` | [PolymorphicMethodMapMapper.cs](../Mappa.Samples/PolymorphicMethodMapMapper.cs) |
| Compatible map-method reuse | `CompatibleMapMethod` | [CompatibleMapMethodMapper.cs](../Mappa.Samples/CompatibleMapMethodMapper.cs) |
| Protobuf optional fields | `ProtobufOptional` | [ProtobufOptionalMapper.cs](../Mappa.Samples/ProtobufOptionalMapper.cs) |
| Generated code warnings | `PragmaWarning` | [PragmaWarningSettingMapper.cs](../Mappa.Samples/PragmaWarningSettingMapper.cs) |

## Global configuration (.editorconfig)

All `MappaSettings` properties can also be configured globally via `.editorconfig` keys prefixed with `mappa.` (for example `mappa.datetimeformat`, `mappa.fastcollections`). The `mappa.debug` key enables generator debug diagnostics. See the full key table in [Mappa attributes — .editorconfig](./mappa-attributes.md#editorconfig).

No dedicated sample demonstrates `.editorconfig`-only configuration; settings are demonstrated via `[MappaSettings]` in [Mappa.Samples](../Mappa.Samples).

## Polymorphism behaviors

`MappaTypeMappingDefaultBehavior` values supported at runtime (see [MappaTypeMapping and MappaTypeMappingDefault](./mappa-attributes.md#mappatypemapping-and-mappatypemappingdefault)):

| Behavior | Description | Sample |
|----------|-------------|--------|
| `Throw` | Throws `ArgumentOutOfRangeException`, or a custom exception type when specified. | [PolymorphismMappers.cs](../Mappa.Samples/PolymorphismMappers.cs) |
| `Default` | Returns `default` for the target type. | [PolymorphismMappers.cs](../Mappa.Samples/PolymorphismMappers.cs) |
| `Null` | Returns `null`. | [PolymorphismMappers.cs](../Mappa.Samples/PolymorphismMappers.cs) |
| `MapSourceType` | Maps the runtime source type to the method target type (or an explicit type in the attribute). | [PolymorphismMappers.cs](../Mappa.Samples/PolymorphismMappers.cs) |
| `InvokeMethod` | Invokes a named method (static or instance, on the mapper or an external type) to perform the mapping. | [PolymorphismMappers.cs](../Mappa.Samples/PolymorphismMappers.cs) |

`Undefined` is reserved and should not be used.

## Enum mapping default behaviours

`MappaMapEnumDefaultBehavior` values supported via `[MappaMapEnumDefault]` (see [MappaMapEnumMember, MappaMapEnumIgnore, and MappaMapEnumDefault](./mappa-attributes.md#mappamapenummember-mappamapenumignore-and-mappamapenumdefault)):

| Behavior | Description | Sample |
|----------|-------------|--------|
| `Throw` | Throws `ArgumentOutOfRangeException` when an enum value cannot be mapped (default). | [EnumMappingConfigurationMappers.cs](../Mappa.Samples/EnumMappingConfigurationMappers.cs) |
| `UseDefaultValue` | Returns the enum, integral, or string default provided in the attribute. | [EnumMappingConfigurationMappers.cs](../Mappa.Samples/EnumMappingConfigurationMappers.cs) |

## Dependencies and custom mapping methods

| Feature | Description | Sample |
|---------|-------------|--------|
| Custom instance map methods | Hand-written instance methods coexist with generated partial maps. | [MapMethodStrategyWithUserCustomInstanceMethodMapper.cs](../Mappa.Samples/MapMethodStrategyWithUserCustomInstanceMethodMapper.cs) |
| Custom static map methods | Hand-written static methods coexist with generated partial maps. | [MapMethodStrategyWithUserCustomStaticMethodMapper.cs](../Mappa.Samples/MapMethodStrategyWithUserCustomStaticMethodMapper.cs) |
| `[MappaDependency]` fields and properties | Instance dependencies whose types provide map methods. | [MapMethodStrategyWithDependencyMapper.cs](../Mappa.Samples/MapMethodStrategyWithDependencyMapper.cs) |
| `[MappaStaticDependency]` helper types | Static helper classes whose methods are available during resolution. | [MapMethodStrategyWithDependencyMapper.cs](../Mappa.Samples/MapMethodStrategyWithDependencyMapper.cs) |
| Inherited dependency members | Dependency fields, properties, and methods on mapper or dependency base classes. | [MapMethodStrategyWithInheritedMapMethodMapper.cs](../Mappa.Samples/MapMethodStrategyWithInheritedMapMethodMapper.cs) |
| Protobuf mapper dependency | `[MappaDependency]` on `Mappa.Dependency.Protobuf.MappaProtobufMapper`. | [MappaDependencyProtobufMapper.cs](../Mappa.Samples/MappaDependencyProtobufMapper.cs) |
| Excluding methods from resolution | `[MappaIgnore]` on mapper or dependency methods. | [MappaIgnoreMappers.cs](../Mappa.Samples/MappaIgnoreMappers.cs) |

## Integration packages

See also [NuGet packages](./nuget.md) and [Protobuf and Bson dependency packages](./tutorial.md#protobuf-and-bson-dependency-packages).

| Package | Feature | Documented in | Sample |
|---------|---------|---------------|--------|
| `Mappa` | Attributes and `MappaContext`. | [NuGet](./nuget.md) | All samples |
| `Mappa.Generator` | Compile-time mapping source generator. | [NuGet](./nuget.md), [Algorithm](./mappa-generator-algorithm.md) | All samples |
| `Mappa.Dependency.Protobuf` | Mapping for `Google.Protobuf.WellKnownTypes` types. | [Tutorial](./tutorial.md), [NuGet](./nuget.md) | [MappaDependencyProtobufMapper.cs](../Mappa.Samples/MappaDependencyProtobufMapper.cs), [ProtobufOptionalMapper.cs](../Mappa.Samples/ProtobufOptionalMapper.cs), [ReadOnlyTargetCollectionMapper.cs](../Mappa.Samples/ReadOnlyTargetCollectionMapper.cs) |
| `Mappa.Dependency.Protobuf.DependencyInjection` | `RegisterMappaProtobuf` for dependency injection. | [Tutorial](./tutorial.md), [NuGet](./nuget.md) | *(documented only — no sample)* |
| `Mappa.Dependency.Bson` | Mapping for `MongoDB.Bson` types. | [Tutorial](./tutorial.md), [NuGet](./nuget.md) | *(documented only — no sample)* |
| `Mappa.Dependency.Bson.DependencyInjection` | `RegisterMappaBson` for dependency injection. | [Tutorial](./tutorial.md), [NuGet](./nuget.md) | *(documented only — no sample)* |

## Diagnostics

Mappa emits compile-time diagnostics **MP00000** through **MP00073** (errors and warnings). See the full catalog in [Errors and warnings](./error-codes.md). Examples tied to features include **MP00039** (partial enum-to-enum mapping) demonstrated in [EnumToEnumMapper.cs](../Mappa.Samples/EnumToEnumMapper.cs), **MP00040** (missing `[Description]` on enum members), **MP00041** (ambiguous enum mapping), **MP00042** (ambiguous invoke-method resolution for `[MappaInvokeMethod]` and polymorphism `[MappaTypeMappingDefault]` invoke-method defaults), **MP00045** (unresolved before/after map hook), **MP00046** (duplicate class/method before/after map hook registration), **MP00047**–**MP00054** (enum mapping configuration attribute validation) demonstrated in [EnumMappingConfigurationMappers.cs](../Mappa.Samples/EnumMappingConfigurationMappers.cs), **MP00062**–**MP00064** (object factory duplicate, unresolved, and projection incompatibility) demonstrated in [MappaObjectFactoryMapper.cs](../Mappa.Samples/MappaObjectFactoryMapper.cs), **MP00065**–**MP00066** (`[MappaMustMapTargetProperty]` unmapped required property and required-property-in-list) demonstrated in [MappaMustMapTargetPropertyAttributeMapper.cs](../Mappa.Samples/MappaMustMapTargetPropertyAttributeMapper.cs), **MP00067**–**MP00069** (inaccessible-member `UnsafeAccessor` support, invalid target flags, and projection incompatibility) demonstrated in [InaccessibleMembersMapper.cs](../Mappa.Samples/InaccessibleMembersMapper.cs), and **MP00070**–**MP00073** (`[MappaDependencyInjection]` non-partial, attribute conflict, no eligible interfaces, and static mapper skipped) demonstrated in [MappaDependencyInjectionRegistrar.cs](../Mappa.Samples/MappaDependencyInjectionRegistrar.cs).
