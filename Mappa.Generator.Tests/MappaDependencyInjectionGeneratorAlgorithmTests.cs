// <copyright file="MappaDependencyInjectionGeneratorAlgorithmTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Mappa.Generator.Algorithm;
using Mappa.Generator.Tests.Abstractions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaDependencyInjectionGeneratorAlgorithm"/>.
/// </summary>
public sealed class MappaDependencyInjectionGeneratorAlgorithmTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// When a candidate class has no <c>[MappaDependencyInjection]</c> attribute data,
    /// the algorithm returns without emitting sources.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ExecuteSkipsClassWithoutDependencyInjectionAttributeData()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public static partial class Registrar
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var classDeclaration = GetClassDeclaration(compilation, "Registrar");

        var runResult = RunAlgorithm(compilation, [classDeclaration]);

        runResult.Diagnostics.Should().BeEmpty();
        runResult.GeneratedTrees.Should().BeEmpty();
    }

    /// <summary>
    /// Null entries in the candidate array are skipped while valid registrars still generate.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ExecuteSkipsNullClassDeclarations()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaDependencyInjection]
                              public static partial class Registrar
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var classDeclaration = GetClassDeclaration(compilation, "Registrar");

        var runResult = RunAlgorithm(compilation, [null, classDeclaration]);

        runResult.Diagnostics.Should().BeEmpty();
        runResult.GeneratedTrees.Should().HaveCount(1);
    }

    /// <summary>
    /// When the declared class symbol cannot be resolved, the algorithm returns without emitting sources.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ExecuteSkipsClassWhenDeclaredSymbolCannotBeResolved()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaDependencyInjection]
                              public static partial class Registrar
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var classDeclaration = GetClassDeclaration(compilation, "Registrar");

        var runResult = RunAlgorithm(
            compilation,
            [classDeclaration],
            static (_, _, _) => null);

        runResult.Diagnostics.Should().BeEmpty();
        runResult.GeneratedTrees.Should().BeEmpty();
    }

    private static ClassDeclarationSyntax GetClassDeclaration(Compilation compilation, string className)
        => compilation.SyntaxTrees
            .SelectMany(syntaxTree => syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>())
            .Single(declaration => declaration.Identifier.Text == className);

    private static GeneratorDriverRunResult RunAlgorithm(
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax?> classDeclarations,
        Func<Compilation, ClassDeclarationSyntax, CancellationToken, INamedTypeSymbol?>? classSymbolResolver = null)
    {
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new DependencyInjectionAlgorithmHarness(classDeclarations, classSymbolResolver).AsSourceGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out _,
            out _,
            TestContext.Current.CancellationToken);
        return driver.GetRunResult();
    }

    /// <summary>
    /// Harness generator that runs <see cref="MappaDependencyInjectionGeneratorAlgorithm"/>
    /// against a fixed set of class declarations.
    /// </summary>
    private sealed class DependencyInjectionAlgorithmHarness
        : IIncrementalGenerator
    {
        private readonly ImmutableArray<ClassDeclarationSyntax?> classDeclarations;
        private readonly Func<Compilation, ClassDeclarationSyntax, CancellationToken, INamedTypeSymbol?>? classSymbolResolver;

        public DependencyInjectionAlgorithmHarness(
            ImmutableArray<ClassDeclarationSyntax?> classDeclarations,
            Func<Compilation, ClassDeclarationSyntax, CancellationToken, INamedTypeSymbol?>? classSymbolResolver = null)
        {
            this.classDeclarations = classDeclarations;
            this.classSymbolResolver = classSymbolResolver;
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterSourceOutput(
                context.CompilationProvider,
                (productionContext, compilation) =>
                {
                    new MappaDependencyInjectionGeneratorAlgorithm(
                            productionContext,
                            compilation,
                            this.classDeclarations,
                            this.classSymbolResolver)
                        .Execute();
                });
        }
    }
}