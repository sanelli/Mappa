// <copyright file="PolymorphismMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="PolymorphismMapStrategy"/>.
/// </summary>
// TODO [#49] Test with interface.
// TODO [#49] Test with nested classes.
// TODO [#49] Test identity detector is bypassed when polymorphysm can instead be applied.
// TODO [#49] Test with explicit throw behaviour without class.
// TODO [#49] Test with explicit throw behaviour with exception class.
// TODO [#49] Test with explicit map to behaviour without type.
// TODO [#49] Test with explicit map to behaviour failing because target is interface.
// TODO [#49] Test with explicit map to behaviour failing because target is virtual.
// TODO [#49] Test with explicit map to behaviour with specific type.
// TODO [#49] Test with explicit map to behaviour with null.
// TODO [#49] Test with explicit map to behaviour with default.
// TODO [#49] Test with invoke method to behaviour with static method in mapper with single parameter.
// TODO [#49] Test with invoke method to behaviour with non-static method in mapper.
// TODO [#49] Test with invoke method to behaviour with static method in a different class mapper.
// TODO [#49] Test with invoke method to behaviour with static method in mapper with context parameter.
// TODO [#49] Test with invoke method to behaviour with static method defined in mapper base class.
public sealed class PolymorphismMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test mapping works between classes using multiple
    /// <see cref="MappaTypeMappingAttribute"/> and no
    /// <see cref="MappaTypeMappingDefaultAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithMultipleClassesSubTypeMappingAttributes()
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
    /// <see cref="MappaTypeMappingAttribute"/> and no
    /// <see cref="MappaTypeMappingDefaultAttribute"/> and the input types
    /// are nullable.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapByApplyingNullabilityFirst()
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
                                      public partial TargetBaseClass? Map(SourceBaseClass? input);
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
                NullableAnnotation.Annotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.SourceBaseClass",
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
#pragma warning disable S125
                    /*
                          Mappa.Generator.Tests.UnitTests.SourceCode.TargetBaseClass? __mappa_tmp_1;
                          if (input is not null)
                          {
                             Mappa.Generator.Tests.UnitTests.SourceCode.SourceBaseClass __mappa_tmp_2 = input;

                             Mappa.Generator.Tests.UnitTests.SourceCode.TargetBaseClass? __mappa_tmp_3;
                             switch (__mappa_tmp_2)
                             {
                                case Mappa.Generator.Tests.UnitTests.SourceCode.SourceThirdDerivedClass __mappa_tmp_4:
                                {
                                   string __mappa_tmp_5 = __mappa_tmp_4.ThirdDerivedClassProperty;
                                   long __mappa_tmp_6 = long.Parse(__mappa_tmp_5);
                                   System.DateTime __mappa_tmp_7 = __mappa_tmp_4.SecondDerivedClassProperty;
                                   System.DateOnly __mappa_tmp_8 = System.DateOnly.FromDateTime(__mappa_tmp_7);
                                   byte __mappa_tmp_9 = __mappa_tmp_4.BaseClassProperty;
                                   Mappa.Generator.Tests.UnitTests.SourceCode.TargetThirdDerivedClass __mappa_tmp_10 = new Mappa.Generator.Tests.UnitTests.SourceCode.TargetThirdDerivedClass()
                                   {
                                   ThirdDerivedClassProperty = __mappa_tmp_6,
                                   SecondDerivedClassProperty = __mappa_tmp_8,
                                   BaseClassProperty = __mappa_tmp_9,
                                   };
                                   __mappa_tmp_3 = __mappa_tmp_10;
                                   break;
                                }

                                case Mappa.Generator.Tests.UnitTests.SourceCode.SourceSecondDerivedClass __mappa_tmp_11:
                                {
                                   System.DateTime __mappa_tmp_12 = __mappa_tmp_11.SecondDerivedClassProperty;
                                   System.DateOnly __mappa_tmp_13 = System.DateOnly.FromDateTime(__mappa_tmp_12);
                                   byte __mappa_tmp_14 = __mappa_tmp_11.BaseClassProperty;
                                   Mappa.Generator.Tests.UnitTests.SourceCode.TargetSecondDerivedClass __mappa_tmp_15 = new Mappa.Generator.Tests.UnitTests.SourceCode.TargetSecondDerivedClass()
                                   {
                                   SecondDerivedClassProperty = __mappa_tmp_13,
                                   BaseClassProperty = __mappa_tmp_14,
                                   };
                                   __mappa_tmp_3 = __mappa_tmp_15;
                                   break;
                                }

                                case Mappa.Generator.Tests.UnitTests.SourceCode.SourceFirstDerivedClass __mappa_tmp_16:
                                {
                                   float __mappa_tmp_17 = __mappa_tmp_16.FirstDerivedClassProperty;
                                   string __mappa_tmp_18 = __mappa_tmp_17.ToString();
                                   byte __mappa_tmp_19 = __mappa_tmp_16.BaseClassProperty;
                                   Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirstDerivedClass __mappa_tmp_20 = new Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirstDerivedClass()
                                   {
                                   FirstDerivedClassProperty = __mappa_tmp_18,
                                   BaseClassProperty = __mappa_tmp_19,
                                   };
                                   __mappa_tmp_3 = __mappa_tmp_20;
                                   break;
                                }

                                default:
                                {
                                   throw new global::System.ArgumentOutOfRangeException(nameof(__mappa_tmp_2));
                                }
                             }
                             __mappa_tmp_1 = __mappa_tmp_3;
                          }
                          else
                          {
                             __mappa_tmp_1 = (Mappa.Generator.Tests.UnitTests.SourceCode.TargetBaseClass?) null;
                          }

                          return __mappa_tmp_1;
                     */
#pragma warning restore S125

                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetBaseClass?",
                            "__mappa_tmp_1"))
                        .HasNextSyntaxNode(statementAssertions => statementAssertions.BeIfStatementSyntax(
                            conditionAssertions => conditionAssertions.BeIsPatternExpressionSyntax(
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                patternAssertions => patternAssertions.BeUnaryPatternSyntax(SyntaxKind.NotKeyword, argumentAssertions => argumentAssertions.BeConstantPatternSyntax(null))),
                            themStatementAssertions => { /* TODO [#49] Add assertions. */ },
                            elseStatementAssertions =>
                            {
                                elseStatementAssertions
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(elseStatement =>
                                    {
                                        elseStatement.BeAssignmentExpressionStatement(
                                            leftAssertions => leftAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                            rightAssertions =>
                                                rightAssertions.BeCastExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TargetBaseClass?", expressionAssertions => expressionAssertions.BeLiteralExpressionSyntax(null)));
                                    });
                            }))
                        .HasNextSyntaxNode(statementAssertions => statementAssertions.BeReturnStatement(expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }
}