// <copyright file="NullableToNonNullableMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="NullableToNonNullableMapStrategy"/>.
/// </summary>
public class NullableToNonNullableMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created between two enums.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNullableToNonNullable()
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
                public partial int Map(TestEnum? input);
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