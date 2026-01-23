// <copyright file="PolymorphismMapStrategyIntegrationTests.MapSourceType.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="PolymorphismMapStrategy"/> and
/// <see cref="MappaTypeMappingDefaultAttribute"/> with <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/>.
/// </summary>
public sealed partial class PolymorphismMapStrategyIntegrationTests
{
    /// <summary>
    /// Test mapping works between classes using multiple
    /// <see cref="MappaTypeMappingAttribute"/> and
    /// <see cref="MappaTypeMappingDefaultAttribute"/>:
    /// - <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/>;
    /// - no specific target type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapBetweenClassesUsingPolymorphismAndMapToSourceTypeWithoutExplicitTargetType()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class SourceBaseClass 
                                  {
                                     public byte BaseClassProperty {get; set;}
                                  }
                                  
                                  public class SourceFirstDerivedClass : SourceBaseClass
                                  {
                                     public float FirstDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class SourceSecondDerivedClass : SourceBaseClass
                                  {
                                     public DateTime SecondDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class SourceThirdDerivedClass : SourceSecondDerivedClass
                                  {
                                     public string ThirdDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class TargetBaseClass 
                                  {
                                     public int BaseClassProperty {get; set;}
                                  }
                                  
                                  public class TargetFirstDerivedClass : TargetBaseClass
                                  {
                                     public string FirstDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class TargetSecondDerivedClass : TargetBaseClass
                                  {
                                     public DateOnly SecondDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class TargetThirdDerivedClass : TargetSecondDerivedClass
                                  {
                                     public long ThirdDerivedClassProperty {get; set;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetThirdDerivedClass), typeof(SourceThirdDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetSecondDerivedClass), typeof(SourceSecondDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType)]
                                      public partial TargetBaseClass Map(SourceBaseClass input);
                                  }
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetBaseClass",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.SourceBaseClass",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TargetBaseClass", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeSwitchStatementSyntax(
                            switchExpression => switchExpression.BeIdentifierNameSyntax("input"),
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCasePattern();
                                labelAssertions[0]
                                    .AsCasePattern()
                                    .HasPattern(pattern => pattern.BeDeclarationPatternSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.SourceThirdDerivedClass", "__mappa_tmp_2"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(8)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(string).ToString(),
                                        "__mappa_tmp_3",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.ThirdDerivedClassProperty")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(long).ToString(),
                                        "__mappa_tmp_4",
                                        initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                            "long.Parse",
                                            parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        "System.DateTime",
                                        "__mappa_tmp_5",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.SecondDerivedClassProperty")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        "System.DateOnly",
                                        "__mappa_tmp_6",
                                        initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                            "System.DateOnly.FromDateTime",
                                            parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_5"))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(byte).ToString(),
                                        "__mappa_tmp_7",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.BaseClassProperty")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.TargetThirdDerivedClass",
                                        "__mappa_tmp_8",
                                        initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetThirdDerivedClass",
                                            ("ThirdDerivedClassProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")),
                                            ("SecondDerivedClassProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_6")),
                                            ("BaseClassProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_7")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_8")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            },
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCasePattern();
                                labelAssertions[0]
                                    .AsCasePattern()
                                    .HasPattern(pattern => pattern.BeDeclarationPatternSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.SourceSecondDerivedClass", "__mappa_tmp_9"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(6)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(DateTime).ToString(),
                                        "__mappa_tmp_10",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_9.SecondDerivedClassProperty")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        "System.DateOnly",
                                        "__mappa_tmp_11",
                                        initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                            "System.DateOnly.FromDateTime",
                                            parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_10"))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(byte).ToString(),
                                        "__mappa_tmp_12",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_9.BaseClassProperty")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.TargetSecondDerivedClass",
                                        "__mappa_tmp_13",
                                        initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetSecondDerivedClass",
                                            ("SecondDerivedClassProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_11")),
                                            ("BaseClassProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_12")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_13")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            },
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCasePattern();
                                labelAssertions[0]
                                    .AsCasePattern()
                                    .HasPattern(pattern => pattern.BeDeclarationPatternSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.SourceFirstDerivedClass", "__mappa_tmp_14"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(6)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(float).ToString(),
                                        "__mappa_tmp_15",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_14.FirstDerivedClassProperty")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(string).ToString(),
                                        "__mappa_tmp_16",
                                        initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax("__mappa_tmp_15.ToString")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(byte).ToString(),
                                        "__mappa_tmp_17",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_14.BaseClassProperty")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirstDerivedClass",
                                        "__mappa_tmp_18",
                                        initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirstDerivedClass",
                                            ("FirstDerivedClassProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_16")),
                                            ("BaseClassProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_17")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_18")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            },
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsDefault();

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(4)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(byte).ToString(),
                                        "__mappa_tmp_19",
                                        expressionAssertions => expressionAssertions.BeMemberAccessExpressionSyntax("input.BaseClassProperty")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.TargetBaseClass",
                                        "__mappa_tmp_20",
                                        initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetBaseClass",
                                            ("BaseClassProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_19")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(
                                        leftAssertions => leftAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                        rightAssertions => rightAssertions.BeIdentifierNameSyntax("__mappa_tmp_20")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test mapping works between classes using multiple
    /// <see cref="MappaTypeMappingAttribute"/> and
    /// <see cref="MappaTypeMappingDefaultAttribute"/>:
    /// - <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/>;
    /// - with explicit target type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapBetweenClassesUsingPolymorphismAndMapToSourceTypeWithExplicitTargetType()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class SourceBaseClass 
                                  {
                                     public byte BaseClassProperty {get; set;}
                                  }
                                  
                                  public class SourceFirstDerivedClass : SourceBaseClass
                                  {
                                     public float FirstDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class SourceSecondDerivedClass : SourceBaseClass
                                  {
                                     public DateTime SecondDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class SourceThirdDerivedClass : SourceSecondDerivedClass
                                  {
                                     public string ThirdDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class TargetBaseClass 
                                  {
                                     public int BaseClassProperty {get; set;}
                                  }
                                  
                                  public class TargetFirstDerivedClass : TargetBaseClass
                                  {
                                     public string FirstDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class TargetSecondDerivedClass : TargetBaseClass
                                  {
                                     public DateOnly SecondDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class TargetThirdDerivedClass : TargetSecondDerivedClass
                                  {
                                     public long ThirdDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class AnotherTargetBaseClass : TargetBaseClass
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetThirdDerivedClass), typeof(SourceThirdDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetSecondDerivedClass), typeof(SourceSecondDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType, typeof(AnotherTargetBaseClass))]
                                      public partial TargetBaseClass Map(SourceBaseClass input);
                                  }
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetBaseClass",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.SourceBaseClass",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TargetBaseClass", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeSwitchStatementSyntax(
                            switchExpression => switchExpression.BeIdentifierNameSyntax("input"),
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCasePattern();
                                labelAssertions[0]
                                    .AsCasePattern()
                                    .HasPattern(pattern => pattern.BeDeclarationPatternSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.SourceThirdDerivedClass", "__mappa_tmp_2"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(8)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(string).ToString(),
                                        "__mappa_tmp_3",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.ThirdDerivedClassProperty")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(long).ToString(),
                                        "__mappa_tmp_4",
                                        initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                            "long.Parse",
                                            parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        "System.DateTime",
                                        "__mappa_tmp_5",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.SecondDerivedClassProperty")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        "System.DateOnly",
                                        "__mappa_tmp_6",
                                        initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                            "System.DateOnly.FromDateTime",
                                            parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_5"))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(byte).ToString(),
                                        "__mappa_tmp_7",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.BaseClassProperty")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.TargetThirdDerivedClass",
                                        "__mappa_tmp_8",
                                        initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetThirdDerivedClass",
                                            ("ThirdDerivedClassProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")),
                                            ("SecondDerivedClassProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_6")),
                                            ("BaseClassProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_7")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_8")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            },
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCasePattern();
                                labelAssertions[0]
                                    .AsCasePattern()
                                    .HasPattern(pattern => pattern.BeDeclarationPatternSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.SourceSecondDerivedClass", "__mappa_tmp_9"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(6)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(DateTime).ToString(),
                                        "__mappa_tmp_10",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_9.SecondDerivedClassProperty")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        "System.DateOnly",
                                        "__mappa_tmp_11",
                                        initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                            "System.DateOnly.FromDateTime",
                                            parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_10"))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(byte).ToString(),
                                        "__mappa_tmp_12",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_9.BaseClassProperty")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.TargetSecondDerivedClass",
                                        "__mappa_tmp_13",
                                        initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetSecondDerivedClass",
                                            ("SecondDerivedClassProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_11")),
                                            ("BaseClassProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_12")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_13")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            },
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCasePattern();
                                labelAssertions[0]
                                    .AsCasePattern()
                                    .HasPattern(pattern => pattern.BeDeclarationPatternSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.SourceFirstDerivedClass", "__mappa_tmp_14"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(6)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(float).ToString(),
                                        "__mappa_tmp_15",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_14.FirstDerivedClassProperty")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(string).ToString(),
                                        "__mappa_tmp_16",
                                        initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax("__mappa_tmp_15.ToString")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(byte).ToString(),
                                        "__mappa_tmp_17",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_14.BaseClassProperty")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirstDerivedClass",
                                        "__mappa_tmp_18",
                                        initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirstDerivedClass",
                                            ("FirstDerivedClassProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_16")),
                                            ("BaseClassProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_17")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_18")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            },
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsDefault();

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(4)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(byte).ToString(),
                                        "__mappa_tmp_19",
                                        expressionAssertions => expressionAssertions.BeMemberAccessExpressionSyntax("input.BaseClassProperty")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.AnotherTargetBaseClass",
                                        "__mappa_tmp_20",
                                        initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.AnotherTargetBaseClass",
                                            ("BaseClassProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_19")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(
                                        leftAssertions => leftAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                        rightAssertions => rightAssertions.BeIdentifierNameSyntax("__mappa_tmp_20")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test diagnostic is returned when the MapSourceType target type is the map method
    /// return type and that type is an interface.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticReturnedWhenInvokeMapSourceTypeTargetTheInterfaceTypeReturnedFromTheMapMethod()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBaseClass 
                                  {
                                     public byte BaseClassProperty {get; set;}
                                  }

                                  public class SourceFirstDerivedClass : SourceBaseClass
                                  {
                                     public float FirstDerivedClassProperty {get; set;}
                                  }

                                  public class SourceSecondDerivedClass : SourceBaseClass
                                  {
                                     public DateTime SecondDerivedClassProperty {get; set;}
                                  }

                                  public class SourceThirdDerivedClass : SourceSecondDerivedClass
                                  {
                                     public string ThirdDerivedClassProperty {get; set;}
                                  }

                                  public interface ITargetBaseClass 
                                  {
                                     int BaseClassProperty {get; set;}
                                  }

                                  public class TargetFirstDerivedClass : ITargetBaseClass
                                  {
                                     public string FirstDerivedClassProperty {get; set;}
                                     public int BaseClassProperty {get; set;}
                                  }

                                  public class TargetSecondDerivedClass : ITargetBaseClass
                                  {
                                     public DateOnly SecondDerivedClassProperty {get; set;}
                                     public int BaseClassProperty {get; set;}
                                  }

                                  public class TargetThirdDerivedClass : TargetSecondDerivedClass
                                  {
                                     public long ThirdDerivedClassProperty {get; set;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetThirdDerivedClass), typeof(SourceThirdDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetSecondDerivedClass), typeof(SourceSecondDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType)]
                                      public partial ITargetBaseClass Map(SourceBaseClass input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.TypeMustBeConcrete, "Mappa.Generator.Tests.UnitTests.SourceCode.ITargetBaseClass");
    }

    /// <summary>
    /// Test diagnostic is returned when the MapSourceType target type is a specific type
    /// and that type is an interface.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticReturnedWhenInvokeMapSourceTypeTargetTheInterfaceSpecifiedInTheAttribute()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBaseClass 
                                  {
                                     public byte BaseClassProperty {get; set;}
                                  }

                                  public class SourceFirstDerivedClass : SourceBaseClass
                                  {
                                     public float FirstDerivedClassProperty {get; set;}
                                  }

                                  public class SourceSecondDerivedClass : SourceBaseClass
                                  {
                                     public DateTime SecondDerivedClassProperty {get; set;}
                                  }

                                  public class SourceThirdDerivedClass : SourceSecondDerivedClass
                                  {
                                     public string ThirdDerivedClassProperty {get; set;}
                                  }
                                  
                                  public interface ITargetBaseClass 
                                  {
                                      int BaseClassProperty {get; set;}
                                  }

                                  public interface IAnotherTargetBaseClass : ITargetBaseClass
                                  {
                                  }

                                  public class TargetFirstDerivedClass : ITargetBaseClass
                                  {
                                     public string FirstDerivedClassProperty {get; set;}
                                     public int BaseClassProperty {get; set;}
                                  }

                                  public class TargetSecondDerivedClass : ITargetBaseClass
                                  {
                                     public DateOnly SecondDerivedClassProperty {get; set;}
                                     public int BaseClassProperty {get; set;}
                                  }

                                  public class TargetThirdDerivedClass : TargetSecondDerivedClass
                                  {
                                     public long ThirdDerivedClassProperty {get; set;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetThirdDerivedClass), typeof(SourceThirdDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetSecondDerivedClass), typeof(SourceSecondDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType, typeof(IAnotherTargetBaseClass))]
                                      public partial ITargetBaseClass Map(SourceBaseClass input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, "Mappa.Generator.Tests.UnitTests.SourceCode.SourceBaseClass", "Mappa.Generator.Tests.UnitTests.SourceCode.IAnotherTargetBaseClass");
    }

    /// <summary>
    /// Test diagnostic is returned when the MapSourceType target type is the map method
    /// return type and that type is an abstract class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticReturnedWhenInvokeMapSourceTypeTargetTheAbstractClassTypeReturnedFromTheMapMethod()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBaseClass 
                                  {
                                     public byte BaseClassProperty {get; set;}
                                  }

                                  public class SourceFirstDerivedClass : SourceBaseClass
                                  {
                                     public float FirstDerivedClassProperty {get; set;}
                                  }

                                  public class SourceSecondDerivedClass : SourceBaseClass
                                  {
                                     public DateTime SecondDerivedClassProperty {get; set;}
                                  }

                                  public class SourceThirdDerivedClass : SourceSecondDerivedClass
                                  {
                                     public string ThirdDerivedClassProperty {get; set;}
                                  }

                                  public abstract class TargetBaseClass 
                                  {
                                     public int BaseClassProperty {get; set;}
                                  }

                                  public class TargetFirstDerivedClass : TargetBaseClass
                                  {
                                     public string FirstDerivedClassProperty {get; set;}
                                  }

                                  public class TargetSecondDerivedClass : TargetBaseClass
                                  {
                                     public DateOnly SecondDerivedClassProperty {get; set;}
                                  }

                                  public class TargetThirdDerivedClass : TargetSecondDerivedClass
                                  {
                                     public long ThirdDerivedClassProperty {get; set;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetThirdDerivedClass), typeof(SourceThirdDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetSecondDerivedClass), typeof(SourceSecondDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType)]
                                      public partial TargetBaseClass Map(SourceBaseClass input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.TypeMustBeConcrete, "Mappa.Generator.Tests.UnitTests.SourceCode.TargetBaseClass");
    }

    /// <summary>
    /// Test diagnostic is returned when the MapSourceType target type is a specific type
    /// and that type is an abstract class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticReturnedWhenInvokeMapSourceTypeTargetTheAbstractClassSpecifiedInTheAttribute()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBaseClass 
                                  {
                                     public byte BaseClassProperty {get; set;}
                                  }

                                  public class SourceFirstDerivedClass : SourceBaseClass
                                  {
                                     public float FirstDerivedClassProperty {get; set;}
                                  }

                                  public class SourceSecondDerivedClass : SourceBaseClass
                                  {
                                     public DateTime SecondDerivedClassProperty {get; set;}
                                  }

                                  public class SourceThirdDerivedClass : SourceSecondDerivedClass
                                  {
                                     public string ThirdDerivedClassProperty {get; set;}
                                  }

                                  public class TargetBaseClass
                                  {
                                       public int BaseClassProperty {get; set;}
                                  }
                                  
                                  public abstract class AbstractTargetBaseClass : TargetBaseClass
                                  {
                                  }

                                  public class TargetFirstDerivedClass : TargetBaseClass
                                  {
                                     public string FirstDerivedClassProperty {get; set;}
                                  }

                                  public class TargetSecondDerivedClass : TargetBaseClass
                                  {
                                     public DateOnly SecondDerivedClassProperty {get; set;}
                                  }

                                  public class TargetThirdDerivedClass : TargetSecondDerivedClass
                                  {
                                     public long ThirdDerivedClassProperty {get; set;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetThirdDerivedClass), typeof(SourceThirdDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetSecondDerivedClass), typeof(SourceSecondDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType, typeof(AbstractTargetBaseClass))]
                                      public partial TargetBaseClass Map(SourceBaseClass input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, "Mappa.Generator.Tests.UnitTests.SourceCode.SourceBaseClass", "Mappa.Generator.Tests.UnitTests.SourceCode.AbstractTargetBaseClass");
    }

    /// <summary>
    /// Test diagnostic is returned when explicit map type does not inherit
    /// from map method returned class type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenExplicitMapTypeDoesNotInheritFromMapMethodClassType()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class SourceBaseClass 
                                  {
                                     public byte BaseClassProperty {get; set;}
                                  }
                                  
                                  public class SourceFirstDerivedClass : SourceBaseClass
                                  {
                                     public float FirstDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class SourceSecondDerivedClass : SourceBaseClass
                                  {
                                     public DateTime SecondDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class SourceThirdDerivedClass : SourceSecondDerivedClass
                                  {
                                     public string ThirdDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class TargetBaseClass 
                                  {
                                     public int BaseClassProperty {get; set;}
                                  }
                                  
                                  public class TargetFirstDerivedClass : TargetBaseClass
                                  {
                                     public string FirstDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class TargetSecondDerivedClass : TargetBaseClass
                                  {
                                     public DateOnly SecondDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class TargetThirdDerivedClass : TargetSecondDerivedClass
                                  {
                                     public long ThirdDerivedClassProperty {get; set;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetThirdDerivedClass), typeof(SourceThirdDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetSecondDerivedClass), typeof(SourceSecondDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType, typeof(string)]
                                      public partial TargetBaseClass Map(SourceBaseClass input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ExplicitTargetTypeDoesNotDeriveMapMethodTargetType, "string", "Mappa.Generator.Tests.UnitTests.SourceCode.TargetBaseClass");
    }

    /// <summary>
    /// Test diagnostic is returned when explicit map type does not inherit
    /// from map method returned interface type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenExplicitMapTypeDoesNotInheritFromMapMethodInterfaceType()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class SourceBaseClass 
                                  {
                                     public byte BaseClassProperty {get; set;}
                                  }
                                  
                                  public class SourceFirstDerivedClass : SourceBaseClass
                                  {
                                     public float FirstDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class SourceSecondDerivedClass : SourceBaseClass
                                  {
                                     public DateTime SecondDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class SourceThirdDerivedClass : SourceSecondDerivedClass
                                  {
                                     public string ThirdDerivedClassProperty {get; set;}
                                  }
                                  
                                  public interface ITargetBaseClass 
                                  {
                                  int BaseClassProperty {get; set;}
                                  }
                                  
                                  public class TargetFirstDerivedClass : ITargetBaseClass
                                  {
                                     public string FirstDerivedClassProperty {get; set;}
                                     public int BaseClassProperty {get; set;}
                                  }
                                  
                                  public class TargetSecondDerivedClass : ITargetBaseClass
                                  {
                                     public DateOnly SecondDerivedClassProperty {get; set;}
                                     public int BaseClassProperty {get; set;}
                                  }
                                  
                                  public class TargetThirdDerivedClass : TargetSecondDerivedClass
                                  {
                                     public long ThirdDerivedClassProperty {get; set;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetThirdDerivedClass), typeof(SourceThirdDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetSecondDerivedClass), typeof(SourceSecondDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType, typeof(string)]
                                      public partial ITargetBaseClass Map(SourceBaseClass input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ExplicitTargetTypeDoesNotDeriveMapMethodTargetType, "string", "Mappa.Generator.Tests.UnitTests.SourceCode.ITargetBaseClass");
    }

    /// <summary>
    /// Test diagnostic is returned when <see cref="MappaTypeMappingDefaultAttribute"/>
    /// define a mapping that cannot be satisfied.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenMappaTypeMappingDefaultAttributeDefineAMappingThatCannotBeSatisfied()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class SourceBaseClass 
                                  {
                                     public byte BaseClassProperty {get; set;}
                                  }
                                  
                                  public class SourceFirstDerivedClass : SourceBaseClass
                                  {
                                     public float FirstDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class TargetBaseClass 
                                  {
                                     public int BaseClassProperty {get; set;}
                                  }
                                  
                                  public class TargetFirstDerivedClass : TargetBaseClass
                                  {
                                     public float FirstDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class TargetSecondDerivedClass : TargetBaseClass
                                  {
                                     public TargetSecondDerivedClass(int  baseClassProperty, int anotherProperty)
                                     {
                                        this.BaseClassProperty = baseClassProperty;
                                     }
                                  }
                                  
                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType, typeof(TargetSecondDerivedClass))]
                                      public partial TargetBaseClass Map(SourceBaseClass input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotIdentifyStrategy,
                "Mappa.Generator.Tests.UnitTests.SourceCode.SourceBaseClass",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetSecondDerivedClass");
    }

    /// <summary>
    /// Test diagnostic is returned when trying to mapping default
    /// to an interface.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTryingToMapDefaultToAnInterface()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class SourceBaseClass 
                                  {
                                     public byte BaseClassProperty {get; set;}
                                  }
                                  
                                  public class SourceFirstDerivedClass : SourceBaseClass
                                  {
                                     public float FirstDerivedClassProperty {get; set;}
                                  }
                                  
                                  public interface ITargetBaseClass 
                                  {
                                      int BaseClassProperty {get; set;}
                                  }
                                  
                                  public class TargetFirstDerivedClass : ITargetBaseClass
                                  {
                                     public float FirstDerivedClassProperty {get; set;}
                                     public int BaseClassProperty {get; set;}
                                  }
                                  
                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType)]
                                      public partial ITargetBaseClass Map(SourceBaseClass input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.TypeMustBeConcrete,
                "Mappa.Generator.Tests.UnitTests.SourceCode.ITargetBaseClass");
    }

    /// <summary>
    /// Test diagnostic is returned when trying mapping to default
    /// to an abstract class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTryingToMapDefaultToAnAbstractClass()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class SourceBaseClass 
                                  {
                                     public byte BaseClassProperty {get; set;}
                                  }
                                  
                                  public class SourceFirstDerivedClass : SourceBaseClass
                                  {
                                     public float FirstDerivedClassProperty {get; set;}
                                  }
                                  
                                  public abstract class TargetBaseClass 
                                  {
                                      public int BaseClassProperty {get; set;}
                                  }
                                  
                                  public class TargetFirstDerivedClass : TargetBaseClass
                                  {
                                     public float FirstDerivedClassProperty {get; set;}
                                  }
                                  
                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType)]
                                      public partial TargetBaseClass Map(SourceBaseClass input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.TypeMustBeConcrete,
                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetBaseClass");
    }
}