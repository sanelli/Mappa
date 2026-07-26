# Tutorial
This tutorial shows how to use the Mappa source generator.

## Setting up the project
First, create a new project:
```powershell
dotnet new console --name MappaTutorial
```

Add the required libraries:
```powershell
dotnet add MappaTutorial package Mappa
dotnet add MappaTutorial package Mappa.Generator
```

Optionally, edit your `MappaTutorial.csproj` file to emit the source generated files by adding the following property:
```xml
<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
```

## Define some classes
Add a couple of classes to the project.

First, create the source class in `Source.cs`:
```csharp
using System.Collections.Generic;

namespace MappaTutorial;

public sealed class Source
{
    public int PropertyOne { get; set; }
    public IEnumerable<int> PropertyTwo { get; set; }
    public StringSplitOptions PropertyThree { get; set; }
}
```

Then create the target class:
```csharp
using System.Collections.Generic;

namespace MappaTutorial;

public sealed class Target
{
    public long PropertyOne { get; set; }
    public IEnumerable<string> PropertyTwo { get; set; }
    public string PropertyThree { get; set; }
}
```

## Define the mapper
Now that we have a source and a target class, define the mapper in `Mapper.cs`:
```csharp
using Mappa;
using Mappa.Attributes;

namespace MappaTutorial;

[Mappa]
public sealed partial class Mapper
{
    public partial Target Map(Source input);
}
```

We defined a mapper using a standard class, but you could also define a static class with extension methods in `SourceExtensions.cs`:
```csharp
using Mappa;
using Mappa.Attributes;

namespace MappaTutorial;

[Mappa]
public static partial class SourceExtensions
{
    public static partial Target ToTarget(this Source input);
}
```

Neither the original `Source` nor `Target` contains any Mappa reference or attribute, making Mappa suitable for existing classes and classes where the source is not available.

## Compile and run the project
Update the main method to use the mapper:
```csharp
using MappaTutorial;

Source source = new()
{
    PropertyOne = 45,
    PropertyTwo = ["4", "77", "987"],
    PropertyThree = StringSplitOptions.RemoveEmptyEntries,
};

var mapper = new Mapper();
var target1 = mapper.Map(source);
System.Console.WriteLine(target1.PropertyOne);
foreach (var item in target1.PropertyTwo)
{
    System.Console.WriteLine(item);
}
System.Console.WriteLine(target1.PropertyThree);

var target2 = source.ToTarget();
System.Console.WriteLine(target2.PropertyOne);
foreach (var item in target2.PropertyTwo)
{
    System.Console.WriteLine(item);
}
System.Console.WriteLine(target2.PropertyThree);
```

Run the project:
```powershell
dotnet run --project MappaTutorial
```

On the console you will see the following output:
```
45
4
77
987
RemoveEmptyEntries
45
4
77
987
RemoveEmptyEntries
```

## Advanced topics

### Ignoring methods via MappaIgnore
By default, Mappa considers every suitable method on the mapper class (and its dependencies) when resolving mappings. Use `[MappaIgnore]` to exclude a method from that search:

```csharp
[Mappa]
public sealed partial class Mapper
{
    public partial Target Map(Source input);

    [MappaIgnore]
    private static string CustomIntToString(int input) => $"ignored {input}";
}
```

See also: [MappaIgnoreMappers.cs](../Mappa.Samples/MappaIgnoreMappers.cs).

### MappaSettings attribute
`[MappaSettings]` controls culture and format when parsing or converting values. Settings can be applied to the mapper class or to individual methods. Method-level settings override class-level settings, which override `.editorconfig` values.

Parse with invariant culture:
```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial decimal MapDecimal(string input);
}
```

Convert to string with format and culture:
```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaSettings(DecimalFormat = "N2", CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial string MapDecimal(decimal input);
}
```

Parse date/time values with `DateTimeStyles` (for example to allow leading and trailing whitespace):
```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaSettings(DateTimeStyle = DateTimeStyles.AllowWhiteSpaces)]
    public partial DateTime MapDateTime(string input);

    [MappaSettings(
        DateTimeOffsetFormat = "dd-MM-yyyy HH:mm:ss",
        CultureInfoSetting = CultureInfoSetting.InvariantCulture,
        DateTimeOffsetStyle = DateTimeStyles.AllowWhiteSpaces)]
    public partial DateTimeOffset MapDateTimeOffset(string input);
}
```

