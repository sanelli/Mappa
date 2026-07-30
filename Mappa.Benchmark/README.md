# Mappa.Benchmark

BenchmarkDotNet suite comparing **Mappa**, **Mapperly**, **Mapster**, and **AutoMapper** on common mapping scenarios.

## How to run

From the repository root (PowerShell):

```powershell
.\Scripts\RunBenchmarks.ps1
```

CI uses the chart subset only:

```powershell
.\Scripts\RunBenchmarks.ps1 -ChartBenchmarksOnly
```

Jobs use BenchmarkDotNet **Short** run mode. Prefer a Release rebuild first (`.\Scripts\RebuildUponVersionChange.ps1`).

Outputs are written under `.mappa-benchmark/` (gitignored). See [Documentation/development.md](../Documentation/development.md) for the full list of artifacts and gist publishing via `UpdateBenchmarkGists.ps1`.

## Interpreting results

- **AutoMapper is the ratio baseline** (`[Benchmark(Baseline = true)]`) on every scenario.
- **Lower mean time and lower allocated bytes are better.**
- Summary tables report nanoseconds and bytes; the TIME/MEMORY bar charts plot **microseconds** and **kilobytes** with a **50** unit Y-axis tick.

## Scenario suite

| Area | Benchmark | Notes |
|------|-----------|--------|
| Object graph | `SpotifyBenchmark` | Nested album/artist/tracks; fixed Bogus seed |
| Objects | `ClassToClassBenchmark`, `RecordToRecordBenchmark`, `StructToStructBenchmark` | Same property shape across kinds |
| Enums | `EnumToIntBenchmark`, `IntToEnumBenchmark`, `EnumToStringBenchmark`, `StringToEnumBenchmark`, `EnumToEnumBenchmark` | |
| Collections | `ArrayToListBenchmark`, `ListToArrayBenchmark`, `ListToHashSetBenchmark`, `DictionaryBenchmark` | |
| Fast collections | `FastListToArrayBenchmark` | Mappa with `FastCollections`; others use their default path |
| Memory | `MemoryToArrayBenchmark`, `ArrayToMemoryBenchmark` | |
| Polymorphism | `PolymorphicBenchmark` | Derived-type maps |
| IQueryable | `IQueryableProjectionBenchmark` | In-memory `AsQueryable()` + `.ToList()` |
| Nested DTO | `NestedDtoBenchmark` | Multi-level parent/child |

### Chart / CI subset

`MAPPA-BENCHMARK-TIME.svg` and `MAPPA-BENCHMARK-MEMORY.svg` (and CI `-ChartBenchmarksOnly`) include:

- `ArrayToListBenchmark`
- `DictionaryBenchmark`
- `ListToArrayBenchmark`
- `FastListToArrayBenchmark`
- `IQueryableProjectionBenchmark`
- `NestedDtoBenchmark`
- `ListToHashSetBenchmark`

## Spotify attribution

The Spotify object-graph models are adapted from [mjebrahimi/Benchmark.netCoreMappers](https://github.com/mjebrahimi/Benchmark.netCoreMappers). See [`Spotify/README.md`](Spotify/README.md).