// <copyright file="GeneratedResultsAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="GeneratedResults"/>.
/// </summary>
public sealed class GeneratedResultsAssertions
    : ObjectAssertions<GeneratedResults, GeneratedResultsAssertions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeneratedResultsAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    public GeneratedResultsAssertions(GeneratedResults value)
        : base(value)
    {
    }

    /// <summary>
    /// Assert that no source code has been generated.
    /// </summary>
    /// <returns>The assertions instance.</returns>
    public GeneratedResultsAssertions NotHaveGeneratedAnySourceCode()
    {
        var runResult = this.HaveOneResult();
        runResult.Should().NotHaveDiagnostics();
        runResult.Should().NotHaveSources();
        return this;
    }

    private GeneratorRunResult HaveOneResult()
    {
        var runResults = this.Subject.Driver.GetRunResult().Results;
        runResults.Should().HaveCount(1);
        return runResults.Single();
    }
}