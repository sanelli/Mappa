// <copyright file="ValueTransformerIntegrationTests.Dependency.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Dependency integration tests for value transformers (<c>MappaTransformerFromDependency</c>).
/// </summary>
public sealed partial class ValueTransformerIntegrationTests
{
    /// <summary>
    /// Test dependency transformers are ignored when <c>MappaTransformerFromDependency</c> is not set.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task IgnoresDependencyTransformersByDefault()
    {
        var sourceCode = BuildDependencyMapper(classAttributes: string.Empty, methodAttributes: string.Empty);

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
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Test dependency transformers of a <c>[MappaDependency]</c> member are applied when enabled on the class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AppliesMappaDependencyTransformersWhenEnabledOnClass()
    {
        var sourceCode = BuildDependencyMapper(
            classAttributes: "[MappaSettings(MappaTransformerFromDependency = BooleanSetting.Enable)]",
            methodAttributes: string.Empty);

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
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.DependencyProperty.DependencyStep", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test the method-level setting disables dependency transformers enabled on the class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MethodSettingDisablesDependencyTransformersEnabledOnClass()
    {
        var sourceCode = BuildDependencyMapper(
            classAttributes: "[MappaSettings(MappaTransformerFromDependency = BooleanSetting.Enable)]",
            methodAttributes: "[MappaSettings(MappaTransformerFromDependency = BooleanSetting.Disable)]");

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
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Test the method-level setting enables dependency transformers when the class does not set them.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MethodSettingEnablesDependencyTransformers()
    {
        var sourceCode = BuildDependencyMapper(
            classAttributes: string.Empty,
            methodAttributes: "[MappaSettings(MappaTransformerFromDependency = BooleanSetting.Enable)]");

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
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.DependencyProperty.DependencyStep", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test <c>mappa.transformerfromdependency</c> enables dependency transformers from <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EditorConfigEnablesDependencyTransformers()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.transformerfromdependency = enable
                                    """;

        var sourceCode = BuildDependencyMapper(classAttributes: string.Empty, methodAttributes: string.Empty);

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

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
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.DependencyProperty.DependencyStep", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test the class-level setting overrides <c>mappa.transformerfromdependency</c> from <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ClassSettingDisablesDependencyTransformersEnabledInEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.transformerfromdependency = enable
                                    """;

        var sourceCode = BuildDependencyMapper(
            classAttributes: "[MappaSettings(MappaTransformerFromDependency = BooleanSetting.Disable)]",
            methodAttributes: string.Empty);

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

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
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Test dependency transformers of a <c>[MappaStaticDependency]</c> type are applied when enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AppliesStaticDependencyTransformersWhenEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public static class StaticDependency
                                  {
                                      [MappaTransformer]
                                      public static string StaticDependencyStep(string value) => value;
                                  }

                                  [Mappa]
                                  [MappaSettings(MappaTransformerFromDependency = BooleanSetting.Enable)]
                                  [MappaStaticDependency(typeof(StaticDependency))]
                                  public sealed partial class Mapper
                                  {
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
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "global::Mappa.Generator.Tests.UnitTests.SourceCode.StaticDependency.StaticDependencyStep", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test static dependency transformers are ignored when the setting is not enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task IgnoresStaticDependencyTransformersWhenNotEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public static class StaticDependency
                                  {
                                      [MappaTransformer]
                                      public static string StaticDependencyStep(string value) => value;
                                  }

                                  [Mappa]
                                  [MappaStaticDependency(typeof(StaticDependency))]
                                  public sealed partial class Mapper
                                  {
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
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Test a static map method only uses static dependency transformers, never instance dependency ones.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task StaticMapMethodOnlyUsesStaticDependencyTransformers()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class InstanceDependency
                                  {
                                      [MappaTransformer]
                                      public string InstanceDependencyStep(string value) => value;
                                  }

                                  public static class StaticDependency
                                  {
                                      [MappaTransformer]
                                      public static string StaticDependencyStep(string value) => value;
                                  }

                                  [Mappa]
                                  [MappaSettings(MappaTransformerFromDependency = BooleanSetting.Enable)]
                                  [MappaStaticDependency(typeof(StaticDependency))]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private readonly InstanceDependency instanceDependency = new();

                                      public static partial string Map(int input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.DependencyDoesNotProvideAnyViableMethod, "instanceDependency")
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
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "global::Mappa.Generator.Tests.UnitTests.SourceCode.StaticDependency.StaticDependencyStep", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    private static string BuildDependencyMapper(string classAttributes, string methodAttributes)
        => $$"""
             #nullable enable
             using Mappa.Attributes;

             namespace Mappa.Generator.Tests.UnitTests.SourceCode;

             public sealed class Dependency
             {
                 [MappaTransformer]
                 public string DependencyStep(string value) => value;
             }

             [Mappa]
             {{classAttributes}}
             public sealed partial class Mapper
             {
                 [MappaDependency]
                 private Dependency DependencyProperty { get; } = new();

                 {{methodAttributes}}
                 public partial string Map(int input);
             }
             """;
}