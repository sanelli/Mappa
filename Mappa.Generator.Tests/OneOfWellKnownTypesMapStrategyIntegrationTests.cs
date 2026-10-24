// <copyright file="OneOfWellKnownTypesMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for general OneOf unwrap and ungated identity wrap strategies.
/// </summary>
public sealed class OneOfWellKnownTypesMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string StringIntBoolOneOfTypeName = "OneOf.OneOf<string, int, bool>";

    private const string AnimalDogOneOfTypeName = "OneOf.OneOf<Mappa.Generator.Tests.UnitTests.SourceCode.Animal, Mappa.Generator.Tests.UnitTests.SourceCode.Dog>";

    private const string DogCatOneOfTypeName = "OneOf.OneOf<Mappa.Generator.Tests.UnitTests.SourceCode.Dog, Mappa.Generator.Tests.UnitTests.SourceCode.Cat>";

    private const string DogStringOneOfTypeName = "OneOf.OneOf<Mappa.Generator.Tests.UnitTests.SourceCode.Dog, string>";

    private const string DuplicateIntOneOfTypeName = "OneOf.OneOf<int, int>";

    private const string AnimalTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.Animal";

    private const string IAnimalTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.IAnimal";

    private const string MappaExceptionTypeName = "global::Mappa.MappaException";

    private const string UnwrapStringMessage =
        "Cannot complete mapping from 'OneOf.OneOf<string, int, bool>' to 'string': the OneOf value is not T0.";

    private const string UnwrapAnimalExactMessage =
        "Cannot complete mapping from 'OneOf.OneOf<Mappa.Generator.Tests.UnitTests.SourceCode.Animal, Mappa.Generator.Tests.UnitTests.SourceCode.Dog>' to 'Mappa.Generator.Tests.UnitTests.SourceCode.Animal': the OneOf value is not T0.";

    private const string UnwrapAnimalAssignableMessage =
        "Cannot complete mapping from 'OneOf.OneOf<Mappa.Generator.Tests.UnitTests.SourceCode.Dog, string>' to 'Mappa.Generator.Tests.UnitTests.SourceCode.Animal': the OneOf value is not T0.";

    /// <summary>
    /// Maps <c>OneOf&lt;string, int, bool&gt;</c> to <see cref="string"/> via the exact T0 arm.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapOneOfToExactArmWhenOneOfWellKnownTypesEnabled()
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
                                      public partial string Map(OneOf<string, int, bool> input);
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
                StringIntBoolOneOfTypeName,
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
                                        message => message.BeLiteralExpressionSyntax(UnwrapStringMessage)))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Prefers an exact arm over an assignable derived arm when unwrapping to a base type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PrefersExactArmOverAssignableArmWhenUnwrappingOneOf()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Animal { }
                                  public sealed class Dog : Animal { }

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Animal Map(OneOf<Animal, Dog> input);
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
                AnimalTypeName,
                NullableAnnotation.NotAnnotated,
                AnimalDogOneOfTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                AnimalTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeConditionalExpressionSyntax(
                                    condition => condition.BeMemberAccessExpressionSyntax("input.IsT0"),
                                    whenTrue => whenTrue.BeMemberAccessExpressionSyntax("input.AsT0"),
                                    whenFalse => whenFalse.BeThrowExpressionSyntax(
                                        MappaExceptionTypeName,
                                        message => message.BeLiteralExpressionSyntax(UnwrapAnimalExactMessage)))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps via a unique assignable arm when no exact arm matches.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapOneOfViaUniqueAssignableArmWhenOneOfWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Animal { }
                                  public sealed class Dog : Animal { }

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Animal Map(OneOf<Dog, string> input);
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
                AnimalTypeName,
                NullableAnnotation.NotAnnotated,
                DogStringOneOfTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                AnimalTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeConditionalExpressionSyntax(
                                    condition => condition.BeMemberAccessExpressionSyntax("input.IsT0"),
                                    whenTrue => whenTrue.BeMemberAccessExpressionSyntax("input.AsT0"),
                                    whenFalse => whenFalse.BeThrowExpressionSyntax(
                                        MappaExceptionTypeName,
                                        message => message.BeLiteralExpressionSyntax(UnwrapAnimalAssignableMessage)))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Refuses unwrap when multiple arms are assignable to the target.
    /// Uses an interface target so empty-constructor fallback cannot apply.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapOneOfWhenAssignableArmsAreAmbiguous()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using OneOf;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public interface IAnimal { }
                                  public sealed class Dog : IAnimal { }
                                  public sealed class Cat : IAnimal { }

                                  [Mappa]
                                  [MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial IAnimal Map(OneOf<Dog, Cat> input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert — NoMap stub is still emitted with the cannot-identify diagnostic.
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, DogCatOneOfTypeName, IAnimalTypeName)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                IAnimalTypeName,
                NullableAnnotation.NotAnnotated,
                DogCatOneOfTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// Refuses unwrap when the OneOf has duplicate arm types matching the target.
    /// Uses <see cref="int"/> so <c>ToString</c> fallthrough cannot apply.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapOneOfWhenExactArmsAreDuplicated()
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
                                      public partial int Map(OneOf<int, int> input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, DuplicateIntOneOfTypeName, "int")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                DuplicateIntOneOfTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// Identity still maps an exact arm type into <c>OneOf</c> without enabling well-known strategies.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringToOneOfViaIdentityWithoutOneOfWellKnownTypes()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using OneOf;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial OneOf<int, string> Map(string input);
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
                "OneOf.OneOf<int, string>",
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
}