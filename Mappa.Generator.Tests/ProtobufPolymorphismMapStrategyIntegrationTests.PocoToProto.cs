// <copyright file="ProtobufPolymorphismMapStrategyIntegrationTests.PocoToProto.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="ProtobufPolymorphismMapStrategy"/> (POCO to protobuf).
/// </summary>
public sealed partial class ProtobufPolymorphismMapStrategyIntegrationTests
{
    /// <summary>
    /// Maps a POCO hierarchy to a protobuf message: the target is created with its empty constructor,
    /// each arm assigns the oneof property and <see cref="MappaProtobufTypeMappingDefaultBehavior.NoMap"/>
    /// leaves the oneof unset.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapPocoHierarchyToProtoOneofWithNoMapBehavior()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase")]
                                                      [MappaProtobufTypeMapping("Ok", typeof(OkDto))]
                                                      [MappaProtobufTypeMapping("Error", typeof(ErrorDto))]
                                                      [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.NoMap)]
                                                      public partial EnvelopeMessage Map(ResultDto input);
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
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                EnvelopeMessageType,
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(EnvelopeMessageType)))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeSwitchStatementSyntax(
                            switchExpression => switchExpression.BeIdentifierNameSyntax("input"),
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCasePattern();
                                labelAssertions[0]
                                    .AsCasePattern()
                                    .HasPattern(pattern => pattern.BeDeclarationPatternSyntax(OkDtoType, "__mappa_tmp_2"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(5)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_3", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Value")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_4", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Description")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                            OkReplyMessageType,
                                            "__mappa_tmp_5",
                                            expressionAssertions => expressionAssertions.BeObjectCreationExpressionSyntax(
                                                OkReplyMessageType,
                                                ("Value", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                                ("Description", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Ok"), right => right.BeIdentifierNameSyntax("__mappa_tmp_5")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            },
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCasePattern();
                                labelAssertions[0]
                                    .AsCasePattern()
                                    .HasPattern(pattern => pattern.BeDeclarationPatternSyntax(ErrorDtoType, "__mappa_tmp_6"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(5)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(IntType, "__mappa_tmp_7", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_6.Code")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_8", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_6.Description")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                            ErrorReplyMessageType,
                                            "__mappa_tmp_9",
                                            expressionAssertions => expressionAssertions.BeObjectCreationExpressionSyntax(
                                                ErrorReplyMessageType,
                                                ("Code", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_7")),
                                                ("Description", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_8")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Error"), right => right.BeIdentifierNameSyntax("__mappa_tmp_9")))
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
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Maps a POCO hierarchy to a protobuf message and throws when the source type does not match
    /// any arm because no <see cref="MappaProtobufTypeMappingDefaultAttribute"/> is defined.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapPocoHierarchyToProtoOneofWithDefaultThrowBehavior()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase")]
                                                      [MappaProtobufTypeMapping("Ok", typeof(OkDto))]
                                                      [MappaProtobufTypeMapping("Error", typeof(ErrorDto))]
                                                      public partial EnvelopeMessage Map(ResultDto input);
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
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                EnvelopeMessageType,
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(EnvelopeMessageType)))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeSwitchStatementSyntax(
                            switchExpression => switchExpression.BeIdentifierNameSyntax("input"),
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCasePattern();
                                labelAssertions[0]
                                    .AsCasePattern()
                                    .HasPattern(pattern => pattern.BeDeclarationPatternSyntax(OkDtoType, "__mappa_tmp_2"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(5)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_3", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Value")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_4", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Description")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                            OkReplyMessageType,
                                            "__mappa_tmp_5",
                                            expressionAssertions => expressionAssertions.BeObjectCreationExpressionSyntax(
                                                OkReplyMessageType,
                                                ("Value", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                                ("Description", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Ok"), right => right.BeIdentifierNameSyntax("__mappa_tmp_5")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            },
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCasePattern();
                                labelAssertions[0]
                                    .AsCasePattern()
                                    .HasPattern(pattern => pattern.BeDeclarationPatternSyntax(ErrorDtoType, "__mappa_tmp_6"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(5)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(IntType, "__mappa_tmp_7", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_6.Code")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_8", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_6.Description")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                            ErrorReplyMessageType,
                                            "__mappa_tmp_9",
                                            expressionAssertions => expressionAssertions.BeObjectCreationExpressionSyntax(
                                                ErrorReplyMessageType,
                                                ("Code", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_7")),
                                                ("Description", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_8")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Error"), right => right.BeIdentifierNameSyntax("__mappa_tmp_9")))
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
    /// Maps a POCO hierarchy to a protobuf message and maps unmatched source values into the
    /// message of the arm selected by <see cref="MappaProtobufTypeMappingDefaultBehavior.MapTo"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapPocoHierarchyToProtoOneofWithMapToBehavior()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase")]
                                                      [MappaProtobufTypeMapping("Ok", typeof(OkDto))]
                                                      [MappaProtobufTypeMapping("Error", typeof(ErrorDto))]
                                                      [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.MapTo, "Fallback")]
                                                      public partial EnvelopeMessage Map(ResultDto input);
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
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                EnvelopeMessageType,
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(EnvelopeMessageType)))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeSwitchStatementSyntax(
                            switchExpression => switchExpression.BeIdentifierNameSyntax("input"),
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCasePattern();
                                labelAssertions[0]
                                    .AsCasePattern()
                                    .HasPattern(pattern => pattern.BeDeclarationPatternSyntax(OkDtoType, "__mappa_tmp_2"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(5)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_3", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Value")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_4", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Description")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                            OkReplyMessageType,
                                            "__mappa_tmp_5",
                                            expressionAssertions => expressionAssertions.BeObjectCreationExpressionSyntax(
                                                OkReplyMessageType,
                                                ("Value", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                                ("Description", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Ok"), right => right.BeIdentifierNameSyntax("__mappa_tmp_5")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            },
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCasePattern();
                                labelAssertions[0]
                                    .AsCasePattern()
                                    .HasPattern(pattern => pattern.BeDeclarationPatternSyntax(ErrorDtoType, "__mappa_tmp_6"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(5)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(IntType, "__mappa_tmp_7", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_6.Code")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_8", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_6.Description")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                            ErrorReplyMessageType,
                                            "__mappa_tmp_9",
                                            expressionAssertions => expressionAssertions.BeObjectCreationExpressionSyntax(
                                                ErrorReplyMessageType,
                                                ("Code", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_7")),
                                                ("Description", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_8")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Error"), right => right.BeIdentifierNameSyntax("__mappa_tmp_9")))
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
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_10", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Description")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                            FallbackMessageType,
                                            "__mappa_tmp_11",
                                            expressionAssertions => expressionAssertions.BeObjectCreationExpressionSyntax(
                                                FallbackMessageType,
                                                ("Description", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_10")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Fallback"), right => right.BeIdentifierNameSyntax("__mappa_tmp_11")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_1"));
                });
    }
}