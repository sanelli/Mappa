// <copyright file="IdentityStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests related to the identity strategy.
/// </summary>
public sealed class IdentityStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created when source and target type are the
    /// very same non reference type and nullable is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToSameReferenceWhenNullableDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string Map(string input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveMethods(1)
            .HaveMethod(
                typeof(string),
                NullableAnnotation.None,
                "Map",
                new[] { (typeof(string), NullableAnnotation.None, "input") },
                methodDeclarationSyntaxAssertions =>
                {
                    methodDeclarationSyntaxAssertions
                        .HaveGeneratedCodeAttribute()
                        .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
                        .HaveBody(blockSyntaxAssertions =>
                        {
                            blockSyntaxAssertions
                                .HasSyntaxNodes(1)
                                .HasNextSyntaxNode(nodeAssertions =>
                                {
                                    nodeAssertions.IsReturnStatement(expressionSyntaxAssertions =>
                                    {
                                        expressionSyntaxAssertions.IsIdentifierName("input");
                                    });
                                });
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source and target type are the
    /// very same non reference type and nullable is enabled but not applied.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToSameReferenceWhenNullableEnabledAndNotApplied()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string Map(string input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveMethods(1)
            .HaveMethod(
                typeof(string),
                NullableAnnotation.NotAnnotated,
                "Map",
                new[] { (typeof(string), NullableAnnotation.NotAnnotated, "input") },
                methodDeclarationSyntaxAssertions =>
                {
                    methodDeclarationSyntaxAssertions
                        .HaveGeneratedCodeAttribute()
                        .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
                        .HaveBody(blockSyntaxAssertions =>
                        {
                            blockSyntaxAssertions
                                .HasSyntaxNodes(1)
                                .HasNextSyntaxNode(nodeAssertions =>
                                {
                                    nodeAssertions.IsReturnStatement(expressionSyntaxAssertions =>
                                    {
                                        expressionSyntaxAssertions.IsIdentifierName("input");
                                    });
                                });
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source and target type are the
    /// very same non reference type and nullable is enabled and applied.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToSameReferenceWhenNullableEnabledAndApplied()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string? Map(string? input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveMethods(1)
            .HaveMethod(
                typeof(string),
                NullableAnnotation.Annotated,
                "Map",
                new[] { (typeof(string), NullableAnnotation.Annotated, "input") },
                methodDeclarationSyntaxAssertions =>
                {
                    methodDeclarationSyntaxAssertions
                        .HaveGeneratedCodeAttribute()
                        .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
                        .HaveBody(blockSyntaxAssertions =>
                        {
                            blockSyntaxAssertions
                                .HasSyntaxNodes(1)
                                .HasNextSyntaxNode(nodeAssertions =>
                                {
                                    nodeAssertions.IsReturnStatement(expressionSyntaxAssertions =>
                                    {
                                        expressionSyntaxAssertions.IsIdentifierName("input");
                                    });
                                });
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source and target type are the
    /// very same non reference type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNonReferenceTypeToSameNonReferenceWhenNullableDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial int Map(int input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveMethods(1)
            .HaveMethod(
                typeof(int),
                NullableAnnotation.NotAnnotated,
                "Map",
                new[] { (typeof(int), NullableAnnotation.NotAnnotated, "input") },
                methodDeclarationSyntaxAssertions =>
                {
                    methodDeclarationSyntaxAssertions
                        .HaveGeneratedCodeAttribute()
                        .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
                        .HaveBody(blockSyntaxAssertions =>
                        {
                            blockSyntaxAssertions
                                .HasSyntaxNodes(1)
                                .HasNextSyntaxNode(nodeAssertions =>
                                {
                                    nodeAssertions.IsReturnStatement(expressionSyntaxAssertions =>
                                    {
                                        expressionSyntaxAssertions.IsIdentifierName("input");
                                    });
                                });
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source and target type are the
    /// very same non reference type and target type is nullable.
    /// Also the nullability has been disabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNonReferenceTypeToSameNullableNonReferenceWhenNullableDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial int? Map(int input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveMethods(1)
            .HaveMethod(
                typeof(int?),
                NullableAnnotation.Annotated,
                "Map",
                new[] { (typeof(int), NullableAnnotation.NotAnnotated, "input") },
                methodDeclarationSyntaxAssertions =>
                {
                    methodDeclarationSyntaxAssertions
                        .HaveGeneratedCodeAttribute()
                        .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
                        .HaveBody(blockSyntaxAssertions =>
                        {
                            blockSyntaxAssertions
                                .HasSyntaxNodes(1)
                                .HasNextSyntaxNode(nodeAssertions =>
                                {
                                    nodeAssertions.IsReturnStatement(expressionSyntaxAssertions =>
                                    {
                                        expressionSyntaxAssertions.IsIdentifierName("input");
                                    });
                                });
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source and target type are the
    /// very same non reference type and target type is nullable.
    /// Also the nullability has been enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNonReferenceTypeToSameNullableNonReferenceWhenNullableEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial int? Map(int input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveMethods(1)
            .HaveMethod(
                typeof(int?),
                NullableAnnotation.Annotated,
                "Map",
                new[] { (typeof(int), NullableAnnotation.NotAnnotated, "input") },
                methodDeclarationSyntaxAssertions =>
                {
                    methodDeclarationSyntaxAssertions
                        .HaveGeneratedCodeAttribute()
                        .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
                        .HaveBody(blockSyntaxAssertions =>
                        {
                            blockSyntaxAssertions
                                .HasSyntaxNodes(1)
                                .HasNextSyntaxNode(nodeAssertions =>
                                {
                                    nodeAssertions.IsReturnStatement(expressionSyntaxAssertions =>
                                    {
                                        expressionSyntaxAssertions.IsIdentifierName("input");
                                    });
                                });
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source is a non reference type
    /// and the target type is <see cref="object"/> and nullable is disabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNonReferenceTypeObjectWhenNullableDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial object Map(int input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveMethods(1)
            .HaveMethod(
                typeof(object),
                NullableAnnotation.None,
                "Map",
                new[] { (typeof(int), NullableAnnotation.NotAnnotated, "input") },
                methodDeclarationSyntaxAssertions =>
                {
                    methodDeclarationSyntaxAssertions
                        .HaveGeneratedCodeAttribute()
                        .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
                        .HaveBody(blockSyntaxAssertions =>
                        {
                            blockSyntaxAssertions
                                .HasSyntaxNodes(1)
                                .HasNextSyntaxNode(nodeAssertions =>
                                {
                                    nodeAssertions.IsReturnStatement(expressionSyntaxAssertions =>
                                    {
                                        expressionSyntaxAssertions.IsIdentifierName("input");
                                    });
                                });
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source is a non reference type
    /// and the target type is nullable <see cref="object"/> and
    /// nullable is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNonReferenceTypeNullableObjectWhenNullableIsEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial object? Map(int input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveMethods(1)
            .HaveMethod(
                typeof(object),
                NullableAnnotation.Annotated,
                "Map",
                new[] { (typeof(int), NullableAnnotation.NotAnnotated, "input") },
                methodDeclarationSyntaxAssertions =>
                {
                    methodDeclarationSyntaxAssertions
                        .HaveGeneratedCodeAttribute()
                        .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
                        .HaveBody(blockSyntaxAssertions =>
                        {
                            blockSyntaxAssertions
                                .HasSyntaxNodes(1)
                                .HasNextSyntaxNode(nodeAssertions =>
                                {
                                    nodeAssertions.IsReturnStatement(expressionSyntaxAssertions =>
                                    {
                                        expressionSyntaxAssertions.IsIdentifierName("input");
                                    });
                                });
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from reference type
    /// to <see cref="object"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToObjectWhenNullableDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial object Map(string input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveMethods(1)
            .HaveMethod(
                typeof(object),
                NullableAnnotation.None,
                "Map",
                new[] { (typeof(string), NullableAnnotation.None, "input") },
                methodDeclarationSyntaxAssertions =>
                {
                    methodDeclarationSyntaxAssertions
                        .HaveGeneratedCodeAttribute()
                        .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
                        .HaveBody(blockSyntaxAssertions =>
                        {
                            blockSyntaxAssertions
                                .HasSyntaxNodes(1)
                                .HasNextSyntaxNode(nodeAssertions =>
                                {
                                    nodeAssertions.IsReturnStatement(expressionSyntaxAssertions =>
                                    {
                                        expressionSyntaxAssertions.IsIdentifierName("input");
                                    });
                                });
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from reference type
    /// to nullable <see cref="object"/> when nullable is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToNullableObjectWhenNullableEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial object? Map(string input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveMethods(1)
            .HaveMethod(
                typeof(object),
                NullableAnnotation.Annotated,
                "Map",
                new[] { (typeof(string), NullableAnnotation.NotAnnotated, "input") },
                methodDeclarationSyntaxAssertions =>
                {
                    methodDeclarationSyntaxAssertions
                        .HaveGeneratedCodeAttribute()
                        .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
                        .HaveBody(blockSyntaxAssertions =>
                        {
                            blockSyntaxAssertions
                                .HasSyntaxNodes(1)
                                .HasNextSyntaxNode(nodeAssertions =>
                                {
                                    nodeAssertions.IsReturnStatement(expressionSyntaxAssertions =>
                                    {
                                        expressionSyntaxAssertions.IsIdentifierName("input");
                                    });
                                });
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source and target type are the
    /// very same non reference type and nullable is enabled and applied.
    /// Also the target type is nullable while the source is not nullable.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToSameNullableReferenceWhenNullableEnabledAndApplied()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string? Map(string input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveMethods(1)
            .HaveMethod(
                typeof(string),
                NullableAnnotation.Annotated,
                "Map",
                new[] { (typeof(string), NullableAnnotation.NotAnnotated, "input") },
                methodDeclarationSyntaxAssertions =>
                {
                    methodDeclarationSyntaxAssertions
                        .HaveGeneratedCodeAttribute()
                        .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
                        .HaveBody(blockSyntaxAssertions =>
                        {
                            blockSyntaxAssertions
                                .HasSyntaxNodes(1)
                                .HasNextSyntaxNode(nodeAssertions =>
                                {
                                    nodeAssertions.IsReturnStatement(expressionSyntaxAssertions =>
                                    {
                                        expressionSyntaxAssertions.IsIdentifierName("input");
                                    });
                                });
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from reference type
    /// to <see cref="object"/> when nullable is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToObjectWhenNullableEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial object Map(string input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveMethods(1)
            .HaveMethod(
                typeof(object),
                NullableAnnotation.NotAnnotated,
                "Map",
                new[] { (typeof(string), NullableAnnotation.NotAnnotated, "input") },
                methodDeclarationSyntaxAssertions =>
                {
                    methodDeclarationSyntaxAssertions
                        .HaveGeneratedCodeAttribute()
                        .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
                        .HaveBody(blockSyntaxAssertions =>
                        {
                            blockSyntaxAssertions
                                .HasSyntaxNodes(1)
                                .HasNextSyntaxNode(nodeAssertions =>
                                {
                                    nodeAssertions.IsReturnStatement(expressionSyntaxAssertions =>
                                    {
                                        expressionSyntaxAssertions.IsIdentifierName("input");
                                    });
                                });
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when an implicit conversion
    /// can be applied between the types.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingImplicitConversion()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  internal sealed partial class Mapper
                                  {
                                      internal partial long Map(int input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.InternalKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveMethods(1)
            .HaveMethod(
                typeof(long),
                NullableAnnotation.NotAnnotated,
                "Map",
                new[] { (typeof(int), NullableAnnotation.NotAnnotated, "input") },
                methodDeclarationSyntaxAssertions =>
                {
                    methodDeclarationSyntaxAssertions
                        .HaveGeneratedCodeAttribute()
                        .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.InternalKeyword)
                        .HaveBody(blockSyntaxAssertions =>
                        {
                            blockSyntaxAssertions
                                .HasSyntaxNodes(1)
                                .HasNextSyntaxNode(nodeAssertions =>
                                {
                                    nodeAssertions.IsReturnStatement(expressionSyntaxAssertions =>
                                    {
                                        expressionSyntaxAssertions.IsIdentifierName("input");
                                    });
                                });
                        });
                });
    }
}