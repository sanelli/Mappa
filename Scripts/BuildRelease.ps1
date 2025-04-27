foreach ($project in $(
        "Mappa", "Mappa.Generator",
        "Mappa.Dependency.Protobuf", "Mappa.Dependency.Protobuf.DependencyInjection",
        "Mappa.Dependency.Bson", "Mappa.Dependency.Bson.DependencyInjection"))
{
    dotnet clean -c Release $project
    dotnet build -c Release $project
    if (-not $?)
    {
        exit 1
    }
}

exit 0