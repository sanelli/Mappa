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
That script builds every project with **`-c Release`**.

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

Runs [BenchmarkDotNet](https://benchmarkdotnet.org/) in **Release** with job **Short** (same job used by CI). Outputs land under the gitignored `.mappa-benchmark/` folder:

- `Benchmark.Summary.md` — mean time (ns) and allocated bytes for AutoMapper / Mapster / Mapperly / Mappa
- `MAPPA-BENCHMARK-TIME.svg` / `MAPPA-BENCHMARK-MEMORY.svg` — grouped bar charts for the shared chart subset
- `history-table.md` — Mappa-only rows (`TIME_NS` / `ALLOC_B`) for gist history

Useful switches:

- `-ChartBenchmarksOnly` — run only the benchmarks used by the TIME/MEMORY SVGs (what CI uses)
- `-SkipRun` — regenerate markdown/SVG outputs from existing `BenchmarkDotNet.Artifacts/results`
- `-Filter "<pattern>"` — custom BenchmarkDotNet filter (default `*`)
- `-ListAvailable` — list matching benchmarks without running them

After a local run (or on `main` CI), publish history + SVGs to the shared gist with:

`./Scripts/UpdateBenchmarkGists.ps1`

That script downloads `MAPPA-BENCHMARK-HISTORY.md` from gist [`7f4a85bc809328b4821b03125f9190cb`](https://gist.github.com/sanelli/7f4a85bc809328b4821b03125f9190cb), appends `history-table.md`, regenerates the TIME/MEMORY history SVGs, and uploads:

- `MAPPA-BENCHMARK-HISTORY.md`
- `MAPPA-BENCHMARK-TIME.svg`
- `MAPPA-BENCHMARK-MEMORY.svg`
- `MAPPA-BENCHMARK-TIME-HISTORY.svg`
- `MAPPA-BENCHMARK-MEMORY-HISTORY.svg`

See [`Mappa.Benchmark/README.md`](../Mappa.Benchmark/README.md) for the scenario suite and how to read results.

## References
- [Roslyn API FAQ](https://github.com/dotnet/roslyn-sdk/blob/main/samples/CSharp/APISamples/FAQ.cs)
- [Source generator Cookbook](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md)
- [Incremental Generators](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md)