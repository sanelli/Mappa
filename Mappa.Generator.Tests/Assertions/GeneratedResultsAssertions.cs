// <copyright file="GeneratedResultsAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="GeneratedResults"/>.
/// </summary>
public sealed class GeneratedResultsAssertions
    : ObjectAssertions<GeneratedResults, GeneratedResultsAssertions>
{
    private GeneratorRunResult? generatorRunResult;

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
        runResult.Should().NotHaveSources();
        return this;
    }

    /// <summary>
    /// Assert that one source code has been generated.
    /// </summary>
    /// <returns>The assertions instance.</returns>
    public GeneratedResultsAssertions HaveGeneratedOneSourceCode()
    {
        var runResult = this.HaveOneResult();
        runResult.Should().HaveSources(1);
        return this;
    }

    /// <summary>
    /// Check it contains a specific diagnostic.
    /// </summary>
    /// <param name="diagnosticDescriptor">The specific diagnostic descriptor.</param>
    /// <param name="parameters">Parameters used to generate the message.</param>
    /// <returns>The assertions instance.</returns>
    public GeneratedResultsAssertions ContainDiagnostic(
        DiagnosticDescriptor diagnosticDescriptor,
        params string[] parameters)
    {
        var runResult = this.HaveOneResult();
        runResult.Should().ContainDiagnostic(diagnosticDescriptor, parameters);
        return this;
    }

    /// <summary>
    /// Check it contains a specific number of diagnostics diagnostic.
    /// </summary>
    /// <param name="count">The expected number of diagnostics.</param>
    /// <returns>The assertions instance.</returns>
    public GeneratedResultsAssertions HaveDiagnostics(int count)
    {
        var runResult = this.HaveOneResult();
        runResult.Should().HaveDiagnostics(count);
        return this;
    }

    /// <summary>
    /// Check it contains no diagnostics.
    /// </summary>
    /// <returns>The assertions instance.</returns>
    public GeneratedResultsAssertions NotHaveDiagnostics()
    {
        var runResult = this.HaveOneResult();
        runResult.Should().NotHaveDiagnostics();
        return this;
    }

    private GeneratorRunResult HaveOneResult()
    {
        if (this.generatorRunResult is null)
        {
            var runResults = this.Subject.Driver.GetRunResult().Results;
            runResults.Should().HaveCount(1);

            this.generatorRunResult = runResults.Single();
        }

        return this.generatorRunResult.Value;
    }
}