// <copyright file="MaxRuntimeDepthIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Reflection;

using AwesomeAssertions;

using Mappa.Generator.Helpers;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <c>MaxRuntimeDepth</c> codegen (<c>MaxDepth</c> init,
/// nested <c>IncreaseDepth</c> wraps) and runtime overflow behavior.
/// </summary>
public sealed class MaxRuntimeDepthIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string Ns = "Mappa.Generator.Tests.UnitTests.SourceCode";
    private const string Level2SourceType = $"{Ns}.Level2Source";
    private const string Level2TargetType = $"{Ns}.Level2Target";
    private const string Level1SourceType = $"{Ns}.Level1Source";
    private const string Level1TargetType = $"{Ns}.Level1Target";
    private const string Level0SourceType = $"{Ns}.Level0Source";
    private const string Level0TargetType = $"{Ns}.Level0Target";

    private const string ThreeLevelTypes = """
                                           public class Level2Source
                                           {
                                               public int Value { get; set; }
                                           }

                                           public class Level2Target
                                           {
                                               public int Value { get; set; }
                                           }

                                           public class Level1Source
                                           {
                                               public Level2Source Child { get; set; } = null!;
                                           }

                                           public class Level1Target
                                           {
                                               public Level2Target Child { get; set; } = null!;
                                           }

                                           public class Level0Source
                                           {
                                               public Level1Source Child { get; set; } = null!;
                                           }

                                           public class Level0Target
                                           {
                                               public Level1Target Child { get; set; } = null!;
                                           }
                                           """;

    private static string ReferenceManager
        => $"{ReferenceHandlingCodeGenerator.AccessorTypeName}.{ReferenceHandlingCodeGenerator.AccessorMethodName}(context)";

    /// <summary>
    /// Nested reference-type mappings are wrapped in <c>using (IncreaseDepth())</c>
    /// and the root assigns <c>MaxDepth</c> when <c>MaxRuntimeDepth</c> is greater than zero.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NestedMappingWrapsIncreaseDepthAndAssignsMaxDepth()
    {
        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{ThreeLevelTypes}}

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaSettings(MaxRuntimeDepth = 2)]
                               public partial Level0Target Map(Level0Source input, MappaContext context);
                           }
                           #nullable restore
                           """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .NotHaveCompilationErrors()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                Level0TargetType,
                NullableAnnotation.NotAnnotated,
                Level0SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions => AssertThreeLevelMaxRuntimeDepthMap(blockSyntaxAssertions, 2));
    }

    /// <summary>
    /// Exceeding <c>MaxRuntimeDepth</c> at runtime throws <see cref="MappaException"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxRuntimeDepthOverflowThrowsMappaException()
    {
        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{ThreeLevelTypes}}

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaSettings(MaxRuntimeDepth = 1)]
                               public partial Level0Target Map(Level0Source input, MappaContext context);
                           }
                           #nullable restore
                           """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert syntax
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .NotHaveCompilationErrors()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                Level0TargetType,
                NullableAnnotation.NotAnnotated,
                Level0SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions => AssertThreeLevelMaxRuntimeDepthMap(blockSyntaxAssertions, 1));

        // Assert runtime overflow
        var assembly = CompileToAssembly(generatedResults.OutputCompilation);
        var (mapper, mapMethod, input, context) = CreateThreeLevelRuntimeFixture(assembly, value: 42);

        var act = () => mapMethod.Invoke(mapper, [input, context]);

        act.Should().Throw<TargetInvocationException>()
            .WithInnerException<MappaException>()
            .WithMessage("The maximum runtime mapping depth of 1 has been reached.");
    }

    /// <summary>
    /// Mapping succeeds when nesting stays within <c>MaxRuntimeDepth</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxRuntimeDepthWithinLimitSucceeds()
    {
        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{ThreeLevelTypes}}

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaSettings(MaxRuntimeDepth = 2)]
                               public partial Level0Target Map(Level0Source input, MappaContext context);
                           }
                           #nullable restore
                           """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .NotHaveCompilationErrors();

        var assembly = CompileToAssembly(generatedResults.OutputCompilation);
        var (mapper, mapMethod, input, context) = CreateThreeLevelRuntimeFixture(assembly, value: 42);

        // Act
        var result = mapMethod.Invoke(mapper, [input, context]);

        // Assert
        result.Should().NotBeNull();
        var level0TargetType = assembly.GetType(Level0TargetType)
            ?? throw new InvalidOperationException("Level0Target type was not found.");
        var level1TargetType = assembly.GetType(Level1TargetType)
            ?? throw new InvalidOperationException("Level1Target type was not found.");
        var level2TargetType = assembly.GetType(Level2TargetType)
            ?? throw new InvalidOperationException("Level2Target type was not found.");

        var child1 = level0TargetType.GetProperty("Child")!.GetValue(result);
        child1.Should().NotBeNull().And.BeOfType(level1TargetType);
        var child2 = level1TargetType.GetProperty("Child")!.GetValue(child1);
        child2.Should().NotBeNull().And.BeOfType(level2TargetType);
        level2TargetType.GetProperty("Value")!.GetValue(child2).Should().Be(42);
    }

    /// <summary>
    /// Method-level <c>MaxRuntimeDepth</c> overrides the class setting on a shared <see cref="MappaContext"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MethodMaxRuntimeDepthOverridesClassOnSharedContext()
    {
        // Arrange — distinct source/target pairs (MP00005 forbids duplicate type-pair maps).
        const string deepLevel0Source = $"{Ns}.DeepLevel0Source";
        const string deepLevel0Target = $"{Ns}.DeepLevel0Target";
        const string shallowLevel0Source = $"{Ns}.ShallowLevel0Source";
        const string shallowLevel0Target = $"{Ns}.ShallowLevel0Target";

        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           public class DeepLevel2Source { public int Value { get; set; } }
                           public class DeepLevel2Target { public int Value { get; set; } }
                           public class DeepLevel1Source { public DeepLevel2Source Child { get; set; } = null!; }
                           public class DeepLevel1Target { public DeepLevel2Target Child { get; set; } = null!; }
                           public class DeepLevel0Source { public DeepLevel1Source Child { get; set; } = null!; }
                           public class DeepLevel0Target { public DeepLevel1Target Child { get; set; } = null!; }

                           public class ShallowLevel2Source { public int Value { get; set; } }
                           public class ShallowLevel2Target { public int Value { get; set; } }
                           public class ShallowLevel1Source { public ShallowLevel2Source Child { get; set; } = null!; }
                           public class ShallowLevel1Target { public ShallowLevel2Target Child { get; set; } = null!; }
                           public class ShallowLevel0Source { public ShallowLevel1Source Child { get; set; } = null!; }
                           public class ShallowLevel0Target { public ShallowLevel1Target Child { get; set; } = null!; }

                           [Mappa]
                           [MappaSettings(MaxRuntimeDepth = 5)]
                           public sealed partial class Mapper
                           {
                               public partial DeepLevel0Target MapDeep(DeepLevel0Source input, MappaContext context);

                               [MappaSettings(MaxRuntimeDepth = 1)]
                               public partial ShallowLevel0Target MapShallow(ShallowLevel0Source input, MappaContext context);
                           }
                           #nullable restore
                           """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert syntax: each method initializes its own MaxDepth
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .NotHaveCompilationErrors()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                "MapDeep",
                [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
                false,
                deepLevel0Target,
                NullableAnnotation.NotAnnotated,
                "input",
                deepLevel0Source,
                "context",
                NullableAnnotation.NotAnnotated,
                RefKind.None,
                false,
                RefKind.None,
                2,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions => AssertThreeLevelMaxRuntimeDepthMap(
                    blockSyntaxAssertions,
                    5,
                    $"{Ns}.DeepLevel2Source",
                    $"{Ns}.DeepLevel2Target",
                    $"{Ns}.DeepLevel1Source",
                    $"{Ns}.DeepLevel1Target",
                    deepLevel0Target,
                    temporaryOffset: 0))
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                "MapShallow",
                [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
                false,
                shallowLevel0Target,
                NullableAnnotation.NotAnnotated,
                "input",
                shallowLevel0Source,
                "context",
                NullableAnnotation.NotAnnotated,
                RefKind.None,
                false,
                RefKind.None,
                2,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions => AssertThreeLevelMaxRuntimeDepthMap(
                    blockSyntaxAssertions,
                    1,
                    $"{Ns}.ShallowLevel2Source",
                    $"{Ns}.ShallowLevel2Target",
                    $"{Ns}.ShallowLevel1Source",
                    $"{Ns}.ShallowLevel1Target",
                    shallowLevel0Target,
                    temporaryOffset: 8));

        // Assert runtime on shared context: deep succeeds, shallow throws
        var assembly = CompileToAssembly(generatedResults.OutputCompilation);
        var mapperType = assembly.GetType($"{Ns}.Mapper")
            ?? throw new InvalidOperationException("Mapper type was not found.");
        var mapper = Activator.CreateInstance(mapperType)
            ?? throw new InvalidOperationException("Mapper instance was not created.");
        var mapDeep = mapperType.GetMethod("MapDeep", BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException("MapDeep was not found.");
        var mapShallow = mapperType.GetMethod("MapShallow", BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException("MapShallow was not found.");

        var deepInput = CreateThreeLevelSource(
            assembly,
            $"{Ns}.DeepLevel0Source",
            $"{Ns}.DeepLevel1Source",
            $"{Ns}.DeepLevel2Source",
            value: 7);
        var shallowInput = CreateThreeLevelSource(
            assembly,
            $"{Ns}.ShallowLevel0Source",
            $"{Ns}.ShallowLevel1Source",
            $"{Ns}.ShallowLevel2Source",
            value: 7);
        var context = new MappaContext();

        var deepResult = mapDeep.Invoke(mapper, [deepInput, context]);
        deepResult.Should().NotBeNull();

        var shallowAct = () => mapShallow.Invoke(mapper, [shallowInput, context]);
        shallowAct.Should().Throw<TargetInvocationException>()
            .WithInnerException<MappaException>()
            .WithMessage("The maximum runtime mapping depth of 1 has been reached.");
    }

    private static void AssertThreeLevelMaxRuntimeDepthMap(BlockSyntaxAssertions blockSyntaxAssertions, int maxDepth)
        => AssertThreeLevelMaxRuntimeDepthMap(
            blockSyntaxAssertions,
            maxDepth,
            Level2SourceType,
            Level2TargetType,
            Level1SourceType,
            Level1TargetType,
            Level0TargetType,
            temporaryOffset: 0);

    private static void AssertThreeLevelMaxRuntimeDepthMap(
        BlockSyntaxAssertions blockSyntaxAssertions,
        int maxDepth,
        string level2SourceType,
        string level2TargetType,
        string level1SourceType,
        string level1TargetType,
        string level0TargetType,
        int temporaryOffset)
    {
        string Tmp(int index) => $"__mappa_tmp_{index + temporaryOffset}";

        blockSyntaxAssertions
            .HasSyntaxNodesCount(6)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeAssignmentExpressionStatement(
                    leftExpressionAssertions => leftExpressionAssertions.BeMemberAccessExpressionSyntax($"{ReferenceManager}.MaxDepth"),
                    rightExpressionAssertions => rightExpressionAssertions.BeLiteralExpressionSyntax(maxDepth));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    level1SourceType,
                    Tmp(1),
                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Child"));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(level1TargetType, Tmp(7));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeUsingStatementSyntax(
                    expressionAssertions => expressionAssertions.BeInvocationExpressionSyntax($"{ReferenceManager}.IncreaseDepth"),
                    statementAssertions =>
                    {
                        statementAssertions
                            .BeBlockStatement()
                            .AsBlock()
                            .HasSyntaxNodesCount(5)
                            .HasNextSyntaxNode(outerSyntaxNodeAssertions =>
                            {
                                outerSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                    level2SourceType,
                                    Tmp(2),
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax($"{Tmp(1)}.Child"));
                            })
                            .HasNextSyntaxNode(outerSyntaxNodeAssertions =>
                            {
                                outerSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(level2TargetType, Tmp(5));
                            })
                            .HasNextSyntaxNode(outerSyntaxNodeAssertions =>
                            {
                                outerSyntaxNodeAssertions.BeUsingStatementSyntax(
                                    expressionAssertions => expressionAssertions.BeInvocationExpressionSyntax($"{ReferenceManager}.IncreaseDepth"),
                                    statementAssertions =>
                                    {
                                        statementAssertions
                                            .BeBlockStatement()
                                            .AsBlock()
                                            .HasSyntaxNodesCount(3)
                                            .HasNextSyntaxNode(innerSyntaxNodeAssertions =>
                                            {
                                                innerSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                    typeof(int).ToString(),
                                                    Tmp(3),
                                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax($"{Tmp(2)}.Value"));
                                            })
                                            .HasNextSyntaxNode(innerSyntaxNodeAssertions =>
                                            {
                                                innerSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                    level2TargetType,
                                                    Tmp(4),
                                                    initializationAssertions =>
                                                    {
                                                        initializationAssertions.BeObjectCreationExpressionSyntax(
                                                            level2TargetType,
                                                            ("Value", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax(Tmp(3))));
                                                    });
                                            })
                                            .HasNextSyntaxNode(innerSyntaxNodeAssertions =>
                                            {
                                                innerSyntaxNodeAssertions.BeAssignmentExpressionStatement(
                                                    Tmp(5),
                                                    rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax(Tmp(4)));
                                            });
                                    });
                            })
                            .HasNextSyntaxNode(outerSyntaxNodeAssertions =>
                            {
                                outerSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                    level1TargetType,
                                    Tmp(6),
                                    initializationAssertions =>
                                    {
                                        initializationAssertions.BeObjectCreationExpressionSyntax(
                                            level1TargetType,
                                            ("Child", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax(Tmp(5))));
                                    });
                            })
                            .HasNextSyntaxNode(outerSyntaxNodeAssertions =>
                            {
                                outerSyntaxNodeAssertions.BeAssignmentExpressionStatement(
                                    Tmp(7),
                                    rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax(Tmp(6)));
                            });
                    });
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    level0TargetType,
                    Tmp(8),
                    initializationAssertions =>
                    {
                        initializationAssertions.BeObjectCreationExpressionSyntax(
                            level0TargetType,
                            ("Child", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax(Tmp(7))));
                    });
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(Tmp(8)));
            });
    }

    private static (object Mapper, MethodInfo MapMethod, object Input, MappaContext Context) CreateThreeLevelRuntimeFixture(
        Assembly assembly,
        int value)
    {
        var mapperType = assembly.GetType($"{Ns}.Mapper")
            ?? throw new InvalidOperationException("Mapper type was not found.");
        var mapper = Activator.CreateInstance(mapperType)
            ?? throw new InvalidOperationException("Mapper instance was not created.");
        var mapMethod = mapperType.GetMethod("Map", BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException("Map method was not found.");
        var input = CreateThreeLevelSource(assembly, Level0SourceType, Level1SourceType, Level2SourceType, value);
        return (mapper, mapMethod, input, new MappaContext());
    }

    private static object CreateThreeLevelSource(
        Assembly assembly,
        string level0SourceTypeName,
        string level1SourceTypeName,
        string level2SourceTypeName,
        int value)
    {
        var level0SourceType = assembly.GetType(level0SourceTypeName)
            ?? throw new InvalidOperationException($"{level0SourceTypeName} was not found.");
        var level1SourceType = assembly.GetType(level1SourceTypeName)
            ?? throw new InvalidOperationException($"{level1SourceTypeName} was not found.");
        var level2SourceType = assembly.GetType(level2SourceTypeName)
            ?? throw new InvalidOperationException($"{level2SourceTypeName} was not found.");

        var level2 = Activator.CreateInstance(level2SourceType)
            ?? throw new InvalidOperationException($"{level2SourceTypeName} instance was not created.");
        level2SourceType.GetProperty("Value")!.SetValue(level2, value);

        var level1 = Activator.CreateInstance(level1SourceType)
            ?? throw new InvalidOperationException($"{level1SourceTypeName} instance was not created.");
        level1SourceType.GetProperty("Child")!.SetValue(level1, level2);

        var level0 = Activator.CreateInstance(level0SourceType)
            ?? throw new InvalidOperationException($"{level0SourceTypeName} instance was not created.");
        level0SourceType.GetProperty("Child")!.SetValue(level0, level1);

        return level0;
    }

    private static Assembly CompileToAssembly(Compilation compilation)
    {
        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream);
        emitResult.Success.Should().BeTrue(string.Join(Environment.NewLine, emitResult.Diagnostics));
        return Assembly.Load(stream.ToArray());
    }
}