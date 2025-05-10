// <copyright file="ParamsAndInRefKindParametersSupportIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests to check that we support <c>in</c> and <c>params</c>.
/// </summary>
public sealed class ParamsAndInRefKindParametersSupportIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test that input parameter can be <c>in</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestInputParameterCanBeIn()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      public static partial long Map(in int input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveMapMethod(
                typeof(long).ToString(),
                NullableAnnotation.NotAnnotated,
                "input",
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                RefKind.In,
                false,
                null,
                RefKind.None,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                        {
                            expressionSyntaxAssertions.BeIdentifierNameSyntax("input");
                        }));
                });
    }

    /// <summary>
    /// Test that context parameter can be <c>in</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestContextParameterCanBeIn()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      public static partial long Map(int input, in MappaContext context);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveMapMethod(
                typeof(long).ToString(),
                NullableAnnotation.NotAnnotated,
                "input",
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                RefKind.None,
                false,
                "context",
                RefKind.In,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                        {
                            expressionSyntaxAssertions.BeIdentifierNameSyntax("input");
                        }));
                });
    }

    /// <summary>
    /// Test that both input and context parameter can be <c>in</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestBothInputParameterAndContextParameterCanBeIn()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      public static partial long Map(in int input, in MappaContext context);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveMapMethod(
                typeof(long).ToString(),
                NullableAnnotation.NotAnnotated,
                "input",
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                RefKind.In,
                false,
                "context",
                RefKind.In,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                        {
                            expressionSyntaxAssertions.BeIdentifierNameSyntax("input");
                        }));
                });
    }

    // TODO [#164] Test input parameter can be params.
    // TODO [#164] Test method from local dependency is picked up when input is in.
    // TODO [#164] Test method from local dependency is picked up when input is params.
    // TODO [#164] Test method from local dependency is picked up when context is in.
    // TODO [#164] Test method from [MappaDependency] field dependency is picked up when input is in.
    // TODO [#164] Test method from [MappaDependency] field dependency is picked up when input is params.
    // TODO [#164] Test method from [MappaDependency] field dependency is picked up when context is in.
    // TODO [#164] Test method from [MappaStaticDependency] dependency is picked up when input is in.
    // TODO [#164] Test method from [MappaStaticDependency] dependency is picked up when input is params.
    // TODO [#164] Test method from [MappaStaticDependency] dependency is picked up when context is in.
    // TODO [#164] Test input parameter cannot be out -> no code generated.
    // TODO [#164] Test input parameter cannot be ref -> no code generated.
    // TODO [#164] Test method from local class is ignored for dependency when input is ref.
    // TODO [#164] Test method from local class is ignored for dependency when context is ref.
    // TODO [#164] Test method from local class is ignored for dependency when input is out.
    // TODO [#164] Test method from local class is ignored for dependency when context is out.
    // TODO [#164] Test method from [MappaDependency] field is ignored when input is ref.
    // TODO [#164] Test method from [MappaDependency] field is ignored when context is ref.
    // TODO [#164] Test method from [MappaDependency] field is ignored when input is out.
    // TODO [#164] Test method from [MappaDependency] field is ignored when context is out.
    // TODO [#164] Test method from [MappaStaticDependency] is ignored when input is ref.
    // TODO [#164] Test method from [MappaStaticDependency] is ignored when context is ref.
    // TODO [#164] Test method from [MappaStaticDependency] is ignored when input is out.
    // TODO [#164] Test method from [MappaStaticDependency] is ignored when context is out.
}