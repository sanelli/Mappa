// <copyright file="ReadonlyStackPropertyMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests around <see cref="ReadonlyStackPropertyMapStrategy"/>.
/// </summary>
public sealed class ReadonlyStackPropertyMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test that a mapping can be created from an array to a get-only <see cref="Stack{T}"/> property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToReadonlyPropertyStackFromArrayWhenTargetSetterIsNotProvided()
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
                                      public Stack<string> PropertyA {get;}
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
                                            "__mappa_tmp_1.PropertyA.Push",
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
    /// Test that a mapping can be created from an array to a get-only <see cref="System.Collections.Concurrent.ConcurrentStack{T}"/> property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToReadonlyPropertyConcurrentStackFromArrayWhenTargetSetterIsNotProvided()
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
                                      public ConcurrentStack<string> PropertyA {get;}
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
                                            "__mappa_tmp_1.PropertyA.Push",
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
    /// Test that a mapping can be created from <see cref="IEnumerable{T}"/> to a get-only <see cref="Stack{T}"/> property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToReadonlyPropertyStackFromIEnumerableWhenTargetSetterIsNotProvided()
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
                                      public Stack<string> PropertyA {get;}
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
                                            "__mappa_tmp_1.PropertyA.Push",
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
    /// Test that a mapping can be created to a <see cref="Stack{T}"/> property with a private setter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToReadonlyPropertyStackFromArrayWhenTargetSetterIsPrivate()
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
                                      public Stack<string> PropertyA {get; private set;}
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
                                            "__mappa_tmp_1.PropertyA.Push",
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
    /// Test that a mapping can be created to a custom type derived from <see cref="Stack{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToReadonlyPropertyStackFromArrayWhenTargetIsCustomDerivedType()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class TargetStack : Stack<string>
                                  {
                                  }

                                  public class Source
                                  {
                                      public int[] PropertyA {get;}
                                  }

                                  public class Target
                                  {
                                      public TargetStack PropertyA {get;}
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
                                            "__mappa_tmp_1.PropertyA.Push",
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
    /// Test that a warning is returned when the source property is not enumerable and the target is a <see cref="System.Collections.Concurrent.ConcurrentStack{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapToConcurrentStackWhenSourcePropertyIsNotEnumerable()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System.Collections.Concurrent;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA {get;}
                                      public int[] PropertyB {get;}
                                  }

                                  public class Target
                                  {
                                      public ConcurrentStack<string> PropertyA {get;}
                                      public Stack<string> PropertyB {get;}
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
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotMapNonRequiredProperty, "Mappa.Generator.Tests.UnitTests.SourceCode.Target", "PropertyA")
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
                            initializerExpressionAssertions => initializerExpressionAssertions.BeMemberAccessExpressionSyntax("input.PropertyB")))
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
                                            "__mappa_tmp_1.PropertyB.Push",
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