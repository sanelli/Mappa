// <copyright file="InvokeConstructorMapStrategyWithInterfacesIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests to map from interfaces.
/// </summary>
public sealed class InvokeConstructorMapStrategyWithInterfacesIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test for bug <see href="https://github.com/sanelli/Mappa/issues/159">#153</see>.
    /// Test that Mappa can map from an interface and all the base interfaces.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [Bug("#159")]
    [IntegrationTest]
    public async Task CanMapFromInterfaceAndBaseInterfaces()
    {
        // Arrange
        const string sourceCode = """
                                #nullable enable
                                using Mappa.Attributes;

                                namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                public interface IBase1
                                {
                                    public int PropertyB {get; set;}
                                }
                                
                                public interface IBase2
                                {
                                    public char PropertyC {get; set;}
                                }
                                
                                public interface IBase3
                                    : IBase1, IBase2
                                {
                                    public bool PropertyD {get; set;}
                                }
                                
                                public interface IBase4
                                {
                                    public float PropertyE {get; set;}
                                }
                                
                                public interface ISource
                                    : IBase3, IBase4
                                {
                                   public string PropertyA {get; set;}
                                }
                                
                                public class Target
                                {
                                   public string PropertyA {get; set;}
                                   public int PropertyB {get; set;}
                                   public char PropertyC {get; set;}
                                   public bool PropertyD {get; set;}
                                   public float PropertyE {get; set;}
                                }
                                  
                                [Mappa]
                                public sealed partial class Mapper
                                {
                                    public partial Target Map(ISource input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.ISource",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(7)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string",
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.PropertyA")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "int",
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.PropertyB")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "char",
                                "__mappa_tmp_3",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.PropertyC")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "bool",
                                "__mappa_tmp_4",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.PropertyD")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "float",
                                "__mappa_tmp_5",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.PropertyE")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_6",
                                initializerAssertions =>
                                    initializerAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("PropertyA", parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")),
                                        ("PropertyB", parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyC", parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                        ("PropertyD", parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")),
                                        ("PropertyE", parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_5")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_6")));
                });
    }

    /// <summary>
    /// Test mapping works from an interface.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromInterface()
    {
        // Arrange
        const string sourceCode = """
                                #nullable enable
                                using Mappa.Attributes;

                                namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                
                                public interface ISource
                                {
                                   public string Property {get; set;}
                                }
                                
                                public class Target
                                {
                                   public string Property {get; set;}
                                }
                                  
                                [Mappa]
                                public sealed partial class Mapper
                                {
                                    public partial Target Map(ISource input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.ISource",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string",
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializerAssertions =>
                                    initializerAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }
}