// <copyright file="EnumerableOrCollectionToCollectionMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="EnumerableOrCollectionToCollectionMapStrategy"/>.
/// </summary>
public sealed class EnumerableOrCollectionToCollectionMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created between <see cref="ICollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapICollectionToICollection()
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
                                      public partial ICollection<long> Map(ICollection<int> input);
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
                typeof(ICollection<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(ICollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created between two <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIEnumerableToIEnumerable()
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
                                      public partial IEnumerable<long> Map(IEnumerable<int> input);
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
                typeof(IEnumerable<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

     /// <summary>
    /// Test a mapping can be created between <see cref="IReadOnlyCollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIReadOnlyCollectionToIReadOnlyCollection()
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
                                      public partial IReadOnlyCollection<long> Map(IReadOnlyCollection<int> input);
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
                typeof(IReadOnlyCollection<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IReadOnlyCollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="ICollection{T}"/>
    /// to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapICollectionToIEnumerable()
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
                                      public partial IEnumerable<long> Map(ICollection<int> input);
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
                typeof(IEnumerable<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(ICollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="ICollection{T}"/>
    /// to <see cref="IReadOnlyCollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapICollectionToIReadOnlyCollection()
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
                                      public partial IReadOnlyCollection<long> Map(ICollection<int> input);
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
                typeof(IReadOnlyCollection<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(ICollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="IEnumerable{T}"/>
    /// to <see cref="ICollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIEnumerableToICollection()
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
                                      public partial ICollection<long> Map(IEnumerable<int> input);
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
                typeof(ICollection<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="IEnumerable{T}"/>
    /// to <see cref="IReadOnlyCollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIEnumerableToIReadOnlyCollection()
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
                                      public partial IReadOnlyCollection<long> Map(IEnumerable<int> input);
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
                typeof(IReadOnlyCollection<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="IReadOnlyCollection{T}"/>
    /// to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIReadOnlyCollectionToIEnumerable()
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
                                      public partial IEnumerable<long> Map(IReadOnlyCollection<int> input);
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
                typeof(IEnumerable<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IReadOnlyCollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="IReadOnlyCollection{T}"/>
    /// to <see cref="ICollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIReadOnlyCollectionTICollection()
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
                                      public partial ICollection<long> Map(IReadOnlyCollection<int> input);
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
                typeof(ICollection<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IReadOnlyCollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="ICollection{T}"/>
    /// to <see cref="IList{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapICollectionToIList()
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
                                      public partial IList<long> Map(ICollection<int> input);
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
                typeof(IList<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(ICollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="IEnumerable{T}"/>
    /// to <see cref="IList{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIEnumerableToIList()
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
                                      public partial IList<long> Map(IEnumerable<int> input);
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
                typeof(IList<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="IReadOnlyCollection{T}"/>
    /// to <see cref="IList{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIReadOnlyCollectionToIList()
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
                                      public partial IList<long> Map(IReadOnlyCollection<int> input);
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
                typeof(IList<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IReadOnlyCollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }
}