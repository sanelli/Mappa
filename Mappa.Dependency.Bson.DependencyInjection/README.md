# Mappa.Dependency.Bson.DependencyInjection
Dependency injection utility methods for [Mappa Bson](https://www.nuget.org/packages/Mappa.Dependency.Bson/).

## Registration
Call `RegisterMappaBson` on your `IServiceCollection` to register `MappaBsonMapper` and `IMappaBsonMapper` as singletons:

```csharp
using Mappa.Dependency.Bson.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.RegisterMappaBson();
```

You can then inject `IMappaBsonMapper` or `MappaBsonMapper` into your mapper dependencies.

Relevant packages:
- [Mappa](https://www.nuget.org/packages/Mappa/): attributes and `MappaContext` used to drive the source generator;
- [Mappa source generator](https://www.nuget.org/packages/Mappa.Generator/): source generator that automatically generates mappings between classes and value types;
- [Mappa Bson](https://www.nuget.org/packages/Mappa.Dependency.Bson/): methods to map `MongoDB.Bson` objects from the [MongoDB.Bson](https://www.nuget.org/packages/MongoDB.Bson) package into common objects.

Visit the Mappa [documentation](https://github.com/sanelli/Mappa/blob/main/Documentation/README.md) to learn more.
