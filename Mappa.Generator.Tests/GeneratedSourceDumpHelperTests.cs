// <copyright file="GeneratedSourceDumpHelperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="GeneratedSourceDumpHelper"/>.
/// </summary>
public sealed class GeneratedSourceDumpHelperTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="GeneratedSourceDumpHelper.SanitizeFileNameFragment"/> replaces invalid characters.
    /// </summary>
    [Fact]
    [UnitTest]
    public void SanitizeFileNameFragmentReplacesInvalidCharacters()
    {
        var sanitized = GeneratedSourceDumpHelper.SanitizeFileNameFragment(@"System.Span<byte> path/name");

        sanitized.Should().Be("System.Span_byte__path_name");
    }

    /// <summary>
    /// Test <see cref="GeneratedSourceDumpHelper.SanitizeFileNameFragment"/> returns a fallback for blank values.
    /// </summary>
    /// <param name="value">The blank value.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [UnitTest]
    public void SanitizeFileNameFragmentReturnsEmptyFallbackForBlankValues(string? value)
    {
        GeneratedSourceDumpHelper.SanitizeFileNameFragment(value).Should().Be("empty");
    }

    /// <summary>
    /// Test <see cref="GeneratedSourceDumpHelper.FormatTheoryArgument"/> formats supported values.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <param name="expected">The expected formatted value.</param>
    [Theory]
    [InlineData(null, "null")]
    [InlineData("byte[]", "byte[]")]
    [InlineData(true, "True")]
    [InlineData(false, "False")]
    [InlineData(42, "42")]
    [UnitTest]
    public void FormatTheoryArgumentFormatsSupportedValues(object? argument, string expected)
    {
        GeneratedSourceDumpHelper.FormatTheoryArgument(argument).Should().Be(expected);
    }

    /// <summary>
    /// Test <see cref="GeneratedSourceDumpHelper.BuildDumpFileName"/> for ordinary tests.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildDumpFileNameForOrdinaryTest()
    {
        var fileName = GeneratedSourceDumpHelper.BuildDumpFileName(
            "GuidStrategyIntegrationTests",
            "CanMapFromGuid",
            theoryArguments: null,
            invocationIndex: 1);

        fileName.Should().Be("GuidStrategyIntegrationTests_CanMapFromGuid.g.cs");
    }

    /// <summary>
    /// Test <see cref="GeneratedSourceDumpHelper.BuildDumpFileName"/> includes sanitized theory parameters.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildDumpFileNameForTheoryIncludesSanitizedParameters()
    {
        var fileName = GeneratedSourceDumpHelper.BuildDumpFileName(
            "GuidStrategyIntegrationTests",
            "CanMapFromGuid",
            ["System.Span<byte>"],
            invocationIndex: 1);

        fileName.Should().Be("GuidStrategyIntegrationTests_CanMapFromGuid_System.Span_byte_.g.cs");
    }

    /// <summary>
    /// Test <see cref="GeneratedSourceDumpHelper.BuildDumpFileName"/> appends the invocation index after the first dump.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildDumpFileNameAppendsInvocationIndexAfterFirstDump()
    {
        var fileName = GeneratedSourceDumpHelper.BuildDumpFileName(
            "SomeTests",
            "SomeMethod",
            theoryArguments: null,
            invocationIndex: 2);

        fileName.Should().Be("SomeTests_SomeMethod_2.g.cs");
    }

    /// <summary>
    /// Test <see cref="GeneratedSourceDumpHelper.TryDumpGeneratedSources"/> writes a dump when generation succeeds.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [UnitTest]
    public async Task TryDumpGeneratedSourcesWritesFileWhenGenerationSucceeds()
    {
        var dumpDirectory = Path.Combine(Path.GetTempPath(), "mappa-dump-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dumpDirectory);

        try
        {
            const string sourceCode = """
                                      #nullable enable
                                      using System;
                                      using Mappa.Attributes;

                                      namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                      [Mappa]
                                      public sealed partial class Mapper
                                      {
                                          public partial int Map(int input);
                                      }
                                      """;

            var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);
            GeneratedSourceDumpHelper.TryDumpGeneratedSources(generatedResults.Driver, dumpDirectory);

            var files = Directory.GetFiles(dumpDirectory, "*.g.cs");
            files.Should().NotBeEmpty();
            var content = await File.ReadAllTextAsync(files[0], CancellationToken.None).ConfigureAwait(true);
            content.Should().NotBeNullOrWhiteSpace();
        }
        finally
        {
            if (Directory.Exists(dumpDirectory))
            {
                Directory.Delete(dumpDirectory, recursive: true);
            }
        }
    }

    /// <summary>
    /// Test <see cref="GeneratedSourceDumpHelper.TryDumpGeneratedSources"/> does not write when no sources are generated.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [UnitTest]
    public async Task TryDumpGeneratedSourcesDoesNotWriteWhenNoSourcesAreGenerated()
    {
        var dumpDirectory = Path.Combine(Path.GetTempPath(), "mappa-dump-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dumpDirectory);

        try
        {
            const string sourceCode = """
                                      #nullable enable
                                      namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                      public sealed class NotAMapper
                                      {
                                      }
                                      """;

            var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);
            GeneratedSourceDumpHelper.TryDumpGeneratedSources(generatedResults.Driver, dumpDirectory);

            Directory.GetFiles(dumpDirectory, "*.g.cs").Should().BeEmpty();
        }
        finally
        {
            if (Directory.Exists(dumpDirectory))
            {
                Directory.Delete(dumpDirectory, recursive: true);
            }
        }
    }
}