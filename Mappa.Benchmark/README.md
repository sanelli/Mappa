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

Jobs use BenchmarkDotNet **Default** run mode (more iterations/warmups than Short — longer wall-clock, lower noise). Prefer a Release rebuild first (`.\Scripts\RebuildUponVersionChange.ps1`).

Outputs are written under `.mappa-benchmark/` (gitignored). See [Documentation/development.md](../Documentation/development.md) for the full list of artifacts and gist publishing via `UpdateBenchmarkGists.ps1`.

## Interpreting results

- **AutoMapper is the ratio baseline** (`[Benchmark(Baseline = true)]`) on every scenario.
- **Lower mean time and lower allocated bytes are better.**
- Summary tables report nanoseconds and bytes; the TIME bar chart uses a **25** unit Y-axis tick and the MEMORY bar chart a **100** unit tick (µs / KB). Percentage charts emphasize the **100%** guide in red (dashed) and bold value labels **above** 100%.

## Scenario suite

| Area | Benchmark | Notes |
|------|-----------|--------|
| Object graph | `SpotifyBenchmark` | Nested album/artist/tracks; fixed Bogus seed |
| Objects | `ClassToClassBenchmark`, `RecordToRecordBenchmark`, `StructToStructBenchmark` | Same property shape across kinds; fixed Bogus seed |
| Enums | `EnumToIntBenchmark`, `IntToEnumBenchmark`, `EnumToStringBenchmark`, `StringToEnumBenchmark`, `EnumToEnumBenchmark` | Fixed Bogus seed |
| Collections | `ArrayToListBenchmark`, `ListToArrayBenchmark`, `ListToHashSetBenchmark`, `DictionaryBenchmark` | Fixed Bogus seed |
| Fast collections | `FastListToArrayBenchmark` | Mappa with `FastCollections`; others use their default path; fixed Bogus seed |
| Memory | `MemoryToArrayBenchmark`, `ArrayToMemoryBenchmark` | Fixed Bogus seed |
| Polymorphism | `PolymorphicBenchmark` | Derived-type maps; fixed Bogus seed |
| IQueryable | `IQueryableProjectionBenchmark` | In-memory `AsQueryable()` + `.ToList()`; fixed Bogus seed |
| Nested DTO | `NestedDtoBenchmark` | 5-level graph, polymorphic parties/line items, arrays/lists/sets/dicts/queues/stacks/Memory, get-only notes list, enum↔enum/string/int (~1000 elements); fixed Bogus seed |

### Chart / CI subset

`MAPPA-BENCHMARK-COMPARISON.svg`, `MAPPA-BENCHMARK-TIME.svg`, `MAPPA-BENCHMARK-MEMORY.svg`, `MAPPA-BENCHMARK-TIME-PERCENTAGES.svg`, and `MAPPA-BENCHMARK-MEMORY-PERCENTAGES.svg` (and CI `-ChartBenchmarksOnly`) include:

- `ArrayToListBenchmark`
- `ListToArrayBenchmark`
- `FastListToArrayBenchmark`
- `DictionaryBenchmark`
- `NestedDtoBenchmark`
- `IQueryableProjectionBenchmark`
- `StringToEnumBenchmark`
- `EnumToStringBenchmark`
- `ReferenceReusingSharedDagBenchmark`

The comparison SVG lists the best time and best memory mapper(s) per scenario (ties included). When Mappa is not among the winners, the cell also shows the percentage delta versus Mappa. Memory winners are `n/a` for `StringToEnumBenchmark` and `EnumToStringBenchmark` (same exclusions as the absolute and percentage memory charts).

## Spotify attribution

The Spotify object-graph models are adapted from [mjebrahimi/Benchmark.netCoreMappers](https://github.com/mjebrahimi/Benchmark.netCoreMappers). See [`Spotify/README.md`](Spotify/README.md).