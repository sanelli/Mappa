// <copyright file="IdentityStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests related to the identity strategy.
/// </summary>
public sealed class IdentityStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string IdentityDeepCopyPersonTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.IdentityDeepCopyPerson";
    private const string IdentityNestedChildTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.IdentityNestedChild";
    private const string IdentityNestedStructTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.IdentityNestedStruct";
    private const string IdentityNestedContainerTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.IdentityNestedContainer";
    private const string IdentityNestedChildListTypeName = "global::System.Collections.Generic.List<Mappa.Generator.Tests.UnitTests.SourceCode.IdentityNestedChild>";
    private const string IdentityMapDeepCopyMemberwiseCloneInvocation = "global::Mappa.MappaCloning.MemberwiseClone";

    /// <summary>
    /// Test a mapping can be created when source and target type are the
    /// very same non reference type and nullable is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToSameReferenceWhenNullableDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string Map(string input);
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
                typeof(string).ToString(),
                NullableAnnotation.None,
                typeof(string).ToString(),
                NullableAnnotation.None,
                NullableSetup.Disable,
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
    /// Test a mapping can be created when source and target type are the
    /// very same non reference type and nullable is enabled but not applied.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToSameReferenceWhenNullableEnabledAndNotApplied()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string Map(string input);
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
            .HaveCommentHeader()
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
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
    /// Test a mapping can be created when source and target type are the
    /// very same non reference type and nullable is enabled and applied.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToSameReferenceWhenNullableEnabledAndApplied()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string? Map(string? input);
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
            .HaveCommentHeader()
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                NullableAnnotation.Annotated,
                typeof(string).ToString(),
                NullableAnnotation.Annotated,
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
    /// Test a mapping can be created when source and target type are the
    /// very same non reference type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNonReferenceTypeToSameNonReferenceWhenNullableDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
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
            .HaveCommentHeader()
            .HaveDefaultMapMethod(
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                NullableSetup.Disable,
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
    /// Test a mapping can be created when source and target type are the
    /// very same non reference type and target type is nullable.
    /// Also the nullability has been disabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNonReferenceTypeToSameNullableNonReferenceWhenNullableDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial int? Map(int input);
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
            .HaveCommentHeader()
            .HaveDefaultMapMethod(
                typeof(int?).ToString(),
                NullableAnnotation.Annotated,
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                NullableSetup.Disable,
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
    /// Test a mapping can be created when source and target type are the
    /// very same non reference type and target type is nullable.
    /// Also the nullability has been enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNonReferenceTypeToSameNullableNonReferenceWhenNullableEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial int? Map(int input);
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
            .HaveCommentHeader()
            .HaveDefaultMapMethod(
                typeof(int?).ToString(),
                NullableAnnotation.Annotated,
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
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
    /// Test a mapping can be created when source is a non reference type
    /// and the target type is <see cref="object"/> and nullable is disabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNonReferenceTypeObjectWhenNullableDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial object Map(int input);
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
            .HaveCommentHeader()
            .HaveDefaultMapMethod(
                typeof(object).ToString(),
                NullableAnnotation.None,
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                NullableSetup.Disable,
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
    /// Test a mapping can be created when source is a non reference type
    /// and the target type is nullable <see cref="object"/> and
    /// nullable is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNonReferenceTypeNullableObjectWhenNullableIsEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial object? Map(int input);
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
            .HaveCommentHeader()
            .HaveDefaultMapMethod(
                typeof(object).ToString(),
                NullableAnnotation.Annotated,
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
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
    /// Test a mapping can be created from reference type
    /// to <see cref="object"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToObjectWhenNullableDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial object Map(string input);
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
            .HaveCommentHeader()
            .HaveDefaultMapMethod(
                typeof(object).ToString(),
                NullableAnnotation.None,
                typeof(string).ToString(),
                NullableAnnotation.None,
                NullableSetup.Disable,
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
    /// Test a mapping can be created from reference type
    /// to nullable <see cref="object"/> when nullable is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToNullableObjectWhenNullableEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial object? Map(string input);
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
            .HaveCommentHeader()
            .HaveDefaultMapMethod(
                typeof(object).ToString(),
                NullableAnnotation.Annotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
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
    /// Test a mapping can be created when source and target type are the
    /// very same non reference type and nullable is enabled and applied.
    /// Also the target type is nullable while the source is not nullable.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToSameNullableReferenceWhenNullableEnabledAndApplied()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string? Map(string input);
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
            .HaveCommentHeader()
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                NullableAnnotation.Annotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
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
    /// Test a mapping can be created from reference type
    /// to <see cref="object"/> when nullable is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToObjectWhenNullableEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial object Map(string input);
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
            .HaveCommentHeader()
            .HaveDefaultMapMethod(
                typeof(object).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
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
    /// Test a mapping can be created when an implicit conversion
    /// can be applied between the types.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingImplicitConversion()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  internal sealed partial class Mapper
                                  {
                                      internal partial long Map(int input);
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
            .HaveCommentHeader()
            .HaveDefaultMapMethod(
                typeof(long).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                NullableSetup.Disable,
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
    /// Test same-type reference mapping uses <see cref="IdentityMapDeepCopySetting.DeepCopy"/>
    /// and emits <see cref="object.MemberwiseClone"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToSameReferenceWhenIdentityMapDeepCopyIsDeepCopy()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class IdentityDeepCopyPerson
                                  {
                                      public string Name = string.Empty;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(IdentityMapDeepCopy = IdentityMapDeepCopySetting.DeepCopy)]
                                      public partial IdentityDeepCopyPerson Map(IdentityDeepCopyPerson input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                IdentityDeepCopyPersonTypeName,
                IdentityDeepCopyPersonTypeName,
                AssertMemberwiseCloneIdentityMapBody);
    }

    /// <summary>
    /// Test <see cref="IdentityMapDeepCopySetting.NestedDeepCopy"/> on a reference type
    /// emits a clone and nested field assignments.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToSameReferenceWhenIdentityMapDeepCopyIsNestedDeepCopy()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class IdentityNestedChild
                                  {
                                      public string Name = string.Empty;

                                      public IdentityNestedChild()
                                      {
                                      }

                                      public IdentityNestedChild(IdentityNestedChild other)
                                      {
                                          this.Name = other.Name;
                                      }
                                  }

                                  public sealed class IdentityDeepCopyPerson
                                  {
                                      public IdentityNestedChild Child = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(IdentityMapDeepCopy = IdentityMapDeepCopySetting.NestedDeepCopy)]
                                      public partial IdentityDeepCopyPerson Map(IdentityDeepCopyPerson input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                IdentityDeepCopyPersonTypeName,
                IdentityDeepCopyPersonTypeName,
                AssertNestedDeepCopyPersonIdentityMapBody);
    }

    /// <summary>
    /// Test <see cref="IdentityMapDeepCopySetting.NestedDeepCopy"/> on a struct root
    /// uses struct copy instead of <see cref="object.MemberwiseClone"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStructToSameStructWhenIdentityMapDeepCopyIsNestedDeepCopy()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class IdentityNestedChild
                                  {
                                      public string Name = string.Empty;

                                      public IdentityNestedChild()
                                      {
                                      }

                                      public IdentityNestedChild(IdentityNestedChild other)
                                      {
                                          this.Name = other.Name;
                                      }
                                  }

                                  public struct IdentityNestedStruct
                                  {
                                      public IdentityNestedChild Child;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(IdentityMapDeepCopy = IdentityMapDeepCopySetting.NestedDeepCopy)]
                                      public partial IdentityNestedStruct Map(IdentityNestedStruct input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                IdentityNestedStructTypeName,
                IdentityNestedStructTypeName,
                AssertNestedDeepCopyStructIdentityMapBody);
    }

    /// <summary>
    /// Test primitive same-type identity mapping ignores <see cref="IdentityMapDeepCopySetting.DeepCopy"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNonReferenceTypeToSameNonReferenceWhenIdentityMapDeepCopyIsDeepCopy()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(IdentityMapDeepCopy = IdentityMapDeepCopySetting.DeepCopy)]
                                      public partial int Map(int input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(int).ToString(),
                typeof(int).ToString(),
                AssertShallowIdentityMapBody);
    }

    /// <summary>
    /// Test <see cref="IdentityMapDeepCopySetting.NestedDeepCopy"/> maps collection fields
    /// using the container strategy instead of identity pass-through.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToSameReferenceWhenIdentityMapDeepCopyIsNestedDeepCopyWithCollectionField()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class IdentityNestedChild
                                  {
                                      public string Name = string.Empty;

                                      public IdentityNestedChild()
                                      {
                                      }

                                      public IdentityNestedChild(IdentityNestedChild other)
                                      {
                                          this.Name = other.Name;
                                      }
                                  }

                                  public sealed class IdentityNestedContainer
                                  {
                                      public List<IdentityNestedChild> Children = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(IdentityMapDeepCopy = IdentityMapDeepCopySetting.NestedDeepCopy)]
                                      public partial IdentityNestedContainer Map(IdentityNestedContainer input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                IdentityNestedContainerTypeName,
                IdentityNestedContainerTypeName,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(6)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                IdentityNestedContainerTypeName,
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeCastExpressionSyntax(
                                    IdentityNestedContainerTypeName,
                                    castExpressionAssertions => castExpressionAssertions.BeInvocationExpressionSyntax(
                                        IdentityMapDeepCopyMemberwiseCloneInvocation,
                                        argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                IdentityNestedChildListTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Children"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                IdentityNestedChildListTypeName,
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                    IdentityNestedChildListTypeName,
                                    firstParameterSyntaxAssertions => firstParameterSyntaxAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Count")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions => declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_4", 0),
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Count")),
                                incrementorAssertions => incrementorAssertions.BePrefixUnaryExpressionSyntax(
                                    SyntaxKind.PlusPlusToken,
                                    operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")),
                                statementAssertions => statementAssertions.BeBlockStatement());
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeAssignmentExpressionStatement(
                                leftExpressionAssertions => leftExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Children"),
                                rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                            {
                                expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_1");
                            });
                        });
                });
    }

    /// <summary>
    /// Test <c>.editorconfig</c> <c>mappa.identitymapdeepcopy</c> applies to same-type reference mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToSameReferenceWhenIdentityMapDeepCopyIsSetInEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.identitymapdeepcopy = DeepCopy
                                    """;

        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class IdentityDeepCopyPerson
                                  {
                                      public string Name = string.Empty;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial IdentityDeepCopyPerson Map(IdentityDeepCopyPerson input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                IdentityDeepCopyPersonTypeName,
                IdentityDeepCopyPersonTypeName,
                AssertMemberwiseCloneIdentityMapBody);
    }

    /// <summary>
    /// Test class-level <see cref="MappaSettingsAttribute.IdentityMapDeepCopy"/> overrides
    /// <c>.editorconfig</c> for same-type reference mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToSameReferenceWhenClassIdentityMapDeepCopyOverridesEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.identitymapdeepcopy = DeepCopy
                                    """;

        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class IdentityDeepCopyPerson
                                  {
                                      public string Name = string.Empty;
                                  }

                                  [Mappa]
                                  [MappaSettings(IdentityMapDeepCopy = IdentityMapDeepCopySetting.ShallowCopy)]
                                  public sealed partial class Mapper
                                  {
                                      public partial IdentityDeepCopyPerson Map(IdentityDeepCopyPerson input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                IdentityDeepCopyPersonTypeName,
                IdentityDeepCopyPersonTypeName,
                AssertShallowIdentityMapBody);
    }

    /// <summary>
    /// Test method-level <see cref="MappaSettingsAttribute.IdentityMapDeepCopy"/> overrides
    /// class-level and <c>.editorconfig</c> settings for same-type reference mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToSameReferenceWhenMethodIdentityMapDeepCopyOverridesClassAndEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.identitymapdeepcopy = DeepCopy
                                    """;

        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class IdentityNestedChild
                                  {
                                      public string Name = string.Empty;

                                      public IdentityNestedChild()
                                      {
                                      }

                                      public IdentityNestedChild(IdentityNestedChild other)
                                      {
                                          this.Name = other.Name;
                                      }
                                  }

                                  public sealed class IdentityDeepCopyPerson
                                  {
                                      public IdentityNestedChild Child = null!;
                                  }

                                  [Mappa]
                                  [MappaSettings(IdentityMapDeepCopy = IdentityMapDeepCopySetting.ShallowCopy)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(IdentityMapDeepCopy = IdentityMapDeepCopySetting.NestedDeepCopy)]
                                      public partial IdentityDeepCopyPerson Map(IdentityDeepCopyPerson input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                IdentityDeepCopyPersonTypeName,
                IdentityDeepCopyPersonTypeName,
                AssertNestedDeepCopyPersonIdentityMapBody);
    }

    private static void AssertNestedDeepCopyPersonIdentityMapBody(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(6)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    IdentityDeepCopyPersonTypeName,
                    "__mappa_tmp_1",
                    initializationAssertions => initializationAssertions.BeCastExpressionSyntax(
                        IdentityDeepCopyPersonTypeName,
                        castExpressionAssertions => castExpressionAssertions.BeInvocationExpressionSyntax(
                            IdentityMapDeepCopyMemberwiseCloneInvocation,
                            argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input"))));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    IdentityNestedChildTypeName,
                    "__mappa_tmp_2",
                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Child"));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    IdentityNestedChildTypeName,
                    "__mappa_tmp_3",
                    initializationAssertions => initializationAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    IdentityNestedChildTypeName,
                    "__mappa_tmp_4",
                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                        IdentityNestedChildTypeName,
                        firstParameterSyntaxAssertions => firstParameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeAssignmentExpressionStatement(
                    leftExpressionAssertions => leftExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Child"),
                    rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4"));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                {
                    expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_1");
                });
            });
    }

    private static void AssertNestedDeepCopyStructIdentityMapBody(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(6)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    IdentityNestedStructTypeName,
                    "__mappa_tmp_1",
                    initializationAssertions => initializationAssertions.BeIdentifierNameSyntax("input"));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    IdentityNestedChildTypeName,
                    "__mappa_tmp_2",
                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Child"));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    IdentityNestedChildTypeName,
                    "__mappa_tmp_3",
                    initializationAssertions => initializationAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    IdentityNestedChildTypeName,
                    "__mappa_tmp_4",
                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                        IdentityNestedChildTypeName,
                        firstParameterSyntaxAssertions => firstParameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeAssignmentExpressionStatement(
                    leftExpressionAssertions => leftExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Child"),
                    rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4"));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                {
                    expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_1");
                });
            });
    }

    private static void AssertMemberwiseCloneIdentityMapBody(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(2)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    IdentityDeepCopyPersonTypeName,
                    "__mappa_tmp_1",
                    initializationAssertions => initializationAssertions.BeCastExpressionSyntax(
                        IdentityDeepCopyPersonTypeName,
                        castExpressionAssertions => castExpressionAssertions.BeInvocationExpressionSyntax(
                            IdentityMapDeepCopyMemberwiseCloneInvocation,
                            argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input"))));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                {
                    expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_1");
                });
            });
    }

    private static void AssertShallowIdentityMapBody(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(1)
            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
            {
                expressionSyntaxAssertions.BeIdentifierNameSyntax("input");
            }));
    }
}