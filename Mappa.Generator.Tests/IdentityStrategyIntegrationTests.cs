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
}