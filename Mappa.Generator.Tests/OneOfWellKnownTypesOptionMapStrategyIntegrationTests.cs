// <copyright file="OneOfWellKnownTypesOptionMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for OneOf Option (<c>OneOf&lt;T, None&gt;</c>) well-known strategies.
/// </summary>
public sealed class OneOfWellKnownTypesOptionMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string StringOptionTypeName = "OneOf.OneOf<string, OneOf.Types.None>";

    private const string IntOptionTypeName = "OneOf.OneOf<int, OneOf.Types.None>";

    private const string FromT0StringMember = "global::OneOf.OneOf<string, global::OneOf.Types.None>.FromT0";

    private const string FromT1StringMember = "global::OneOf.OneOf<string, global::OneOf.Types.None>.FromT1";

    private const string FromT0IntMember = "global::OneOf.OneOf<int, global::OneOf.Types.None>.FromT0";

    private const string FromT1IntMember = "global::OneOf.OneOf<int, global::OneOf.Types.None>.FromT1";

    private const string MappaExceptionTypeName = "global::Mappa.MappaException";

    private const string OptionNoneToStringMessage =
        "Cannot complete mapping from 'OneOf.OneOf<string, OneOf.Types.None>' to 'string': the OneOf option value is None.";

    /// <summary>
    /// When <c>OneOfWellKnownTypes</c> is undefined, Option unwrap to a non-string target cannot be mapped
    /// (string would fall through to <c>ToString</c>).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapOptionToIntWhenOneOfWellKnownTypesUndefined()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using OneOf;
                                  using OneOf.Types;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial int Map(OneOf<int, None> input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, IntOptionTypeName, "int")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                IntOptionTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// Maps Option to non-nullable <see cref="string"/> under NRT by throwing when None.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapOptionToStringWhenOneOfWellKnownTypesEnabledNullableEnable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;
                                  using OneOf.Types;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial string Map(OneOf<string, None> input);
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
                StringOptionTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeConditionalExpressionSyntax(
                                    condition => condition.BeMemberAccessExpressionSyntax("input.IsT0"),
                                    whenTrue => whenTrue.BeMemberAccessExpressionSyntax("input.AsT0"),
                                    whenFalse => whenFalse.BeThrowExpressionSyntax(
                                        MappaExceptionTypeName,
                                        message => message.BeLiteralExpressionSyntax(OptionNoneToStringMessage)))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps Option to nullable <see cref="string"/> under NRT by returning null when None.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapOptionToNullableStringWhenOneOfWellKnownTypesEnabledNullableEnable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;
                                  using OneOf.Types;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial string? Map(OneOf<string, None> input);
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
                NullableAnnotation.Annotated,
                StringOptionTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString() + "?",
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeConditionalExpressionSyntax(
                                    condition => condition.BeMemberAccessExpressionSyntax("input.IsT0"),
                                    whenTrue => whenTrue.BeMemberAccessExpressionSyntax("input.AsT0"),
                                    whenFalse => whenFalse.BeLiteralExpressionSyntax(null))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps non-nullable <see cref="string"/> to Option under NRT via <c>FromT0</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringToOptionWhenOneOfWellKnownTypesEnabledNullableEnable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;
                                  using OneOf.Types;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial OneOf<string, None> Map(string input);
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
                StringOptionTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                StringOptionTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                    FromT0StringMember,
                                    parameter => parameter.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps nullable <see cref="string"/> to Option under NRT, using None when null.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNullableStringToOptionWhenOneOfWellKnownTypesEnabledNullableEnable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;
                                  using OneOf.Types;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial OneOf<string, None> Map(string? input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert — Option wrap owns null → None (nullable strategy is skipped for Option targets).
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                StringOptionTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                StringOptionTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeConditionalExpressionSyntax(
                                    condition => condition.BeIsPatternExpressionSyntax(
                                        expression => expression.BeIdentifierNameSyntax("input"),
                                        pattern => pattern.BeUnaryPatternSyntax(
                                            SyntaxKind.NotKeyword,
                                            argument => argument.BeConstantPatternSyntax(null))),
                                    whenTrue => whenTrue.BeInvocationExpressionSyntax(
                                        FromT0StringMember,
                                        parameter => parameter.BeCastExpressionSyntax(
                                            typeof(string).ToString(),
                                            expression => expression.BeIdentifierNameSyntax("input"))),
                                    whenFalse => whenFalse.BeInvocationExpressionSyntax(
                                        FromT1StringMember,
                                        parameter => parameter.BeDefaultExpressionSyntax("OneOf.Types.None")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps Option to <see cref="string"/> under NRT off by returning null when None.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapOptionToStringWhenOneOfWellKnownTypesEnabledNullableDisable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;
                                  using OneOf.Types;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial string Map(OneOf<string, None> input);
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
                NullableAnnotation.None,
                StringOptionTypeName,
                NullableAnnotation.NotAnnotated,
                NullableSetup.Disable,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeConditionalExpressionSyntax(
                                    condition => condition.BeMemberAccessExpressionSyntax("input.IsT0"),
                                    whenTrue => whenTrue.BeMemberAccessExpressionSyntax("input.AsT0"),
                                    whenFalse => whenFalse.BeLiteralExpressionSyntax(null))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps Option to <see cref="int"/> under NRT off by returning <c>default</c> when None.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapOptionToIntWhenOneOfWellKnownTypesEnabledNullableDisable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;
                                  using OneOf.Types;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial int Map(OneOf<int, None> input);
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
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                IntOptionTypeName,
                NullableAnnotation.NotAnnotated,
                NullableSetup.Disable,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeConditionalExpressionSyntax(
                                    condition => condition.BeMemberAccessExpressionSyntax("input.IsT0"),
                                    whenTrue => whenTrue.BeMemberAccessExpressionSyntax("input.AsT0"),
                                    whenFalse => whenFalse.BeDefaultLiteralExpressionSyntax())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps <see cref="string"/> to Option under NRT off, using None when the source is null.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringToOptionWhenOneOfWellKnownTypesEnabledNullableDisable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;
                                  using OneOf.Types;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial OneOf<string, None> Map(string input);
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
                StringOptionTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.None,
                NullableSetup.Disable,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                StringOptionTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeConditionalExpressionSyntax(
                                    condition => condition.BeIsPatternExpressionSyntax(
                                        expression => expression.BeIdentifierNameSyntax("input"),
                                        pattern => pattern.BeUnaryPatternSyntax(
                                            SyntaxKind.NotKeyword,
                                            argument => argument.BeConstantPatternSyntax(null))),
                                    whenTrue => whenTrue.BeInvocationExpressionSyntax(
                                        FromT0StringMember,
                                        parameter => parameter.BeCastExpressionSyntax(
                                            typeof(string).ToString(),
                                            expression => expression.BeIdentifierNameSyntax("input"))),
                                    whenFalse => whenFalse.BeInvocationExpressionSyntax(
                                        FromT1StringMember,
                                        parameter => parameter.BeDefaultExpressionSyntax("OneOf.Types.None")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps <see cref="int"/> to Option under NRT off, using None when the source is <c>default</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIntToOptionWhenOneOfWellKnownTypesEnabledNullableDisable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;
                                  using OneOf.Types;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial OneOf<int, None> Map(int input);
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
                IntOptionTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                NullableSetup.Disable,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                IntOptionTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeConditionalExpressionSyntax(
                                    condition => condition.BePrefixUnaryExpressionSyntax(
                                        SyntaxKind.ExclamationToken,
                                        operand => operand.BeInvocationExpressionSyntax(
                                            "global::System.Collections.Generic.EqualityComparer<int>.Default.Equals",
                                            left => left.BeIdentifierNameSyntax("input"),
                                            right => right.BeDefaultExpressionSyntax(typeof(int).ToString()))),
                                    whenTrue => whenTrue.BeInvocationExpressionSyntax(
                                        FromT0IntMember,
                                        parameter => parameter.BeIdentifierNameSyntax("input")),
                                    whenFalse => whenFalse.BeInvocationExpressionSyntax(
                                        FromT1IntMember,
                                        parameter => parameter.BeDefaultExpressionSyntax("OneOf.Types.None")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }
}