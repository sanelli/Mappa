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
`CaseInsensitivePropertyMap` and `IgnoreUnderscoreForPropertyMap` change how source and target properties (and fields, when field mapping is enabled) are paired by name during constructor-map mapping:

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

### Mapping instance fields

By default Mappa maps properties only. Enable `MapSourceFields` and/or `MapTargetFields` to treat accessible instance fields as mapping members (constructor parameters, empty-constructor initializers, nested paths, and attribute `*PropertyName` parameters). When both a property and a field match the same name, the property wins. Compiler-generated backing fields, `static` / `const` / `ref` / `fixed` fields, and `readonly` target fields are never included. Private or protected fields also require `[MappaAllowInaccessibleSourceMembers]` / `[MappaAllowInaccessibleTargetMembers]` (and `AllowFields` on the target attribute):

```csharp
[Mappa]
[MappaSettings(
    MapSourceFields = BooleanSetting.Enable,
    MapTargetFields = BooleanSetting.Enable)]
public sealed partial class Mapper
{
    public partial Target Map(Source input);
}
```

See also: [MapFieldsMapper.cs](../Mappa.Samples/MapFieldsMapper.cs) and [Mappa attributes — MappaSettings](./mappa-attributes.md#mappasettings).

### Explicit interface property mapping

By default, properties that are explicitly implemented interface members are ignored. Enable `MapSourceExplicitInterfaceProperties` and/or `MapTargetExplicitInterfaceProperties` to map them via a cast to the declaring interface. Target writes always happen after construction (never in an object initializer). After a parameterized constructor, remaining explicit-interface target properties are also assigned when the target setting is enabled:

```csharp
[Mappa]
[MappaSettings(
    MapSourceExplicitInterfaceProperties = BooleanSetting.Enable,
    MapTargetExplicitInterfaceProperties = BooleanSetting.Enable)]
public sealed partial class Mapper
{
    public partial Target Map(Source input);
}
```

These settings are independent of `[MappaAllowInaccessibleSourceMembers]` / `[MappaAllowInaccessibleTargetMembers]`: ordinary private members use `UnsafeAccessor` when opted in, while explicit interface properties always use an interface cast when the Map\* settings are enabled.

See also: [ExplicitInterfacePropertiesMapper.cs](../Mappa.Samples/ExplicitInterfacePropertiesMapper.cs).

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

#### Reference handling

`ReferenceReusing`, `MaxRuntimeDepth`, `MaxCompileTimeDepth`, and `BreakCompileTimeCycles` control how Mappa handles object graphs with cycles, shared references, and deep nesting. Runtime features (`ReferenceReusing` and `MaxRuntimeDepth`) require a `MappaContext` parameter; without it the generator warns (**MP00074** / **MP00075**) and skips reference handling for that method. Prefer a dedicated map method per nested reference type when you want an explicit API surface, and use nullable cycle-closing properties so a `null` edge can terminate recurrence.

**Reference reuse** — enable `ReferenceReusing` and pass a fresh `MappaContext` so generated code records each mapped source→target pair and returns the existing target when the same source instance appears again. Reuse is keyed by **source identity and the declared target type** (`typeof(TTarget)`): the same source can map to different target types independently; a lookup for one `TTarget` does not reuse a target stored under another. `MappaContext` is not thread-safe: use one instance per logical mapping operation and do not share it across concurrent mapping operations without external synchronization (especially important with `ReferenceReusing` or `MaxRuntimeDepth`):

```csharp
[Mappa]
[MappaSettings(ReferenceReusing = BooleanSetting.Enable)]
public sealed partial class ReferenceReusingCycleMapper
{
    public partial ReferenceHandlingPersonTarget MapPerson(
        ReferenceHandlingPersonSource input,
        MappaContext context);

    public partial ReferenceHandlingAddressTarget MapAddress(
        ReferenceHandlingAddressSource input,
        MappaContext context);
}

var mapper = new ReferenceReusingCycleMapper();
var context = new MappaContext();
var target = mapper.MapPerson(source, context);
```

**Runtime depth** — `MaxRuntimeDepth` is a `short`. Use `-1` (`UndefinedDepth`) to inherit. The effective default is `0` (unlimited). A positive value is copied onto the context's reference manager; nested reference-type maps increase depth, and exceeding the limit throws `MappaException`. The root map does not increase depth (`1` allows the root plus one nested level):

```csharp
[Mappa]
[MappaSettings(MaxRuntimeDepth = 2)]
public sealed partial class MaxRuntimeDepthMapper
{
    public partial ReferenceHandlingLevel0Target Map(
        ReferenceHandlingLevel0Source input,
        MappaContext context);
}
```

**Compile-time depth** — `MaxCompileTimeDepth` limits how deep the generator may recurse while discovering strategies (effective default `50`; `0` disables tracking). Exceeding it reports **MP00076** and stops generation for that mapping. This is independent of `MaxRuntimeDepth`.

**Compile-time mapping cycles** — while discovering strategies, the generator tracks the current source/target type-pair stack. By default, re-entering the same pair before the outer discovery completes reports **MP00077**. Add an explicit map method for that pair to break the cycle, or enable `BreakCompileTimeCycles` so the generator synthesizes a **private** map method for the cycling pair, invokes it at the cycle edge, and reports warning **MP00078**. Combine either approach with `ReferenceReusing` when the object graph also cycles at runtime (otherwise a closed cycle can recurse indefinitely).

```csharp
[Mappa]
[MappaSettings(
    BreakCompileTimeCycles = BooleanSetting.Enable,
    ReferenceReusing = BooleanSetting.Enable)]
public sealed partial class BreakCompileTimeCyclesMapper
{
    public partial ReferenceHandlingPersonTarget Map(
        ReferenceHandlingPersonSource input,
        MappaContext context);
}
```

**IQueryable projection** — `ReferenceReusing` and `MaxRuntimeDepth` are not supported on `IQueryable` projection methods and are rejected with **MP00057**.

See also: [ReferenceHandlingMapper.cs](../Mappa.Samples/ReferenceHandlingMapper.cs), [BreakCompileTimeCyclesMapper.cs](../Mappa.Samples/BreakCompileTimeCyclesMapper.cs), [algorithm — reference handling](./mappa-generator-algorithm.md#reference-handling-and-compile-time-guards), [error codes](./error-codes.md).

#### Preferred collection/dictionary interfaces

When a type implements more than one closed version of the same open interface (for example both `IEnumerable<int>` and `IEnumerable<string>`), Mappa picks which closed interface to use. Without attributes, it prefers the unique usable **implicit** implementation, then the unique usable **explicit** one. Use method-level preference attributes to force a specific closed interface:

```csharp
[Mappa]
public sealed partial class PreferredInterfaceMapper
{
    // Unique usable implicit IEnumerable<int>
    public partial List<int> MapImplicitInt(DualEnumerable input);

    // Force explicit IEnumerable<string>
    [MappaSourcePreferredInterfaceImplementation(typeof(DualEnumerable), typeof(IEnumerable<string>))]
    public partial List<string> MapExplicitString(DualEnumerable input);

    // Force string side on a multi-interface target (otherwise unique usable implicit is IEnumerable<int>)
    [MappaTargetPreferredInterfaceImplementation(typeof(DualCollection), typeof(IEnumerable<string>))]
    public partial DualCollection MapToDualCollectionStrings(List<int> input);
}
```

See also: [PreferredInterfaceMapper.cs](../Mappa.Samples/PreferredInterfaceMapper.cs), [attributes — preferred interfaces](./mappa-attributes.md#preferred-interface-implementations), [algorithm — container strategy](./mappa-generator-algorithm.md#7-container-strategy), [upgrade guide](./upgrade.md).

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

#### Prevent Enumerable.Count settings

`PreventEnumerableCount` controls whether the generator calls `Enumerable.Count` when mapping from a source that does not expose `Count` or `Length` (for example `IEnumerable<T>`) to a fixed-size target such as `T[]`, `Span<T>`, `ReadOnlySpan<T>`, `Memory<T>`, or `ReadOnlyMemory<T>`. When enabled, a growable `List<T>` buffer collects mapped elements in a single enumeration and the final target is materialized afterward (for example via `ToArray()`). Sources with a known length (arrays, `List<T>`, `ICollection<T>`, and similar) are unchanged:

```csharp
[Mappa]
[MappaSettings(PreventEnumerableCount = BooleanSetting.Enable)]
public sealed partial class PreventEnumerableCountMapper
{
    public partial int[] MapEnumerableToArray(IEnumerable<MyEnum> input);
    public partial Span<int> MapEnumerableToSpan(IEnumerable<MyEnum> input);
}
```

See also: [PreventEnumerableCountMapper.cs](../Mappa.Samples/PreventEnumerableCountMapper.cs).

#### Compatible map method settings

`CompatibleMapMethod` controls whether nested existing-method reuse may invoke a map method whose parameter type is a base type or interface of the required source and whose return type is a derived type of the required target. The setting is disabled by default so that only exact (or nullability-relaxed) matches are reused:

```csharp
[Mappa]
[MappaSettings(CompatibleMapMethod = BooleanSetting.Enable)]
public sealed partial class CompatibleMapMethodMapper
{
    public DerivedTarget MapInner(BaseSource input) => new DerivedTarget { Value = input.Value + 100 };

    public partial WrapperTarget Map(WrapperSource input);
}
```

See also: [CompatibleMapMethodMapper.cs](../Mappa.Samples/CompatibleMapMethodMapper.cs).

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

### MappaTransformer and MappaApplyTransformer attributes
Value transformers rewrite a value of a type into another value of the same type while mapping. `[MappaTransformer]` registers a transformer for every map method of the mapper; `[MappaApplyTransformer]` applies a named transformer to one map method, optionally only to one source member. Transformers run on the source value before it is mapped and on the mapped value before it is assigned or returned, at the root and at every nested site of the matching type:

```csharp
[Mappa]
[MappaStaticDependency(typeof(SpaceCollapser))]
public sealed partial class Mapper
{
    // Trims every string. Applied to all the map methods of this mapper.
    [MappaTransformer]
    [MappaIgnore]
    private static string Trim(string value) => value.Trim();

    // Upper-cases only the Code member; runs before the class-level Trim.
    [MappaApplyTransformer(nameof(Source.Code), nameof(ToUpper))]
    public partial Target Map(Source input);

    // Transformers may take a MappaContext when the map method has one.
    [MappaApplyTransformer(nameof(Source.Note), nameof(Truncate))]
    public partial NoteTarget MapNote(Source input, MappaContext context);

    // Transformers of dependencies are ignored unless explicitly enabled.
    [MappaSettings(MappaTransformerFromDependency = BooleanSetting.Enable)]
    public partial LabelTarget MapLabel(Source input);

    [MappaIgnore]
    private static string ToUpper(string value) => value.ToUpperInvariant();

    [MappaIgnore]
    private static string Truncate(string value, MappaContext context) { /* ... */ }
}
```

A transformer must have the same input and output type (nullability included) and may be `T Method(T)`, `T Method(in T)`, `T Method(T, MappaContext)` or `T Method(in T, MappaContext)`. Per site the order is: `[MappaApplyTransformer]` attributes, local `[MappaTransformer]` methods, base-class transformers, then dependency transformers when `MappaTransformerFromDependency` is enabled. Invalid signatures warn with **MP00110**, unresolved apply-transformer methods with **MP00111**, unknown source members are errors (**MP00112**), `ClassType` together with `FieldName` warns with **MP00113**, and `[MappaApplyTransformer]` on an `IQueryable` projection method is an error (**MP00114**). Class-level transformers are not applied inside projections but do not invalidate them.

See also: [MappaValueTransformerMapper.cs](../Mappa.Samples/MappaValueTransformerMapper.cs) and [Mappa attributes](./mappa-attributes.md#mappatransformer-and-mappaapplytransformer).


### MappaObjectFactory attribute
`[MappaObjectFactory]` creates a target instance by invoking a named factory instead of `new`. Register factories on the mapper class and/or map method (class and method registrations are unioned). Location overloads match before/after hooks: mapper-local method, static type, or field/property member type.

```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaObjectFactory(typeof(Target), nameof(Create))]
    public partial Target Map(Source input);

    private static Target Create() => new Target { Tag = "factory" };
}
```

Signature tiers (first match wins): `(Source, MappaContext)` or `(Source)` return a fully produced target; `(MappaContext)` or `()` behave like an empty constructor (properties are filled afterward); any other parameter list behaves like a parameterized constructor. Duplicate target-type registrations report **MP00062**; unresolved factories warn with **MP00063** and fall back to constructors; factories are incompatible with `IQueryable` projection (**MP00064**).

See also: [MappaObjectFactoryMapper.cs](../Mappa.Samples/MappaObjectFactoryMapper.cs) and [Mappa attributes](./mappa-attributes.md#mappaobjectfactory).

### Empty-to-empty types
When the target type has a parameterless constructor and **no** mappable members after empty-constructor filters (and no eligible instance fields on the type), Mappa still emits `new TTarget()` with no property initializers. The same rule applies to **nested** empty targets. This covers marker / unit types (for example protobuf `google.protobuf.Empty`). If the target has mappable candidates and none are mapped, empty-constructor selection still fails.

See [EmptyToEmptyMapper.cs](../Mappa.Samples/EmptyToEmptyMapper.cs).

### MappaUseConstructor attribute
`[MappaUseConstructor]` forces empty or parameterized constructor selection when both would otherwise compete, or picks an exact signature among several parameterized constructors. Omit `typeof(T)` to target the map method return type; pass `typeof(TTarget)` (or the optional generic `targetType`) for a nested type.

```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaUseConstructor(ConstructorType.NoParameters)]
    public partial Target MapForceEmpty(Source input);

    [MappaUseConstructor(ConstructorType.WithParameters, typeof(string))]
    public partial Target MapForceParameterized(Source input);

    [MappaUseConstructor<int, string>]
    public partial Target MapExact(Source input);

    [MappaUseConstructor(typeof(NestedTarget), ConstructorType.WithParameters, typeof(string))]
    public partial OuterTarget MapNested(OuterSource input);
}
```

Object factories still take precedence for a registered target type. Diagnostics **MP00087–MP00092** cover missing constructors, invalid configuration, unmappable selections, duplicates, and unused attributes.

See also: [MappaUseConstructorMapper.cs](../Mappa.Samples/MappaUseConstructorMapper.cs) and [Mappa attributes](./mappa-attributes.md#mappauseconstructor).

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
Map methods that take `IQueryable<TSource>` and return `IQueryable<TTarget>` emit deferred `Select` projections suitable for ORM providers such as EF Core. Projection is signature-driven by default. Optionally apply `[MappaProjection]` on the mapper class or method (`Enabled` defaults to `Enable`; use `Enabled = Disable` to fall through to the eager container path).

```csharp
[Mappa]
[MappaProjection]
public static partial class OrderMapper
{
#pragma warning disable MP00086 // Companion MapOrder is inlined into the Select expression by design.
    [MappaUseProperty(nameof(OrderDto.Title), nameof(Order.Name))]
    public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
#pragma warning restore MP00086

    [MappaUseProperty(nameof(OrderDto.Title), nameof(Order.Name))]
    private static partial OrderDto MapOrder(Order order);
}

// Typical EF Core usage:
var dtos = await context.Orders.ProjectToDto().ToListAsync();
```

Element maps may include nested collections (`List<T>` / arrays via nested `Select` + `ToList`/`ToArray`), value tuples, and common `Guid`↔`string` / `Guid`↔`byte[]` conversions. Projection-specific `[MappaSettings]` overrides (`ProjectionGuidFormat`, `ProjectionEnumStringMapSetting`, `ProjectionEnumToEnumMapSetting`, `ProjectionCaseInsensitiveEnumMap`, `ProjectionCultureInfoSetting`, `ProjectionCultureName`) apply only inside projection element strategies and fall back to the non-prefixed settings when unset.

When a private companion element map is replaced by a re-derived structural expression, the generator reports warning **MP00086** (suppress with `#pragma` when the companion is intentional).

Projection limitations:

- No `[MappaBeforeMap]` / `[MappaAfterMap]` hooks and no `MappaContext` parameter on the projection method.
- Nested `IQueryable` properties and polymorphic root element maps are not supported (nested **collections** are supported).
- Prefer numeric or description enum mappings over case-insensitive member-name matching for provider translation.
- Mapping `IQueryable<TSource>` to a concrete collection (for example `List<TTarget>`) materializes eagerly and may emit warning MP00061.
- Generated projection methods are annotated with `[RequiresDynamicCode]` because they build expression trees at runtime; they are **not compatible with [Native AOT](https://learn.microsoft.com/dotnet/core/deploying/native-aot)** deployment.
- Provider translation is not guaranteed for every expression (for example some `Guid`↔`byte[]` shapes); verify against your ORM.

See also: [algorithm — IQueryable projection](./mappa-generator-algorithm.md#7a-iqueryable-projection-strategy), [error codes MP00055–MP00061 / MP00086](./error-codes.md), [IQueryableProjectionMapper.cs](../Mappa.Samples/IQueryableProjectionMapper.cs), and [IQueryableProjectionSqliteMapper.cs](../Mappa.Samples/IQueryableProjectionSqliteMapper.cs).

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

### MappaNullSubstitute attribute
When a source member is null, substitute a constant (`AssignValue`) or call a method (`InvokeMethod`) before mapping continues. The substitute is typed as the **source** member; conversion to the target runs afterward. Mapping `T?` → non-nullable `T` is allowed for that member because the substitute yields non-null `T`.

```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaNullSubstitute(nameof(Target.Name), AssignValue = "unknown")]
    [MappaNullSubstitute(nameof(Target.Nickname), InvokeMethod = nameof(DefaultNickname))]
    public partial Target Map(Source source);

    private static string DefaultNickname(string? value) => "guest";
}
```

`InvokeMethod` is not supported on `IQueryable` projections (**MP00118**). See also: [MappaNullSubstituteMapper.cs](../Mappa.Samples/MappaNullSubstituteMapper.cs), [attributes](./mappa-attributes.md#mappanullsubstitute), diagnostics **MP00115–MP00118**.

### ReplaceNullWithEmptyContainer setting
Enable `MappaSettings.ReplaceNullWithEmptyContainer` (or `mappa.replacenullwithemptycontainer` in `.editorconfig`) so a null source collection or dictionary maps to an empty target container instead of `null` / throw:

```csharp
[Mappa]
[MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable)]
public sealed partial class Mapper
{
    public partial List<string> Map(List<string>? source);
}
```

See also: [ReplaceNullWithEmptyContainerMapper.cs](../Mappa.Samples/ReplaceNullWithEmptyContainerMapper.cs), [attributes](./mappa-attributes.md#replacenullwithemptycontainer).

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

### MappaMustMapTargetProperty attribute
When mapping via an empty constructor, require that listed (or all) non-required target properties are mapped:

```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaMustMapTargetProperty(nameof(Target.PropertyA), nameof(Target.PropertyB))]
    public partial Target MapListed(Source source);
}

[Mappa]
public sealed partial class AllPropertiesMapper
{
    [MappaMustMapTargetProperty]
    public partial Target MapAll(Source source);
}
```

See also: [MappaMustMapTargetPropertyAttributeMapper.cs](../Mappa.Samples/MappaMustMapTargetPropertyAttributeMapper.cs).

### MappaAllowInaccessibleSourceMembers / TargetMembers
Opt in to private or protected properties (and fields when `MapSourceFields` / `MapTargetFields` are enabled) via generated `UnsafeAccessor` helpers (.NET 8+ / C# 12+). Target field writes are gated by `AllowFields` (default `true`):

```csharp
[Mappa]
public sealed partial class Mapper
{
    // All eligible inaccessible source and target members, including a private constructor.
    [MappaAllowInaccessibleSourceMembers]
    [MappaAllowInaccessibleTargetMembers]
    public partial Target MapAll(Source source);

    // Named properties plus private constructor access.
    [MappaAllowInaccessibleSourceMembers("Name")]
    [MappaAllowInaccessibleTargetMembers("Name", "Age")]
    public partial Target MapNamed(Source source);

    // Private constructor only (public setters).
    [MappaAllowInaccessibleSourceMembers("Name")]
    [MappaAllowInaccessibleTargetMembers(AllowProperties = false)]
    public partial TargetWithPublicSetters MapCtorOnly(Source source);

    // Named inaccessible setters only; Age is not opted in.
    [MappaAllowInaccessibleSourceMembers("Name")]
    [MappaAllowInaccessibleTargetMembers("Name", AllowConstructors = false)]
    public partial TargetWithPublicCtor MapPropertiesOnly(Source source);
}
```

See also: [InaccessibleMembersMapper.cs](../Mappa.Samples/InaccessibleMembersMapper.cs).

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

### MappaDependencyInjection attribute
Use `[MappaDependencyInjection]` on a **separate** partial registrar class to generate a method that registers `[Mappa]` mappers with `Microsoft.Extensions.DependencyInjection`. By default only mappers in the same assembly are discovered; use `InjectFromAssemblies` with marker types to also register `[Mappa]` mappers from other assemblies (additive):

```csharp
using Mappa.Attributes;
using Microsoft.Extensions.DependencyInjection;

[Mappa]
public sealed partial class PersonMapper
{
    public partial PersonDto Map(Person source);
}

// Marker type from another assembly that contains [Mappa] mappers:
// using Other.Company.Mappers;

[MappaDependencyInjection(
    "RegisterMappers",
    InjectFromAssemblies = new[] { typeof(Other.Company.Mappers.ExternalMapperMarker) },
    IgnoreType = new[] { typeof(LegacyMapper) })]
public static partial class MapperRegistrar
{
}

// Usage:
var services = new ServiceCollection();
services.RegisterMappers();
var mapper = services.BuildServiceProvider().GetRequiredService<PersonMapper>();
```

Reference `Microsoft.Extensions.DependencyInjection` in the project. Configure lifetime, interface injection, method name, accessibility, exclusions (`IgnoreType`), and cross-assembly discovery (`InjectFromAssemblies`) via attribute properties. See [Mappa attributes — MappaDependencyInjection](./mappa-attributes.md#mappadependencyinjection) for defaults and diagnostics **MP00070**–**MP00073**.

See also: [MappaDependencyInjectionRegistrar.cs](../Mappa.Samples/MappaDependencyInjectionRegistrar.cs) (`IgnoreType`), [MappaDependencyInjectionSameAssemblyRegistrar.cs](../Mappa.Samples/MappaDependencyInjectionSameAssemblyRegistrar.cs), and [MappaDependencyInjectionMapper.cs](../Mappa.Samples/MappaDependencyInjectionMapper.cs).

### Polymorphism support
Use `[MappaTypeMapping]` to map different concrete source types to different target types. Use `[MappaTypeMappingDefault]` to define the fallback behaviour. The generic helpers `[MappaTypeMapping<TTarget, TSource>]` and `[MappaTypeMappingDefault<TDefault>]` (for `MapSourceType` with an explicit target) are equivalent to the `typeof` forms:

```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaTypeMapping<TargetFirst, SourceFirst>]
    [MappaTypeMapping<TargetSecond, SourceSecond>]
    public partial TargetBase Map(SourceBase source);
}
```

The non-generic form remains supported:

```csharp
[MappaTypeMapping(typeof(TargetFirst), typeof(SourceFirst))]
[MappaTypeMapping(typeof(TargetSecond), typeof(SourceSecond))]
```

See also: [PolymorphismMappers.cs](../Mappa.Samples/PolymorphismMappers.cs) and [Mappa attributes](./mappa-attributes.md#mappatypemapping-and-mappatypemappingdefault).

### Protobuf oneof polymorphism
Protobuf `oneof` fields with **message-typed** arms can be mapped polymorphically. Use `[MappaProtobufPolymorphismSetup]` to name the generated `{Oneof}Case` property, one `[MappaProtobufTypeMapping]` per arm and, optionally, `[MappaProtobufTypeMappingDefault]` for the fallback (`Throw` by default, `NoMap`, or `MapTo` a named target arm). All arguments use the (target, source) order.

Given this protobuf definition:

```proto
message OkPayload { string Value = 1; }
message ErrorPayload { int32 Code = 1; }
message ResultMessage
{
  oneof Result
  {
    OkPayload Ok = 1;
    ErrorPayload Error = 2;
  }
}
```

Protobuf to POCO (dispatches on `source.ResultCase`):

```csharp
[Mappa]
public sealed partial class Mapper
{
    [MappaProtobufPolymorphismSetup("ResultCase")]
    [MappaProtobufTypeMapping(typeof(OkModel), "Ok")]
    [MappaProtobufTypeMapping(typeof(ErrorModel), "Error")]
    public partial ResultModel Map(ResultMessage source);
}
```

POCO to protobuf (dispatches on the runtime type of the source; unlisted POCOs leave the oneof unset):

```csharp
[MappaProtobufPolymorphismSetup("ResultCase")]
[MappaProtobufTypeMapping("Ok", typeof(OkModel))]
[MappaProtobufTypeMapping("Error", typeof(ErrorModel))]
[MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.NoMap)]
public partial ResultMessage Map(ResultModel source);
```

Protobuf to protobuf (the two-argument setup is (target case property, source case property); unmatched sources, including an unset oneof, are mapped into the `Fallback` arm of the target):

```csharp
[MappaProtobufPolymorphismSetup("ReplyCase", "ResultCase")]
[MappaProtobufTypeMapping("Ok", "Ok")]
[MappaProtobufTypeMapping("Error", "Error")]
[MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.MapTo, "Fallback")]
public partial ReplyMessage Map(ResultMessage source);
```

`NoMap` and `MapTo` require a protobuf target; proto→POCO supports only `Throw`. The protobuf attributes cannot be combined with `[MappaTypeMapping]` on the same method.

See also: [ProtobufOneofPolymorphismMappers.cs](../Mappa.Samples/ProtobufOneofPolymorphismMappers.cs) and [Mappa attributes](./mappa-attributes.md#protobuf-oneof-polymorphism).

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

### Bson and Protobuf well-known types
The generator can emit mappings for common MongoDB Bson and Google Protobuf well-known types when you opt in with `[MappaSettings]`. Reference the library packages yourself — there are no separate `Mappa.Dependency.*` packages.

**Migration from removed dependency packages**

1. Remove package references to `Mappa.Dependency.Bson`, `Mappa.Dependency.Protobuf`, and their `.DependencyInjection` companions.
2. Add [MongoDB.Bson](https://www.nuget.org/packages/MongoDB.Bson/) and/or [Google.Protobuf](https://www.nuget.org/packages/Google.Protobuf/) as needed.
3. Enable `BsonWellKnownTypes` and/or `ProtobufWellKnownTypes` on `[MappaSettings]` (class or method).
4. Delete `[MappaDependency]` fields that pointed at `MappaBsonMapper` / `MappaProtobufMapper`, and stop calling `RegisterMappaBson` / `RegisterMappaProtobuf`.

```csharp
using Mappa.Attributes;
using Google.Protobuf.WellKnownTypes;
using MongoDB.Bson;

[Mappa]
[MappaSettings(
    BsonWellKnownTypes = BooleanSetting.Enable,
    ProtobufWellKnownTypes = BooleanSetting.Enable)]
public sealed partial class Mapper
{
    public partial ObjectId MapToObjectId(string source);
    public partial Timestamp MapToTimestamp(DateTime source);
}
```

Supported pairs (when the setting is `Enable` and the type is in the compilation) include `ObjectId` ↔ `string`/`byte[]`, `Decimal128` → `decimal`, `Timestamp`/`Duration` date-time conversions, and protobuf scalar wrappers. See [algorithm §4b](./mappa-generator-algorithm.md#4b-bson-well-known-types-strategy) and [§4c](./mappa-generator-algorithm.md#4c-protobuf-well-known-types-strategy).

See also: [BsonWellKnownTypesMapper.cs](../Mappa.Samples/BsonWellKnownTypesMapper.cs) and [ProtobufWellKnownTypesMapper.cs](../Mappa.Samples/ProtobufWellKnownTypesMapper.cs).

### OneOf well-known types
The generator can emit mappings for [OneOf](https://www.nuget.org/packages/OneOf/) / `OneOfBase` (including the Option shape `OneOf<T, None>`) when you opt in with `[MappaSettings]`. Reference the [OneOf](https://www.nuget.org/packages/OneOf/) package yourself — there is no separate `Mappa.Dependency.OneOf` package. `IQueryable` projection involving OneOf remains unsupported.

```csharp
using Mappa.Attributes;
using OneOf;
using OneOf.Types;

[Mappa]
[MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
public sealed partial class Mapper
{
    public partial string Map(OneOf<string, None> input); // throws MappaException when None
    public partial string? MapNullable(OneOf<string, None> input); // null when None
    public partial OneOf<string, None> MapToOption(string? input); // None when null
    public partial string MapUnion(OneOf<string, int> input); // unique exact/assignable arm
    public partial OneOf<string, int> MapIdentity(string input); // identity wrap (also works with the setting off)
}
```

See also: [OneOfWellKnownTypesMapper.cs](../Mappa.Samples/OneOfWellKnownTypesMapper.cs).

## Inline mapping with MappaInterceptor

Use `[MappaInterceptor]` when you want mapping inlined at call sites via C# interceptors instead of (or in addition to) classic partial `Map` methods.

### Setup

1. Tag a **partial**, **non-static** class with `[MappaInterceptor]` (optional constructor argument renames the stub from `MappaMap`).
2. List the mapper namespace in the project file:

```xml
<PropertyGroup>
  <MappaInterceptorsNamespaces>MappaTutorial</MappaInterceptorsNamespaces>
</PropertyGroup>
```

### Basic call site

```csharp
using Mappa;
using Mappa.Attributes;

namespace MappaTutorial;

[MappaInterceptor]
public sealed partial class InlineMapper
{
    public Target Map(Source input) => this.MappaMap<Target>(input);
}
```

The generator emits interceptable stubs on the class and an interceptor method per discovered call site. The stub takes `object` (struct sources are boxed); the interceptor casts to the inferred source type and runs the same strategy chain as `[Mappa]`.

### Configure with `With`

Pass settings, static dependencies, and method attributes as `new` expressions only (compile-time configuration; runtime is a no-op identity):

```csharp
public Target Map(Source input) =>
    this.MappaMap<Target>(input)
        .With(
            mappaMethodAttributes:
            [
                new MappaUsePropertyAttribute(nameof(Target.PropertyOne), nameof(Source.PropertyOne)),
                new MappaAssignFromConstantAttribute(nameof(Target.PropertyThree), "fixed"),
            ]);
```

Supported method attributes are those that implement `IMappaMethodAttribute` (most method-level Mappa attributes except `MappaSettings`, which has its own `With` parameter). `[MappaProjection]` and `IQueryable` mappings are **not** supported on this route (**MP00125** / **MP00126**).

### Interceptor-only method dependencies

Mark helper map methods with `[MappaInterceptorMethodDependency]` when they should be reused as nested dependencies on the interceptor path but must not participate on a co-located `[Mappa]` path:

```csharp
[MappaInterceptor]
public sealed partial class InlineMapper
{
    [MappaInterceptorMethodDependency]
    private static long MapId(int id) => id;

    public Target Map(Source input) => this.MappaMap<Target>(input);
}
```

See also: [MappaInterceptorAttributeMapper.cs](../Mappa.Samples/MappaInterceptorAttributeMapper.cs), [MappaInterceptorStrategyMapper.cs](../Mappa.Samples/MappaInterceptorStrategyMapper.cs), [MappaInterceptorDependencyMapper.cs](../Mappa.Samples/MappaInterceptorDependencyMapper.cs), [attributes](./mappa-attributes.md#mappainterceptor).

For a complete catalog of samples, see [Mappa.Samples](../Mappa.Samples/README.md).