// <copyright file="BsonWellKnownTypesMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

using Microsoft.CodeAnalysis;

using MongoDB.Bson;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for BSON well-known type mapping strategies.
/// </summary>
public sealed class BsonWellKnownTypesMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string ObjectIdTypeName = "MongoDB.Bson.ObjectId";
    private const string Decimal128TypeName = "MongoDB.Bson.Decimal128";
    private const string ObjectIdParseMember = "global::MongoDB.Bson.ObjectId.Parse";
    private const string Decimal128ToDecimalMember = "global::MongoDB.Bson.Decimal128.ToDecimal";

    /// <summary>
    /// Maps root <see cref="string"/> to <see cref="ObjectId"/> via <c>ObjectId.Parse</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringToObjectIdWhenBsonWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(BsonWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial MongoDB.Bson.ObjectId Map(string input);
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
                ObjectIdTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                ObjectIdTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                    ObjectIdParseMember,
                                    parameter => parameter.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="ObjectId"/> to <see cref="string"/> via <c>ToString</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapObjectIdToStringWhenBsonWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(BsonWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial string Map(MongoDB.Bson.ObjectId input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                ObjectIdTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                    "input.ToString")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>byte[]</c> to <see cref="ObjectId"/> via constructor.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapByteArrayToObjectIdWhenBsonWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(BsonWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial MongoDB.Bson.ObjectId Map(byte[] input);
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
                ObjectIdTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(byte[]).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                ObjectIdTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    ObjectIdTypeName,
                                    parameter => parameter.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="ObjectId"/> to <c>byte[]</c> via <c>ToByteArray</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapObjectIdToByteArrayWhenBsonWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(BsonWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial byte[] Map(MongoDB.Bson.ObjectId input);
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
                typeof(byte[]).ToString(),
                NullableAnnotation.NotAnnotated,
                ObjectIdTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(byte[]).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                    "input.ToByteArray")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps nullable <see cref="string"/> to nullable <see cref="ObjectId"/> with null wrap.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNullableStringToNullableObjectIdWhenBsonWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(BsonWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial MongoDB.Bson.ObjectId? Map(string? input);
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
                typeof(ObjectId?).ToString(),
                NullableAnnotation.Annotated,
                typeof(string).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(typeof(ObjectId?).ToString(), "__mappa_tmp_1"))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions =>
                                {
                                    conditionAssertions.BeIsPatternExpressionSyntax(
                                        expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                        patternAssertions => patternAssertions.BeUnaryPatternSyntax(SyntaxKind.NotKeyword, argumentAssertions => argumentAssertions.BeConstantPatternSyntax(null)));
                                },
                                thenStatementAssertions =>
                                {
                                    thenStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(syntaxNode => syntaxNode.BeLocalDeclarationStatementSyntax(
                                            typeof(string).ToString(),
                                            "__mappa_tmp_2",
                                            assertInitialization => assertInitialization.BeIdentifierNameSyntax("input")))
                                        .HasNextSyntaxNode(syntaxNode => syntaxNode.BeLocalDeclarationStatementSyntax(
                                            ObjectIdTypeName,
                                            "__mappa_tmp_3",
                                            expression => expression.BeInvocationExpressionSyntax(
                                                ObjectIdParseMember,
                                                parameter => parameter.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                                        .HasNextSyntaxNode(syntaxNode => syntaxNode.BeAssignmentExpressionStatement(
                                            left => left.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                            right => right.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                },
                                elseStatementAssertions =>
                                {
                                    elseStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(defaultSyntaxNode => defaultSyntaxNode.BeAssignmentExpressionStatement(
                                            left =>
                                            {
                                                left.BeIdentifierNameSyntax("__mappa_tmp_1");
                                            },
                                            right =>
                                            {
                                                right.BeCastExpressionSyntax(
                                                    "MongoDB.Bson.ObjectId?",
                                                    expression => expression.BeLiteralExpressionSyntax(null));
                                            }));
                                }))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="decimal"/> to <see cref="Decimal128"/> via constructor.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDecimalToDecimal128WhenBsonWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(BsonWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial MongoDB.Bson.Decimal128 Map(decimal input);
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
                Decimal128TypeName,
                NullableAnnotation.NotAnnotated,
                typeof(decimal).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                Decimal128TypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    Decimal128TypeName,
                                    parameter => parameter.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="Decimal128"/> to <see cref="decimal"/> via <c>ToDecimal</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDecimal128ToDecimalWhenBsonWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(BsonWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial decimal Map(MongoDB.Bson.Decimal128 input);
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
                typeof(decimal).ToString(),
                NullableAnnotation.NotAnnotated,
                Decimal128TypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(decimal).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                    Decimal128ToDecimalMember,
                                    parameter => parameter.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// When <c>BsonWellKnownTypes</c> is unset, <see cref="ObjectId"/> to <c>byte[]</c> cannot be mapped
    /// (no well-known strategy; constructor detection does not apply to arrays).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapObjectIdToByteArrayWhenBsonWellKnownTypesUndefined()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial byte[] Map(MongoDB.Bson.ObjectId input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert — without the BSON strategy the constructor detector cannot target byte[],
        // so generation fails and no mapping source is produced.
        generatedResults.Should()
            .HaveDiagnostics(1)
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// When <c>BsonWellKnownTypes</c> is disabled, <see cref="ObjectId"/> to <c>byte[]</c> cannot be mapped
    /// (no well-known strategy; constructor detection does not apply to arrays).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapObjectIdToByteArrayWhenBsonWellKnownTypesDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(BsonWellKnownTypes = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial byte[] Map(MongoDB.Bson.ObjectId input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert — without the BSON strategy the constructor detector cannot target byte[],
        // so generation fails and no mapping source is produced.
        generatedResults.Should()
            .HaveDiagnostics(1)
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// When <c>BsonWellKnownTypes</c> is disabled, <see cref="Decimal128"/> to <see cref="decimal"/> cannot be mapped
    /// (no well-known strategy; constructor detection does not apply to <see cref="decimal"/>).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapDecimal128ToDecimalWhenBsonWellKnownTypesDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(BsonWellKnownTypes = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial decimal Map(MongoDB.Bson.Decimal128 input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotIdentifyStrategy,
                Decimal128TypeName,
                "decimal")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(decimal).ToString(),
                NullableAnnotation.NotAnnotated,
                Decimal128TypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }
}