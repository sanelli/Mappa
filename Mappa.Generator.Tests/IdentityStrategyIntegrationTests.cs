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
            .HaveGeneratedOneSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        var methodDeclarationSyntaxAssertions = namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveGeneratedCodeAttribute()
            .HaveMethods(1)
            .HaveMethod(
                typeof(string),
                NullableAnnotation.None,
                "Map",
                (typeof(string), NullableAnnotation.None, "input"));
        var blockSyntaxAssertions = methodDeclarationSyntaxAssertions
            .HaveGeneratedCodeAttribute()
            .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
            .HaveBody();
        blockSyntaxAssertions
            .HaveSingleReturnStatementWithIdentifierExpression("input");
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
            .HaveGeneratedOneSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        var methodDeclarationSyntaxAssertions = namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveGeneratedCodeAttribute()
            .HaveMethods(1)
            .HaveMethod(
                typeof(string),
                NullableAnnotation.NotAnnotated,
                "Map",
                (typeof(string), NullableAnnotation.NotAnnotated, "input"));
        var blockSyntaxAssertions = methodDeclarationSyntaxAssertions
            .HaveGeneratedCodeAttribute()
            .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
            .HaveBody();
        blockSyntaxAssertions
            .HaveSingleReturnStatementWithIdentifierExpression("input");
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
            .HaveGeneratedOneSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        var methodDeclarationSyntaxAssertions = namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveGeneratedCodeAttribute()
            .HaveMethods(1)
            .HaveMethod(
                typeof(string),
                NullableAnnotation.Annotated,
                "Map",
                (typeof(string), NullableAnnotation.Annotated, "input"));
        var blockSyntaxAssertions = methodDeclarationSyntaxAssertions
            .HaveGeneratedCodeAttribute()
            .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
            .HaveBody();
        blockSyntaxAssertions
            .HaveSingleReturnStatementWithIdentifierExpression("input");
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
            .HaveGeneratedOneSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        var methodDeclarationSyntaxAssertions = namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveGeneratedCodeAttribute()
            .HaveMethods(1)
            .HaveMethod(
                typeof(int),
                NullableAnnotation.NotAnnotated,
                "Map",
                (typeof(int), NullableAnnotation.NotAnnotated, "input"));
        var blockSyntaxAssertions = methodDeclarationSyntaxAssertions
            .HaveGeneratedCodeAttribute()
            .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
            .HaveBody();
        blockSyntaxAssertions
            .HaveSingleReturnStatementWithIdentifierExpression("input");
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
            .HaveGeneratedOneSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        var methodDeclarationSyntaxAssertions = namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveGeneratedCodeAttribute()
            .HaveMethods(1)
            .HaveMethod(
                typeof(object),
                NullableAnnotation.None,
                "Map",
                (typeof(string), NullableAnnotation.None, "input"));
        var blockSyntaxAssertions = methodDeclarationSyntaxAssertions
            .HaveGeneratedCodeAttribute()
            .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
            .HaveBody();
        blockSyntaxAssertions
            .HaveSingleReturnStatementWithIdentifierExpression("input");
    }

    /// <summary>
    /// Test a mapping can be created from reference type
    /// to same nullable <see cref="object"/> when nullable is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToNullableReferenceTypeWhenNullableEnabled()
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
            .HaveGeneratedOneSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        var methodDeclarationSyntaxAssertions = namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveGeneratedCodeAttribute()
            .HaveMethods(1)
            .HaveMethod(
                typeof(object),
                NullableAnnotation.Annotated,
                "Map",
                (typeof(string), NullableAnnotation.NotAnnotated, "input"));
        var blockSyntaxAssertions = methodDeclarationSyntaxAssertions
            .HaveGeneratedCodeAttribute()
            .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
            .HaveBody();
        blockSyntaxAssertions
            .HaveSingleReturnStatementWithIdentifierExpression("input");
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
            .HaveGeneratedOneSourceCode()
            .WithCompilationUnit();
        var namespaceDeclarationSyntaxAssertions = compilationUnitSyntaxAssertions
            .HaveCommentHeader()
            .HaveFileScopedNamespace()
            .HaveNamespaceIdentifier("Mappa.Generator.Tests.UnitTests.SourceCode")
            .HaveClasses(1);
        var methodDeclarationSyntaxAssertions = namespaceDeclarationSyntaxAssertions
            .HaveClass("Mapper")
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveGeneratedCodeAttribute()
            .HaveMethods(1)
            .HaveMethod(
                typeof(string),
                NullableAnnotation.Annotated,
                "Map",
                (typeof(string), NullableAnnotation.NotAnnotated, "input"));
        var blockSyntaxAssertions = methodDeclarationSyntaxAssertions
            .HaveGeneratedCodeAttribute()
            .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
            .HaveBody();
        blockSyntaxAssertions
            .HaveSingleReturnStatementWithIdentifierExpression("input");
    }
}