Style settings apply when parsing `string` to `DateTime`, `DateTimeOffset`, `DateOnly`, or `TimeOnly` only. They do not affect `ToString` generation. As with culture and format, method-level settings override class-level settings, which override `.editorconfig` values.

Use `GlobalDateTimeStyle` to apply one default [DateTimeStyles](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.datetimestyles) to all date/time parse targets. Type-specific properties such as `DateTimeStyle` override the global default when both are set:

```csharp
[Mappa]
[MappaSettings(GlobalDateTimeStyle = DateTimeStyles.AllowWhiteSpaces)]
public sealed partial class Mapper
{
    public partial DateTime MapDateTime(string input);

    public partial DateTimeOffset MapDateTimeOffset(string input);

    [MappaSettings(DateTimeStyle = DateTimeStyles.AssumeUniversal)]
    public partial DateTime MapDateTimeWithOverride(string input);
}
```

Parse numeric values with `NumberStyles` (for example to allow grouping separators and parentheses):
```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaSettings(IntStyle = NumberStyles.AllowThousands | NumberStyles.AllowParentheses)]
    public partial int MapInteger(string input);

    [MappaSettings(
        DecimalStyle = NumberStyles.AllowThousands | NumberStyles.AllowParentheses | NumberStyles.AllowDecimalPoint,
        CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
    public partial decimal MapDecimal(string input);
}
```

Numeric style settings apply when parsing `string` to the corresponding numeric type only. They do not affect `ToString` generation. Override hierarchy is the same as for culture, format, and date/time styles.

Use `GlobalNumberStyle` to apply one default [NumberStyles](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.numberstyles) to all numeric parse targets. Type-specific properties such as `IntStyle` override the global default when both are set:

```csharp
[Mappa]
[MappaSettings(GlobalNumberStyle = NumberStyles.AllowThousands | NumberStyles.AllowParentheses)]
public sealed partial class Mapper
{
    public partial int MapInteger(string input);

    [MappaSettings(IntStyle = NumberStyles.AllowParentheses)]
    public partial int MapIntegerWithOverride(string input);
}
```

Numeric format properties (for example `IntFormat`, `DecimalFormat`) apply to `ToString` only. Culture applies to both parsing and converting to `string`.

