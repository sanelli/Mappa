// <copyright file="GeneratedResults.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Tests.Models;

/// <summary>
/// Describe the generated results.
/// </summary>
/// <param name="Driver">The generator driver.</param>
/// <param name="OutputCompilation">The compilation of the output.</param>
/// <param name="Diagnostics">The generated diagnostics.</param>
public sealed record GeneratedResults(GeneratorDriver Driver, Compilation OutputCompilation, IReadOnlyCollection<Diagnostic> Diagnostics);