// <copyright file="MappaGeneratorClassAlgorithmUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;

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
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should().NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it has no parameters.
    /// </summary>
    /// <returns>The asyc task.</returns>
    [Fact]
    [UnitTest]
    public async Task PartialMethodsWithArity0GenerateADiagnosticError()
    {
        // Arrange
        var sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial long Map();
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should().HaveDiagnostics(1);
        generatedResults.Should().ContainDiagnostic(MappaDiagnosticDescriptors.MethodHasInvalidNumberOfParameters, "Map");
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it has two parameters.
    /// </summary>
    /// <returns>The asyc task.</returns>
    [Fact]
    [UnitTest]
    public async Task PartialMethodsWithArity2GenerateADiagnosticError()
    {
        // Arrange
        var sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial long Map(int input1, int input2);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should().HaveDiagnostics(1);
        generatedResults.Should().ContainDiagnostic(MappaDiagnosticDescriptors.MethodHasInvalidNumberOfParameters, "Map");
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it returns void.
    /// </summary>
    /// <returns>The asyc task.</returns>
    [Fact]
    [UnitTest]
    public async Task PartialMethodsReturningVoidGeneratesADiagnosticError()
    {
        // Arrange
        var sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial void Map(int input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should().HaveDiagnostics(1);
        generatedResults.Should().ContainDiagnostic(MappaDiagnosticDescriptors.MethodIsVoid, "Map");
    }
}