// <copyright file="ReadonlyQueuePropertyMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests around <see cref="ReadonlyQueuePropertyMapStrategy"/>.
/// </summary>
public sealed class ReadonlyQueuePropertyMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test that a mapping can be created from an array to a get-only <see cref="Queue{T}"/> property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToReadonlyPropertyQueueFromArrayWhenTargetSetterIsNotProvided()
    {
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
                                      public Queue<string> PropertyA {get;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_1",
                            initializerExpressionAssertions => initializerExpressionAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(int[]).ToString(),
                            "__mappa_tmp_2",
                            initializerExpressionAssertions => initializerExpressionAssertions.BeMemberAccessExpressionSyntax("input.PropertyA")))
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
                                            "__mappa_tmp_1.PropertyA.Enqueue",
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
    /// Test that a mapping can be created from an array to a get-only <see cref="System.Collections.Concurrent.ConcurrentQueue{T}"/> property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToReadonlyPropertyConcurrentQueueFromArrayWhenTargetSetterIsNotProvided()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System.Collections.Concurrent;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int[] PropertyA {get;}
                                  }

                                  public class Target
                                  {
                                      public ConcurrentQueue<string> PropertyA {get;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_1",
                            initializerExpressionAssertions => initializerExpressionAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(int[]).ToString(),
                            "__mappa_tmp_2",
                            initializerExpressionAssertions => initializerExpressionAssertions.BeMemberAccessExpressionSyntax("input.PropertyA")))
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
                                            "__mappa_tmp_1.PropertyA.Enqueue",
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
    /// Test that a mapping can be created from <see cref="IEnumerable{T}"/> to a get-only <see cref="Queue{T}"/> property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToReadonlyPropertyQueueFromIEnumerableWhenTargetSetterIsNotProvided()
    {
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
                                      public Queue<string> PropertyA {get;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_1",
                            initializerExpressionAssertions => initializerExpressionAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(IEnumerable<int>).ToString(),
                            "__mappa_tmp_2",
                            initializerExpressionAssertions => initializerExpressionAssertions.BeMemberAccessExpressionSyntax("input.PropertyA")))
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
                                            "__mappa_tmp_1.PropertyA.Enqueue",
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
    /// Test that a mapping can be created to a <see cref="Queue{T}"/> property with a private setter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToReadonlyPropertyQueueFromArrayWhenTargetSetterIsPrivate()
    {
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
                                      public Queue<string> PropertyA {get; private set;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_1",
                            initializerExpressionAssertions => initializerExpressionAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(int[]).ToString(),
                            "__mappa_tmp_2",
                            initializerExpressionAssertions => initializerExpressionAssertions.BeMemberAccessExpressionSyntax("input.PropertyA")))
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
                                            "__mappa_tmp_1.PropertyA.Enqueue",
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
    /// Test that a mapping can be created to a custom type derived from <see cref="Queue{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToReadonlyPropertyQueueFromArrayWhenTargetIsCustomDerivedType()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class TargetQueue : Queue<string>
                                  {
                                  }

                                  public class Source
                                  {
                                      public int[] PropertyA {get;}
                                  }

                                  public class Target
                                  {
                                      public TargetQueue PropertyA {get;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_1",
                            initializerExpressionAssertions => initializerExpressionAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(int[]).ToString(),
                            "__mappa_tmp_2",
                            initializerExpressionAssertions => initializerExpressionAssertions.BeMemberAccessExpressionSyntax("input.PropertyA")))
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
                                            "__mappa_tmp_1.PropertyA.Enqueue",
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