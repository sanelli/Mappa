// <copyright file="CollectionToCollectionMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="CollectionToCollectionMapStrategy"/>.
/// </summary>
// TODO [#105] impl IEnumerable<int> -> IEnumerable<string>.
// TODO [#105] int[] -> IEnumerable<string>.
// TODO [#105] Span<int> -> IEnumerable<string>.
// TODO [#105] ReadOnlySpan<int> -> IEnumerable<string>.
// TODO [#105] Memory<int> -> IEnumerable<string>.
// TODO [#105] ReadOnlyMemory<int> -> IEnumerable<string>.
// TODO [#105] is IList<int> -> IEnumerable<string>.
// TODO [#105] impl IList<int> -> IEnumerable<string>.
// TODO [#105] int[] -> string[].
// TODO [#105] int[] -> Span<long>.
// TODO [#105] int[] -> ReadOnlySpan<long>.
// TODO [#105] int[] -> Memory<long>.
// TODO [#105] int[] -> ReadOnlyMemory<long>.
// TODO [#105] ICollection<int> -> string[].
// TODO [#105] impl ICollection<int> -> string[].
// TODO [#105] ICollection<int> -> Span<long>.
// TODO [#105] ICollection<int> -> ReadOnlySpan<long>.
// TODO [#105] ICollection<int> -> Memory<long>.
// TODO [#105] ICollection<int> -> ReadOnlyMemory<long>.
// TODO [#105] IEnumerable<int> -> string[].
// TODO [#105] impl IEnumerable<int> -> string[].
// TODO [#105] IEnumerable<int> -> Span<long>.
// TODO [#105] IEnumerable<int> -> ReadOnlySpan<long>.
// TODO [#105] IEnumerable<int> -> Memory<long>.
// TODO [#105] IEnumerable<int> -> ReadOnlyMemory<long>.
public class CollectionToCollectionMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToIEnumerable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial IEnumerable<string> Map(IEnumerable<int> input);
                                  }

                                  #nullable restore
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    // TODO [#105] Add assertions.
                });
    }
}