# Mappa.Dependency.Protobuf.DependencyInjection
Dependency injection utility methods for [Mappa Protobuf](https://www.nuget.org/packages/Mappa.Dependency.Protobuf/).

## Registration
Call `RegisterMappaProtobuf` on your `IServiceCollection` to register `MappaProtobufMapper` and `IMappaProtobufMapper` as singletons:

```csharp
using Mappa.Dependency.Protobuf.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.RegisterMappaProtobuf();
```

You can then inject `IMappaProtobufMapper` or `MappaProtobufMapper` into your mapper dependencies.

Relevant packages:
- [Mappa](https://www.nuget.org/packages/Mappa/): attributes and `MappaContext` used to drive the source generator;
- [Mappa source generator](https://www.nuget.org/packages/Mappa.Generator/): source generator that automatically generates mappings between classes and value types;
- [Mappa Protobuf](https://www.nuget.org/packages/Mappa.Dependency.Protobuf/): methods to map `Google.Protobuf.WellKnownTypes` objects from the [Google.Protobuf](https://www.nuget.org/packages/Google.Protobuf) package into common objects.

Visit the Mappa [documentation](https://github.com/sanelli/Mappa/blob/main/Documentation/README.md) to learn more.
