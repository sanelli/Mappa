// <copyright file="Program.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

await BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .RunAsync(args, DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator))
    .ConfigureAwait(false);