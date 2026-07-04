// <copyright file="CollectionToCollectionMapStrategyIntegrationTests.EnumerableConcreteType.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="MappaSettingsAttribute.EnumerableConcreteType"/>.
/// </summary>
public sealed partial class CollectionToCollectionMapStrategyIntegrationTests
{
    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="IEnumerable{T}"/>
    /// with <see cref="MappaSettingsAttribute.EnumerableConcreteType"/> set to <see cref="EnumerableConcreteTypeSetting.Array"/> on method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToIEnumerableWhenEnumerableConcreteTypeIsArrayOnMethod()
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
                                      [MappaSettings(EnumerableConcreteType = EnumerableConcreteTypeSetting.Array)]
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
                AssertIEnumerableToIEnumerableUsesArrayBufferBody);
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="IEnumerable{T}"/>
    /// with <see cref="MappaSettingsAttribute.EnumerableConcreteType"/> set to <see cref="EnumerableConcreteTypeSetting.Array"/> on class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToIEnumerableWhenEnumerableConcreteTypeIsArrayOnClass()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(EnumerableConcreteType = EnumerableConcreteTypeSetting.Array)]
                                  public sealed partial class Mapper
                                  {
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
                AssertIEnumerableToIEnumerableUsesArrayBufferBody);
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="List{T}"/> still uses <see cref="List{T}"/>
    /// when <see cref="MappaSettingsAttribute.EnumerableConcreteType"/> is <see cref="EnumerableConcreteTypeSetting.Array"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToListWhenEnumerableConcreteTypeIsArray()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(EnumerableConcreteType = EnumerableConcreteTypeSetting.Array)]
                                  public sealed partial class Mapper
                                  {
                                      public partial List<string> Map(IEnumerable<int> input);
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
                typeof(List<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
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
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="ICollection{T}"/>
    /// with <see cref="MappaSettingsAttribute.EnumerableConcreteType"/> set to <see cref="EnumerableConcreteTypeSetting.Array"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToICollectionWhenEnumerableConcreteTypeIsArray()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(EnumerableConcreteType = EnumerableConcreteTypeSetting.Array)]
                                  public sealed partial class Mapper
                                  {
                                      public partial ICollection<string> Map(IEnumerable<int> input);
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
                typeof(ICollection<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                AssertIEnumerableToIEnumerableUsesArrayBufferBody);
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="IEnumerable{T}"/>
    /// with <see cref="MappaSettingsAttribute.EnumerableConcreteType"/> set to <see cref="EnumerableConcreteTypeSetting.Array"/> in <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToIEnumerableWhenEnumerableConcreteTypeIsSetInEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.enumerableconcretetype = Array
                                    """;

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

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

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
                AssertIEnumerableToIEnumerableUsesArrayBufferBody);
    }

    /// <summary>
    /// Test class-level <see cref="MappaSettingsAttribute.EnumerableConcreteType"/> overrides
    /// <c>.editorconfig</c> for <see cref="IEnumerable{T}"/> to <see cref="IEnumerable{T}"/> mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToIEnumerableWhenClassEnumerableConcreteTypeOverridesEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.enumerableconcretetype = Array
                                    """;

        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(EnumerableConcreteType = EnumerableConcreteTypeSetting.List)]
                                  public sealed partial class Mapper
                                  {
                                      public partial IEnumerable<string> Map(IEnumerable<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                AssertIEnumerableToIEnumerableUsesListBufferBody);
    }

    /// <summary>
    /// Test method-level <see cref="MappaSettingsAttribute.EnumerableConcreteType"/> overrides
    /// class-level and <c>.editorconfig</c> settings for <see cref="IEnumerable{T}"/> to <see cref="IEnumerable{T}"/> mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToIEnumerableWhenMethodEnumerableConcreteTypeOverridesClassAndEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.enumerableconcretetype = Array
                                    """;

        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(EnumerableConcreteType = EnumerableConcreteTypeSetting.List)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(EnumerableConcreteType = EnumerableConcreteTypeSetting.Array)]
                                      public partial IEnumerable<string> Map(IEnumerable<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                AssertIEnumerableToIEnumerableUsesArrayBufferBody);
    }

    private static void AssertIEnumerableToIEnumerableUsesArrayBufferBody(BlockSyntaxAssertions blockSyntaxAssertions)
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

    private static void AssertIEnumerableToIEnumerableUsesListBufferBody(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
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
                syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                    .BeIdentifierNameSyntax("__mappa_tmp_1")));
    }
}