// <copyright file="ParamsAndInRefKindParametersSupportIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
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

    /// <summary>
    /// Test that input parameter can be <c>params</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestInputParameterCanBeParamsArray()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      public static partial int[] Map(params int[] input);
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
                typeof(int[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int[]).ToString(),
                NullableAnnotation.NotAnnotated,
                RefKind.None,
                true,
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
    /// Test that local dependency method is picked up when input parameter is <c>in</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestLocalDependencyMethodIsPickedUpWhenInputIsInRefKind()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public record Target(long Property){}
                                  public record Source(int Property){}

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public long MapIntToLong(in int input) => input + 1;
                                  
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax(
                                    "this.MapIntToLong",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeReturnStatement(expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Test that local dependency method is picked up when input parameter is <c>params</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestLocalDependencyMethodIsPickedUpWhenInputIsParams()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public record Target(long[] Property){}
                                  public record Source(int[] Property){}

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public long[] MapIntToLong(params int[] input) => Array.Empty<long>();
                                  
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long[]).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax(
                                    "this.MapIntToLong",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeReturnStatement(expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Test that local dependency method is picked up when context parameter is <c>in</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestLocalDependencyMethodIsPickedUpWhenContextIsInRefKind()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public record Target(long Property){}
                                  public record Source(int Property){}

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public long MapIntToLong(int input, in MappaContext context) => input + 1;
                                  
                                      public partial Target Map(Source input, MappaContext context);
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
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax(
                                    "this.MapIntToLong",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("context"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeReturnStatement(expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Test that <see cref="MappaDependencyAttribute"/> dependency method is picked up when input parameter is <c>in</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestMappaDependencyAttributeDependencyMethodIsPickedUpWhenInputIsInRefKind()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public record Target(long Property){}
                                  public record Source(int Property){}
                                  
                                  public sealed class Dependency {
                                    public long MapIntToLong(in int input) => input + 1;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private readonly Dependency dependency = new Dependency();
                                  
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax(
                                    "this.dependency.MapIntToLong",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeReturnStatement(expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Test that <see cref="MappaDependencyAttribute"/> dependency method is picked up when input parameter is <c>params</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestMappaDependencyAttributeDependencyMethodIsPickedUpWhenInputIsParams()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public record Target(long[] Property){}
                                  public record Source(int[] Property){}

                                  public sealed class Dependency {
                                     public long[] MapIntToLong(params int[] input) => Array.Empty<long>();
                                  }
                                  
                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private readonly Dependency dependency = new Dependency();
                                  
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long[]).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax(
                                    "this.dependency.MapIntToLong",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeReturnStatement(expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Test that  <see cref="MappaDependencyAttribute"/> dependency method is picked up when context parameter is <c>in</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestMappaDependencyAttributeDependencyMethodIsPickedUpWhenContextIsInRefKind()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public record Target(long Property){}
                                  public record Source(int Property){}

                                  public sealed class Dependency {
                                     public long MapIntToLong(int input, in MappaContext context) => input + 1;
                                  }
                                  
                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private readonly Dependency dependency = new Dependency();
                                      
                                      public partial Target Map(Source input, MappaContext context);
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
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax(
                                    "this.dependency.MapIntToLong",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("context"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeReturnStatement(expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Test that <see cref="MappaStaticDependencyAttribute"/> dependency method is picked up when input parameter is <c>in</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestMappaStaticDependencyAttributeDependencyMethodIsPickedUpWhenInputIsInRefKind()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public record Target(long Property){}
                                  public record Source(int Property){}
                                  
                                  public static class Dependency {
                                    public static long MapIntToLong(in int input) => input + 1;
                                  }

                                  [Mappa]
                                  [MappaStaticDependency(typeof(Dependency))]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax(
                                    "global::Mappa.Generator.Tests.UnitTests.SourceCode.Dependency.MapIntToLong",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeReturnStatement(expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Test that <see cref="MappaStaticDependencyAttribute"/> dependency method is picked up when input parameter is <c>params</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestMappaStaticDependencyAttributeDependencyMethodIsPickedUpWhenInputIsParams()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public record Target(long[] Property){}
                                  public record Source(int[] Property){}

                                  public static class Dependency {
                                     public static long[] MapIntToLong(params int[] input) => Array.Empty<long>();
                                  }
                                  
                                  [Mappa]
                                  [MappaStaticDependency(typeof(Dependency))]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long[]).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax(
                                    "global::Mappa.Generator.Tests.UnitTests.SourceCode.Dependency.MapIntToLong",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeReturnStatement(expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Test that  <see cref="MappaStaticDependencyAttribute"/> dependency method is picked up when context parameter is <c>in</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestMappaStaticDependencyAttributeDependencyMethodIsPickedUpWhenContextIsInRefKind()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public record Target(long Property){}
                                  public record Source(int Property){}

                                  public static class Dependency {
                                     public static long MapIntToLong(int input, in MappaContext context) => input + 1;
                                  }
                                  
                                  [Mappa]
                                  [MappaStaticDependency(typeof(Dependency))]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input, MappaContext context);
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
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax(
                                    "global::Mappa.Generator.Tests.UnitTests.SourceCode.Dependency.MapIntToLong",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("context"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeReturnStatement(expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Test that input parameter cannot be <c>ref</c> or <c>out</c>.
    /// </summary>
    /// <param name="refKind">The ref kind to test.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [IntegrationTest]
    [InlineData("ref")]
    [InlineData("out")]
    public async Task TestNoMappingIsGeneratedWhenInputIsUnsupportedRefKind(string refKind)
    {
        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public static partial class Mapper
                          {
                              public static partial long Map({{refKind}} int input);
                          }
                          #nullable restore
                          """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Test that context parameter cannot be <c>ref</c> or <c>out</c>.
    /// </summary>
    /// <param name="refKind">The ref kind to test.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [IntegrationTest]
    [InlineData("ref")]
    [InlineData("out")]
    public async Task TestNoMappingIsGeneratedWhenContextIsUnsupportedRefKind(string refKind)
    {
        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           [Mappa]
                           public static partial class Mapper
                           {
                               public static partial long Map(int input, {{refKind}} MappaContext context);
                           }
                           #nullable restore
                           """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Test that local dependency method not is picked up when input parameter is <c>ref</c>
    /// or <c>out</c>.
    /// </summary>
    /// <param name="refKind">The unsupported reference kind.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [IntegrationTest]
    [InlineData("out")]
    [InlineData("ref")]
    public async Task TestLocalDependencyMethodIsNotPickedUpWhenInputIsUnsupportedRefKind(string refKind)
    {
        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                          
                          public record Target(long Property){}
                          public record Source(int Property){}

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              public long MapIntToLong({{refKind}} int input) => input + 1;
                          
                              public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeReturnStatement(expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Test that local dependency method not is picked up when context parameter is <c>ref</c>
    /// or <c>out</c>.
    /// </summary>
    /// <param name="refKind">The unsupported reference kind.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [IntegrationTest]
    [InlineData("out")]
    [InlineData("ref")]
    public async Task TestLocalDependencyMethodIsNotPickedUpWhenContextIsUnsupportedRefKind(string refKind)
    {
        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                          
                          public record Target(long Property){}
                          public record Source(int Property){}

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              public long MapIntToLong(int input, {{refKind}} MappaContext context) => input + 1;
                          
                              public partial Target Map(Source input, MappaContext context);
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
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeReturnStatement(expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Test that <see cref="MappaDependencyAttribute"/> dependency method not is picked up when
    /// input parameter is <c>ref</c> or <c>out</c>.
    /// </summary>
    /// <param name="refKind">The unsupported reference kind.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [IntegrationTest]
    [InlineData("out")]
    [InlineData("ref")]
    public async Task TestMappaDependencyAttributeDependencyMethodIsNotPickedUpWhenInputIsUnsupportedRefKind(string refKind)
    {
        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                          
                          public record Target(long Property){}
                          public record Source(int Property){}
                          
                          public sealed class Dependency
                          {
                              public long MapIntToLong({{refKind}} int input) => input + 1;
                          }

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaDependency]
                              private readonly Dependency dependency = new();
                          
                              public partial Target Map(Source input);
                          }
                          #nullable restore
                          """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.DependencyDoesNotProvideAnyViableMethod, "dependency")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeReturnStatement(expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Test that <see cref="MappaDependencyAttribute"/> dependency method not is
    /// picked up when context parameter is <c>ref</c> or <c>out</c>.
    /// </summary>
    /// <param name="refKind">The unsupported reference kind.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [IntegrationTest]
    [InlineData("out")]
    [InlineData("ref")]
    public async Task TestMappaDependencyAttributeDependencyMethodIsNotPickedUpWhenContextIsUnsupportedRefKind(string refKind)
    {
        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                          
                          public record Target(long Property){}
                          public record Source(int Property){}
                          
                          public sealed class Dependency
                          {
                            public long MapIntToLong(int input, {{refKind}} MappaContext context) => input + 1;
                          }

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaDependency]
                              private readonly Dependency dependency = new Dependency();
                          
                              public partial Target Map(Source input, MappaContext context);
                          }
                          #nullable restore
                          """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.DependencyDoesNotProvideAnyViableMethod, "dependency")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeReturnStatement(expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Test that <see cref="MappaStaticDependencyAttribute"/> dependency method not is picked up when
    /// input parameter is <c>ref</c> or <c>out</c>.
    /// </summary>
    /// <param name="refKind">The unsupported reference kind.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [IntegrationTest]
    [InlineData("out")]
    [InlineData("ref")]
    public async Task TestMappaStaticDependencyAttributeDependencyMethodIsNotPickedUpWhenInputIsUnsupportedRefKind(string refKind)
    {
        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                          
                          public record Target(long Property){}
                          public record Source(int Property){}
                          
                          public static class Dependency
                          {
                              public static long MapIntToLong({{refKind}} int input) => input + 1;
                          }

                          [Mappa]
                          [MappaStaticDependency(typeof(Dependency))]
                          public sealed partial class Mapper
                          {
                              public partial Target Map(Source input);
                          }
                          #nullable restore
                          """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.DependencyDoesNotProvideAnyViableMethod, "Mappa.Generator.Tests.UnitTests.SourceCode.Dependency")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeReturnStatement(expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Test that <see cref="MappaStaticDependencyAttribute"/> dependency method not is
    /// picked up when context parameter is <c>ref</c> or <c>out</c>.
    /// </summary>
    /// <param name="refKind">The unsupported reference kind.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [IntegrationTest]
    [InlineData("out")]
    [InlineData("ref")]
    public async Task TestMappaStaticDependencyAttributeDependencyMethodIsNotPickedUpWhenContextIsUnsupportedRefKind(string refKind)
    {
        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                          
                          public record Target(long Property){}
                          public record Source(int Property){}
                          
                          public static class Dependency
                          {
                            public static long MapIntToLong(int input, {{refKind}} MappaContext context) => input + 1;
                          }

                          [Mappa]
                          [MappaStaticDependency(typeof(Dependency))]
                          public sealed partial class Mapper
                          {
                              public partial Target Map(Source input, MappaContext context);
                          }
                          #nullable restore
                          """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.DependencyDoesNotProvideAnyViableMethod, "Mappa.Generator.Tests.UnitTests.SourceCode.Dependency")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(nodeAssertions =>
                            nodeAssertions.BeReturnStatement(expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }
}