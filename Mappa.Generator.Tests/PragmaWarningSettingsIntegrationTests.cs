// <copyright file="PragmaWarningSettingsIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests around the <see cref="MappaSettingsAttribute.PragmaWarning"/>.
/// </summary>
public sealed class PragmaWarningSettingsIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Tests that we can set <see cref="PragmaWarningSetting.NoBlock"/>
    /// on the class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PragmaWarningIsSetOnTheClassAsNoBlock()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(PragmaWarning = PragmaWarningSetting.NoBlock)]
                                  public sealed partial class Mapper
                                  {
                                      public partial int Map(int input);
                                  }
                                  #nullable restore
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
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                NullableSetup.Enable,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                        {
                            expressionSyntaxAssertions.BeIdentifierNameSyntax("input");
                        }));
                });
    }

    /// <summary>
    /// Tests that we can set <see cref="PragmaWarningSetting.Disable"/>
    /// on the class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PragmaWarningIsSetOnTheClassAsNoDisable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(PragmaWarning = PragmaWarningSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial int Map(int input);
                                  }
                                  #nullable restore
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
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                NullableSetup.Enable,
                PragmaWarning.Disable,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                        {
                            expressionSyntaxAssertions.BeIdentifierNameSyntax("input");
                        }));
                });
    }

    /// <summary>
    /// Tests that we can set <see cref="PragmaWarningSetting.NoBlock"/>
    /// on the method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PragmaWarningIsSetOnTheMethodAsNoBlock()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(PragmaWarning = PragmaWarningSetting.NoBlock)]
                                      public partial int Map(int input);
                                  }
                                  #nullable restore
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
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                NullableSetup.Enable,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                        {
                            expressionSyntaxAssertions.BeIdentifierNameSyntax("input");
                        }));
                });
    }

    /// <summary>
    /// Tests that we can set <see cref="PragmaWarningSetting.Disable"/>
    /// on the method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PragmaWarningIsSetOnTheMethodNoDisable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(PragmaWarning = PragmaWarningSetting.Disable)]
                                      public partial int Map(int input);
                                  }
                                  #nullable restore
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
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                NullableSetup.Enable,
                PragmaWarning.Disable,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                        {
                            expressionSyntaxAssertions.BeIdentifierNameSyntax("input");
                        }));
                });
    }

    /// <summary>
    /// Tests that we can set <see cref="PragmaWarningSetting.NoBlock"/>
    /// on the method and  <see cref="PragmaWarningSetting.NoBlock"/>
    /// on the class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PragmaWarningIsSetOnTheMethodAsNoBlockButDisableOnTheClass()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(PragmaWarning = PragmaWarningSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(PragmaWarning = PragmaWarningSetting.NoBlock)]
                                      public partial int Map(int input);
                                  }
                                  #nullable restore
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
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                NullableSetup.Enable,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                        {
                            expressionSyntaxAssertions.BeIdentifierNameSyntax("input");
                        }));
                });
    }

    /// <summary>
    /// Tests that we can set <see cref="PragmaWarningSetting.Disable"/>
    /// on the method but <see cref="PragmaWarningSetting.NoBlock"/>
    /// on the class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PragmaWarningIsSetOnTheMethodDisableButNoBlockOnTheClass()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(PragmaWarning = PragmaWarningSetting.NoBlock)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(PragmaWarning = PragmaWarningSetting.Disable)]
                                      public partial int Map(int input);
                                  }
                                  #nullable restore
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
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                NullableSetup.Enable,
                PragmaWarning.Disable,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                        {
                            expressionSyntaxAssertions.BeIdentifierNameSyntax("input");
                        }));
                });
    }
}