# Mappa
Mapper using source generators.

## NuGet
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

The projects automatically store all NuGet packages in `.packages\` folder.

## Scripts
### Check version matches
`./Scripts/CheckVersions.ps1`

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