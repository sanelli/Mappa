# Mappa.Dependency.Protobuf
Methods to map `Google.Protobuf.WellKnownTypes` objects from [Google.Protobuf](https://www.nuget.org/packages/Google.Protobuf) package into common objects.
List of supported mappings:
- `Google.Protobuf.WellKnownTypes.Timestamp` <-> [`System.DateTime`](https://learn.microsoft.com/dotnet/api/system.datetime)
- `Google.Protobuf.WellKnownTypes.Timestamp` <-> [`System.DateTimeOffset`](https://learn.microsoft.com/dotnet/api/system.datetimeoffset)
- `Google.Protobuf.WellKnownTypes.Timestamp` <-> [`System.DateOnly`](https://learn.microsoft.com/dotnet/api/system.dateonly) (only for .NET 6 or greater)
- `Google.Protobuf.WellKnownTypes.Timestamp` -> [`System.TimeOnly`](https://learn.microsoft.com/dotnet/api/system.timeonly) (only for .NET 6 or greater)
- `Google.Protobuf.WellKnownTypes.Duration` <-> [`System.TimeSpan`](https://learn.microsoft.com/dotnet/api/system.timespan)

Relevant packages:
- [Mappa](https://www.nuget.org/packages/Mappa/): define attribute and classes that can be used by the [Mappa Source Generator](https://www.nuget.org/packages/Mappa.Generator/);
- [Mappa source generator](https://www.nuget.org/packages/Mappa.Generator/): source generator that allows to automatically generate mapping between classes and value types;
- [Mappa Protobuf dependency](https://www.nuget.org/packages/Mappa.Dependency.Protobuf.DependencyInjection/): utility methods to register the Protobuf mapper.

Visit the Mappa [documentation](https://github.com/sanelli/Mappa/blob/main/Documentation/README.md) to learn more.