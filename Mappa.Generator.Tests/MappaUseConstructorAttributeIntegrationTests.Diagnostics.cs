// <copyright file="MappaUseConstructorAttributeIntegrationTests.Diagnostics.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Diagnostic integration tests for <c>MappaUseConstructor</c>.
/// </summary>
public sealed partial class MappaUseConstructorAttributeIntegrationTests
{
    /// <summary>
    /// NoParameters with no empty constructor reports MP00087.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NoParametersWithoutEmptyConstructorReportsMp00087()
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
                                      public Target(string propertyA) { }
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
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MappaUseConstructorNoEmptyConstructor, "Map", TargetType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertNoMapStubBody);
    }

    /// <summary>
    /// WithParameters with a missing signature reports MP00088.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task WithParametersMissingSignatureReportsMp00088()
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
                                      [MappaUseConstructor(ConstructorType.WithParameters, typeof(int))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MappaUseConstructorNoMatchingParameterizedConstructor, "Map", TargetType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertNoMapStubBody);
    }

    /// <summary>
    /// NoParameters with parameter types reports MP00089.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NoParametersWithParameterTypesReportsMp00089()
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
                                      public string PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseConstructor(ConstructorType.NoParameters, typeof(int))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaUseConstructorInvalidConfiguration,
                "Map",
                TargetType,
                "ConstructorType.NoParameters must not specify parameter types")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertNoMapStubBody);
    }

    /// <summary>
    /// WithParameters without parameter types reports MP00089.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task WithParametersWithoutParameterTypesReportsMp00089()
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
                                      [MappaUseConstructor(ConstructorType.WithParameters)]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaUseConstructorInvalidConfiguration,
                "Map",
                TargetType,
                "ConstructorType.WithParameters must specify parameter types")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertNoMapStubBody);
    }

    /// <summary>
    /// Selected parameterized constructor that cannot map reports MP00090.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task SelectedConstructorThatCannotMapReportsMp00090()
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
                                      public Target(int other) { }
                                      public string PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseConstructor(ConstructorType.WithParameters, typeof(int))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MappaUseConstructorCannotMap, "Map", TargetType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertNoMapStubBody);
    }

    /// <summary>
    /// Forced empty constructor that cannot map required properties reports MP00090.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ForcedEmptyConstructorThatCannotMapReportsMp00090()
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

                                  public class Nested
                                  {
                                      private Nested() { }
                                  }

                                  public class Target
                                  {
                                      public Target() { }
                                      public Target(string propertyA) { }
                                      public required Nested Nested { get; set; }
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
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MappaUseConstructorCannotMap, "Map", TargetType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertNoMapStubBody);
    }

    /// <summary>
    /// Duplicate attributes for the same type report MP00091.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DuplicateAttributesForSameTypeReportMp00091()
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
                                      public string PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseConstructor(ConstructorType.NoParameters)]
                                      [MappaUseConstructor(typeof(Target), ConstructorType.NoParameters)]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.DuplicateMappaUseConstructorForTargetType, "Map", TargetType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertNoMapStubBody);
    }

    /// <summary>
    /// Unused attribute targeting an unrelated type reports MP00092.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task UnusedAttributeReportsMp00092()
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
                                      public string PropertyA { get; set; }
                                  }

                                  public class Unused
                                  {
                                      public Unused() { }
                                      public string PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseConstructor(typeof(Unused), ConstructorType.NoParameters)]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MappaUseConstructorAttributeNotUsed, "Map", $"{SourceNamespace}.Unused")
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

    private static void AssertNoMapStubBody(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(1)
            .HasNextSyntaxNode(node => node.BeReturnStatement());
    }
}