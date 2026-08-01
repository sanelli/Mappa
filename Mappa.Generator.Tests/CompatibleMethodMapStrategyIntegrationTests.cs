// <copyright file="CompatibleMethodMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="CompatibleMethodMapStrategy"/>.
/// </summary>
public sealed class CompatibleMethodMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test nested mapping reuses a compatible method when <see cref="MappaSettingsAttribute.CompatibleMapMethod"/>
    /// is enabled on the method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCompatibleMethodWhenEnabledOnMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class BaseSource { public int A { get; set; } }
                                  public class DerivedSource : BaseSource { }

                                  public class BaseTarget { public int A { get; set; } }
                                  public class DerivedTarget : BaseTarget { }

                                  public class Source { public DerivedSource Property { get; set; } }
                                  public class Target { public BaseTarget Property { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public DerivedTarget MapInner(BaseSource input)
                                      {
                                          return new DerivedTarget { A = input.A };
                                      }

                                      [MappaSettings(CompatibleMapMethod = BooleanSetting.Enable)]
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                NullableSetup.Enable,
                blockSyntaxAssertions =>
                {
                    AssertCompatibleMapInnerInvocation(blockSyntaxAssertions);
                });
    }

    /// <summary>
    /// Test nested mapping reuses a compatible method when <see cref="MappaSettingsAttribute.CompatibleMapMethod"/>
    /// is enabled on the class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCompatibleMethodWhenEnabledOnClass()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class BaseSource { public int A { get; set; } }
                                  public class DerivedSource : BaseSource { }

                                  public class BaseTarget { public int A { get; set; } }
                                  public class DerivedTarget : BaseTarget { }

                                  public class Source { public DerivedSource Property { get; set; } }
                                  public class Target { public BaseTarget Property { get; set; } }

                                  [Mappa]
                                  [MappaSettings(CompatibleMapMethod = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public DerivedTarget MapInner(BaseSource input)
                                      {
                                          return new DerivedTarget { A = input.A };
                                      }

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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                NullableSetup.Enable,
                blockSyntaxAssertions =>
                {
                    AssertCompatibleMapInnerInvocation(blockSyntaxAssertions);
                });
    }

    /// <summary>
    /// Test nested mapping does not reuse a compatible method when the setting is unset.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotMapUsingCompatibleMethodWhenSettingIsUnset()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class BaseSource { public int A { get; set; } }
                                  public class DerivedSource : BaseSource { }

                                  public class BaseTarget { public int A { get; set; } }
                                  public class DerivedTarget : BaseTarget { }

                                  public class Source { public DerivedSource Property { get; set; } }
                                  public class Target { public BaseTarget Property { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public DerivedTarget MapInner(BaseSource input)
                                      {
                                          return new DerivedTarget { A = input.A };
                                      }

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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                NullableSetup.Enable,
                blockSyntaxAssertions =>
                {
                    AssertConstructorMappingWithoutMapInner(blockSyntaxAssertions);
                });
    }

    /// <summary>
    /// Test nested mapping does not reuse a compatible method when the setting is disabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotMapUsingCompatibleMethodWhenDisabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class BaseSource { public int A { get; set; } }
                                  public class DerivedSource : BaseSource { }

                                  public class BaseTarget { public int A { get; set; } }
                                  public class DerivedTarget : BaseTarget { }

                                  public class Source { public DerivedSource Property { get; set; } }
                                  public class Target { public BaseTarget Property { get; set; } }

                                  [Mappa]
                                  [MappaSettings(CompatibleMapMethod = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      public DerivedTarget MapInner(BaseSource input)
                                      {
                                          return new DerivedTarget { A = input.A };
                                      }

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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                NullableSetup.Enable,
                blockSyntaxAssertions =>
                {
                    AssertConstructorMappingWithoutMapInner(blockSyntaxAssertions);
                });
    }

    /// <summary>
    /// Test nested mapping reuses a compatible method whose parameter is an interface implemented by the source.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCompatibleMethodWithInterfaceParameter()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public interface ISource { int A { get; } }
                                  public class DerivedSource : ISource { public int A { get; set; } }

                                  public class BaseTarget { public int A { get; set; } }
                                  public class DerivedTarget : BaseTarget { }

                                  public class Source { public DerivedSource Property { get; set; } }
                                  public class Target { public BaseTarget Property { get; set; } }

                                  [Mappa]
                                  [MappaSettings(CompatibleMapMethod = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public DerivedTarget MapInner(ISource input)
                                      {
                                          return new DerivedTarget { A = input.A };
                                      }

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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                NullableSetup.Enable,
                blockSyntaxAssertions =>
                {
                    AssertCompatibleMapInnerInvocation(blockSyntaxAssertions);
                });
    }

    /// <summary>
    /// Test nested mapping prefers an exact map method over a compatible map method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PrefersExactMapMethodOverCompatibleMapMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class BaseSource { public int A { get; set; } }
                                  public class DerivedSource : BaseSource { }

                                  public class BaseTarget { public int A { get; set; } }
                                  public class DerivedTarget : BaseTarget { }

                                  public class Source { public DerivedSource Property { get; set; } }
                                  public class Target { public BaseTarget Property { get; set; } }

                                  [Mappa]
                                  [MappaSettings(CompatibleMapMethod = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public DerivedTarget MapCompatible(BaseSource input)
                                      {
                                          return new DerivedTarget { A = input.A };
                                      }

                                      public BaseTarget MapExact(DerivedSource input)
                                      {
                                          return new BaseTarget { A = input.A };
                                      }

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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                NullableSetup.Enable,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.DerivedSource",
                            "__mappa_tmp_1",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.BaseTarget",
                            "__mappa_tmp_2",
                            initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax(
                                "this.MapExact",
                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_3",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3"));
                });
    }

    /// <summary>
    /// Test a compatible method that requires <see cref="Mappa.MappaContext"/> is not reused when the root method does not provide one.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotMapUsingCompatibleMethodWhenContextIsRequiredButNotProvided()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class BaseSource { public int A { get; set; } }
                                  public class DerivedSource : BaseSource { }

                                  public class BaseTarget { public int A { get; set; } }
                                  public class DerivedTarget : BaseTarget { }

                                  public class Source { public DerivedSource Property { get; set; } }
                                  public class Target { public BaseTarget Property { get; set; } }

                                  [Mappa]
                                  [MappaSettings(CompatibleMapMethod = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public DerivedTarget MapInner(BaseSource input, MappaContext context)
                                      {
                                          return new DerivedTarget { A = input.A };
                                      }

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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                NullableSetup.Enable,
                blockSyntaxAssertions =>
                {
                    AssertConstructorMappingWithoutMapInner(blockSyntaxAssertions);
                });
    }

    /// <summary>
    /// Test a compatible method that requires <see cref="Mappa.MappaContext"/> is reused when the root method provides one.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCompatibleMethodWhenContextIsProvided()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class BaseSource { public int A { get; set; } }
                                  public class DerivedSource : BaseSource { }

                                  public class BaseTarget { public int A { get; set; } }
                                  public class DerivedTarget : BaseTarget { }

                                  public class Source { public DerivedSource Property { get; set; } }
                                  public class Target { public BaseTarget Property { get; set; } }

                                  [Mappa]
                                  [MappaSettings(CompatibleMapMethod = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public DerivedTarget MapInner(BaseSource input, MappaContext context)
                                      {
                                          return new DerivedTarget { A = input.A };
                                      }

                                      public partial Target Map(Source input, MappaContext context);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.DerivedSource",
                            "__mappa_tmp_1",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.BaseTarget",
                            "__mappa_tmp_2",
                            initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax(
                                "this.MapInner",
                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("context"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_3",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3"));
                });
    }

    /// <summary>
    /// Test nested mapping reuses a compatible method when enabled via <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCompatibleMethodWhenEnabledInEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.compatiblemapmethod = enable
                                    """;

        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class BaseSource { public int A { get; set; } }
                                  public class DerivedSource : BaseSource { }

                                  public class BaseTarget { public int A { get; set; } }
                                  public class DerivedTarget : BaseTarget { }

                                  public class Source { public DerivedSource Property { get; set; } }
                                  public class Target { public BaseTarget Property { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public DerivedTarget MapInner(BaseSource input)
                                      {
                                          return new DerivedTarget { A = input.A };
                                      }

                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                NullableSetup.Enable,
                blockSyntaxAssertions =>
                {
                    AssertCompatibleMapInnerInvocation(blockSyntaxAssertions);
                });
    }

    private static void AssertCompatibleMapInnerInvocation(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(4)
            .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                "Mappa.Generator.Tests.UnitTests.SourceCode.DerivedSource",
                "__mappa_tmp_1",
                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
            .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                "Mappa.Generator.Tests.UnitTests.SourceCode.BaseTarget",
                "__mappa_tmp_2",
                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax(
                    "this.MapInner",
                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
            .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                "__mappa_tmp_3",
                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                    ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
            .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3"));
    }

    private static void AssertConstructorMappingWithoutMapInner(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(5)
            .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                "Mappa.Generator.Tests.UnitTests.SourceCode.DerivedSource",
                "__mappa_tmp_1",
                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
            .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                "int",
                "__mappa_tmp_2",
                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.A")))
            .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                "Mappa.Generator.Tests.UnitTests.SourceCode.BaseTarget",
                "__mappa_tmp_3",
                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                    "Mappa.Generator.Tests.UnitTests.SourceCode.BaseTarget",
                    ("A", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
            .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                "__mappa_tmp_4",
                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                    ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
            .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4"));
    }
}