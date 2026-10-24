// <copyright file="OneOfWellKnownTypesMapStrategyCoverageTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Coverage-oriented integration tests for OneOf well-known strategies.
/// </summary>
public sealed class OneOfWellKnownTypesMapStrategyCoverageTests
    : MappaGeneratorAbstractUnitTests
{
    private const string Namespace = "Mappa.Generator.Tests.UnitTests.SourceCode";

    private const string IntOptionTypeName = "OneOf.OneOf<int, OneOf.Types.None>";

    private const string StringOptionTypeName = "OneOf.OneOf<string, OneOf.Types.None>";

    private const string StringOptionBaseTypeName = Namespace + ".StringOption";

    private const string AnimalOrStringTypeName = Namespace + ".AnimalOrString";

    private const string AnimalTypeName = Namespace + ".Animal";

    private const string FromT0IntMember = "global::OneOf.OneOf<int, global::OneOf.Types.None>.FromT0";

    private const string FromT1IntMember = "global::OneOf.OneOf<int, global::OneOf.Types.None>.FromT1";

    private const string FromT0StringOptionMember = "global::OneOf.OneOf<string, global::OneOf.Types.None>.FromT0";

    private const string FromT1StringOptionMember = "global::OneOf.OneOf<string, global::OneOf.Types.None>.FromT1";

    /// <summary>
    /// Maps Option to <c>int?</c> under NRT by returning null when None.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapOptionToNullableIntWhenOneOfWellKnownTypesEnabledNullableEnable()
    {
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
                                      public partial int? Map(OneOf<int, None> input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(int?).ToString(),
                NullableAnnotation.Annotated,
                IntOptionTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node =>
                            node.BeLocalDeclarationStatementSyntax(
                                "int?",
                                "__mappa_tmp_1",
                                init => init.BeConditionalExpressionSyntax(
                                    condition => condition.BeMemberAccessExpressionSyntax("input.IsT0"),
                                    whenTrue => whenTrue.BeMemberAccessExpressionSyntax("input.AsT0"),
                                    whenFalse => whenFalse.BeLiteralExpressionSyntax(null))))
                        .HasNextSyntaxNode(node =>
                            node.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps Option to <c>int?</c> under NRT off by returning <c>default</c> when None.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapOptionToNullableIntWhenOneOfWellKnownTypesEnabledNullableDisable()
    {
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
                                      public partial int? Map(OneOf<int, None> input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(int?).ToString(),
                NullableAnnotation.Annotated,
                IntOptionTypeName,
                NullableAnnotation.NotAnnotated,
                NullableSetup.Disable,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node =>
                            node.BeLocalDeclarationStatementSyntax(
                                "int?",
                                "__mappa_tmp_1",
                                init => init.BeConditionalExpressionSyntax(
                                    condition => condition.BeMemberAccessExpressionSyntax("input.IsT0"),
                                    whenTrue => whenTrue.BeMemberAccessExpressionSyntax("input.AsT0"),
                                    whenFalse => whenFalse.BeDefaultLiteralExpressionSyntax())))
                        .HasNextSyntaxNode(node =>
                            node.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps <c>int?</c> to Option via <c>HasValue</c> / <c>None</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNullableIntToOptionWhenOneOfWellKnownTypesEnabled()
    {
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
                                      public partial OneOf<int, None> Map(int? input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                IntOptionTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(int?).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node =>
                            node.BeLocalDeclarationStatementSyntax(
                                IntOptionTypeName,
                                "__mappa_tmp_1",
                                init => init.BeConditionalExpressionSyntax(
                                    condition => condition.BeMemberAccessExpressionSyntax("input.HasValue"),
                                    whenTrue => whenTrue.BeInvocationExpressionSyntax(
                                        FromT0IntMember,
                                        arg => arg.BeMemberAccessExpressionSyntax("input.Value")),
                                    whenFalse => whenFalse.BeInvocationExpressionSyntax(
                                        FromT1IntMember,
                                        arg => arg.BeDefaultExpressionSyntax("OneOf.Types.None")))))
                        .HasNextSyntaxNode(node =>
                            node.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Wraps into an Option-shaped <c>OneOfBase</c> via <c>new Target(OneOf.FromT0(...))</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanWrapIntoOneOfBaseOptionViaConstructorWhenOneOfWellKnownTypesEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;
                                  using OneOf.Types;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class StringOption : OneOfBase<string, None>
                                  {
                                      public StringOption(OneOf<string, None> input)
                                          : base(input)
                                      {
                                      }
                                  }

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial StringOption Map(string? input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                StringOptionBaseTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node =>
                            node.BeLocalDeclarationStatementSyntax(
                                StringOptionBaseTypeName,
                                "__mappa_tmp_1",
                                init => init.BeConditionalExpressionSyntax(
                                    condition => condition.BeIsPatternExpressionSyntax(
                                        expression => expression.BeIdentifierNameSyntax("input"),
                                        pattern => pattern.BeUnaryPatternSyntax(
                                            SyntaxKind.NotKeyword,
                                            argument => argument.BeConstantPatternSyntax(null))),
                                    whenTrue => whenTrue.BeObjectCreationExpressionSyntax(
                                        StringOptionBaseTypeName,
                                        arg => arg.BeInvocationExpressionSyntax(
                                            FromT0StringOptionMember,
                                            parameter => parameter.BeCastExpressionSyntax(
                                                typeof(string).ToString(),
                                                expression => expression.BeIdentifierNameSyntax("input")))),
                                    whenFalse => whenFalse.BeObjectCreationExpressionSyntax(
                                        StringOptionBaseTypeName,
                                        arg => arg.BeInvocationExpressionSyntax(
                                            FromT1StringOptionMember,
                                            parameter => parameter.BeDefaultExpressionSyntax("OneOf.Types.None"))))))
                        .HasNextSyntaxNode(node =>
                            node.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Wraps a derived type into a <c>OneOfBase</c> arm via the unique assignable arm.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanWrapDerivedIntoOneOfBaseAssignableArmWhenOneOfWellKnownTypesEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Animal { }
                                  public sealed class Dog : Animal { }

                                  public sealed class AnimalOrString : OneOfBase<Animal, string>
                                  {
                                      public AnimalOrString(OneOf<Animal, string> input)
                                          : base(input)
                                      {
                                      }
                                  }

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial AnimalOrString Map(Dog input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                AnimalOrStringTypeName,
                NullableAnnotation.NotAnnotated,
                Namespace + ".Dog",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node =>
                            node.BeLocalDeclarationStatementSyntax(
                                AnimalOrStringTypeName,
                                "__mappa_tmp_1",
                                init => init.BeObjectCreationExpressionSyntax(
                                    AnimalOrStringTypeName,
                                    arg => arg.BeInvocationExpressionSyntax(
                                        "global::OneOf.OneOf<global::Mappa.Generator.Tests.UnitTests.SourceCode.Animal, string>.FromT0",
                                        parameter => parameter.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(node =>
                            node.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Unwraps Option to an assignable target via general unwrap (not Option exact unwrap).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanUnwrapOptionToAssignableTargetWhenOneOfWellKnownTypesEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;
                                  using OneOf.Types;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Animal { }
                                  public sealed class Dog : Animal { }

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Animal Map(OneOf<Dog, None> input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                AnimalTypeName,
                NullableAnnotation.NotAnnotated,
                "OneOf.OneOf<Mappa.Generator.Tests.UnitTests.SourceCode.Dog, OneOf.Types.None>",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node =>
                            node.BeLocalDeclarationStatementSyntax(
                                AnimalTypeName,
                                "__mappa_tmp_1",
                                init => init.BeConditionalExpressionSyntax(
                                    condition => condition.BeMemberAccessExpressionSyntax("input.IsT0"),
                                    whenTrue => whenTrue.BeMemberAccessExpressionSyntax("input.AsT0"),
                                    whenFalse => whenFalse.BeThrowExpressionSyntax(
                                        "global::Mappa.MappaException",
                                        message => message.BeLiteralExpressionSyntax(
                                            "Cannot complete mapping from 'OneOf.OneOf<Mappa.Generator.Tests.UnitTests.SourceCode.Dog, OneOf.Types.None>' to 'Mappa.Generator.Tests.UnitTests.SourceCode.Animal': the OneOf value is not T0.")))))
                        .HasNextSyntaxNode(node =>
                            node.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Refuses wrapping a mismatched nullable value type into Option.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapMismatchedNullableIntToStringOption()
    {
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
                                      public partial OneOf<string, None> Map(int? input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, "int?", StringOptionTypeName)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                StringOptionTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(int?).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(node => node.BeReturnStatement());
                });
    }
}