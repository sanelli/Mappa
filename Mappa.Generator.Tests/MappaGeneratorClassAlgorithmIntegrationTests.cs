// <copyright file="MappaGeneratorClassAlgorithmIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tess for <see cref="MappaGeneratorClassAlgorithm"/>.
/// </summary>
public sealed class MappaGeneratorClassAlgorithmIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Check that non-partial method of a class marked with
    /// <see cref="MappaAttribute"/> is ignored and not code
    /// is actually being generated for that.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NonPartialMethodsAreIgnoredAndNotDiagnosticIsReported()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public long Map(int input) => input;
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it has no parameters.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PartialMethodsWithArity0GenerateADiagnosticError()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial long Map();
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MethodHasInvalidNumberOfParameters, "Map");
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it has two parameters.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PartialMethodsWithArity2GenerateADiagnosticError()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial long Map(int input1, int input2);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MethodHasInvalidMappaContextParameter, "Map");
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it returns <see cref="Void"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PartialMethodsReturningVoidGeneratesADiagnosticError()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial void Map(int input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MethodIsVoid, "Map");
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it returns <see cref="Task"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PartialMethodsReturningTaskGeneratesADiagnosticError()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;
            using System.Threading.Tasks;
            
            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial Task Map(int input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MethodReturnsTaskType, "Map");
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it returns <see cref="Task{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PartialMethodsReturningTaskTGeneratesADiagnosticError()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;
            using System.Threading.Tasks;
            
            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial Task<string> Map(int input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MethodReturnsTaskType, "Map");
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it returns <see cref="ValueTask"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PartialMethodsReturningValueTaskGeneratesADiagnosticError()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;
            using System.Threading.Tasks;
            
            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial ValueTask Map(int input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MethodReturnsTaskType, "Map");
    }

    /// <summary>
    /// Check that partial method of a class marked with
    /// <see cref="MappaAttribute"/> is generate a diagnostic
    /// error when it returns <see cref="ValueTask{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PartialMethodsReturningValueTaskTGeneratesADiagnosticError()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;
            using System.Threading.Tasks;
            
            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial ValueTask<string> Map(int input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MethodReturnsTaskType, "Map");
    }

    /// <summary>
    /// Check that it is not possible generating two methods
    /// with the same mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task SourceClassCannotContainTwoMethodsDefiningTheSameMapping()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial int Map(int input);
                public partial int AnotherMap(int input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.DuplicatedMapping, "AnotherMap", "int ", "int ");
    }

    /// <summary>
    /// Check that the appropriate diagnostic is reported
    /// when a mapping cannot be identified.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ADiagnosticErrorIsReportedWhenTheMappingCannotBeGenerated()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial int Map(long input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, "long", "int");
    }

    /// <summary>
    /// Asset that multiple mappers can be generated inside the
    /// very same compilation unit / file.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanGenerateMultipleMappersFromInsideTheSameFile()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial InnerTarget Map(InnerSource input);
                                  }

                                  [Mappa]
                                  public sealed partial class AnotherMapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        const string innerSourceType = "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource";
        const string innerTargetType = "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget";
        const string sourceType = "Mappa.Generator.Tests.UnitTests.SourceCode.Source";
        const string targetType = "Mappa.Generator.Tests.UnitTests.SourceCode.Target";

        var compilationUnits = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode(howMany: 2)
            .WithCompilationUnits(2);
        compilationUnits.Should().HaveCount(2);

        foreach (var compilationUnit in compilationUnits)
        {
            var className = compilationUnit.Subject.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Single()
                .Identifier
                .Text;

            if (className.Equals("Mapper", StringComparison.Ordinal))
            {
                compilationUnit
                    .HaveMapMethod(
                        "Mapper",
                        [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                        "Map",
                        [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
                        false,
                        innerTargetType,
                        NullableAnnotation.NotAnnotated,
                        "input",
                        innerSourceType,
                        null,
                        NullableAnnotation.NotAnnotated,
                        RefKind.None,
                        false,
                        RefKind.None,
                        1,
                        NullableSetup.Enable,
                        PragmaWarning.NoBlock,
                        blockSyntaxAssertions =>
                        {
                            blockSyntaxAssertions
                                .HasSyntaxNodesCount(3)
                                .HasNextSyntaxNode(syntaxNodeAssertions =>
                                {
                                    syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(int).ToString(),
                                        "__mappa_tmp_1",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.A"));
                                })
                                .HasNextSyntaxNode(syntaxNodeAssertions =>
                                {
                                    syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                        innerTargetType,
                                        "__mappa_tmp_2",
                                        initializationAssertions =>
                                        {
                                            initializationAssertions.BeObjectCreationExpressionSyntax(
                                                innerTargetType,
                                                ("A", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                        });
                                })
                                .HasNextSyntaxNode(syntaxNodeAssertions =>
                                {
                                    syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                });
                        });
            }
            else
            {
                className.Should().Be("AnotherMapper");
                compilationUnit
                    .HaveMapMethod(
                        "AnotherMapper",
                        [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                        "Map",
                        [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
                        false,
                        targetType,
                        NullableAnnotation.NotAnnotated,
                        "input",
                        sourceType,
                        null,
                        NullableAnnotation.NotAnnotated,
                        RefKind.None,
                        false,
                        RefKind.None,
                        1,
                        NullableSetup.Enable,
                        PragmaWarning.NoBlock,
                        blockSyntaxAssertions =>
                        {
                            blockSyntaxAssertions
                                .HasSyntaxNodesCount(5)
                                .HasNextSyntaxNode(syntaxNodeAssertions =>
                                {
                                    syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                        innerSourceType,
                                        "__mappa_tmp_1",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                                })
                                .HasNextSyntaxNode(syntaxNodeAssertions =>
                                {
                                    syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                        typeof(int).ToString(),
                                        "__mappa_tmp_2",
                                        initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.A"));
                                })
                                .HasNextSyntaxNode(syntaxNodeAssertions =>
                                {
                                    syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                        innerTargetType,
                                        "__mappa_tmp_3",
                                        initializationAssertions =>
                                        {
                                            initializationAssertions.BeObjectCreationExpressionSyntax(
                                                innerTargetType,
                                                ("A", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                        });
                                })
                                .HasNextSyntaxNode(syntaxNodeAssertions =>
                                {
                                    syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                        targetType,
                                        "__mappa_tmp_4",
                                        initializationAssertions =>
                                        {
                                            initializationAssertions.BeObjectCreationExpressionSyntax(
                                                targetType,
                                                ("Property", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                        });
                                })
                                .HasNextSyntaxNode(syntaxNodeAssertions =>
                                {
                                    syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4"));
                                });
                        });
            }
        }
    }
}