See also: [InvokeParseMapper.cs](../Mappa.Samples/InvokeParseMapper.cs), [InvokeToStringMapper.cs](../Mappa.Samples/InvokeToStringMapper.cs), and [Mappa attributes](./mappa-attributes.md#mappasettings).

### Property name matching settings
`CaseInsensitivePropertyMap` and `IgnoreUnderscoreForPropertyMap` change how source and target properties are paired by name during constructor-map mapping:

```csharp
[Mappa]
[MappaSettings(
    CaseInsensitivePropertyMap = BooleanSetting.Enable,
    IgnoreUnderscoreForPropertyMap = BooleanSetting.Enable)]
public sealed partial class Mapper
{
    public partial Target Map(Source input);
}
```

See also: [PropertyMapNameSettingsMapper.cs](../Mappa.Samples/PropertyMapNameSettingsMapper.cs).

### Enum matching settings

#### Case-insensitive matching

`CaseInsensitiveEnumMap` enables case-insensitive matching of enum member names when mapping between enums or from `string` to an enum. When `EnumStringMapSetting` or `EnumToEnumMapSetting` is `Description`, the same setting also applies case-insensitively to `Description` attribute values:

```csharp
[Mappa]
[MappaSettings(CaseInsensitiveEnumMap = BooleanSetting.Enable)]
public sealed partial class Mapper
{
    public partial MyEnum Map(string input);
    public partial TargetEnum Map(SourceEnum input);
}
```

With this setting enabled for string-to-enum mapping, `"one"`, `"ONE"`, and `"One"` all map to `MyEnum.One`. By default, only the exact member name matches (for example `"One"`).

For enum-to-enum mapping, member names are compared case-insensitively when `CaseInsensitiveEnumMap` is enabled and `EnumToEnumMapSetting` is unset or `MemberName`. This allows source and target enums to use different casing for otherwise matching names.

See also: [CaseInsensitiveEnumMapper.cs](../Mappa.Samples/CaseInsensitiveEnumMapper.cs), [CaseInsensitiveEnumToEnumMapper.cs](../Mappa.Samples/CaseInsensitiveEnumToEnumMapper.cs).

#### Description-based string mapping

`EnumStringMapSetting` selects how enum members are paired with string values when mapping between an enum and `string`. Set `Description` to match by each member's `[Description]` attribute value instead of the member name:

```csharp
using System.ComponentModel;

public enum MyEnum
{
    [Description("First")]
    One,
    [Description("Second")]
    Two,
}

[Mappa]
[MappaSettings(EnumStringMapSetting = EnumStringMapSetting.Description)]
public sealed partial class Mapper
{
    public partial string MapToString(MyEnum input);
    public partial MyEnum MapToEnum(string input);
}
```

With `Description`, `MyEnum.One` maps to `"First"` and `"First"` maps back to `MyEnum.One`. Every enum member used in the mapping must have a non-empty `[Description]` attribute; duplicate Description values on the same enum are reported as errors.

Combine with `CaseInsensitiveEnumMap` for case-insensitive Description matching (for example `"first"` maps to `MyEnum.One`).

See also: [DescriptionEnumToStringMapper.cs](../Mappa.Samples/DescriptionEnumToStringMapper.cs), [DescriptionStringToEnumMapper.cs](../Mappa.Samples/DescriptionStringToEnumMapper.cs).

#### Enum-to-enum matching settings

`EnumToEnumMapSetting` selects how enum-to-enum mappings pair source and target members. By default, members are matched by name. Set `NumericValue` to match by underlying numeric value instead:

```csharp
[Mappa]
[MappaSettings(EnumToEnumMapSetting = EnumToEnumMapSetting.NumericValue)]
public sealed partial class Mapper
{
    public partial TargetEnum Map(SourceEnum input);
}
```

With `NumericValue`, source members are mapped to target members that share the same constant value, even when member names differ. The generator emits an explicit `switch` with per-case assignments (no cross-enum cast). When multiple target members share the same value, the first target member name in alphabetical order is used.

Set `Description` to match source and target members by shared `[Description]` attribute values instead of member names:

```csharp
[Mappa]
[MappaSettings(EnumToEnumMapSetting = EnumToEnumMapSetting.Description)]
public sealed partial class Mapper
{
    public partial TargetEnum Map(SourceEnum input);
}
```

Both enums must define a non-empty `[Description]` on every member used in the mapping. Duplicate Description values within an enum, or ambiguous pairings between source and target enums, are reported as errors.

See also: [NumericValueEnumToEnumMapper.cs](../Mappa.Samples/NumericValueEnumToEnumMapper.cs), [DescriptionEnumToEnumMapper.cs](../Mappa.Samples/DescriptionEnumToEnumMapper.cs).

### Enum mapping configuration attributes

`[MappaMapEnumMember]`, `[MappaMapEnumIgnore]`, and `[MappaMapEnumDefault]` override or extend settings-based enum matching on map methods. They apply to root enum maps and to nested enum properties on class/struct maps. All three are bidirectional and allow multiple instances.

**Member override** — remap a specific enum member to an integral, string, or other enum value:

```csharp
[MappaMapEnumMember<ConfigStatus>(ConfigStatus.Inactive, 99)]
public partial int Map(ConfigStatus input);
```

**Ignore** — exclude a member from mapping (fallback via `[MappaMapEnumDefault]`, throw by default):

```csharp
[MappaMapEnumIgnore<ConfigStatus>(ConfigStatus.Deprecated)]
public partial int Map(ConfigStatus input);
```

**Default `UseDefaultValue`** — return a fallback when a value cannot be mapped:

```csharp
[MappaMapEnumDefault<ConfigStatus>(MappaMapEnumDefaultBehavior.UseDefaultValue, 42)]
public partial int Map(ConfigStatus input);
```

**Multi-enum class defaults** — one `[MappaMapEnumDefault]` per distinct enum type on a class/struct map:

```csharp
[MappaMapEnumDefault<ConfigStatus>(MappaMapEnumDefaultBehavior.Throw)]
[MappaMapEnumDefault<ConfigPriority>(MappaMapEnumDefaultBehavior.UseDefaultValue, 0)]
public partial EnumConfigMultiDefaultTargetModel Map(EnumConfigMultiDefaultSourceModel input);
```

When intentional partial enum-to-enum mapping leaves unpaired source members, suppress warning **MP00039** with `#pragma warning disable MP00039` / `#pragma warning restore MP00039` around the map method (see [EnumMappingConfigurationMappers.cs](../Mappa.Samples/EnumMappingConfigurationMappers.cs)).

See also: [Mappa attributes — enum mapping configuration](./mappa-attributes.md#mappamapenummember-mappamapenumignore-and-mappamapenumdefault), [EnumMappingConfigurationMappers.cs](../Mappa.Samples/EnumMappingConfigurationMappers.cs).

#### Identity map deep copy settings

`IdentityMapDeepCopy` controls how the generator copies a type to itself when the identity strategy applies. The default is `ShallowCopy` (return the same reference). Set `DeepCopy` to clone the root instance without recursively copying nested references. Set `NestedDeepCopy` to clone the root and recursively map every accessible instance field (including reference-type fields inside struct roots):

```csharp
[Mappa]
[MappaSettings(IdentityMapDeepCopy = IdentityMapDeepCopySetting.DeepCopy)]
public sealed partial class IdentityMapDeepCopyDeepMapper
{
    public partial Person Map(Person input);
}
```

Because Mappa allows only one map method per source/target pair in a mapper class, use separate mapper classes for each mode (shallow, deep, nested). Primitives, enums, and `string` always assign and ignore this setting. Same-type constructor-parameter mapping in the constructor detector always uses shallow pass-through.

See also: [IdentityMapDeepCopyMapper.cs](../Mappa.Samples/IdentityMapDeepCopyMapper.cs).

#### Enumerable concrete type settings

`EnumerableConcreteType` controls the concrete buffer used when a collection mapping targets a sequence-like interface. The default is `List` (`List<T>` with `Add`). Set `Array` to allocate `T[]` instead (indexer insertion). Concrete `List<T>` return types always remain lists. The setting applies to interface targets such as `IEnumerable<T>`, `IList<T>`, `ICollection<T>`, `IReadOnlyList<T>`, and `IReadOnlyCollection<T>`:

```csharp
[Mappa]
[MappaSettings(EnumerableConcreteType = EnumerableConcreteTypeSetting.Array)]
public sealed partial class EnumerableConcreteTypeArrayMapper
{
    public partial IEnumerable<int> Map(IEnumerable<MyEnum> input);
}
```

See also: [EnumerableConcreteTypeMapper.cs](../Mappa.Samples/EnumerableConcreteTypeMapper.cs).

#### Dictionary assignment settings

`DictionaryAssignment` controls how entries are inserted when mapping between dictionaries. The default is `Indexer` (`target[key] = value`). Set `Add` to call `IDictionary<TKey, TValue>.Add(key, value)` instead. Both modes produce equivalent results for unique keys. The setting applies to dictionary return types and get-only dictionary properties:

```csharp
[Mappa]
[MappaSettings(DictionaryAssignment = DictionaryAssignmentSetting.Add)]
public sealed partial class DictionaryAssignmentAddMapper
{
    public partial Dictionary<string, string> Map(Dictionary<int, MyEnum> input);
}
```

See also: [DictionaryAssignmentMapper.cs](../Mappa.Samples/DictionaryAssignmentMapper.cs).

### MappaInvokeMethod attribute
When mapping structured types, `[MappaInvokeMethod]` forces a target property or constructor parameter to be mapped by invoking a named method:

```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaInvokeMethod(nameof(Target.ParamA), nameof(CustomMap))]
    public partial Target Map(Source source);

    private static string CustomMap(Source source, int property)
        => $"{source.ParamA}/{property}";
}
```

See also: [MappaInvokeMethodAttributeMappers.cs](../Mappa.Samples/MappaInvokeMethodAttributeMappers.cs) and [Mappa attributes](./mappa-attributes.md#mappainvokemethodattribute).

### MappaBeforeMap and MappaAfterMap attributes
`[MappaBeforeMap]` and `[MappaAfterMap]` invoke named hooks around the generated body of a root mapping method. Class-level before hooks run before method-level before hooks; method-level after hooks run before class-level after hooks:

```csharp
[Mappa]
[MappaBeforeMap(nameof(ClassBefore))]
[MappaAfterMap(nameof(ClassAfter))]
public sealed partial class Mapper
{
    [MappaBeforeMap(nameof(MethodBefore))]
    [MappaAfterMap(nameof(MethodAfter))]
    public partial Person Map(Person input, MappaContext context);

    private void ClassBefore(ref Person input, MappaContext context) { /* ... */ }
    private void MethodBefore(ref Person input) { /* ... */ }
    private void MethodAfter(ref Person target) { /* ... */ }
    private void ClassAfter(ref Person target) { /* ... */ }
}
```

Hooks must return `void` and may accept no parameters, `MappaContext`, `ref T` (source for before, target for after), or `ref T` plus `MappaContext`. The `ref` type must match exactly. Unresolved hooks warn with **MP00045**; a class/method duplicate warns with **MP00046** and runs once.

See also: [MappaBeforeAfterMapHooksAttributeMapper.cs](../Mappa.Samples/MappaBeforeAfterMapHooksAttributeMapper.cs) and [Mappa attributes](./mappa-attributes.md#mappabeforemap-and-mappaaftermap).

### MappaUseProperty attribute
When source and target property names differ, `[MappaUseProperty]` selects which source property supplies a target member:

```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaUseProperty(nameof(Target.ParamA), nameof(Source.ParamB))]
    [MappaUseProperty(nameof(Target.ParamB), nameof(Source.ParamA))]
    public partial Target Map(Source source);
}
```

See also: [MappaUsePropertyAttributeMapper.cs](../Mappa.Samples/MappaUsePropertyAttributeMapper.cs).

### Nested property paths
Attributes that accept `TargetPropertyName` and/or `SourcePropertyName` also support **dot-separated nested property paths**. Use this when the value lives under nested members rather than on the root source or target type:

```csharp
[Mappa]
public sealed partial class Mapper
{
    // Map nested target Address.City / Address.ZipCode from a deeper source chain.
    [MappaUseProperty("Address.City", "Location.Address.City")]
    [MappaUseProperty("Address.ZipCode", "Location.Address.ZipCode")]
    public partial PersonTarget Map(LocationSource source);
}
```

The same path syntax works on `[MappaInvokeMethod]`, `[MappaAssignFromConstant]`, `[MappaAssignFromContext]`, `[MappaAssignToContext]`, and `[MappaIgnoreTargetProperty]`. The generator trims paths while mapping nested types, reuses nested source receivers for leaf chains, and emits `?.` / `.` from each receiver's nullability. `?? throw` is added only when the chain can be null and the target cannot. Nested `[MappaAssignToContext]` stores the leaf from the constructed result (for example `result.Address.City`).

See also: [Mappa attributes — Nested property paths](./mappa-attributes.md#nested-property-paths), [algorithm](./mappa-generator-algorithm.md#nested-property-paths), and [NestedPropertyPathAttributeMapper.cs](../Mappa.Samples/NestedPropertyPathAttributeMapper.cs).

### IQueryable projection
Map methods that take `IQueryable<TSource>` and return `IQueryable<TTarget>` emit deferred `Select` projections suitable for ORM providers such as EF Core. No dedicated attribute is required — the signature selects the strategy:

```csharp
[Mappa]
public static partial class OrderMapper
{
    [MappaUseProperty(nameof(OrderDto.Title), nameof(Order.Name))]
    public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);

    [MappaUseProperty(nameof(OrderDto.Title), nameof(Order.Name))]
    private static partial OrderDto MapOrder(Order order);
}

// Typical EF Core usage:
var dtos = await context.Orders.ProjectToDto().ToListAsync();
```

Projection limitations:

- No `[MappaBeforeMap]` / `[MappaAfterMap]` hooks and no `MappaContext` parameter on the projection method.
- Nested `IQueryable` properties and polymorphic root element maps are not supported.
- Prefer numeric or description enum mappings over case-insensitive member-name matching for provider translation.
- Mapping `IQueryable<TSource>` to a concrete collection (for example `List<TTarget>`) materializes eagerly and may emit warning MP00061.
- Generated projection methods are annotated with `[RequiresDynamicCode]` because they build expression trees at runtime; they are **not compatible with [Native AOT](https://learn.microsoft.com/dotnet/core/deploying/native-aot)** deployment.

See also: [algorithm — IQueryable projection](./mappa-generator-algorithm.md#7a-iqueryable-projection-strategy), [error codes MP00055–MP00061](./error-codes.md), and [IQueryableProjectionMapper.cs](../Mappa.Samples/IQueryableProjectionMapper.cs).

### MappaAssignFromConstant attribute
Assign a compile-time constant to a target property or constructor parameter:

```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaAssignFromConstant(nameof(Target.Status), "Active")]
    public partial Target Map(Source source);
}
```

See also: [MappaAssignFromConstantAttributeMapper.cs](../Mappa.Samples/MappaAssignFromConstantAttributeMapper.cs).

### MappaAssignFromContext attribute
Read a value from `MappaContext` and assign it to a target member. The map method must accept `MappaContext` as its second parameter:

```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaAssignFromContext(nameof(Target.ParamA), "CustomValue")]
    public partial Target Map(Source input, MappaContext context);
}
```

See also: [MappaAssignFromContextAttributeMapper.cs](../Mappa.Samples/MappaAssignFromContextAttributeMapper.cs).

### MappaAssignToContext attribute
After mapping completes, store a target member value in `MappaContext`:

```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaAssignToContext("ParamA", nameof(Target.ParamA))]
    public partial Target Map(Source input, MappaContext context);
}
```

See also: [MappaAssignToContextAttributeMapper.cs](../Mappa.Samples/MappaAssignToContextAttributeMapper.cs).

### MappaIgnoreTargetProperty attribute
When mapping via an empty constructor, exclude a target property from property-initializer mapping:

```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaIgnoreTargetProperty(nameof(Target.IgnoredProperty))]
    public partial Target Map(Source source);
}
```

See also: [MappaIgnoreTargetPropertyAttributeMapper.cs](../Mappa.Samples/MappaIgnoreTargetPropertyAttributeMapper.cs).

### MappaDependency and MappaStaticDependency
Register external mapping methods via `[MappaDependency]` on a field or property, or via `[MappaStaticDependency]` on a static helper class:

```csharp
[Mappa]
[MappaStaticDependency(typeof(StaticMappingHelpers))]
public sealed partial class Mapper
{
    [MappaDependency]
    private readonly MyDependencyMapper dependency = new();

    public partial Target Map(Source source);
}
```

Mappa searches dependency types (and their base classes) for suitable mapping methods. When the root map method is `static`, instance dependencies must be declared `static`.

See also: [MapMethodStrategyWithDependencyMapper.cs](../Mappa.Samples/MapMethodStrategyWithDependencyMapper.cs).

### Polymorphism support
Use `[MappaTypeMapping]` to map different concrete source types to different target types. Use `[MappaTypeMappingDefault]` to define the fallback behaviour:

```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaTypeMapping(typeof(TargetFirst), typeof(SourceFirst))]
    [MappaTypeMapping(typeof(TargetSecond), typeof(SourceSecond))]
    public partial TargetBase Map(SourceBase source);
}
```

See also: [PolymorphismMappers.cs](../Mappa.Samples/PolymorphismMappers.cs) and [Mappa attributes](./mappa-attributes.md#mappatypemapping-and-mappatypemappingdefault).

### Get-only collection properties
Mappa can populate get-only collection and dictionary properties after constructing the target object. This also applies to specialized collection types such as `Stack<T>`, `Queue<T>`, and protobuf repeated fields:

```csharp
[Mappa]
public sealed partial class Mapper
{
    public partial TargetWithReadOnlyCollections Map(SourceWithCollections source);
}
```

See also: [ReadOnlyTargetCollectionMapper.cs](../Mappa.Samples/ReadOnlyTargetCollectionMapper.cs).

### MappaSettings ProtobufOptional
When mapping protobuf messages with `optional` fields, enable `ProtobufOptional` so Mappa uses `Has<propertyName>` when reading and writing optional values:

```csharp
[Mappa]
[MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
public sealed partial class Mapper
{
    public partial Target Map(SourceProtobufMessage input);
}
```

See also: [ProtobufOptionalMapper.cs](../Mappa.Samples/ProtobufOptionalMapper.cs).

### Protobuf and Bson dependency packages
Install the dependency packages and register their mappers on your mapper class:

```csharp
using Mappa.Attributes;
using Mappa.Dependency.Protobuf;

[Mappa]
public sealed partial class Mapper
{
    [MappaDependency]
    private readonly MappaProtobufMapper protobufMapper = new();

    public partial Target Map(SourceWithTimestamp source);
}
```

For Bson, use `Mappa.Dependency.Bson.MappaBsonMapper` in the same way to map `ObjectId` to and from `string` or `byte[]`.

With dependency injection, register the mappers via `RegisterMappaProtobuf` or `RegisterMappaBson`:

```csharp
services.RegisterMappaProtobuf();
services.RegisterMappaBson();
```

See also: [MappaDependencyProtobufMapper.cs](../Mappa.Samples/MappaDependencyProtobufMapper.cs), [Mappa.Dependency.Protobuf](../Mappa.Dependency.Protobuf/README.md), and [Mappa.Dependency.Bson](../Mappa.Dependency.Bson/README.md).

For a complete catalog of samples, see [Mappa.Samples](../Mappa.Samples/README.md).