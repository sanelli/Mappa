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

Runs [BenchmarkDotNet](https://benchmarkdotnet.org/) in **Release** with job **Default** (same job used on `main` merge CI). Default uses more iterations/warmups than Short, so wall-clock time is longer but results are less noisy. Pull requests do not run benchmarks. Outputs land under the gitignored `.mappa-benchmark/` folder:

- `Benchmark.Summary.md` — mean time (ns) and allocated bytes for AutoMapper / Mapster / Mapperly / Mappa
- `Benchmark.Comparison.md` — winner table for the chart subset (best time / best memory; ties list Mappa first; non-Mappa winners include % delta vs Mappa; sole Mappa wins include smaller side delta + second-best mapper name when values differ)
- `MAPPA-BENCHMARK-COMPARISON.svg` — SVG winner table for the same chart subset (memory winners are `n/a` for `StringToEnumBenchmark` / `EnumToStringBenchmark`, matching the memory charts)
- `MAPPA-BENCHMARK-TIME.svg` / `MAPPA-BENCHMARK-MEMORY.svg` — grouped bar charts for the shared chart subset
- `MAPPA-BENCHMARK-TIME-PERCENTAGES.svg` / `MAPPA-BENCHMARK-MEMORY-PERCENTAGES.svg` — competitor / Mappa percentage bar charts for the same subset

Useful switches:

- `-ChartBenchmarksOnly` — run only the benchmarks used by the TIME/MEMORY/comparison SVGs (what `main` merge CI uses)
- `-SkipRun` — regenerate markdown/SVG outputs from existing `BenchmarkDotNet.Artifacts/results`
- `-Filter "<pattern>"` — custom BenchmarkDotNet filter (default `*`)
- `-ListAvailable` — list matching benchmarks without running them

After a local run (or on `main` CI), publish the latest-run SVGs to the shared gist with:

`./Scripts/UpdateBenchmarkGists.ps1`

That script uploads to gist [`7f4a85bc809328b4821b03125f9190cb`](https://gist.github.com/sanelli/7f4a85bc809328b4821b03125f9190cb):

- `MAPPA-BENCHMARK-COMPARISON.svg`
- `MAPPA-BENCHMARK-TIME.svg`
- `MAPPA-BENCHMARK-MEMORY.svg`
- `MAPPA-BENCHMARK-TIME-PERCENTAGES.svg`
- `MAPPA-BENCHMARK-MEMORY-PERCENTAGES.svg`

Gist uploads (including coverage badges/history via `./Scripts/UpdateCodeCoverageGists.ps1`) retry up to **5** times with a random **2–10 second** delay between attempts on transient API errors (for example HTTP 409).

Code coverage history uses version-override merge when publishing `MAPPA-CODE-COVERAGE-HISTORY.MD`.

See [`Mappa.Benchmark/README.md`](../Mappa.Benchmark/README.md) for the scenario suite and how to read results.

## References
- [Roslyn API FAQ](https://github.com/dotnet/roslyn-sdk/blob/main/samples/CSharp/APISamples/FAQ.cs)
- [Source generator Cookbook](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md)
- [Incremental Generators](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md)