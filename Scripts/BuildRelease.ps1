foreach ($project in $(
        "Mappa", "Mappa.Generator"))
{
    dotnet clean -c Release $project
    dotnet build -c Release $project
    if (-not $?)
    {
        exit 1
    }
}

exit 0