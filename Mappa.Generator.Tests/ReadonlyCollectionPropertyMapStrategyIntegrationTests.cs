// <copyright file="ReadonlyCollectionPropertyMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests around <see cref="ReadonlyCollectionPropertyMapStrategy"/>.
/// </summary>
// TODO [#7] Add extra tests to make sure this works when setter exists but is not accessible.
public sealed class ReadonlyCollectionPropertyMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test that a mapping can be created from:
    /// - from <see cref="Array"/> of <see cref="int"/> property;
    /// - to <see cref="ICollection{T}"/> or <see cref="string"/> property;
    /// - target property does not have a setter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToReadonlyPropertyCollectionFromArrayWhenTargetSetterIsNotProvidedAndIsOfTypeCollection()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      int[] PropertyA {get;}
                                  }

                                  public class Target
                                  {
                                      ICollection<string> PropertyA {get;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    // TODO [#87] Implement me.
                });
    }

    // TODO [#87] Test int[] -> List<>.
    // TODO [#87] Test IList[] -> ICollection<>.
    // TODO [#87] Test IList[] -> List<>.
}