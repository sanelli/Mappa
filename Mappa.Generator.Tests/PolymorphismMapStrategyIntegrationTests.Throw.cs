// <copyright file="PolymorphismMapStrategyIntegrationTests.Throw.cs" company="Stefano Anelli">
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
    /// <see cref="MappaTypeMappingDefaultAttribute"/>
    /// with value <see cref="MappaTypeMappingDefaultBehavior.Throw"/>
    /// without explicit exception.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapBetweenClassesUsingPolymorphismAndThrowBehaviorWithoutExplicitException()
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
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.Throw)]
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
                                caseBodyAssertions[0].AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statementAssertions =>
                                        statementAssertions
                                            .BeThrowStatementSyntax<ArgumentOutOfRangeException>(exceptionParameterAssertions =>
                                                exceptionParameterAssertions.BeNameOf(paramAssertions =>
                                                    paramAssertions.BeIdentifierNameSyntax("input"))));
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test mapping works between classes using multiple
    /// <see cref="MappaTypeMappingAttribute"/> and
    /// <see cref="MappaTypeMappingDefaultAttribute"/>
    /// with value <see cref="MappaTypeMappingDefaultBehavior.Throw"/>
    /// with explicit exception with empty constructor only.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapBetweenClassesUsingPolymorphismAndThrowBehaviorWithExplicitExceptionWithEmptyConstructorOnly()
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
                                  
                                  public class CustomException : System.Exception
                                  {
                                     public CustomException() { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetThirdDerivedClass), typeof(SourceThirdDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetSecondDerivedClass), typeof(SourceSecondDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.Throw, typeof(CustomException)]
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
                                caseBodyAssertions[0].AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions
                                        .BeThrowStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.CustomException"));
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test mapping works between classes using multiple
    /// <see cref="MappaTypeMappingAttribute"/> and
    /// <see cref="MappaTypeMappingDefaultAttribute"/>
    /// with value <see cref="MappaTypeMappingDefaultBehavior.Throw"/>
    /// with explicit exception with string constructor only.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapBetweenClassesUsingPolymorphismAndThrowBehaviorWithExplicitExceptionWithStringConstructorOnly()
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
                                  
                                  public class CustomException : System.Exception
                                  {
                                     public CustomException(string message) : base(message) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetThirdDerivedClass), typeof(SourceThirdDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetSecondDerivedClass), typeof(SourceSecondDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.Throw, typeof(CustomException)]
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
                                caseBodyAssertions[0].AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions
                                        .BeThrowStatementSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.CustomException",
                                            paramAssertions => paramAssertions.BeNameOf(expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"))));
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test mapping works between classes using multiple
    /// <see cref="MappaTypeMappingAttribute"/> and
    /// <see cref="MappaTypeMappingDefaultAttribute"/>
    /// with value <see cref="MappaTypeMappingDefaultBehavior.Throw"/>
    /// with explicit exception with both empty constructor
    /// and string constructor.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapBetweenClassesUsingPolymorphismAndThrowBehaviorWithExplicitExceptionWithBothStringConstructorAndEmptyConstructor()
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
                                  
                                  public class CustomException : System.Exception
                                  {
                                     public CustomException() { }
                                     public CustomException(string message) : base(message) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetThirdDerivedClass), typeof(SourceThirdDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetSecondDerivedClass), typeof(SourceSecondDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.Throw, typeof(CustomException)]
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
                                caseBodyAssertions[0].AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions
                                        .BeThrowStatementSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.CustomException",
                                            paramAssertions => paramAssertions.BeNameOf(expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"))));
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test a diagnostic is returned when the type used by
    /// <see cref="MappaTypeMappingDefaultAttribute"/>
    /// with value <see cref="MappaTypeMappingDefaultBehavior.Throw"/>
    /// is not an exception.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenThExceptionTypeForThrowIsNotAnException()
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
                                  
                                  public class CustomException
                                  {
                                     public CustomException() { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetThirdDerivedClass), typeof(SourceThirdDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetSecondDerivedClass), typeof(SourceSecondDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.Throw, typeof(CustomException)]
                                      public partial TargetBaseClass Map(SourceBaseClass input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.TypeMustBeAnException, "Mappa.Generator.Tests.UnitTests.SourceCode.CustomException");
    }

    /// <summary>
    /// Test a diagnostic is returned when the type used by
    /// <see cref="MappaTypeMappingDefaultAttribute"/>
    /// with value <see cref="MappaTypeMappingDefaultBehavior.Throw"/>
    /// is defined as <c>abstract</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenThExceptionTypeForThrowIsVirtual()
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
                                  
                                  public abstract class CustomException : Exception
                                  {
                                     public CustomException() { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetThirdDerivedClass), typeof(SourceThirdDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetSecondDerivedClass), typeof(SourceSecondDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.Throw, typeof(CustomException)]
                                      public partial TargetBaseClass Map(SourceBaseClass input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.TypeMustBeConcrete, "Mappa.Generator.Tests.UnitTests.SourceCode.CustomException");
    }

    /// <summary>
    /// Test a diagnostic is returned when the type used by
    /// <see cref="MappaTypeMappingDefaultAttribute"/>
    /// with value <see cref="MappaTypeMappingDefaultBehavior.Throw"/>
    /// does not have a constructor with no parameters or a constructor
    /// with only one string parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenThExceptionDoesNotHaveEitherAnEmptyConstructorOrAConstructorWithOneStringParameter()
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
                                  
                                  public class CustomException : Exception
                                  {
                                     public CustomException(string a, string b) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetThirdDerivedClass), typeof(SourceThirdDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetSecondDerivedClass), typeof(SourceSecondDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.Throw, typeof(CustomException)]
                                      public partial TargetBaseClass Map(SourceBaseClass input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.TypeMustHaveAConstructorWithNoParametersOrAConstructorWithOneStringParameter, "Mappa.Generator.Tests.UnitTests.SourceCode.CustomException");
    }

    /// <summary>
    /// Test a diagnostic is returned when the type used by
    /// <see cref="MappaTypeMappingDefaultAttribute"/>
    /// with value <see cref="MappaTypeMappingDefaultBehavior.Throw"/>
    /// has only a constructor with one parameter but that parameter is
    /// not of type <see cref="string"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenThExceptionHasAConstructorWithOneParameterOfTypeThatIsNotString()
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
                                  
                                  public class CustomException : Exception
                                  {
                                     public CustomException(int x) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetThirdDerivedClass), typeof(SourceThirdDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetSecondDerivedClass), typeof(SourceSecondDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFirstDerivedClass), typeof(SourceFirstDerivedClass))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.Throw, typeof(CustomException)]
                                      public partial TargetBaseClass Map(SourceBaseClass input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.TypeMustHaveAConstructorWithNoParametersOrAConstructorWithOneStringParameter, "Mappa.Generator.Tests.UnitTests.SourceCode.CustomException");
    }
}