// <copyright file="OneOfWellKnownTypesOneOfBaseAndNestedMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for OneOfBase unwrap/wrap, nested POCO Option mapping, projection refusal, and higher arity.
/// </summary>
public sealed class OneOfWellKnownTypesOneOfBaseAndNestedMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string Namespace = "Mappa.Generator.Tests.UnitTests.SourceCode";

    private const string StringOrIntTypeName = Namespace + ".StringOrInt";

    private const string LockedUnionTypeName = Namespace + ".LockedUnion";

    private const string SourceTypeName = Namespace + ".Source";

    private const string TargetTypeName = Namespace + ".Target";

    private const string FourArmOneOfTypeName = "OneOf.OneOf<string, int, bool, long>";

    private const string MappaExceptionTypeName = "global::Mappa.MappaException";

    private const string UnwrapStringOrIntMessage =
        "Cannot complete mapping from 'Mappa.Generator.Tests.UnitTests.SourceCode.StringOrInt' to 'string': the OneOf value is not T0.";

    private const string UnwrapFourArmMessage =
        "Cannot complete mapping from 'OneOf.OneOf<string, int, bool, long>' to 'long': the OneOf value is not T3.";

    /// <summary>
    /// Unwraps a <c>OneOfBase</c>-derived source to an exact arm.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanUnwrapOneOfBaseDerivedSourceWhenOneOfWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class StringOrInt : OneOfBase<string, int>
                                  {
                                      public StringOrInt(OneOf<string, int> input)
                                          : base(input)
                                      {
                                      }
                                  }

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial string Map(StringOrInt input);
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
                StringOrIntTypeName,
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
                                        message => message.BeLiteralExpressionSyntax(UnwrapStringOrIntMessage)))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Wraps a value into a constructible <c>OneOfBase</c> subclass via <c>new Target(OneOf.FromTn(...))</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanWrapIntoOneOfBaseViaConstructorWhenOneOfWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class StringOrInt : OneOfBase<string, int>
                                  {
                                      public StringOrInt(OneOf<string, int> input)
                                          : base(input)
                                      {
                                      }
                                  }

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial StringOrInt Map(string input);
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
                StringOrIntTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                StringOrIntTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    StringOrIntTypeName,
                                    parameter => parameter.BeInvocationExpressionSyntax(
                                        "global::OneOf.OneOf<string, int>.FromT0",
                                        arg => arg.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps into a <c>OneOfBase</c> subclass that exposes public implicits from arm types (via identity).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanWrapIntoOneOfBaseViaImplicitWhenOneOfWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class StringOrInt : OneOfBase<string, int>
                                  {
                                      private StringOrInt(OneOf<string, int> input)
                                          : base(input)
                                      {
                                      }

                                      public static implicit operator StringOrInt(string value)
                                          => new StringOrInt(value);

                                      public static implicit operator StringOrInt(int value)
                                          => new StringOrInt(value);
                                  }

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial StringOrInt Map(string input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert — public arm implicits are handled by identity (ungated / still works with the setting on).
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                StringOrIntTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("input")));
                });
    }

    /// <summary>
    /// Refuses wrapping into a <c>OneOfBase</c> subclass with no public construction path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotWrapIntoOneOfBaseWithoutPublicConstruction()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class LockedUnion : OneOfBase<string, int>
                                  {
                                      private LockedUnion(OneOf<string, int> input)
                                          : base(input)
                                      {
                                      }
                                  }

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial LockedUnion Map(string input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, "string", LockedUnionTypeName)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                LockedUnionTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// Maps a nested Option property on a POCO when well-known strategies are enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNestedOptionPropertyWhenOneOfWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;
                                  using OneOf.Types;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class Source
                                  {
                                      public OneOf<string, None> Value { get; set; }
                                  }

                                  public sealed class Target
                                  {
                                      public string Value { get; set; } = string.Empty;
                                  }

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
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
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "OneOf.OneOf<string, OneOf.Types.None>",
                                "__mappa_tmp_1",
                                init => init.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                assertInitialization => assertInitialization.BeConditionalExpressionSyntax(
                                    condition => condition.BeMemberAccessExpressionSyntax("__mappa_tmp_1.IsT0"),
                                    whenTrue => whenTrue.BeMemberAccessExpressionSyntax("__mappa_tmp_1.AsT0"),
                                    whenFalse => whenFalse.BeThrowExpressionSyntax(
                                        MappaExceptionTypeName,
                                        message => message.BeLiteralExpressionSyntax(
                                            "Cannot complete mapping from 'OneOf.OneOf<string, OneOf.Types.None>' to 'string': the OneOf option value is None.")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_3",
                                init => init.BeObjectCreationExpressionSyntax(
                                    TargetTypeName,
                                    ("Value", value => value.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Refuses <see cref="System.Linq.IQueryable{T}"/> projection when the mapping involves OneOf well-known strategies.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotProjectOneOfPropertyWhenOneOfWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;
                                  using OneOf.Types;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class Source
                                  {
                                      public OneOf<string, None> Value { get; set; }
                                  }

                                  public sealed class Target
                                  {
                                      public string Value { get; set; } = string.Empty;
                                  }

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public static partial class Mapper
                                  {
                                      public static partial IQueryable<Target> ProjectToDto(this IQueryable<Source> query);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert — OneOf well-known strategies are not allowlisted for IQueryable projection.
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.ProjectionMappingNotSupported,
                "ProjectToDto",
                "OneOfOptionToValueMapStrategy")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                "ProjectToDto",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                true,
                $"System.Linq.IQueryable<{TargetTypeName}>",
                NullableAnnotation.NotAnnotated,
                "query",
                $"System.Linq.IQueryable<{SourceTypeName}>",
                NullableAnnotation.NotAnnotated,
                1,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(node => node.BeReturnStatement());
                });
    }

    /// <summary>
    /// Unwraps a higher-arity <c>OneOf</c> to a unique exact arm (T3).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanUnwrapHigherArityOneOfToExactArmWhenOneOfWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial long Map(OneOf<string, int, bool, long> input);
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
                typeof(long).ToString(),
                NullableAnnotation.NotAnnotated,
                FourArmOneOfTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeConditionalExpressionSyntax(
                                    condition => condition.BeMemberAccessExpressionSyntax("input.IsT3"),
                                    whenTrue => whenTrue.BeMemberAccessExpressionSyntax("input.AsT3"),
                                    whenFalse => whenFalse.BeThrowExpressionSyntax(
                                        MappaExceptionTypeName,
                                        message => message.BeLiteralExpressionSyntax(UnwrapFourArmMessage)))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }
}