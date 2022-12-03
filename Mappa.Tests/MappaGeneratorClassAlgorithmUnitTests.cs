// <copyright file="MappaGeneratorClassAlgorithmUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Tests;

/// <summary>
/// Unit tess for <see cref="MappaGeneratorClassAlgorithm"/>.
/// </summary>
public sealed class MappaGeneratorClassAlgorithmUnitTests
    : Abstractions.MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Check that non-partial method of a class marked with
    /// <see cref="MappaAttribute"/> is ignored and not code
    /// is actually being generated for that.
    /// </summary>
    /// <returns>The asyc task.</returns>
    [Fact]
    [UnitTest]
    public async Task NonPartialMethodsAreIgnoredAndNotDiagnosticIsReported()
    {
        // Arrange
        var sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public long Map(int input) => input;
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None);

        // Assert
        generatedResults.Should().NotHaveGeneratedAnySourceCode();
    }
}