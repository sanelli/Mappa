# Mappa.Dependency.Protobuf
Methods to map `Google.Protobuf.WellKnownTypes` objects from the [Google.Protobuf](https://www.nuget.org/packages/Google.Protobuf) package into common objects.

## Supported mappings
The [`IMappaProtobufMapper`](IMappaProtobufMapper.cs) interface provides the following mappings:
- `Google.Protobuf.WellKnownTypes.Timestamp` ↔ `System.DateTime` (nullable variants supported)
- `Google.Protobuf.WellKnownTypes.Timestamp` ↔ `System.DateTimeOffset` (nullable variants supported)
- `Google.Protobuf.WellKnownTypes.Timestamp` ↔ `System.DateOnly` (.NET 6 or greater; nullable variants supported)
- `Google.Protobuf.WellKnownTypes.Timestamp` → `System.TimeOnly` (.NET 6 or greater; nullable variants supported)
- `Google.Protobuf.WellKnownTypes.Duration` ↔ `System.TimeSpan` (nullable variants supported)

When mapping protobuf messages with `optional` fields, enable the `ProtobufOptional` setting on `MappaSettings`. See [Mappa attributes](https://github.com/sanelli/Mappa/blob/main/Documentation/mappa-attributes.md) for details.

## Usage with Mappa
Register a `MappaProtobufMapper` (or `IMappaProtobufMapper`) instance as a dependency on your mapper class:

```csharp
using Mappa.Attributes;
using Mappa.Dependency.Protobuf;

[Mappa]
public sealed partial class MyMapper
{
    [MappaDependency]
    private readonly MappaProtobufMapper protobufMapper = new();

    public partial MyTarget Map(MySource source);
}
```

Mappa will invoke the dependency mapper methods when mapping between protobuf well-known types and .NET date/time types.

`MappaProtobufMapper` is marked with `[Mappa]` so `[MappaDependencyInjection]` registrars can discover it via `InjectFromAssemblies` (for example `typeof(MappaProtobufMapper)`), including from other assemblies. The mapping methods remain hand-written; this package does not run the Mappa source generator.

Relevant packages:
- [Mappa](https://www.nuget.org/packages/Mappa/): attributes and `MappaContext` used to drive the source generator;
- [Mappa source generator](https://www.nuget.org/packages/Mappa.Generator/): source generator that automatically generates mappings between classes and value types;
- [Mappa Protobuf dependency](https://www.nuget.org/packages/Mappa.Dependency.Protobuf.DependencyInjection/): utility methods to register the Protobuf mapper with dependency injection.

Visit the Mappa [documentation](https://github.com/sanelli/Mappa/blob/main/Documentation/README.md) to learn more.
