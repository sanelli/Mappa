// <copyright file="TupleToTupleMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="TupleToTupleMapStrategy"/> strategy.
/// </summary>
// TODO [#42] Test Tuple<...> -> (...).
// TODO [#42] Test Tuple<...> -> ( named ).
// TODO [#42] (...) -> Test Tuple<...>.
// TODO [#42] (...) -> ( named ).
// TODO [#42] ( named ) -> Test Tuple<...>.
// TODO [#42] ( named ) -> (...).
public sealed class TupleToTupleMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created from <see cref="Tuple{T1,T2}"/>
    /// to <see cref="Tuple{T1,T2}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapSystemTupleToSystemTuple()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;
                                  using System;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Tuple<string, string> Map(Tuple<int, string> input);
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
                typeof(Tuple<string, string>).ToString(),
                NullableAnnotation.None,
                typeof(Tuple<int, string>).ToString(),
                NullableAnnotation.None,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("int", "__mappa_tmp_1", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Item1"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("string", "__mappa_tmp_2", initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax("__mappa_tmp_1.ToString"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("string", "__mappa_tmp_3", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Item2"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(Tuple<string, string>).ToString(),
                                "__mappa_tmp_4",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(Tuple<string, string>).ToString(),
                                    [
                                        parametersAssertions => parametersAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        parametersAssertions => parametersAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                    ],
                                    []));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created between two tuple
    /// with anonymous elements.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTupleWithAnonymousElementsToTupleWithAnonymousElements()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;
                                  using System;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial (string, string) Map((int, string) input);
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
    /// Test a mapping can be created between two tuple
    /// with named elements.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTupleWithNamedElementsToTupleWithNamedElements()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;
                                  using System;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial (string First, string Second) Map((int Alfa, string Beta) input);
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