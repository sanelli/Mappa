// <copyright file="GeneratorRunResultAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Globalization;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="GeneratorRunResult"/>.
/// </summary>
[DebuggerNonUserCode]
public sealed class GeneratorRunResultAssertions
    : ObjectAssertions<GeneratorRunResult, GeneratorRunResultAssertions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeneratorRunResultAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    internal GeneratorRunResultAssertions(GeneratorRunResult value)
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

    /// <summary>
    /// Assert that the generator results have <paramref name="count"/> sources.
    /// </summary>
    /// <param name="count">The expected number of sources.</param>
    /// <returns>The assertions itself.</returns>
    public GeneratorRunResultAssertions HaveSources(int count)
    {
        this.Subject.GeneratedSources.Should().HaveCount(count);
        return this;
    }

    /// <summary>
    /// Assert that the generator results have <paramref name="count"/> diagnostics.
    /// </summary>
    /// <param name="count">The number of expected diagnostics.</param>
    /// <returns>The assertions itself.</returns>
    public GeneratorRunResultAssertions HaveDiagnostics(int count)
    {
        this.Subject.Diagnostics.Should().HaveCount(count);
        return this;
    }

    /// <summary>
    /// Assert that the generator results have a specific diagnostic.
    /// </summary>
    /// <param name="diagnosticDescriptor">The specific diagnostic descriptor.</param>
    /// <param name="parameters">Parameters used to generate the message.</param>
    /// <returns>The assertions itself.</returns>
    public GeneratorRunResultAssertions ContainDiagnostic(
        DiagnosticDescriptor diagnosticDescriptor,
        params object?[] parameters)
    {
        ArgumentNullException.ThrowIfNull(diagnosticDescriptor);
        try
        {
            var expectedMessage = string.Format(
                CultureInfo.CurrentCulture,
                diagnosticDescriptor.MessageFormat.ToString(CultureInfo.CurrentCulture),
                parameters);
            this.Subject.Diagnostics.Should().Contain(diagnostic =>
                diagnostic.Descriptor.Equals(diagnosticDescriptor) &&
                diagnostic.GetMessage(CultureInfo.CurrentCulture).Equals(expectedMessage, StringComparison.Ordinal));
        }
 #pragma warning disable CA1031
        catch
 #pragma warning restore CA1031
        {
            // Skip it
        }

        return this;
    }
}