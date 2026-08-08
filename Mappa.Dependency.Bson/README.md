# Mappa.Dependency.Bson
Methods to map `MongoDB.Bson` objects from the [MongoDB.Bson](https://www.nuget.org/packages/MongoDB.Bson) package into common objects.

## Supported mappings
The [`IMappaBsonMapper`](IMappaBsonMapper.cs) interface provides the following mappings:
- `MongoDB.Bson.ObjectId` ← `string`
- `MongoDB.Bson.ObjectId` ← `byte[]`
- `MongoDB.Bson.ObjectId?` ← `string?`
- `MongoDB.Bson.ObjectId` → `string`
- `MongoDB.Bson.ObjectId?` → `string?`
- `MongoDB.Bson.ObjectId` → `byte[]`

## Usage with Mappa
Register a `MappaBsonMapper` (or `IMappaBsonMapper`) instance as a dependency on your mapper class:

```csharp
using Mappa.Attributes;
using Mappa.Dependency.Bson;

[Mappa]
public sealed partial class MyMapper
{
    [MappaDependency]
    private readonly MappaBsonMapper bsonMapper = new();

    public partial MyTarget Map(MySource source);
}
```

Mappa will invoke the dependency mapper methods when mapping between `ObjectId` and `string` or `byte[]`.

`MappaBsonMapper` is marked with `[Mappa]` so `[MappaDependencyInjection]` registrars can discover it via `InjectFromAssemblies` (for example `typeof(MappaBsonMapper)`), including from other assemblies. The mapping methods remain hand-written; this package does not run the Mappa source generator.

Relevant packages:
- [Mappa](https://www.nuget.org/packages/Mappa/): attributes and `MappaContext` used to drive the source generator;
- [Mappa source generator](https://www.nuget.org/packages/Mappa.Generator/): source generator that automatically generates mappings between classes and value types;
- [Mappa Bson dependency](https://www.nuget.org/packages/Mappa.Dependency.Bson.DependencyInjection/): utility methods to register the Bson mapper with dependency injection.

Visit the Mappa [documentation](https://github.com/sanelli/Mappa/blob/main/Documentation/README.md) to learn more.
