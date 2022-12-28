// <copyright file="ShouldExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Models;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Fluent assertions extension methods.
/// </summary>
public static class ShouldExtensions
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

    /// <summary>
    /// Begin assertions on an object of type <see cref="AttributeSyntax"/>.
    /// </summary>
    /// <param name="attributeSyntax">The target of the assertions.</param>
    /// <returns>The assertions object.</returns>
    public static AttributeSyntaxAssertions Should(this AttributeSyntax attributeSyntax) => new(attributeSyntax);
}