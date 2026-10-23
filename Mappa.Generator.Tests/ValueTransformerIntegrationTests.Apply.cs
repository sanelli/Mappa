// <copyright file="ValueTransformerIntegrationTests.Apply.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <c>[MappaApplyTransformer]</c> attribute.
/// </summary>
public sealed partial class ValueTransformerIntegrationTests
{
    /// <summary>
    /// Test the <c>(methodName)</c> constructor resolves the transformer on the mapper.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyTransformerByMethodNameUsingMapperMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaApplyTransformer(nameof(Normalize))]
                                      public partial string Map(int input);

                                      private string Normalize(string value) => value.Trim();
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                typeof(int).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.Normalize", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test the <c>(classType, methodName)</c> constructor resolves a static transformer on the given type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyTransformerByClassTypeAndMethodName()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public static class Normalizers
                                  {
                                      public static string Normalize(string value) => value.Trim();
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaApplyTransformer(typeof(Normalizers), nameof(Normalizers.Normalize))]
                                      public partial string Map(int input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                typeof(int).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "global::Mappa.Generator.Tests.UnitTests.SourceCode.Normalizers.Normalize", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test the <c>(sourcePropertyName, methodName)</c> constructor only applies the transformer to the named source member.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyTransformerOnlyToNamedSourceProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string Name { get; set; } = string.Empty;
                                      public string Nick { get; set; } = string.Empty;
                                  }

                                  public class Target
                                  {
                                      public string Name { get; set; } = string.Empty;
                                      public string Nick { get; set; } = string.Empty;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaApplyTransformer(nameof(Source.Name), nameof(Normalize))]
                                      public partial Target Map(Source input);

                                      [MappaIgnore]
                                      private string Normalize(string value) => value.Trim();
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                $"{SourceNamespace}.Target",
                $"{SourceNamespace}.Source",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(6)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeMemberAccessExpressionSyntax("input.Name")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.Normalize", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_3", "this.Normalize", "__mappa_tmp_2"))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_4", initialization => initialization.BeMemberAccessExpressionSyntax("input.Nick")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            $"{SourceNamespace}.Target",
                            "__mappa_tmp_5",
                            initialization => initialization.BeObjectCreationExpressionSyntax(
                                $"{SourceNamespace}.Target",
                                ("Name", property => property.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                ("Nick", property => property.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_5"));
                });
    }

    /// <summary>
    /// Test the <c>(sourcePropertyName, classType, methodName)</c> constructor applies a static transformer
    /// to a nested source path only.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyTransformerFromClassTypeToNestedSourcePath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public static class Normalizers
                                  {
                                      public static string Normalize(string value) => value.Trim();
                                  }

                                  public class SourceChild
                                  {
                                      public string Name { get; set; } = string.Empty;
                                      public string Nick { get; set; } = string.Empty;
                                  }

                                  public class TargetChild
                                  {
                                      public string Name { get; set; } = string.Empty;
                                      public string Nick { get; set; } = string.Empty;
                                  }

                                  public class Source
                                  {
                                      public SourceChild Child { get; set; } = new();
                                  }

                                  public class Target
                                  {
                                      public TargetChild Child { get; set; } = new();
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaApplyTransformer("Child.Name", typeof(Normalizers), nameof(Normalizers.Normalize))]
                                      public partial Target Map(Source input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                $"{SourceNamespace}.Target",
                $"{SourceNamespace}.Source",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(8)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax($"{SourceNamespace}.SourceChild", "__mappa_tmp_1", initialization => initialization.BeMemberAccessExpressionSyntax("input.Child")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_2", initialization => initialization.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Name")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_3", "global::Mappa.Generator.Tests.UnitTests.SourceCode.Normalizers.Normalize", "__mappa_tmp_2"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_4", "global::Mappa.Generator.Tests.UnitTests.SourceCode.Normalizers.Normalize", "__mappa_tmp_3"))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_5", initialization => initialization.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Nick")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            $"{SourceNamespace}.TargetChild",
                            "__mappa_tmp_6",
                            initialization => initialization.BeObjectCreationExpressionSyntax(
                                $"{SourceNamespace}.TargetChild",
                                ("Name", property => property.BeIdentifierNameSyntax("__mappa_tmp_4")),
                                ("Nick", property => property.BeIdentifierNameSyntax("__mappa_tmp_5")))))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            $"{SourceNamespace}.Target",
                            "__mappa_tmp_7",
                            initialization => initialization.BeObjectCreationExpressionSyntax(
                                $"{SourceNamespace}.Target",
                                ("Child", property => property.BeIdentifierNameSyntax("__mappa_tmp_6")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_7"));
                });
    }

    /// <summary>
    /// Test <c>FieldName</c> resolves the transformer on an instance field of the mapper.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyTransformerFromInstanceFieldUsingFieldName()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class Normalizer
                                  {
                                      public string Normalize(string value) => value.Trim();
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private readonly Normalizer normalizer = new();

                                      [MappaApplyTransformer(nameof(Normalizer.Normalize), FieldName = nameof(normalizer))]
                                      public partial string Map(int input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                typeof(int).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.normalizer.Normalize", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test <c>FieldName</c> combined with a source property name restricts the field transformer to that member.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyTransformerFromFieldUsingFieldNameAndSourceProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class Normalizer
                                  {
                                      public string Normalize(string value) => value.Trim();
                                  }

                                  public class Source
                                  {
                                      public string Name { get; set; } = string.Empty;
                                      public string Nick { get; set; } = string.Empty;
                                  }

                                  public class Target
                                  {
                                      public string Name { get; set; } = string.Empty;
                                      public string Nick { get; set; } = string.Empty;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private readonly Normalizer normalizer = new();

                                      [MappaApplyTransformer(nameof(Source.Nick), nameof(Normalizer.Normalize), FieldName = nameof(normalizer))]
                                      public partial Target Map(Source input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                $"{SourceNamespace}.Target",
                $"{SourceNamespace}.Source",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(6)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeMemberAccessExpressionSyntax("input.Name")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_2", initialization => initialization.BeMemberAccessExpressionSyntax("input.Nick")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_3", "this.normalizer.Normalize", "__mappa_tmp_2"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_4", "this.normalizer.Normalize", "__mappa_tmp_3"))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            $"{SourceNamespace}.Target",
                            "__mappa_tmp_5",
                            initialization => initialization.BeObjectCreationExpressionSyntax(
                                $"{SourceNamespace}.Target",
                                ("Name", property => property.BeIdentifierNameSyntax("__mappa_tmp_1")),
                                ("Nick", property => property.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_5"));
                });
    }

    /// <summary>
    /// Test <c>FieldName</c> resolves a static field so a static map method can use an instance method of its value.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyTransformerFromStaticFieldOnStaticMapMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class Normalizer
                                  {
                                      public string Normalize(string value) => value.Trim();
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private static readonly Normalizer SharedNormalizer = new();

                                      [MappaApplyTransformer(nameof(Normalizer.Normalize), FieldName = nameof(SharedNormalizer))]
                                      public static partial string Map(int input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultStaticMapMethod(
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                1,
                false,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "SharedNormalizer.Normalize", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }
}