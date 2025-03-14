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
                                      public int[] PropertyA {get;}
                                  }

                                  public class Target
                                  {
                                      public ICollection<string> PropertyA {get;}
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
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializerExpressionAssertions => initializerExpressionAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int[]).ToString(),
                                "__mappa_tmp_2",
                                initializerExpressionAssertions => initializerExpressionAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertion => declarationAssertion.BeAssignmentFromConstant("int", "__mappa_tmp_3", 0),
                                conditionAssertion => conditionAssertion.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Length")),
                                incrementorAssertion => incrementorAssertion.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                statementSyntaxBaseAssertions => statementSyntaxBaseAssertions
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(3)
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeLocalDeclarationStatementSyntax(
                                            typeof(int).ToString(),
                                            "__mappa_tmp_4",
                                            initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_2", "__mappa_tmp_3"));
                                    })
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeLocalDeclarationStatementSyntax(
                                            typeof(string).ToString(),
                                            "__mappa_tmp_5",
                                            initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_4.ToString"));
                                    })
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeInvocationExpressionSyntaxStatement(
                                            "__mappa_tmp_1.PropertyA.Add",
                                            firstParameter => firstParameter.BeIdentifierNameSyntax("__mappa_tmp_5"));
                                    }));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertion => expressionAssertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test that a mapping can be created from:
    /// - from <see cref="Array"/> of <see cref="int"/> property;
    /// - to <see cref="List{T}"/> or <see cref="string"/> property;
    /// - target property does not have a setter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToReadonlyPropertyCollectionFromArrayWhenTargetSetterIsNotProvidedAndIsOfTypeList()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int[] PropertyA {get;}
                                  }

                                  public class Target
                                  {
                                      public List<string> PropertyA {get;}
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
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializerExpressionAssertions => initializerExpressionAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int[]).ToString(),
                                "__mappa_tmp_2",
                                initializerExpressionAssertions => initializerExpressionAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertion => declarationAssertion.BeAssignmentFromConstant("int", "__mappa_tmp_3", 0),
                                conditionAssertion => conditionAssertion.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Length")),
                                incrementorAssertion => incrementorAssertion.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                statementSyntaxBaseAssertions => statementSyntaxBaseAssertions
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(3)
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeLocalDeclarationStatementSyntax(
                                            typeof(int).ToString(),
                                            "__mappa_tmp_4",
                                            initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_2", "__mappa_tmp_3"));
                                    })
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeLocalDeclarationStatementSyntax(
                                            typeof(string).ToString(),
                                            "__mappa_tmp_5",
                                            initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_4.ToString"));
                                    })
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeInvocationExpressionSyntaxStatement(
                                            "__mappa_tmp_1.PropertyA.Add",
                                            firstParameter => firstParameter.BeIdentifierNameSyntax("__mappa_tmp_5"));
                                    }));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertion => expressionAssertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test that a mapping can be created from:
    /// - from <see cref="IList{T}"/> of <see cref="int"/> property;
    /// - to <see cref="ICollection{T}"/> or <see cref="string"/> property;
    /// - target property does not have a setter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToReadonlyPropertyCollectionFromIListWhenTargetSetterIsNotProvidedAndIsOfTypeCollection()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public IList<int> PropertyA {get;}
                                  }

                                  public class Target
                                  {
                                      public ICollection<string> PropertyA {get;}
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
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializerExpressionAssertions => initializerExpressionAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(IList<int>).ToString(),
                                "__mappa_tmp_2",
                                initializerExpressionAssertions => initializerExpressionAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertion => declarationAssertion.BeAssignmentFromConstant("int", "__mappa_tmp_3", 0),
                                conditionAssertion => conditionAssertion.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Count")),
                                incrementorAssertion => incrementorAssertion.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                statementSyntaxBaseAssertions => statementSyntaxBaseAssertions
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(3)
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeLocalDeclarationStatementSyntax(
                                            typeof(int).ToString(),
                                            "__mappa_tmp_4",
                                            initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_2", "__mappa_tmp_3"));
                                    })
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeLocalDeclarationStatementSyntax(
                                            typeof(string).ToString(),
                                            "__mappa_tmp_5",
                                            initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_4.ToString"));
                                    })
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeInvocationExpressionSyntaxStatement(
                                            "__mappa_tmp_1.PropertyA.Add",
                                            firstParameter => firstParameter.BeIdentifierNameSyntax("__mappa_tmp_5"));
                                    }));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertion => expressionAssertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test that a mapping can be created from:
    /// - from <see cref="IList{T}"/> of <see cref="int"/> property;
    /// - to <see cref="List{T}"/> or <see cref="string"/> property;
    /// - target property does not have a setter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToReadonlyPropertyCollectionFromIListWhenTargetSetterIsNotProvidedAndIsOfTypeList()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public IList<int> PropertyA {get;}
                                  }

                                  public class Target
                                  {
                                      public List<string> PropertyA {get;}
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
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializerExpressionAssertions => initializerExpressionAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(IList<int>).ToString(),
                                "__mappa_tmp_2",
                                initializerExpressionAssertions => initializerExpressionAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertion => declarationAssertion.BeAssignmentFromConstant("int", "__mappa_tmp_3", 0),
                                conditionAssertion => conditionAssertion.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Count")),
                                incrementorAssertion => incrementorAssertion.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                statementSyntaxBaseAssertions => statementSyntaxBaseAssertions
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(3)
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeLocalDeclarationStatementSyntax(
                                            typeof(int).ToString(),
                                            "__mappa_tmp_4",
                                            initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_2", "__mappa_tmp_3"));
                                    })
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeLocalDeclarationStatementSyntax(
                                            typeof(string).ToString(),
                                            "__mappa_tmp_5",
                                            initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_4.ToString"));
                                    })
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeInvocationExpressionSyntaxStatement(
                                            "__mappa_tmp_1.PropertyA.Add",
                                            firstParameter => firstParameter.BeIdentifierNameSyntax("__mappa_tmp_5"));
                                    }));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertion => expressionAssertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test that a mapping can be created from:
    /// - from <see cref="IEnumerable{T}"/> of <see cref="int"/> property;
    /// - to <see cref="List{T}"/> or <see cref="string"/> property;
    /// - target property does not have a setter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToReadonlyPropertyCollectionFromIEnumerableWhenTargetSetterIsNotProvidedAndIsOfTypeList()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                     public IEnumerable<int> PropertyA {get;}
                                  }

                                  public class Target
                                  {
                                     public List<string> PropertyA {get;}
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
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializerExpressionAssertions => initializerExpressionAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(IEnumerable<int>).ToString(),
                                "__mappa_tmp_2",
                                initializerExpressionAssertions => initializerExpressionAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                incrementorAssertion => incrementorAssertion.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                statementSyntaxBaseAssertions => statementSyntaxBaseAssertions
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(2)
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeLocalDeclarationStatementSyntax(
                                            typeof(string).ToString(),
                                            "__mappa_tmp_4",
                                            initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString"));
                                    })
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeInvocationExpressionSyntaxStatement(
                                            "__mappa_tmp_1.PropertyA.Add",
                                            firstParameter => firstParameter.BeIdentifierNameSyntax("__mappa_tmp_4"));
                                    }));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertion => expressionAssertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test that a mapping can be created from:
    /// - from <see cref="Array"/> of <see cref="int"/> property;
    /// - to <see cref="ICollection{T}"/> or <see cref="string"/> property;
    /// - target property does have a private setter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToReadonlyPropertyCollectionFromArrayWhenTargetSetterIsPrivatedAndIsOfTypeCollection()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int[] PropertyA {get;}
                                  }

                                  public class Target
                                  {
                                      public ICollection<string> PropertyA {get; private set;}
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
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializerExpressionAssertions => initializerExpressionAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int[]).ToString(),
                                "__mappa_tmp_2",
                                initializerExpressionAssertions => initializerExpressionAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertion => declarationAssertion.BeAssignmentFromConstant("int", "__mappa_tmp_3", 0),
                                conditionAssertion => conditionAssertion.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Length")),
                                incrementorAssertion => incrementorAssertion.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                statementSyntaxBaseAssertions => statementSyntaxBaseAssertions
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(3)
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeLocalDeclarationStatementSyntax(
                                            typeof(int).ToString(),
                                            "__mappa_tmp_4",
                                            initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_2", "__mappa_tmp_3"));
                                    })
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeLocalDeclarationStatementSyntax(
                                            typeof(string).ToString(),
                                            "__mappa_tmp_5",
                                            initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_4.ToString"));
                                    })
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeInvocationExpressionSyntaxStatement(
                                            "__mappa_tmp_1.PropertyA.Add",
                                            firstParameter => firstParameter.BeIdentifierNameSyntax("__mappa_tmp_5"));
                                    }));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertion => expressionAssertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }
}