// <copyright file="MapFieldsIntegrationTests.AttributesAndExclusions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Attribute and exclusion field mapping integration tests.
/// </summary>
public sealed partial class MapFieldsIntegrationTests
{
    /// <summary>
    /// Resolves <see cref="Mappa.Attributes.MappaUsePropertyAttribute"/> source name against a source field.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMappaUsePropertyAttributeAgainstSourceField()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int SourceValue;
                                  }

                                  public class Target
                                  {
                                      public int TargetValue { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(MapSourceFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty(nameof(Target.TargetValue), "SourceValue")]
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
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.SourceValue")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("TargetValue", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Ignores a target field via <see cref="Mappa.Attributes.MappaIgnoreTargetPropertyAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanIgnoreTargetFieldViaMappaIgnoreTargetPropertyAttribute()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Keep;
                                      public int Drop;
                                  }

                                  public class Target
                                  {
                                      public int Keep;
                                      public int Drop;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapSourceFields = BooleanSetting.Enable, MapTargetFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnoreTargetProperty("Drop")]
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
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Keep")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Keep", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Does not map readonly, static, or auto-property backing fields as targets.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotMapReadonlyStaticOrBackingFieldsAsTargets()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int ReadonlyValue { get; set; }
                                      public int StaticValue { get; set; }
                                      public int PropertyValue { get; set; }
                                  }

                                  public class Target
                                  {
                                      public readonly int ReadonlyValue;
                                      public static int StaticValue;
                                      public int PropertyValue { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(MapTargetFields = BooleanSetting.Enable)]
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
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyValue")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("PropertyValue", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }
}