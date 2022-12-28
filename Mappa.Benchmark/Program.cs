// <copyright file="Program.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable CA1852

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

using Mappa.Benchmark.Benchmarks;

var config = DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator);
BenchmarkRunner.Run<MapStringToObjectBenchmark>(config);
#pragma warning restore CA1852