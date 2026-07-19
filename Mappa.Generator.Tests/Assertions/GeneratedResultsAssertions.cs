// <copyright file="GeneratedResultsAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Globalization;

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Models;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="GeneratedResults"/>.
/// </summary>
[DebuggerNonUserCode]
internal sealed class GeneratedResultsAssertions
    : ObjectAssertions<GeneratedResults, GeneratedResultsAssertions>
{
    private GeneratorRunResult? generatorRunResult;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneratedResultsAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    internal GeneratedResultsAssertions(GeneratedResults value)
        : base(value, AwesomeAssertions.Execution.AssertionChain.GetOrCreate())
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
    /// <param name="howMany">The number of expected generated sources.</param>
    /// <returns>The assertions instance.</returns>
    public GeneratedResultsAssertions HaveGeneratedSourceCode(int howMany = 1)
    {
        var runResult = this.HaveOneResult();
        runResult.Should().HaveSources(howMany);
        return this;
    }

    /// <summary>
    /// Check it contains a specific diagnostic.
    /// </summary>
    /// <param name="diagnosticDescriptor">The specific diagnostic descriptor.</param>
    /// <param name="parameters">Parameters used to generate the message.</param>
    /// <returns>The assertions instance.</returns>
    public GeneratedResultsAssertions HaveDiagnostic(
        DiagnosticDescriptor diagnosticDescriptor,
        params object?[] parameters)
    {
        var runResult = this.HaveOneResult();
        runResult.Should().ContainDiagnostic(diagnosticDescriptor, parameters);
        return this;
    }

    /// <summary>
    /// Check it contains an ambiguous invoke-method resolution diagnostic mentioning the method name.
    /// </summary>
    /// <param name="methodName">The ambiguous method name.</param>
    /// <returns>The assertions instance.</returns>
    public GeneratedResultsAssertions ContainAmbiguousInvokeMethodResolutionDiagnostic(string methodName)
    {
        var runResult = this.HaveOneResult();
        runResult.Diagnostics.Should().ContainSingle(diagnostic =>
            diagnostic.Descriptor.Equals(MappaDiagnosticDescriptors.AmbiguousInvokeMethodResolution) &&
            diagnostic.GetMessage(CultureInfo.CurrentCulture).Contains(
                $"multiple methods named '{methodName}'",
                StringComparison.Ordinal));
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

    /// <summary>
    /// Check the generated output compilation contains no errors.
    /// </summary>
    /// <returns>The assertions instance.</returns>
    public GeneratedResultsAssertions NotHaveCompilationErrors()
    {
        this.Subject.OutputCompilation
            .GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity is DiagnosticSeverity.Error)
            .Should()
            .BeEmpty();
        return this;
    }

    /// <summary>
    /// Check it contains only warnings.
    /// </summary>
    /// <param name="warningIds">The warning identifiers.</param>
    /// <returns>The assertions instance.</returns>
    public GeneratedResultsAssertions HaveOnlyWarnings(params string[] warningIds)
    {
        var runResult = this.HaveOneResult();
        runResult.Should().HaveOnlyWarnings(warningIds);
        return this;
    }

    /// <summary>
    /// Gets the assertions for the syntax tree.
    /// </summary>
    /// <returns>The syntax tree assertions instance.</returns>
    public CompilationUnitSyntaxAssertions WithCompilationUnit()
    {
        this.Subject.OutputCompilation.SyntaxTrees.Should().HaveCount(2);
        var syntaxTree = this.Subject.OutputCompilation.SyntaxTrees.Last();
        var semanticModel = this.Subject.OutputCompilation.GetSemanticModel(syntaxTree);
        return new CompilationUnitSyntaxAssertions(
            (CompilationUnitSyntax)syntaxTree.GetRoot(),
            semanticModel,
            this.Subject.OutputCompilation);
    }

    /// <summary>
    /// Gets the assertions for the syntax tree.
    /// </summary>
    /// <param name="howMany">The number of expected generated syntax trees.</param>
    /// <returns>The syntax tree assertions instance.</returns>
    public CompilationUnitSyntaxAssertions[] WithCompilationUnits(int howMany)
    {
        this.Subject.OutputCompilation.SyntaxTrees.Should().HaveCount(howMany + 1);
        var syntaxTrees = this.Subject.OutputCompilation.SyntaxTrees.Skip(1);

        return syntaxTrees.Select(syntaxTree =>
            {
                var semanticModel = this.Subject.OutputCompilation.GetSemanticModel(syntaxTree);
                return new CompilationUnitSyntaxAssertions(
                    (CompilationUnitSyntax)syntaxTree.GetRoot(),
                    semanticModel,
                    this.Subject.OutputCompilation);
            })
            .ToArray();
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