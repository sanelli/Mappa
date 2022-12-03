// <copyright file="GeneratorRunResultAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="GeneratorRunResult"/>.
/// </summary>
public sealed class GeneratorRunResultAssertions
    : ObjectAssertions<GeneratorRunResult, GeneratorRunResultAssertions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeneratorRunResultAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    public GeneratorRunResultAssertions(GeneratorRunResult value)
        : base(value)
    {
    }

    /// <summary>
    /// Assert that the generator results have no diagnostics.
    /// </summary>
    /// <returns>The assertions itself.</returns>
    public GeneratorRunResultAssertions NotHaveDiagnostics()
    {
        this.Subject.Diagnostics.Should().BeEmpty();
        return this;
    }

    /// <summary>
    /// Assert that the generator results have no sources.
    /// </summary>
    /// <returns>The assertions itself.</returns>
    public GeneratorRunResultAssertions NotHaveSources()
    {
        this.Subject.GeneratedSources.Should().BeEmpty();
        return this;
    }
}