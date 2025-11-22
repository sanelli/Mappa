// <copyright file="TypeMappingStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="TypeMappingStrategy"/>.
/// </summary>
// TODO [#49] Test with interface.
// TODO [#49] Test with nullable.
// TODO [#49] Test with nested classes.
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
public sealed class TypeMappingStrategyIntegrationTests
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
                    #pragma warning disable S125
                    /*
                        Mappa.Generator.Tests.UnitTests.SourceCode.TargetBaseClass __mappa_tmp_1;
                            switch (input)
                            {
                               case Mappa.Generator.Tests.UnitTests.SourceCode.SourceThirdDerivedClass __mappa_tmp_2:
                                  string __mappa_tmp_3 = __mappa_tmp_2.ThirdDerivedClassProperty;
                                  long __mappa_tmp_4 = long.Parse(__mappa_tmp_3);
                                  System.DateTime __mappa_tmp_5 = __mappa_tmp_2.SecondDerivedClassProperty;
                                  System.DateOnly __mappa_tmp_6 = System.DateOnly.FromDateTime(__mappa_tmp_5);
                                  byte __mappa_tmp_7 = __mappa_tmp_2.BaseClassProperty;
                                  Mappa.Generator.Tests.UnitTests.SourceCode.TargetThirdDerivedClass __mappa_tmp_8 = new Mappa.Generator.Tests.UnitTests.SourceCode.TargetThirdDerivedClass()
                                  {
                                  ThirdDerivedClassProperty = __mappa_tmp_4,
                                  SecondDerivedClassProperty = __mappa_tmp_6,
                                  BaseClassProperty = __mappa_tmp_7,
                                  };
                                  __mappa_tmp_1 = __mappa_tmp_8;
                                  break;

                               case Mappa.Generator.Tests.UnitTests.SourceCode.SourceSecondDerivedClass __mappa_tmp_9:
                                  System.DateTime __mappa_tmp_10 = __mappa_tmp_9.SecondDerivedClassProperty;
                                  System.DateOnly __mappa_tmp_11 = System.DateOnly.FromDateTime(__mappa_tmp_10);
                                  byte __mappa_tmp_12 = __mappa_tmp_9.BaseClassProperty;
                                  Mappa.Generator.Tests.UnitTests.SourceCode.TargetSecondDerivedClass __mappa_tmp_13 = new Mappa.Generator.Tests.UnitTests.SourceCode.TargetSecondDerivedClass()
                                  {
                                  SecondDerivedClassProperty = __mappa_tmp_11,
                                  BaseClassProperty = __mappa_tmp_12,
                                  };
                                  __mappa_tmp_1 = __mappa_tmp_13;
                                  break;

                               case Mappa.Generator.Tests.UnitTests.SourceCode.SourceFirstDerivedClass __mappa_tmp_14:
                                  float __mappa_tmp_15 = __mappa_tmp_14.FirstDerivedClassProperty;
                                  string __mappa_tmp_16 = __mappa_tmp_15.ToString();
                                  byte __mappa_tmp_17 = __mappa_tmp_14.BaseClassProperty;
                                  Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirstDerivedClass __mappa_tmp_18 = new Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirstDerivedClass()
                                  {
                                  FirstDerivedClassProperty = __mappa_tmp_16,
                                  BaseClassProperty = __mappa_tmp_17,
                                  };
                                  __mappa_tmp_1 = __mappa_tmp_18;
                                  break;

                               default:
                                  throw new global::System.ArgumentOutOfRangeException(nameof(input));
                            }

                            return __mappa_tmp_1;
                     */

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
                                    .HasNextSyntaxNode(statementAssertions => { /* TODO [#49] Add assertions. */ })
                                    .HasNextSyntaxNode(statementAssertions => { /* TODO [#49] Add assertions. */ })
                                    .HasNextSyntaxNode(statementAssertions => { /* TODO [#49] Add assertions. */ })
                                    .HasNextSyntaxNode(statementAssertions => { /* TODO [#49] Add assertions. */ })
                                    .HasNextSyntaxNode(statementAssertions => { /* TODO [#49] Add assertions. */ })
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
                                    .HasNextSyntaxNode(statementAssertions => { /* TODO [#49] Add assertions. */ })
                                    .HasNextSyntaxNode(statementAssertions => { /* TODO [#49] Add assertions. */ })
                                    .HasNextSyntaxNode(statementAssertions => { /* TODO [#49] Add assertions. */ })
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
                                    .HasNextSyntaxNode(statementAssertions => { /* TODO [#49] Add assertions. */ })
                                    .HasNextSyntaxNode(statementAssertions => { /* TODO [#49] Add assertions. */ })
                                    .HasNextSyntaxNode(statementAssertions => { /* TODO [#49] Add assertions. */ })
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
}