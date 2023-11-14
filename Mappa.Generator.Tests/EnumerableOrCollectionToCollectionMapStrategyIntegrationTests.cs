// <copyright file="EnumerableOrCollectionToCollectionMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="EnumerableOrCollectionToCollectionMapStrategy"/>.
/// </summary>
#pragma warning disable
// TODO: Add tests for all other combinations of input/output types.
#pragma warning enable
public sealed class EnumerableOrCollectionToCollectionMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created between <see cref="ICollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIListToIList()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;
            using System.Collections.Generic;

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
                public partial ICollection<int> Map(ICollection<TestEnum> input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        #pragma warning disable
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();

        // TODO [#42] Add correct assertions.
        compilationUnitSyntaxAssertions.NotBeNull();
        #pragma warning restore
    }

    /// <summary>
    /// Test a mapping can be created between two <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapListToList()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;
            using System.Collections.Generic;

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
                public partial IEnumerable<int> Map(IEnumerable<TestEnum> input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        #pragma warning disable
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();

        // TODO [#42] Add correct assertions.
        compilationUnitSyntaxAssertions.NotBeNull();
        #pragma warning restore
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="IEnumerable{T}"/> to <see cref="List{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapArrayToEnumerable()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;
            using System.Collections.Generic;

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
                public partial List<int> Map(IEnumerable<TestEnum> input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        #pragma warning disable
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();

        // TODO [#42] Add correct assertions.
        compilationUnitSyntaxAssertions.NotBeNull();
        #pragma warning restore
    }
}