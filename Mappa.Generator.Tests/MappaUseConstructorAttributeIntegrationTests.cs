// <copyright file="MappaUseConstructorAttributeIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <c>MappaUseConstructor</c> happy paths.
/// </summary>
public sealed partial class MappaUseConstructorAttributeIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string SourceNamespace = "Mappa.Generator.Tests.UnitTests.SourceCode";
    private const string SourceType = $"{SourceNamespace}.Source";
    private const string TargetType = $"{SourceNamespace}.Target";

    /// <summary>
    /// Forces the empty constructor when a mapping constructor would otherwise win.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ForcesEmptyConstructorOverMappingConstructor()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public Target() { }
                                      public Target(Source source) { }
                                      public string PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseConstructor(ConstructorType.NoParameters)]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_1",
                            init => init.BeMemberAccessExpressionSyntax("input.PropertyA")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(string).ToString(),
                            "__mappa_tmp_2",
                            init => init.BeInvocationExpressionSyntax($"__mappa_tmp_1.{nameof(this.ToString)}")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_3",
                            init => init.BeObjectCreationExpressionSyntax(
                                TargetType,
                                ("PropertyA", prop => prop.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_3"));
                });
    }

    /// <summary>
    /// Forces a parameterized constructor when an empty constructor would otherwise win.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ForcesParameterizedConstructorOverEmptyConstructor()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public Target() { }
                                      public Target(string propertyA) { }
                                      public string PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseConstructor(ConstructorType.WithParameters, typeof(string))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            "string",
                            "__mappa_tmp_1",
                            init => init.BeMemberAccessExpressionSyntax("input.PropertyA")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_2",
                            init => init.BeObjectCreationExpressionSyntax(
                                TargetType,
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Forces a mapping constructor when WithParameters matches the source type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ForcesMappingConstructorWhenSignatureMatchesSourceType()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public Target() { }
                                      public Target(Source source) { }
                                      public string PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseConstructor(ConstructorType.WithParameters, typeof(Source))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            SourceType,
                            "__mappa_tmp_1",
                            init => init.BeIdentifierNameSyntax("input")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_2",
                            init => init.BeObjectCreationExpressionSyntax(
                                TargetType,
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Generic exact-signature form selects the matching parameterized constructor.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task GenericFormSelectsExactParameterizedConstructor()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public string PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public Target() { }
                                      public Target(int propertyA) { }
                                      public Target(int propertyA, string propertyB) { }
                                      public int PropertyA { get; set; }
                                      public string PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseConstructor<int, string>]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_1",
                            init => init.BeMemberAccessExpressionSyntax("input.PropertyA")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            "string",
                            "__mappa_tmp_2",
                            init => init.BeMemberAccessExpressionSyntax("input.PropertyB")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_3",
                            init => init.BeObjectCreationExpressionSyntax(
                                TargetType,
                                first => first.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                second => second.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_3"));
                });
    }

    /// <summary>
    /// Generic form with an explicit target type forces constructor selection on a nested type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task GenericFormWithTargetTypeForcesConstructorOnNestedTargetType()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class NestedSource
                                  {
                                      public string PropertyA { get; set; }
                                  }

                                  public class NestedTarget
                                  {
                                      public NestedTarget() { }
                                      public NestedTarget(string propertyA) { }
                                      public string PropertyA { get; set; }
                                  }

                                  public class Source
                                  {
                                      public NestedSource Nested { get; set; }
                                  }

                                  public class Target
                                  {
                                      public NestedTarget Nested { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseConstructor<string>(typeof(NestedTarget))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            $"{SourceNamespace}.NestedSource",
                            "__mappa_tmp_1",
                            init => init.BeMemberAccessExpressionSyntax("input.Nested")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            "string",
                            "__mappa_tmp_2",
                            init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_1.PropertyA")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            $"{SourceNamespace}.NestedTarget",
                            "__mappa_tmp_3",
                            init => init.BeObjectCreationExpressionSyntax(
                                $"{SourceNamespace}.NestedTarget",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_4",
                            init => init.BeObjectCreationExpressionSyntax(
                                TargetType,
                                ("Nested", prop => prop.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    /// <summary>
    /// Typed form forces constructor selection on a nested target type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TypedFormForcesConstructorOnNestedTargetType()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class NestedSource
                                  {
                                      public string PropertyA { get; set; }
                                  }

                                  public class NestedTarget
                                  {
                                      public NestedTarget() { }
                                      public NestedTarget(string propertyA) { }
                                      public string PropertyA { get; set; }
                                  }

                                  public class Source
                                  {
                                      public NestedSource Nested { get; set; }
                                  }

                                  public class Target
                                  {
                                      public NestedTarget Nested { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseConstructor(typeof(NestedTarget), ConstructorType.WithParameters, typeof(string))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            $"{SourceNamespace}.NestedSource",
                            "__mappa_tmp_1",
                            init => init.BeMemberAccessExpressionSyntax("input.Nested")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            "string",
                            "__mappa_tmp_2",
                            init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_1.PropertyA")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            $"{SourceNamespace}.NestedTarget",
                            "__mappa_tmp_3",
                            init => init.BeObjectCreationExpressionSyntax(
                                $"{SourceNamespace}.NestedTarget",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_4",
                            init => init.BeObjectCreationExpressionSyntax(
                                TargetType,
                                ("Nested", prop => prop.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                });
    }
}