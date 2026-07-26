// <copyright file="CollectionToCollectionMapStrategyIntegrationTests.PreventEnumerableCount.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="MappaSettingsAttribute.PreventEnumerableCount"/>.
/// </summary>
public sealed partial class CollectionToCollectionMapStrategyIntegrationTests
{
    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="Array"/>
    /// with <see cref="MappaSettingsAttribute.PreventEnumerableCount"/> enabled on method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToArrayWhenPreventEnumerableCountIsEnabledOnMethod()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(PreventEnumerableCount = BooleanSetting.Enable)]
                                      public partial string[] Map(IEnumerable<int> input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                AssertIEnumerableToArrayUsesGrowableBufferBody);
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="Array"/>
    /// with <see cref="MappaSettingsAttribute.PreventEnumerableCount"/> enabled on class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToArrayWhenPreventEnumerableCountIsEnabledOnClass()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(PreventEnumerableCount = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial string[] Map(IEnumerable<int> input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                AssertIEnumerableToArrayUsesGrowableBufferBody);
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="Array"/>
    /// when <see cref="MappaSettingsAttribute.PreventEnumerableCount"/> is unset (default behavior).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToArrayWhenPreventEnumerableCountIsDisabledByDefault()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string[] Map(IEnumerable<int> input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                AssertIEnumerableToArrayUsesEnumerableCountBody);
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="Span{T}"/>
    /// with <see cref="MappaSettingsAttribute.PreventEnumerableCount"/> enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToSpanWhenPreventEnumerableCountIsEnabled()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(PreventEnumerableCount = BooleanSetting.Enable)]
                                      public partial System.Span<string> Map(IEnumerable<int> input);
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
                typeof(Span<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(typeof(List<string>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_4",
                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_1.ToArray")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(Span<string>).ToString(),
                                "__mappa_tmp_5",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(Span<string>).ToString(),
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_5")));
                });
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="ReadOnlySpan{T}"/>
    /// with <see cref="MappaSettingsAttribute.PreventEnumerableCount"/> enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToReadOnlySpanWhenPreventEnumerableCountIsEnabled()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(PreventEnumerableCount = BooleanSetting.Enable)]
                                      public partial System.ReadOnlySpan<string> Map(IEnumerable<int> input);
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
                typeof(ReadOnlySpan<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(typeof(List<string>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_4",
                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_1.ToArray")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(ReadOnlySpan<string>).ToString(),
                                "__mappa_tmp_5",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(ReadOnlySpan<string>).ToString(),
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_5")));
                });
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="Memory{T}"/>
    /// with <see cref="MappaSettingsAttribute.PreventEnumerableCount"/> enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToMemoryWhenPreventEnumerableCountIsEnabled()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(PreventEnumerableCount = BooleanSetting.Enable)]
                                      public partial System.Memory<string> Map(IEnumerable<int> input);
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
                typeof(Memory<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(typeof(List<string>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_4",
                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_1.ToArray")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(Memory<string>).ToString(),
                                "__mappa_tmp_5",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(Memory<string>).ToString(),
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_5")));
                });
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="ReadOnlyMemory{T}"/>
    /// with <see cref="MappaSettingsAttribute.PreventEnumerableCount"/> enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToReadOnlyMemoryWhenPreventEnumerableCountIsEnabled()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(PreventEnumerableCount = BooleanSetting.Enable)]
                                      public partial System.ReadOnlyMemory<string> Map(IEnumerable<int> input);
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
                typeof(ReadOnlyMemory<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(typeof(List<string>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_4",
                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_1.ToArray")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(ReadOnlyMemory<string>).ToString(),
                                "__mappa_tmp_5",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(ReadOnlyMemory<string>).ToString(),
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_5")));
                });
    }

    /// <summary>
    /// Test map from <see cref="IList{T}"/> to <see cref="Array"/> still pre-sizes the buffer
    /// when <see cref="MappaSettingsAttribute.PreventEnumerableCount"/> is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIListToArrayWhenPreventEnumerableCountIsEnabled()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(PreventEnumerableCount = BooleanSetting.Enable)]
                                      public partial string[] Map(IList<int> input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IList<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeArrayCreationExpressionSyntax(
                                    typeof(string).ToString(),
                                    sizeAssertion => sizeAssertion.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions => declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_2", 0),
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("input.Count")),
                                incrementorAssertions => incrementorAssertions.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("input", "__mappa_tmp_2")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftExpression => leftExpression.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_2"),
                                                rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="Array"/> to <see cref="Array"/> still pre-sizes the buffer with
    /// <c>Length</c> when <see cref="MappaSettingsAttribute.PreventEnumerableCount"/> is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromArrayToArrayWhenPreventEnumerableCountIsEnabled()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(PreventEnumerableCount = BooleanSetting.Enable)]
                                      public partial string[] Map(int[] input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int[]).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeArrayCreationExpressionSyntax(
                                    typeof(string).ToString(),
                                    sizeAssertion => sizeAssertion.BeMemberAccessExpressionSyntax("input.Length"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions => declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_2", 0),
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("input.Length")),
                                incrementorAssertions => incrementorAssertions.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("input", "__mappa_tmp_2")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftExpression => leftExpression.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_2"),
                                                rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="IEnumerable{T}"/>
    /// when <see cref="MappaSettingsAttribute.EnumerableConcreteType"/> is <see cref="EnumerableConcreteTypeSetting.Array"/>
    /// and <see cref="MappaSettingsAttribute.PreventEnumerableCount"/> is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToIEnumerableWhenEnumerableConcreteTypeIsArrayAndPreventEnumerableCountIsEnabled()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(
                                          EnumerableConcreteType = EnumerableConcreteTypeSetting.Array,
                                          PreventEnumerableCount = BooleanSetting.Enable)]
                                      public partial IEnumerable<string> Map(IEnumerable<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                AssertIEnumerableToArrayUsesGrowableBufferBody);
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="Array"/>
    /// when <see cref="MappaSettingsAttribute.PreventEnumerableCount"/> is enabled in <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToArrayWhenPreventEnumerableCountIsEnabledInEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.preventenumerablecount = enable
                                    """;

        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string[] Map(IEnumerable<int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                AssertIEnumerableToArrayUsesGrowableBufferBody);
    }

    /// <summary>
    /// Test class-level <see cref="MappaSettingsAttribute.PreventEnumerableCount"/> disable overrides
    /// <c>.editorconfig</c> for <see cref="IEnumerable{T}"/> to <see cref="Array"/> mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToArrayWhenPreventEnumerableCountInEditorConfigIsOverriddenByClassAttributeAsDisable()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.preventenumerablecount = enable
                                    """;

        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(PreventEnumerableCount = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial string[] Map(IEnumerable<int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                AssertIEnumerableToArrayUsesEnumerableCountBody);
    }

    /// <summary>
    /// Test method-level <see cref="MappaSettingsAttribute.PreventEnumerableCount"/> overrides
    /// class-level and <c>.editorconfig</c> settings.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToArrayWhenMethodPreventEnumerableCountOverridesClassAndEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.preventenumerablecount = disable
                                    """;

        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(PreventEnumerableCount = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(PreventEnumerableCount = BooleanSetting.Enable)]
                                      public partial string[] Map(IEnumerable<int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                AssertIEnumerableToArrayUsesGrowableBufferBody);
    }

    private static void AssertIEnumerableToArrayUsesGrowableBufferBody(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(4)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    typeof(List<string>).ToString(),
                    "__mappa_tmp_1",
                    initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(typeof(List<string>).ToString())))
            .HasNextSyntaxNode(syntaxNodeAssertions =>
                syntaxNodeAssertions.BeForEachStatementSyntax(
                    typeof(int).ToString(),
                    "__mappa_tmp_2",
                    expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                    statementAssertions =>
                        statementAssertions
                            .BeBlockStatement()
                            .AsBlock()
                            .HasSyntaxNodesCount(2)
                            .HasNextSyntaxNode(foreachStatementAssertions =>
                                foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                    typeof(string).ToString(),
                                    "__mappa_tmp_3",
                                    initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                            .HasNextSyntaxNode(foreachStatementAssertions =>
                                foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                    "__mappa_tmp_1.Add",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
            .HasNextSyntaxNode(syntaxNodeAssertions =>
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    typeof(string[]).ToString(),
                    "__mappa_tmp_4",
                    initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_1.ToArray")))
            .HasNextSyntaxNode(syntaxNodeAssertions =>
                syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                    .BeIdentifierNameSyntax("__mappa_tmp_4")));
    }

    private static void AssertIEnumerableToArrayUsesEnumerableCountBody(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(4)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    typeof(string[]).ToString(),
                    "__mappa_tmp_1",
                    initializerAssertions => initializerAssertions.BeArrayCreationExpressionSyntax(
                        typeof(string).ToString(),
                        sizeAssertions => sizeAssertions.BeInvocationExpressionSyntax(
                            "global::System.Linq.Enumerable.Count<int>",
                            parameter => parameter.BeIdentifierNameSyntax("input")))))
            .HasNextSyntaxNode(syntaxNodeAssertions =>
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    typeof(int).ToString(),
                    "__mappa_tmp_2",
                    initializerAssertions => initializerAssertions.BeLiteralExpressionSyntax(0)))
            .HasNextSyntaxNode(syntaxNodeAssertions =>
                syntaxNodeAssertions.BeForEachStatementSyntax(
                    typeof(int).ToString(),
                    "__mappa_tmp_3",
                    expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                    statementAssertions =>
                        statementAssertions
                            .BeBlockStatement()
                            .AsBlock()
                            .HasSyntaxNodesCount(3)
                            .HasNextSyntaxNode(foreachStatementAssertions =>
                                foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                    typeof(string).ToString(),
                                    "__mappa_tmp_4",
                                    initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                            .HasNextSyntaxNode(foreachStatementAssertions =>
                                foreachStatementAssertions.BeAssignmentExpressionStatement(
                                   leftAssignmentAssertions => leftAssignmentAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_2"),
                                   rightAssignmentAssertions => rightAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))
                            .HasNextSyntaxNode(foreachStatementAssertions =>
                                foreachStatementAssertions.BeAssignmentExpressionStatement(
                                    leftAssignmentAssertions => leftAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    rightAssignmentAssertions => rightAssignmentAssertions.BeBinaryExpressionSyntax(
                                        leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        SyntaxKind.PlusToken,
                                        rightExpressionAssertions => rightExpressionAssertions.BeLiteralExpressionSyntax(1))))))
            .HasNextSyntaxNode(syntaxNodeAssertions =>
                syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                    .BeIdentifierNameSyntax("__mappa_tmp_1")));
    }
}