foreach ($project in $(
    "Mappa", "Mappa.Tests",
    "Mappa.Generator", "Mappa.Generator.Tests",
    "Mappa.Dependency.Protobuf", "Mappa.Dependency.Protobuf.DependencyInjection", "Mappa.Dependency.Protobuf.Tests", "Mappa.Dependency.Protobuf.DependencyInjection.Tests",
    "Mappa.Dependency.Bson", "Mappa.Dependency.Bson.DependencyInjection", "Mappa.Dependency.Bson.Tests", "Mappa.Dependency.Bson.DependencyInjection.Tests",
    "Mappa.Samples", "Mappa.Samples.Tests", "Mappa.Samples.Aot", "Mappa.Benchmark"))
{
    dotnet build $project
    if (-not $?)
    {
        exit 1
    }
}

exit 0