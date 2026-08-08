// <copyright file="MappaGeneratorClassAlgorithmTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Mappa.Generator.Algorithm;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaGeneratorClassAlgorithm"/>.
/// </summary>
public sealed class MappaGeneratorClassAlgorithmTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Null entries in the candidate array are skipped while valid mappers still generate.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ExecuteSkipsNullClassDeclarations()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial int Map(int input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var classDeclaration = GetClassDeclaration(compilation, "Mapper");

        var runResult = RunAlgorithm(compilation, [null, classDeclaration]);

        runResult.Diagnostics.Should().BeEmpty();
        runResult.GeneratedTrees.Should().HaveCount(1);
    }

    private static ClassDeclarationSyntax GetClassDeclaration(Compilation compilation, string className)
        => compilation.SyntaxTrees
            .SelectMany(syntaxTree => syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>())
            .Single(declaration => declaration.Identifier.Text == className);

    private static GeneratorDriverRunResult RunAlgorithm(
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax?> classDeclarations)
    {
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new ClassAlgorithmHarness(classDeclarations).AsSourceGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out _,
            out _,
            TestContext.Current.CancellationToken);
        return driver.GetRunResult();
    }

    /// <summary>
    /// Harness generator that runs <see cref="MappaGeneratorClassAlgorithm"/>
    /// against a fixed set of class declarations.
    /// </summary>
    private sealed class ClassAlgorithmHarness
        : IIncrementalGenerator
    {
        private readonly ImmutableArray<ClassDeclarationSyntax?> classDeclarations;

        public ClassAlgorithmHarness(ImmutableArray<ClassDeclarationSyntax?> classDeclarations)
        {
            this.classDeclarations = classDeclarations;
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterSourceOutput(
                context.CompilationProvider,
                (productionContext, compilation) =>
                {
                    new MappaGeneratorClassAlgorithm(
                            productionContext,
                            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
                            compilation,
                            this.classDeclarations)
                        .Execute();
                });
        }
    }
}