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

Numeric format properties (for example `IntFormat`, `DecimalFormat`) apply to `ToString` only. Culture applies to both parsing and converting to `string`.

See also: [InvokeParseMapper.cs](../Mappa.Samples/InvokeParseMapper.cs), [InvokeToStringMapper.cs](../Mappa.Samples/InvokeToStringMapper.cs), and [Mappa attributes](./mappa-attributes.md#mappasettings).

### Property name matching settings
`ForceCaseInsensitivePropertyMap` and `IgnoreUnderscoreForPropertyMap` change how source and target properties are paired by name during constructor-map mapping:

```csharp
[Mappa]
[MappaSettings(
    ForceCaseInsensitivePropertyMap = BooleanSetting.Enable,
    IgnoreUnderscoreForPropertyMap = BooleanSetting.Enable)]
public sealed partial class Mapper
{
    public partial Target Map(Source input);
}
```

See also: [PropertyMapNameSettingsMapper.cs](../Mappa.Samples/PropertyMapNameSettingsMapper.cs).

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
