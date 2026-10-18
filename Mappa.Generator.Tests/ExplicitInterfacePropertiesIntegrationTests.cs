// <copyright file="ExplicitInterfacePropertiesIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for mapping properties that are explicit interface implementations
/// via <see cref="Mappa.Attributes.MappaSettingsAttribute.MapSourceExplicitInterfaceProperties"/>
/// and <see cref="Mappa.Attributes.MappaSettingsAttribute.MapTargetExplicitInterfaceProperties"/>.
/// </summary>
public sealed class ExplicitInterfacePropertiesIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string SourceTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.Source";
    private const string TargetTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.Target";
    private const string IHasValueTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.IHasValue";
    private const string AccessorsClassName = "__MappaInaccessibleAccessors";

    /// <summary>
    /// Maps an explicit interface source property when <c>MapSourceExplicitInterfaceProperties</c> is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapExplicitInterfaceSourcePropertyWhenMapSourceSettingEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public interface IHasValue
                                  {
                                      int Value { get; set; }
                                  }

                                  public class Source : IHasValue
                                  {
                                      int IHasValue.Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(MapSourceExplicitInterfaceProperties = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax($"(({IHasValueTypeName})input).Value"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Maps an explicit interface target property when <c>MapTargetExplicitInterfaceProperties</c> is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapExplicitInterfaceTargetPropertyWhenMapTargetSettingEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public interface IHasValue
                                  {
                                      int Value { get; set; }
                                  }

                                  public class Source
                                  {
                                      public int Value { get; set; }
                                  }

                                  public class Target : IHasValue
                                  {
                                      int IHasValue.Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(MapTargetExplicitInterfaceProperties = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(TargetTypeName));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Value"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeAssignmentExpressionStatement(
                                leftAssertions => leftAssertions.BeMemberAccessExpressionSyntax($"(({IHasValueTypeName})__mappa_tmp_1).Value"),
                                rightAssertions => rightAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Maps an explicit interface source property into a constructor parameter when the source setting is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapExplicitInterfaceSourceIntoConstructorParameterWhenMapSourceEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public interface IHasValue
                                  {
                                      int Value { get; set; }
                                  }

                                  public class Source : IHasValue
                                  {
                                      int IHasValue.Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public Target(int value)
                                      {
                                          this.Value = value;
                                      }

                                      public int Value { get; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(MapSourceExplicitInterfaceProperties = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax($"(({IHasValueTypeName})input).Value"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Maps a leftover explicit interface target property after a parameterized constructor when the target setting is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapLeftoverExplicitInterfaceTargetPropertyAfterParameterizedConstructorWhenMapTargetEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public interface IHasValue
                                  {
                                      int Value { get; set; }
                                  }

                                  public class Source
                                  {
                                      public int Id { get; set; }
                                      public int Value { get; set; }
                                  }

                                  public class Target : IHasValue
                                  {
                                      public Target(int id)
                                      {
                                          this.Id = id;
                                      }

                                      public int Id { get; set; }

                                      int IHasValue.Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(MapTargetExplicitInterfaceProperties = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Id"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Value"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeAssignmentExpressionStatement(
                                leftAssertions => leftAssertions.BeMemberAccessExpressionSyntax($"(({IHasValueTypeName})__mappa_tmp_2).Value"),
                                rightAssertions => rightAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Does not map explicit interface properties when the settings are disabled; only public properties map.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotMapExplicitInterfacePropertiesWhenSettingsDisabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public interface IHasValue
                                  {
                                      int Value { get; set; }
                                  }

                                  public class Source : IHasValue
                                  {
                                      public int Id { get; set; }

                                      int IHasValue.Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Id { get; set; }

                                      public int Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotMapNonRequiredProperty, TargetTypeName, "Value")
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
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Id"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Id", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Maps a private ordinary source property via <c>UnsafeAccessor</c> and an explicit interface source property via cast
    /// when both <c>MapSourceExplicitInterfaceProperties</c> and <c>MappaAllowInaccessibleSourceMembers</c> are enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapPrivateOrdinaryAndExplicitInterfaceSourceTogetherWithAllowInaccessible()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public interface IHasValue
                                  {
                                      int Value { get; set; }
                                  }

                                  public class Source : IHasValue
                                  {
                                      private int Secret { get; set; }

                                      int IHasValue.Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Secret { get; set; }

                                      public int Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(MapSourceExplicitInterfaceProperties = BooleanSetting.Enable)]
                                      [MappaAllowInaccessibleSourceMembers]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    $"{AccessorsClassName}.__mappa_tmp_2",
                                    argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax($"(({IHasValueTypeName})input).Value"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_4",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Secret", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")),
                                        ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4"));
                        });
                })
            .HaveClass(AccessorsClassName, classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.FileKeyword, SyntaxKind.StaticKeyword)
                    .HaveMethods(1)
                    .HaveExternUnsafeAccessorMethod("__mappa_tmp_2", "Method", "get_Secret");
            });
    }

    /// <summary>
    /// Maps a private ordinary target property via <c>UnsafeAccessor</c> and an explicit interface target property via cast
    /// when both <c>MapTargetExplicitInterfaceProperties</c> and <c>MappaAllowInaccessibleTargetMembers</c> are enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapPrivateOrdinaryAndExplicitInterfaceTargetTogetherWithAllowInaccessible()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public interface IHasValue
                                  {
                                      int Value { get; set; }
                                  }

                                  public class Source
                                  {
                                      public int Secret { get; set; }

                                      public int Value { get; set; }
                                  }

                                  public class Target : IHasValue
                                  {
                                      private int Secret { get; set; }

                                      int IHasValue.Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(MapTargetExplicitInterfaceProperties = BooleanSetting.Enable)]
                                      [MappaAllowInaccessibleTargetMembers]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                        .HasSyntaxNodesCount(6)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(TargetTypeName));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Secret"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeInvocationExpressionSyntaxStatement(
                                $"{AccessorsClassName}.__mappa_tmp_3",
                                argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_4",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Value"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeAssignmentExpressionStatement(
                                leftAssertions => leftAssertions.BeMemberAccessExpressionSyntax($"(({IHasValueTypeName})__mappa_tmp_1).Value"),
                                rightAssertions => rightAssertions.BeIdentifierNameSyntax("__mappa_tmp_4"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                })
            .HaveClass(AccessorsClassName, classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.FileKeyword, SyntaxKind.StaticKeyword)
                    .HaveMethods(1)
                    .HaveExternUnsafeAccessorMethod("__mappa_tmp_3", "Method", "set_Secret");
            });
    }

    /// <summary>
    /// Does not map an explicit interface source property via <c>UnsafeAccessor</c> when the map-source setting is off
    /// even if inaccessible source members are allowed; private ordinary properties still map.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotMapExplicitInterfaceSourceViaUnsafeAccessorWhenMapSourceDisabledButAllowInaccessibleEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public interface IHasValue
                                  {
                                      int Value { get; set; }
                                  }

                                  public class Source : IHasValue
                                  {
                                      private int Secret { get; set; }

                                      int IHasValue.Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Secret { get; set; }

                                      public int Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleSourceMembers]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotMapNonRequiredProperty, TargetTypeName, "Value")
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
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    $"{AccessorsClassName}.__mappa_tmp_2",
                                    argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Secret", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                })
            .HaveClass(AccessorsClassName, classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.FileKeyword, SyntaxKind.StaticKeyword)
                    .HaveMethods(1)
                    .HaveExternUnsafeAccessorMethod("__mappa_tmp_2", "Method", "get_Secret");
            });
    }

    /// <summary>
    /// Does not map an explicit interface source property when it is named in the inaccessible allow-list
    /// but <c>MapSourceExplicitInterfaceProperties</c> is disabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotMapExplicitInterfaceWhenAllowListNamesItButMapSettingDisabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public interface IHasValue
                                  {
                                      int Value { get; set; }
                                  }

                                  public class Source : IHasValue
                                  {
                                      public int Id { get; set; }

                                      int IHasValue.Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Id { get; set; }

                                      public int Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleSourceMembers("Value")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotMapNonRequiredProperty, TargetTypeName, "Value")
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
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Id"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Id", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Does not map an explicit interface target property when <c>MapTargetExplicitInterfaceProperties</c> is disabled
    /// (getter-backed fill is also rejected for explicit interface getters).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotMapExplicitInterfaceTargetWhenMapTargetSettingDisabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public interface IHasValue
                                  {
                                      int Value { get; set; }
                                  }

                                  public class Source
                                  {
                                      public int Id { get; set; }

                                      public int Value { get; set; }
                                  }

                                  public class Target : IHasValue
                                  {
                                      public int Id { get; set; }

                                      int IHasValue.Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Unmapped explicit-interface targets are not reported when not writable (IsReportableEmptyCtorUnmappedProperty).
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
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Id"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Id", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Does not map a write-only target property via getter-backed fill when the setter is inaccessible.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotMapWriteOnlyInaccessibleTargetPropertyViaGetterPath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Id { get; set; }

                                      public int Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      private int valueField;

                                      public int Id { get; set; }

                                      public int Value
                                      {
                                          private set => this.valueField = value;
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Write-only inaccessible setters are not reportable as unmapped when not writable.
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
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Id"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Id", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }
}