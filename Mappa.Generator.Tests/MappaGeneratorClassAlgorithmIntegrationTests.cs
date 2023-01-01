// <copyright file="MappaGeneratorClassAlgorithmIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tess for <see cref="MappaGeneratorClassAlgorithm"/>.
/// </summary>
public sealed class MappaGeneratorClassAlgorithmIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Check that non-partial method of a class marked with
    /// <see cref="MappaAttribute"/> is ignored and not code
    /// is actually being generated for that.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NonPartialMethodsAreIgnoredAndNotDiagnosticIsReported()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public long Map(int input) => input;
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it has no parameters.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PartialMethodsWithArity0GenerateADiagnosticError()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial long Map();
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .ContainDiagnostic(MappaDiagnosticDescriptors.MethodHasInvalidNumberOfParameters, "Map");
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it has two parameters.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PartialMethodsWithArity2GenerateADiagnosticError()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial long Map(int input1, int input2);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .ContainDiagnostic(MappaDiagnosticDescriptors.MethodHasInvalidNumberOfParameters, "Map");
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it returns <see cref="Void"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PartialMethodsReturningVoidGeneratesADiagnosticError()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial void Map(int input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .ContainDiagnostic(MappaDiagnosticDescriptors.MethodIsVoid, "Map");
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it returns <see cref="Task"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PartialMethodsReturningTaskGeneratesADiagnosticError()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;
            using System.Threading.Tasks;
            
            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial Task Map(int input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .ContainDiagnostic(MappaDiagnosticDescriptors.MethodReturnsTaskType, "Map");
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it returns <see cref="Task{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PartialMethodsReturningTaskTGeneratesADiagnosticError()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;
            using System.Threading.Tasks;
            
            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial Task<string> Map(int input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .ContainDiagnostic(MappaDiagnosticDescriptors.MethodReturnsTaskType, "Map");
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it returns <see cref="ValueTask"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PartialMethodsReturningValueTaskGeneratesADiagnosticError()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;
            using System.Threading.Tasks;
            
            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial ValueTask Map(int input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .ContainDiagnostic(MappaDiagnosticDescriptors.MethodReturnsTaskType, "Map");
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it returns <see cref="ValueTask{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PartialMethodsReturningValueTaskTGeneratesADiagnosticError()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;
            using System.Threading.Tasks;
            
            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial ValueTask<string> Map(int input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .ContainDiagnostic(MappaDiagnosticDescriptors.MethodReturnsTaskType, "Map");
    }

    /// <summary>
    /// Check that it is not possible generating two methods
    /// with the same mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task SourceClassCannotContainTwoMethodsDefiningTheSameMapping()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial int Map(int input);
                public partial int AnotherMap(int input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .ContainDiagnostic(MappaDiagnosticDescriptors.DuplicatedMapping, "AnotherMap", "int ", "int ");
    }
}