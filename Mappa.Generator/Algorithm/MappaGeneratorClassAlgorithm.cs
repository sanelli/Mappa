// <copyright file="MappaGeneratorClassAlgorithm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Mappa.Generator.Builders;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mappa.Generator.Algorithm;

/// <summary>
/// Describe the algorithm used to generate a mapper class.
/// </summary>
internal sealed class MappaGeneratorClassAlgorithm
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaGeneratorClassAlgorithm"/> class.
    /// </summary>
    /// <param name="context">The source production context.</param>
    /// <param name="analyzerConfigOptionsProvider">The analyzer settings.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="classDeclarationSyntaxes">The class declaration syntaxes that can be used.</param>
    public MappaGeneratorClassAlgorithm(
        SourceProductionContext context,
        AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider,
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax?> classDeclarationSyntaxes)
    {
        this.Context = context;
        this.AnalyzerConfigOptionsProvider = analyzerConfigOptionsProvider;
        this.Compilation = compilation;
        this.ClassDeclarationSyntaxes = classDeclarationSyntaxes;
    }

    /// <summary>
    /// Gets the source production context.
    /// </summary>
    private SourceProductionContext Context { get; }

    /// <summary>
    /// Gets the analyzer config options provider.
    /// </summary>
    private AnalyzerConfigOptionsProvider AnalyzerConfigOptionsProvider { get; }

    /// <summary>
    /// Gets the compilation.
    /// </summary>
    private Compilation Compilation { get; }

    /// <summary>
    /// Gets the class declaration syntaxes that can be  used to generate a mapper..
    /// </summary>
    private ImmutableArray<ClassDeclarationSyntax?> ClassDeclarationSyntaxes { get; }

    /// <summary>
    /// Execute the algorithm and produces the sources.
    /// </summary>
    internal void Execute()
    {
        var cancellationToken = this.Context.CancellationToken;
        var options = new MappaGlobalOptions();

        // For each class generate the mapper source code.
        // At this point we know that the class declaration syntax is partial
        // and has the [Mappa] attribute.
        foreach (var classDeclarationSyntax in this.ClassDeclarationSyntaxes)
        {
            // Stop if the operation has been cancelled
            cancellationToken.ThrowIfCancellationRequested();

            // Skip null class declaration syntaxes.
            if (classDeclarationSyntax is null)
            {
                return;
            }

            // Execute for a single class.
            this.ExecuteForSingleClass(classDeclarationSyntax, options, cancellationToken);
        }
    }

    private static void GenerateStrategyForEachMethod(
        MappaClassGeneratorContext classContext,
        CancellationToken cancellationToken)
    {
        foreach (var mapMethod in classContext.MapMethods)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (mapMethod.Mapped)
            {
                continue;
            }

            var methodContext = new MappaMethodGeneratorContext(classContext, mapMethod);
            var typeIdentifierAlgorithm = new TypeMapIdentifierAlgorithm(
                methodContext,
                mapMethod.TargetType,
                mapMethod.SourceType,
                mapMethod.SourceParameterName);
            var strategy = typeIdentifierAlgorithm.GetStrategy();
            var methodParameterMapStrategy = new MethodParameterMapStrategy(strategy);
            mapMethod.SetStrategy(methodParameterMapStrategy);
            mapMethod.MarkMapped();
        }
    }

    private void ExecuteForSingleClass(
        ClassDeclarationSyntax classDeclarationSyntax,
        MappaGlobalOptions options,
        CancellationToken cancellationToken)
    {
        // Build the class generator context.
        var classContext = new MappaClassGeneratorContext(options, this.Compilation, classDeclarationSyntax);

        // Gather all the methods that require a mapping.
        foreach (var methodDeclarationSyntax in classDeclarationSyntax.ChildNodes().OfType<MethodDeclarationSyntax>())
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.AcceptMapMethod(methodDeclarationSyntax, classContext, cancellationToken);
        }

        // TODO: Add all methods from references classes and mark them as mapped.

        // Identify the strategy for each method.
        // While generating strategies new methods might be found or requested to be generated.
        while (!classContext.AreAllMethodsMapped())
        {
            GenerateStrategyForEachMethod(classContext, cancellationToken);
        }

        // Build the source code (only if there is something to generate)
        this.GenerateSourceCode(classContext);

        // Report the diagnostics.
        this.ReportAllDiagnostics(classContext);
    }

    private void ReportAllDiagnostics(MappaClassGeneratorContext classContext)
    {
        foreach (var diagnostic in classContext.Diagnostics)
        {
            this.Context.ReportDiagnostic(diagnostic);
        }
    }

    private void GenerateSourceCode(MappaClassGeneratorContext classContext)
    {
        if (!classContext.MapMethods.Any(mapMethod => mapMethod.HasStrategy))
        {
            return;
        }

        var builder = new MappaFileBuilder(classContext);
        var hintName = builder.HintName;
        var sourceFile = builder.BuildSource();
        this.Context.AddSource(hintName, sourceFile);
    }

    private void AcceptMapMethod(
        MethodDeclarationSyntax methodDeclarationSyntax,
        MappaClassGeneratorContext classContext,
        CancellationToken cancellationToken)
    {
        if (!methodDeclarationSyntax.IsPartial())
        {
            return;
        }

        if (!methodDeclarationSyntax.HasArity(1))
        {
            classContext.ReportDiagnostic(MappaDiagnostics.MethodHasInvalidNumberOfParameters(methodDeclarationSyntax));
            return;
        }

        var mapMethod = new MapMethod(methodDeclarationSyntax, classContext.SemanticModel, cancellationToken);

        if (mapMethod.MethodSymbol.IsVoid())
        {
            classContext.ReportDiagnostic(MappaDiagnostics.MethodIsVoid(methodDeclarationSyntax));
            return;
        }

        if (mapMethod.MethodSymbol.ReturnsAnyTaskType(this.Compilation))
        {
            classContext.ReportDiagnostic(MappaDiagnostics.MethodReturnsTaskType(methodDeclarationSyntax));
            return;
        }

        classContext.TryAddMethod(mapMethod);
    }
}