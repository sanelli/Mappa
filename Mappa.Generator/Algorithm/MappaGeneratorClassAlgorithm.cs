// <copyright file="MappaGeneratorClassAlgorithm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

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
    public SourceProductionContext Context { get; }

    /// <summary>
    /// Gets the analyzer config options provider.
    /// </summary>
    public AnalyzerConfigOptionsProvider AnalyzerConfigOptionsProvider { get; }

    /// <summary>
    /// Gets the compilation.
    /// </summary>
    public Compilation Compilation { get; }

    /// <summary>
    /// Gets the class declaration syntaxes that can be  used to generate a mapper..
    /// </summary>
    public ImmutableArray<ClassDeclarationSyntax?> ClassDeclarationSyntaxes { get; }

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
        foreach (var classDeclarationSynax in this.ClassDeclarationSyntaxes)
        {
            // Stop if the operation has been cancelled
            cancellationToken.ThrowIfCancellationRequested();

            // Skip null class declaration syntaxes.
            if (classDeclarationSynax is null)
            {
                continue;
            }

            // Build the class generator context.
            var classContext = new MappaClassGeneratorContext(options, this.Compilation, classDeclarationSynax);

            // Gather all the methods that require a mapping.
            foreach (var methodDeclarationSyntax in classDeclarationSynax.ChildNodes().OfType<MethodDeclarationSyntax>())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!methodDeclarationSyntax.IsPartial())
                {
                    continue;
                }

                if (!methodDeclarationSyntax.HasArity(1))
                {
                    classContext.ReportDiagnostic(MappaDiagnostics.MethodHasInvalidNumberOfParameters(methodDeclarationSyntax));
                    continue;
                }

                var mapMethod = new MapMethod(methodDeclarationSyntax, classContext.SemanticModel, cancellationToken);

                if (!mapMethod.MethodSymbol.IsVoid())
                {
                    classContext.ReportDiagnostic(MappaDiagnostics.MethodIsVoid(methodDeclarationSyntax));
                    continue;
                }

                classContext.TryAddMethod(mapMethod);
            }

            // TODO: Add all methods from references classes and mark them as mapped.

            // Identify the strategy for each method.
            while (!classContext.AreAllMethodsMapped())
            {
                foreach (var mapMethod in classContext.MapMethods)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (mapMethod.Mapped)
                    {
                        continue;
                    }

                    var methodContext = new MappaMethodGeneratorContext(classContext, mapMethod);
                    var typeIdentifierAlgorithm = new TypeMapIdentifierAlgorithm(methodContext, mapMethod.TargetType, mapMethod.SourceType, mapMethod.SourceParameterName);
                    var strategy = typeIdentifierAlgorithm.GetStrategy();
                    var methodParameterMapStrategy = new MethodParameterMapStrategy(strategy);
                    mapMethod.SetStrategy(methodParameterMapStrategy);
                    mapMethod.MarkMapped();
                }
            }

            // TODO: Generate the target code for the class.

            // Report the diagnostics.
            foreach (var diagnostic in classContext.Diagnostics)
            {
                this.Context.ReportDiagnostic(diagnostic);
            }
        }
    }
}