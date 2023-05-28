// <copyright file="IntegralToEnumMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="IntegralToEnumMapStrategy"/>.
/// </summary>
public sealed class IntegralToEnumMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created from a numeric value
    /// to an enum.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIntegralToEnum()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            public enum TestEnum
            {
                One,
                Two,
                Three,
            }

            [Mappa]
            public sealed partial class Mapper
            {
                public partial TestEnum Map(int input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        #pragma warning disable
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();

        // TODO: Add correct assertions
        compilationUnitSyntaxAssertions.NotBeNull();
        #pragma warning restore
    }
}