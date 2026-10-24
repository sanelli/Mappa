// <copyright file="MapFieldsIntegrationTests.InaccessibleAndProjections.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Inaccessible-field and queryable-projection field mapping integration tests.
/// </summary>
public sealed partial class MapFieldsIntegrationTests
{
    private const string AccessorsClassName = "__MappaInaccessibleAccessors";

    /// <summary>
    /// Maps a private source field via <c>UnsafeAccessor</c> when allow-inaccessible is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapPrivateSourceFieldWithAllowInaccessibleSourceMembers()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      private int Value;
                                  }

                                  public class Target
                                  {
                                      public int Value { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(MapSourceFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleSourceMembers]
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
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    $"{AccessorsClassName}.__mappa_tmp_2",
                                    argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3")));
                })
            .HaveClass(AccessorsClassName, classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.FileKeyword, SyntaxKind.StaticKeyword)
                    .HaveMethods(1)
                    .HaveExternUnsafeAccessorMethod("__mappa_tmp_2", "Field", "Value");
            });
    }

    /// <summary>
    /// Maps a public source property to a private target field via <c>UnsafeAccessor</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapPublicSourceToPrivateTargetFieldWithAllowInaccessibleTargetMembers()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      private int Value;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapTargetFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleTargetMembers]
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
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(TargetTypeName)))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeAssignmentExpressionStatement(
                                left => left.BeInvocationExpressionSyntax(
                                    $"{AccessorsClassName}.__mappa_tmp_3",
                                    argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")),
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_2")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1")));
                })
            .HaveClass(AccessorsClassName, classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.FileKeyword, SyntaxKind.StaticKeyword)
                    .HaveMethods(1)
                    .HaveExternUnsafeAccessorMethod("__mappa_tmp_3", "Field", "Value");
            });
    }

    /// <summary>
    /// Projects public fields on an <see cref="System.Linq.IQueryable{T}"/> map method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanProjectPublicFieldsWithIQueryable()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Value;
                                  }

                                  public class Target
                                  {
                                      public int Value;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapSourceFields = BooleanSetting.Enable, MapTargetFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial IQueryable<Target> Map(IQueryable<Source> input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveQueryableProjectionMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                "Map",
                [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
                false,
                "input",
                SourceTypeName,
                TargetTypeName,
                "__mappa_tmp_1",
                elementExpression => elementExpression.BeObjectCreationExpressionSyntax(
                    TargetTypeName,
                    ("Value", property => property.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Value"))));
    }

    /// <summary>
    /// Reports an error when inaccessible attributes are used on a queryable projection with fields enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsReportedWhenInaccessibleAttributesAreUsedOnProjectionWithFieldsEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Value;
                                  }

                                  public class Target
                                  {
                                      public int Value;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapSourceFields = BooleanSetting.Enable, MapTargetFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleSourceMembers]
                                      public partial IQueryable<Target> Map(IQueryable<Source> input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProjectionMethodHasAllowInaccessibleMembers, "Map")
            .NotHaveGeneratedAnySourceCode();
    }
}