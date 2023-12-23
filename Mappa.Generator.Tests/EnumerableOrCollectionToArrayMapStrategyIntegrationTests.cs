// <copyright file="EnumerableOrCollectionToArrayMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="EnumerableOrCollectionToArrayMapStrategy"/> strategy.
/// </summary>
public sealed class EnumerableOrCollectionToArrayMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created from <see cref="ICollection{T}"/>
    /// to array.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapICollectionToArray()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial long[] Map(ICollection<int> input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(long[]).ToString(),
                NullableAnnotation.None,
                typeof(ICollection<int>).ToString(),
                NullableAnnotation.None,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            // TODO [#42] Add correct assertions.
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            // TODO [#42] Add correct assertions.
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            // TODO [#42] Add correct assertions.
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            // TODO [#42] Add correct assertions.
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="IReadOnlyCollection{T}"/>
    /// to array.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIReadOnlyCollectionToArray()
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
                                      public partial int[] Map(IReadOnlyCollection<TestEnum> input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();

        // TODO [#42] Add correct assertions.
        compilationUnitSyntaxAssertions.NotBeNull();
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="IEnumerable{T}"/>
    /// to array.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIEnumerableToArray()
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
                                      public partial int[] Map(IEnumerable<TestEnum> input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();

        // TODO [#42] Add correct assertions.
        compilationUnitSyntaxAssertions.NotBeNull();
    }
}