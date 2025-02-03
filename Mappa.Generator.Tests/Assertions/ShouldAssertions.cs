// <copyright file="ShouldAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Diagnostics;

using Mappa.Generator.Tests.Models;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Fluent assertions extension methods.
/// </summary>
// TODO [#43] Extract to its own project in a different solution/repo.
[DebuggerNonUserCode]
internal static class ShouldAssertions
{
    /// <summary>
    /// Begin asserting on an object of type <see cref="GeneratorRunResult"/>.
    /// </summary>
    /// <param name="generatorRunResult">The target of the assertions.</param>
    /// <returns>The assertions object.</returns>
    public static GeneratorRunResultAssertions Should(this GeneratorRunResult generatorRunResult) => new(generatorRunResult);

    /// <summary>
    /// Begin assertions on an object of type <see cref="GeneratedResults"/>.
    /// </summary>
    /// <param name="generatedResults">The target of the assertions.</param>
    /// <returns>The assertions object.</returns>
    public static GeneratedResultsAssertions Should(this GeneratedResults generatedResults) => new(generatedResults);
}