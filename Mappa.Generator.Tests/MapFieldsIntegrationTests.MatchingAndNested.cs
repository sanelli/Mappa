// <copyright file="MapFieldsIntegrationTests.MatchingAndNested.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Matching and nested-path field mapping integration tests.
/// </summary>
public sealed partial class MapFieldsIntegrationTests
{
    /// <summary>
    /// Maps fields with case-insensitive name matching.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFieldsWithCaseInsensitiveMatching()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int value;
                                  }

                                  public class Target
                                  {
                                      public int Value { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(
                                      MapSourceFields = BooleanSetting.Enable,
                                      CaseInsensitivePropertyMap = BooleanSetting.Enable)]
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
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Maps fields with underscore-insensitive name matching.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFieldsWithIgnoreUnderscoreMatching()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int User_Name;
                                  }

                                  public class Target
                                  {
                                      public int UserName { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(
                                      MapSourceFields = BooleanSetting.Enable,
                                      IgnoreUnderscoreForPropertyMap = BooleanSetting.Enable)]
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
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.User_Name")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("UserName", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Maps a nested type whose members are fields on the source and properties on the target.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNestedTypeUsingSourceFields()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class NestedSource
                                  {
                                      public int Value;
                                  }

                                  public class NestedTarget
                                  {
                                      public int Value { get; set; }
                                  }

                                  public class Source
                                  {
                                      public NestedSource Nested { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public NestedTarget Nested { get; set; } = null!;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapSourceFields = BooleanSetting.Enable)]
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
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.NestedSource",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Nested")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.NestedTarget",
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.NestedTarget",
                                        ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_4",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Nested", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4")));
                });
    }

    /// <summary>
    /// Maps inherited source and target fields.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapInheritedSourceAndTargetFields()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBase
                                  {
                                      public int Value;
                                  }

                                  public class Source : SourceBase
                                  {
                                  }

                                  public class TargetBase
                                  {
                                      public int Value;
                                  }

                                  public class Target : TargetBase
                                  {
                                  }

                                  [Mappa]
                                  [MappaSettings(MapSourceFields = BooleanSetting.Enable, MapTargetFields = BooleanSetting.Enable)]
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
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }
}