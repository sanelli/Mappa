# Development
This document explains how to develop Mappa.

## NuGet Setup
In order to be able to compile you need to setup a `nuget.config` like the following:
```XML
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <add key="NuGet.org" value="https://api.nuget.org/v3/index.json" />
        <add key="Local .packages" value="file://{PATH-TO-YOUR-REPO}/.packages" />
    </packageSources>
</configuration>
```

The `nuget.config` can be generated automatically by running: `./Scripts/CreateNuGetConfig.ps1`

The solution automatically stores all NuGet packages in the `.packages\` folder;
in order to have all the projects compiled, invoke the script `./Scripts/RebuildUponVersionChange.ps1`.

Every time a change in the Mappa or Mappa.Generator project is made, a new version should be generated locally by:
- Updating the `MappaVersion.targets` by increasing the alpha version
- Run the `./Scripts/RebuildUponVersionChange.ps1` script

## Scripts
### Run tests and report code coverage
`./Scripts/RunTestsAndReportCoverage.ps1`

Reports are:
- `.mappa-tests-and-coverage/index.html`
- `.mappa-tests-and-coverage/Summary.md`
- `.mappa-tests-and-coverage/Summary.xml`

### Run benchmarks
`./Scripts/RunBenchmarks.ps1`

## References
- [Roslyn API FAQ](https://github.com/dotnet/roslyn-sdk/blob/main/samples/CSharp/APISamples/FAQ.cs)
- [Source generator Cookbook](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md)
- [Incremental Generators](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md)