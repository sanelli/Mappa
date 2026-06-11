# 🗺️ Mappa
Mappa (Italian for _map_) is a source generator for `C#` that can generate code to allow mapping between types, similarly to what [AutoMapper](https://www.nuget.org/packages/AutoMapper) (and other similar tools) does.

The main difference between Mappa and AutoMapper is that Mappa generates code at compile time while AutoMapper only at runtime;
this has multiple benefits:
- the code generated is optimized by the compiler;
- the code generated is pure C# code that does not require any introspection;
- the code generated can be inspected and debugged by developers;
- the code generated works when [Native AOT](https://learn.microsoft.com/dotnet/core/deploying/native-aot) is required;
- the code can be easily shared across different mappers;
- fine-grained mapping can be obtained via attributes on the mapper methods without having to touch the source or the target classes;
- mapper methods can be inside any class (static classes and extension methods are supported);
- you do not need to specify every type that requires a mapping: if a mapping is missing, Mappa will generate it for you;

## See also
- [Mappa algorithm](./mappa-generator-algorithm.md): description of how the algorithm works;
- [Mappa attributes](./mappa-attributes.md): description of the attributes that can impact code generation;
- [Errors and warnings](./error-codes.md): list of errors and warnings that can be raised by Mappa;
- [Tutorial](./tutorial.md): simple tutorial highlighting the main features of Mappa;
- [Mappa.Samples](../Mappa.Samples): a project containing samples showcasing how to use all Mappa features;
- [Development](./development.md): how to extend Mappa functionality;
- [NuGet](./nuget.md): list of NuGet packages provided;
