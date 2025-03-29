# Mappa.Dependency.Bson
Methods to map `MongoDB.Bson` objects from [MongoDB.Bson](https://www.nuget.org/packages/MongoDB.Bson) package into common objects.
List of supported mappings:
- `MongoDb.Bson.ObjectId` <-> `string`
- `MongoDb.Bson.ObjectId` <-> `byte[]`

Relevant packages:
- [Mappa](https://www.nuget.org/packages/Mappa/): define attribute and classes that can be used by the [Mappa Source Generator](https://www.nuget.org/packages/Mappa.Generator/);
- [Mappa source generator](https://www.nuget.org/packages/Mappa.Generator/): source generator that allows to automatically generate mapping between classes and value types;
- [Mappa Bson dependency](https://www.nuget.org/packages/Mappa.Dependency.Bson.DependencyInjection/): utility methods to register the Bson mapper.

Visit the Mappa [documentation](https://github.com/sanelli/Mappa/blob/main/Documentation/README.md) to learn more.