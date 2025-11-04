// <copyright file="Program.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args, DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator));