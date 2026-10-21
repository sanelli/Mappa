foreach ($project in $(
    "Mappa", "Mappa.Tests",
    "Mappa.Generator", "Mappa.Generator.Tests",
    "Mappa.Samples", "Mappa.Samples.Tests", "Mappa.Samples.Aot", "Mappa.Benchmark"))
{
    dotnet build $project -c Release
    if (-not $?)
    {
        exit 1
    }
}

exit 0