// <copyright file="ProtobufPolymorphismMapStrategyIntegrationTests.ProtoToPoco.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="ProtobufPolymorphismMapStrategy"/> (protobuf to POCO).
/// </summary>
public sealed partial class ProtobufPolymorphismMapStrategyIntegrationTests
{
    /// <summary>
    /// Maps a protobuf message to a POCO hierarchy using the oneof case of the source:
    /// each <see cref="MappaProtobufTypeMappingAttribute"/> arm becomes a <c>case</c> and the
    /// missing <see cref="MappaProtobufTypeMappingDefaultAttribute"/> defaults to throwing.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapProtoOneofToPocoHierarchyWithDefaultThrowBehavior()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      [MappaProtobufTypeMapping(typeof(ErrorDto), "Error")]
                                                      public partial ResultDto Map(ResultMessage input);
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
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(ResultDtoType, "__mappa_tmp_1"))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeSwitchStatementSyntax(
                            switchExpression => switchExpression.BeMemberAccessExpressionSyntax("input.ResultCase"),
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCase();
                                labelAssertions[0].AsCase().HasValue(valueAssertions => valueAssertions.BeMemberAccessExpressionSyntax(ResultOneofCaseType + ".Ok"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(6)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(OkMessageType, "__mappa_tmp_2", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Ok")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_3", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Value")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_4", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Description")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                            OkDtoType,
                                            "__mappa_tmp_5",
                                            expressionAssertions => expressionAssertions.BeObjectCreationExpressionSyntax(
                                                OkDtoType,
                                                ("Value", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                                ("Description", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_5")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            },
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCase();
                                labelAssertions[0].AsCase().HasValue(valueAssertions => valueAssertions.BeMemberAccessExpressionSyntax(ResultOneofCaseType + ".Error"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(6)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(ErrorMessageType, "__mappa_tmp_6", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Error")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(IntType, "__mappa_tmp_7", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_6.Code")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_8", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_6.Description")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                            ErrorDtoType,
                                            "__mappa_tmp_9",
                                            expressionAssertions => expressionAssertions.BeObjectCreationExpressionSyntax(
                                                ErrorDtoType,
                                                ("Code", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_7")),
                                                ("Description", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_8")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_9")))
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
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions
                                            .BeThrowStatementSyntax<ArgumentOutOfRangeException>(exceptionParameterAssertions =>
                                                exceptionParameterAssertions.BeNameOf(parameterAssertions =>
                                                    parameterAssertions.BeIdentifierNameSyntax("input"))));
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Maps a protobuf message to a POCO hierarchy and throws a user defined exception
    /// when the source oneof case does not match any arm.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapProtoOneofToPocoHierarchyWithThrowBehaviorAndCustomException()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      [MappaProtobufTypeMapping(typeof(ErrorDto), "Error")]
                                                      [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.Throw, typeof(CustomException))]
                                                      public partial ResultDto Map(ResultMessage input);
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
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(ResultDtoType, "__mappa_tmp_1"))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeSwitchStatementSyntax(
                            switchExpression => switchExpression.BeMemberAccessExpressionSyntax("input.ResultCase"),
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCase();
                                labelAssertions[0].AsCase().HasValue(valueAssertions => valueAssertions.BeMemberAccessExpressionSyntax(ResultOneofCaseType + ".Ok"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(6)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(OkMessageType, "__mappa_tmp_2", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Ok")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_3", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Value")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_4", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Description")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                            OkDtoType,
                                            "__mappa_tmp_5",
                                            expressionAssertions => expressionAssertions.BeObjectCreationExpressionSyntax(
                                                OkDtoType,
                                                ("Value", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                                ("Description", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_5")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            },
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCase();
                                labelAssertions[0].AsCase().HasValue(valueAssertions => valueAssertions.BeMemberAccessExpressionSyntax(ResultOneofCaseType + ".Error"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(6)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(ErrorMessageType, "__mappa_tmp_6", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Error")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(IntType, "__mappa_tmp_7", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_6.Code")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_8", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_6.Description")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                            ErrorDtoType,
                                            "__mappa_tmp_9",
                                            expressionAssertions => expressionAssertions.BeObjectCreationExpressionSyntax(
                                                ErrorDtoType,
                                                ("Code", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_7")),
                                                ("Description", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_8")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_9")))
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
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions
                                            .BeThrowStatementSyntax(CustomExceptionType));
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// When only <see cref="MappaProtobufPolymorphismSetupAttribute"/> is defined the generated
    /// switch contains only the default branch (which throws).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapProtoOneofToPocoWhenOnlySetupIsDefined()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      public partial ResultDto Map(ResultMessage input);
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
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(ResultDtoType, "__mappa_tmp_1"))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeSwitchStatementSyntax(
                            switchExpression => switchExpression.BeMemberAccessExpressionSyntax("input.ResultCase"),
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsDefault();

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions
                                            .BeThrowStatementSyntax<ArgumentOutOfRangeException>(exceptionParameterAssertions =>
                                                exceptionParameterAssertions.BeNameOf(parameterAssertions =>
                                                    parameterAssertions.BeIdentifierNameSyntax("input"))));
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// A nullable protobuf source and a nullable POCO target are handled by the nullable strategy
    /// which wraps the oneof switch in a null check.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNullableProtoOneofToNullablePocoHierarchy()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      public partial ResultDto? Map(ResultMessage? input);
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
                ResultDtoType + "?",
                NullableAnnotation.Annotated,
                ResultMessageType + "?",
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(ResultDtoType + "?", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeIfStatementSyntax(
                            conditionAssertions => conditionAssertions.BeIsPatternExpressionSyntax(
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                patternAssertions => patternAssertions.BeUnaryPatternSyntax(SyntaxKind.NotKeyword, argumentAssertions => argumentAssertions.BeConstantPatternSyntax(null))),
                            thenStatementAssertions =>
                            {
                                thenStatementAssertions
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(4)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                        ResultMessageType,
                                        "__mappa_tmp_2",
                                        initializationAssertions => initializationAssertions.BeIdentifierNameSyntax("input")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(ResultDtoType, "__mappa_tmp_3"))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeSwitchStatementSyntax(
                                        switchExpression => switchExpression.BeMemberAccessExpressionSyntax("__mappa_tmp_2.ResultCase"),
                                        (labelAssertions, caseBodyAssertions) =>
                                        {
                                            labelAssertions.Should().HaveCount(1);
                                            labelAssertions[0].IsCase();
                                            labelAssertions[0].AsCase().HasValue(valueAssertions => valueAssertions.BeMemberAccessExpressionSyntax(ResultOneofCaseType + ".Ok"));

                                            caseBodyAssertions.Should().HaveCount(1);
                                            caseBodyAssertions[0].BeBlockStatement();
                                            caseBodyAssertions[0]
                                                .AsBlock()
                                                .HasSyntaxNodesCount(6)
                                                .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(OkMessageType, "__mappa_tmp_4", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Ok")))
                                                .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_5", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_4.Value")))
                                                .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_6", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_4.Description")))
                                                .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                                        OkDtoType,
                                                        "__mappa_tmp_7",
                                                        expressionAssertions => expressionAssertions.BeObjectCreationExpressionSyntax(
                                                            OkDtoType,
                                                            ("Value", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_5")),
                                                            ("Description", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_6")))))
                                                .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement("__mappa_tmp_3", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_7")))
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
                                                .HasSyntaxNodesCount(1)
                                                .HasNextSyntaxNode(statementAssertions => statementAssertions
                                                        .BeThrowStatementSyntax<ArgumentOutOfRangeException>(exceptionParameterAssertions =>
                                                            exceptionParameterAssertions.BeNameOf(parameterAssertions =>
                                                                parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                                        }))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(
                                        left => left.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                        right => right.BeIdentifierNameSyntax("__mappa_tmp_3")));
                            },
                            elseStatementAssertions =>
                            {
                                elseStatementAssertions
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(
                                        left => left.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                        right => right.BeCastExpressionSyntax(
                                            ResultDtoType + "?",
                                            expressionAssertions => expressionAssertions.BeLiteralExpressionSyntax(null))));
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_1"));
                });
    }
}