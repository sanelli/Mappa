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
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Tuple<string, string> Map(Tuple<int, string> input);
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
                typeof(Tuple<string, string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Tuple<int, string>).ToString(),
                NullableAnnotation.NotAnnotated,
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
    /// Test a mapping can be created from <see cref="Tuple{T1,T2}"/>
    /// to tuple with anonymous elements.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapSystemTupleToTupleWithAnonymousElements()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial (string, string) Map(Tuple<int, string> input);
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
                "(string, string)",
                NullableAnnotation.NotAnnotated,
                typeof(Tuple<int, string>).ToString(),
                NullableAnnotation.NotAnnotated,
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
                                "(string, string)",
                                "__mappa_tmp_4",
                                initializationAssertions => initializationAssertions.BeTupleExpressionSyntax(
                                        parametersAssertions => parametersAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        parametersAssertions => parametersAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="Tuple{T1,T2}"/>
    /// to tuple with named elements.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapSystemTupleToTupleWithNameElements()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial (string First, string Second) Map(Tuple<int, string> input);
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
                "(string First, string Second)",
                NullableAnnotation.NotAnnotated,
                typeof(Tuple<int, string>).ToString(),
                NullableAnnotation.NotAnnotated,
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
                                "(string First, string Second)",
                                "__mappa_tmp_4",
                                initializationAssertions => initializationAssertions.BeTupleExpressionSyntax(
                                        parametersAssertions => parametersAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        parametersAssertions => parametersAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4"));
                        });
                });
    }

     /// <summary>
    /// Test a mapping can be created from a tuple with anonymous elements
    /// to <see cref="System.Tuple{T1,T2}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTupleWithAnonymousElementsToSystemTuple()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;
                                  using System;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Tuple<string, string> Map((int, string) input);
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
                typeof((int, string)).ToString(),
                NullableAnnotation.NotAnnotated,
                NullableSetup.Disable,
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
    /// Test a mapping can be created from a tuple with anonymous elements
    /// to a tuple with named elements.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTupleWithAnonymousElementsToTupleWithNamedElements()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial (string First, string Second) Map((int, string) input);
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
                "(string First, string Second)",
                NullableAnnotation.NotAnnotated,
                typeof((int, string)).ToString(),
                NullableAnnotation.NotAnnotated,
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
                                "(string First, string Second)",
                                "__mappa_tmp_4",
                                initializationAssertions => initializationAssertions.BeTupleExpressionSyntax(
                                        parametersAssertions => parametersAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        parametersAssertions => parametersAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
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
                                  #nullable enable
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

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof((string, string)).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof((int, string)).ToString(),
                NullableAnnotation.NotAnnotated,
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
                                typeof((string, string)).ToString(),
                                "__mappa_tmp_4",
                                initializationAssertions => initializationAssertions.BeTupleExpressionSyntax(
                                        parametersAssertions => parametersAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        parametersAssertions => parametersAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from tuple
    /// with named elements to <see cref="Tuple{T1,T2}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTupleWithNamedElementsToSystemTuple()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;
                                  using System;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Tuple<string, string> Map((int Alfa, string Beta) input);
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
                "(int Alfa, string Beta)",
                NullableAnnotation.NotAnnotated,
                NullableSetup.Disable,
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
    /// Test a mapping can be created from tuple with named elements
    /// to tuple with anonymous elements.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTupleWithNamedElementsToTupleWithAnonymousElements()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial (string, string) Map((int Alfa, string Beta) input);
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
                "(string, string)",
                NullableAnnotation.NotAnnotated,
                "(int Alfa, string Beta)",
                NullableAnnotation.NotAnnotated,
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
                                "(string, string)",
                                "__mappa_tmp_4",
                                initializationAssertions => initializationAssertions.BeTupleExpressionSyntax(
                                        parametersAssertions => parametersAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        parametersAssertions => parametersAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4"));
                        });
                });
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
                                  #nullable enable
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

       // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "(string First, string Second)",
                NullableAnnotation.NotAnnotated,
                "(int Alfa, string Beta)",
                NullableAnnotation.NotAnnotated,
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
                                "(string First, string Second)",
                                "__mappa_tmp_4",
                                initializationAssertions => initializationAssertions.BeTupleExpressionSyntax(
                                        parametersAssertions => parametersAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        parametersAssertions => parametersAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4"));
                        });
                });
    }
}