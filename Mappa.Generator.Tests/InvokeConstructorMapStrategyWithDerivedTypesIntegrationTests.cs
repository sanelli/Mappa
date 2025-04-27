// <copyright file="InvokeConstructorMapStrategyWithDerivedTypesIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Class to test mapping between structured types works
/// on derived classes.
/// </summary>
public sealed class InvokeConstructorMapStrategyWithDerivedTypesIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test for bug <see href="https://github.com/sanelli/Mappa/issues/153">#153</see>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [Bug("#153")]
    [IntegrationTest]
    public async Task CanMapUsingSingleEmptyMappingConstructor()
    {
        // Arrange
        const string sourceCode = """
                                #nullable enable
                                using Mappa.Attributes;

                                namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                public sealed record RequestTrace(string RequestId);
                                public abstract record BaseResponse(RequestTrace RequestTrace);
                                public abstract record BaseRequest(RequestTrace RequestTrace);

                                public sealed record Response(RequestTrace RequestTrace)
                                  : BaseResponse(RequestTrace);
                                public sealed record Request(RequestTrace RequestTrace)
                                  : BaseRequest(RequestTrace);
                                  
                                [Mappa]
                                public sealed partial class Mapper
                                {
                                    public partial Response Map(Request input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Response",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Request",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.RequestTrace",
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.RequestTrace")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Response",
                                "__mappa_tmp_2",
                                initializerAssertions =>
                                    initializerAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Response",
                                        parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Test mapping works between properties from base classes.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingPropertiesFromBaseClasses()
    {
        // Arrange
        const string sourceCode = """
                                #nullable enable
                                using Mappa.Attributes;

                                namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                
                                public class BaseSource
                                {
                                   public string MapBaseToBase {get; set;}
                                   public string MapBaseToDerived {get; set;}
                                }
                                
                                public class Source
                                    : BaseSource
                                {
                                   public string MapDerivedToBase {get; set;}
                                   public string MapDerivedToDerived {get; set;}
                                }

                                public class BaseTarget
                                {
                                   public string MapBaseToBase {get; set;}
                                   public string MapBaseToDerived {get; set;}
                                }
                                
                                public class Target
                                    : BaseTarget
                                {
                                   public string MapDerivedToBase {get; set;}
                                   public string MapDerivedToDerived {get; set;}
                                }
                                  
                                [Mappa]
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
                        .HasSyntaxNodesCount(6)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string",
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.MapDerivedToBase")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string",
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.MapDerivedToDerived")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string",
                                "__mappa_tmp_3",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.MapBaseToBase")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string",
                                "__mappa_tmp_4",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.MapBaseToDerived")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_5",
                                initializerAssertions =>
                                    initializerAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("MapDerivedToBase", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")),
                                        ("MapDerivedToDerived", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("MapBaseToBase", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                        ("MapBaseToDerived", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_5")));
                });
    }

    /// <summary>
    /// Can map using constructor with parameters when source properties
    /// are coming from derived class and base class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingTargetConstructorParameterUsingPropertiesFromBaseClasses()
    {
        // Arrange
        const string sourceCode = """
                                #nullable enable
                                using Mappa.Attributes;

                                namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                
                                public class BaseSource
                                {
                                   public string PropertyA {get; set;}
                                }
                                
                                public class Source
                                    : BaseSource
                                {
                                   public string PropertyB {get; set;}
                                }
                                
                                public class Target
                                {
                                   public Target(string propertyA, string propertyB) { }
                                }
                                  
                                [Mappa]
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
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string",
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.PropertyA")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string",
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.PropertyB")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializerAssertions =>
                                    initializerAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                        secondParameterAssertions => secondParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Can map using overridden properties in both source and target.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromPropertyToPropertyThatAreOverridden()
    {
        // Arrange
        const string sourceCode = """
                                #nullable enable
                                using Mappa.Attributes;

                                namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                
                                public class BaseSource
                                {
                                   public virtual string VirtualOnSourceProperty {get; set;}
                                }
                                
                                public class Source
                                    : BaseSource
                                {
                                   public override string VirtualOnSourceProperty {get; set;}
                                   public string VirtualOnTargetProperty {get; set;}
                                }

                                public class BaseTarget
                                {
                                   public virtual string VirtualOnTargetProperty {get; set;}
                                }
                                
                                public class Target
                                    : BaseTarget
                                {
                                   public string VirtualOnSourceProperty {get; set;}
                                   public override string VirtualOnTargetProperty {get; set;}
                                }
                                  
                                [Mappa]
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
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string",
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.VirtualOnSourceProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string",
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.VirtualOnTargetProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializerAssertions =>
                                    initializerAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("VirtualOnSourceProperty", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")),
                                        ("VirtualOnTargetProperty", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Can map using hidden properties in both source and target.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromPropertyToPropertyThatAreHidden()
    {
        // Arrange
        const string sourceCode = """
                                #nullable enable
                                using Mappa.Attributes;

                                namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                
                                public class BaseSource
                                {
                                   public string PropertyA {get; set;}
                                   public string PropertyB {get; set;}
                                }
                                
                                public class Source
                                    : BaseSource
                                {
                                   public new string PropertyA {get; set;}
                                   public string PropertyB {get; set;}
                                }

                                public class BaseTarget
                                {
                                  public string PropertyA {get; set;}
                                  public string PropertyB {get; set;}
                                }
                                
                                public class Target
                                    : BaseTarget
                                {
                                  public string PropertyA {get; set;}
                                  public new string PropertyB {get; set;}
                                }
                                  
                                [Mappa]
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
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string",
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.PropertyA")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string",
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.PropertyB")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializerAssertions =>
                                    initializerAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }
}