// <copyright file="ValueTransformerIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for value transformers (<c>[MappaTransformer]</c>).
/// </summary>
public sealed partial class ValueTransformerIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string SourceNamespace = "Mappa.Generator.Tests.UnitTests.SourceCode";

    /// <summary>
    /// Test a class-level transformer is applied to the root source and to the root returned value.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyClassTransformerToRootSourceAndReturnValue()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private string Trim(string value) => value.Trim();

                                      public partial string Map(string input);
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
                typeof(string).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_1", "this.Trim", "input"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.Trim", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test a transformer declared on the mapper base class is applied to the root source and returned value.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyBaseClassTransformerToRootSourceAndReturnValue()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public abstract class MapperBase
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      protected string Trim(string value) => value.Trim();
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper : MapperBase
                                  {
                                      public partial string Map(string input);
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
                typeof(string).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_1", "this.Trim", "input"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.Trim", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test transformers are applied at nested property and collection element sites.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyClassTransformerToNestedPropertyAndCollectionSites()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string Name { get; set; } = string.Empty;
                                      public List<int> Values { get; set; } = new();
                                  }

                                  public class Target
                                  {
                                      public string Name { get; set; } = string.Empty;
                                      public List<string> Values { get; set; } = new();
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private string Trim(string value) => value.Trim();

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
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(string).ToString(),
                            "__mappa_tmp_1",
                            initialization => initialization.BeMemberAccessExpressionSyntax("input.Name")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.Trim", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_3", "this.Trim", "__mappa_tmp_2"))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(List<int>).ToString(),
                            "__mappa_tmp_4",
                            initialization => initialization.BeMemberAccessExpressionSyntax("input.Values")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(List<string>).ToString(),
                            "__mappa_tmp_5",
                            initialization => initialization.BeObjectCreationExpressionSyntax(
                                typeof(List<string>).ToString(),
                                argument => argument.BeMemberAccessExpressionSyntax("__mappa_tmp_4.Count"))))
                        .HasNextSyntaxNode(node => node.BeForStatementSyntax(
                            declaration => declaration.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_6", 0),
                            condition => condition.BeBinaryExpressionSyntax(
                                left => left.BeIdentifierNameSyntax("__mappa_tmp_6"),
                                SyntaxKind.LessThanToken,
                                right => right.BeMemberAccessExpressionSyntax("__mappa_tmp_4.Count")),
                            incrementor => incrementor.BePrefixUnaryExpressionSyntax(
                                SyntaxKind.PlusPlusToken,
                                operand => operand.BeIdentifierNameSyntax("__mappa_tmp_6")),
                            statement => statement
                                .BeBlockStatement()
                                .AsBlock()
                                .HasSyntaxNodesCount(4)
                                .HasNextSyntaxNode(element => element.BeLocalDeclarationStatementSyntax(
                                    typeof(int).ToString(),
                                    "__mappa_tmp_7",
                                    initialization => initialization.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_4", "__mappa_tmp_6")))
                                .HasNextSyntaxNode(element => element.BeLocalDeclarationStatementSyntax(
                                    typeof(string).ToString(),
                                    "__mappa_tmp_8",
                                    initialization => initialization.BeInvocationExpressionSyntax("__mappa_tmp_7.ToString")))
                                .HasNextSyntaxNode(element => AssertInstanceTransformer(element, typeof(string).ToString(), "__mappa_tmp_9", "this.Trim", "__mappa_tmp_8"))
                                .HasNextSyntaxNode(element => element.BeInvocationExpressionSyntaxStatement(
                                    "__mappa_tmp_5.Add",
                                    argument => argument.BeIdentifierNameSyntax("__mappa_tmp_9")))))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            $"{SourceNamespace}.Target",
                            "__mappa_tmp_10",
                            initialization => initialization.BeObjectCreationExpressionSyntax(
                                $"{SourceNamespace}.Target",
                                ("Name", property => property.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                ("Values", property => property.BeIdentifierNameSyntax("__mappa_tmp_5")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_10"));
                });
    }

    /// <summary>
    /// Test an int to string mapping applies transformers on both the source and the target side.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyTransformersOnBothSidesOfIntToStringMapping()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private int Clamp(int value) => value < 0 ? 0 : value;

                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private string Upper(string value) => value.ToUpperInvariant();

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
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(int).ToString(), "__mappa_tmp_1", "this.Clamp", "input"))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_2", initialization => initialization.BeInvocationExpressionSyntax("__mappa_tmp_1.ToString")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_3", "this.Upper", "__mappa_tmp_2"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_3"));
                });
    }

    /// <summary>
    /// Test a transformer receiving <c>MappaContext</c> is invoked with the context of a map method that provides it.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyContextTransformerWhenMapMethodProvidesContext()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private string Trim(string value) => value.Trim();

                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private string Suffix(string value, MappaContext context) => value;

                                      public partial string Map(string input, MappaContext context);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethodWithContext(
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_1", "this.Trim", "input"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.Suffix", "__mappa_tmp_1", "context"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_3", "this.Trim", "__mappa_tmp_2"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_4", "this.Suffix", "__mappa_tmp_3", "context"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    /// <summary>
    /// Test a transformer receiving <c>MappaContext</c> is skipped when the map method does not provide the context.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task SkipsContextTransformerWhenMapMethodDoesNotProvideContext()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private string Trim(string value) => value.Trim();

                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private string Suffix(string value, MappaContext context) => value;

                                      public partial string Map(string input);
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
                typeof(string).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_1", "this.Trim", "input"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.Trim", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test transformers declaring the value as an <c>in</c> parameter, with and without context, are applied.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyTransformersWithInParameter()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private int Increment(in int value) => value + 1;

                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private int Decrement(in int value, MappaContext context) => value - 1;

                                      public partial int Map(int input, MappaContext context);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethodWithContext(
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(int).ToString(), "__mappa_tmp_1", "this.Increment", "input"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(int).ToString(), "__mappa_tmp_2", "this.Decrement", "__mappa_tmp_1", "context"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(int).ToString(), "__mappa_tmp_3", "this.Increment", "__mappa_tmp_2"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(int).ToString(), "__mappa_tmp_4", "this.Decrement", "__mappa_tmp_3", "context"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    /// <summary>
    /// Test an open generic transformer is bound to the type of every matching mapping site.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyOpenGenericTransformerBoundToTheSiteType()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Count { get; set; }
                                      public string Name { get; set; } = string.Empty;
                                  }

                                  public class Target
                                  {
                                      public int Count { get; set; }
                                      public string Name { get; set; } = string.Empty;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private T Passthrough<T>(T value) where T : struct => value;

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
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(int).ToString(), "__mappa_tmp_1", initialization => initialization.BeMemberAccessExpressionSyntax("input.Count")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(int).ToString(), "__mappa_tmp_2", "this.Passthrough", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(int).ToString(), "__mappa_tmp_3", "this.Passthrough", "__mappa_tmp_2"))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_4", initialization => initialization.BeMemberAccessExpressionSyntax("input.Name")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            $"{SourceNamespace}.Target",
                            "__mappa_tmp_5",
                            initialization => initialization.BeObjectCreationExpressionSyntax(
                                $"{SourceNamespace}.Target",
                                ("Count", property => property.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                ("Name", property => property.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_5"));
                });
    }

    /// <summary>
    /// Test a static map method only uses static transformers.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task StaticMapMethodOnlyUsesStaticTransformers()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private string InstanceTrim(string value) => value.Trim();

                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private static string StaticUpper(string value) => value.ToUpperInvariant();

                                      public static partial string Map(string input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                1,
                false,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => AssertStaticTransformer(node, typeof(string).ToString(), "__mappa_tmp_1", "StaticUpper", "input"))
                        .HasNextSyntaxNode(node => AssertStaticTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "StaticUpper", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test only transformers accessible from the mapper are applied: private base members are ignored silently.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task IgnoresTransformersThatAreNotAccessibleFromTheMapper()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public abstract class MapperBase
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private string HiddenTrim(string value) => value.Trim();

                                      [MappaIgnore]
                                      [MappaTransformer]
                                      protected string VisibleTrim(string value) => value.Trim();
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper : MapperBase
                                  {
                                      public partial string Map(string input);
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
                typeof(string).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_1", "this.VisibleTrim", "input"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.VisibleTrim", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test transformers must match the site type exactly, nullability included.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task OnlyAppliesTransformersMatchingTheSiteTypeNullabilityExactly()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string Map(int input);

                                      [MappaTransformer]
                                      private string? NullableTrim(string? value) => value?.Trim();

                                      [MappaTransformer]
                                      private string Trim(string value) => value.Trim();
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
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(string).ToString(),
                            "__mappa_tmp_1",
                            initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.Trim", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    private static void AssertInstanceTransformer(
        SyntaxNodeAssertions nodeAssertions,
        string type,
        string variableName,
        string callee,
        params string[] arguments)
        => nodeAssertions.BeImplicitlyTypedLocalDeclarationStatementSyntax(
            type,
            variableName,
            initialization => initialization.BeInvocationExpressionSyntax(callee, ToIdentifierAssertions(arguments)));

    private static void AssertStaticTransformer(
        SyntaxNodeAssertions nodeAssertions,
        string type,
        string variableName,
        string methodName,
        params string[] arguments)
        => nodeAssertions.BeImplicitlyTypedLocalDeclarationStatementSyntax(
            type,
            variableName,
            initialization => initialization.BeInvocationExpressionUsingIdentifierNameSyntax(methodName, ToIdentifierAssertions(arguments)));

    private static Action<ExpressionSyntaxAssertions>[] ToIdentifierAssertions(string[] identifiers)
        => [.. identifiers.Select(identifier => (Action<ExpressionSyntaxAssertions>)(expression => expression.BeIdentifierNameSyntax(identifier)))];
}