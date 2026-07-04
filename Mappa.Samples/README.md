# Mappa.Samples
This project contains sample mappers that showcase Mappa features. Each mapper is a partial class tagged with `[Mappa]` and demonstrates one or more mapping strategies or attributes.

See the [tutorial](../Documentation/tutorial.md) for a guided introduction, or browse the samples below by topic.

## Core mapping strategies
| Sample | Demonstrates |
|--------|--------------|
| [`IdentityStrategyMapper.cs`](IdentityStrategyMapper.cs) | Identity and implicit conversion mappings |
| [`CollectionToCollectionMapper.cs`](CollectionToCollectionMapper.cs) | Collection-to-collection mapping |
| [`DictionaryToDictionaryMapper.cs`](DictionaryToDictionaryMapper.cs) | Dictionary-to-dictionary mapping |
| [`EnumToEnumMapper.cs`](EnumToEnumMapper.cs) | Enum-to-enum mapping |
| [`DateAndTimeMapper.cs`](DateAndTimeMapper.cs) | Date and time type conversions |
| [`GuidStrategyMapper.cs`](GuidStrategyMapper.cs) | `Guid` ↔ `byte[]` mapping |
| [`TupleToTupleMapper.cs`](TupleToTupleMapper.cs) | Tuple-to-tuple mapping |

## Attributes
| Sample | Demonstrates |
|--------|--------------|
| [`MappaUsePropertyAttributeMapper.cs`](MappaUsePropertyAttributeMapper.cs) | `[MappaUseProperty]` |
| [`MappaAssignFromConstantAttributeMapper.cs`](MappaAssignFromConstantAttributeMapper.cs) | `[MappaAssignFromConstant]` |
| [`MappaAssignFromContextAttributeMapper.cs`](MappaAssignFromContextAttributeMapper.cs) | `[MappaAssignFromContext]` with `MappaContext` |
| [`MappaAssignToContextAttributeMapper.cs`](MappaAssignToContextAttributeMapper.cs) | `[MappaAssignToContext]` with `MappaContext` |
| [`MappaIgnoreTargetPropertyAttributeMapper.cs`](MappaIgnoreTargetPropertyAttributeMapper.cs) | `[MappaIgnoreTargetProperty]` |
| [`MappaIgnoreMappers.cs`](MappaIgnoreMappers.cs) | `[MappaIgnore]` |
| [`MappaInvokeMethodAttributeMappers.cs`](MappaInvokeMethodAttributeMappers.cs) | `[MappaInvokeMethod]` |

## Settings
| Sample | Demonstrates |
|--------|--------------|
| [`InvokeParseMapper.cs`](InvokeParseMapper.cs) | Parse settings: culture and format for date/time, `Guid`, and numeric types |
| [`InvokeToStringMapper.cs`](InvokeToStringMapper.cs) | ToString settings: culture and format |
| [`PropertyMapNameSettingsMapper.cs`](PropertyMapNameSettingsMapper.cs) | `CaseInsensitivePropertyMap`, `IgnoreUnderscoreForPropertyMap` |
| [`CaseInsensitiveEnumMapper.cs`](CaseInsensitiveEnumMapper.cs) | `CaseInsensitiveEnumMap` (string-to-enum) |
| [`CaseInsensitiveEnumToEnumMapper.cs`](CaseInsensitiveEnumToEnumMapper.cs) | `CaseInsensitiveEnumMap` (enum-to-enum) |
| [`DescriptionEnumToStringMapper.cs`](DescriptionEnumToStringMapper.cs) | `EnumStringMapSetting` (enum-to-string) |
| [`DescriptionStringToEnumMapper.cs`](DescriptionStringToEnumMapper.cs) | `EnumStringMapSetting` (string-to-enum) |
| [`DescriptionEnumToEnumMapper.cs`](DescriptionEnumToEnumMapper.cs) | `EnumToEnumMapSetting.Description` |
| [`NumericValueEnumToEnumMapper.cs`](NumericValueEnumToEnumMapper.cs) | `EnumToEnumMapSetting.NumericValue` |
| [`PragmaWarningSettingMapper.cs`](PragmaWarningSettingMapper.cs) | `PragmaWarning` |
| [`FastCollectionToCollectionMapper.cs`](FastCollectionToCollectionMapper.cs) | `FastCollections` |
| [`ContainersWithCapacityConstructorMapper.cs`](ContainersWithCapacityConstructorMapper.cs) | `ContainerCapacityConstructors` |
| [`IdentityMapDeepCopyMapper.cs`](IdentityMapDeepCopyMapper.cs) | `IdentityMapDeepCopy` shallow, deep, and nested same-type mappings (class and struct) |

## Polymorphism
| Sample | Demonstrates |
|--------|--------------|
| [`PolymorphismMappers.cs`](PolymorphismMappers.cs) | `[MappaTypeMapping]`, `[MappaTypeMappingDefault]` |
| [`PolymorphicMethodMapMapper.cs`](PolymorphicMethodMapMapper.cs) | Polymorphic method resolution via existing-method strategy |

## Dependencies
| Sample | Demonstrates |
|--------|--------------|
| [`MapMethodStrategyMapper.cs`](MapMethodStrategyMapper.cs) | Existing-method strategy on the mapper class |
| [`MapMethodStrategyWithUserCustomInstanceMethodMapper.cs`](MapMethodStrategyWithUserCustomInstanceMethodMapper.cs) | Custom instance mapping methods |
| [`MapMethodStrategyWithUserCustomStaticMethodMapper.cs`](MapMethodStrategyWithUserCustomStaticMethodMapper.cs) | Custom static mapping methods |
| [`MappaDependencyProtobufMapper.cs`](MappaDependencyProtobufMapper.cs) | `[MappaDependency]` with Protobuf mapper |

## Protobuf and collections
| Sample | Demonstrates |
|--------|--------------|
| [`ProtobufOptionalMapper.cs`](ProtobufOptionalMapper.cs) | `ProtobufOptional` setting for optional protobuf fields |
| [`ReadOnlyTargetCollectionMapper.cs`](ReadOnlyTargetCollectionMapper.cs) | Get-only collection properties and protobuf collections |

## Native AOT
All samples are also exercised under Native AOT in [Mappa.Samples.Aot](../Mappa.Samples.Aot).
