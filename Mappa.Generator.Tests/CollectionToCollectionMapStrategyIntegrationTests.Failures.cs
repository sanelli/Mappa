// <copyright file="CollectionToCollectionMapStrategyIntegrationTests.Failures.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Assertions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="CollectionToCollectionMapStrategy"/>.
/// </summary>
public sealed partial class CollectionToCollectionMapStrategyIntegrationTests
{
    /// <summary>
    /// Test map targeting class with non-empty constructor cannot be generated.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapTargetingCustomCollectionWithNonEmptyConstructor()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class Target : ICollection<string>
                                  {
                                      public Target(string something) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(IEnumerable<int> input);
                                  }

                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .ContainDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, typeof(IEnumerable<int>).ToString(), "Mappa.Generator.Tests.UnitTests.SourceCode.Target");
    }

    /// <summary>
    /// Test map targeting class with private empty constructor cannot be generated.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapTargetingCustomCollectionWithPrivateEmptyConstructor()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Target : ICollection<string>
                                  {
                                      private Target() { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(IEnumerable<int> input);
                                  }

                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .ContainDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, typeof(IEnumerable<int>).ToString(), "Mappa.Generator.Tests.UnitTests.SourceCode.Target");
    }
}