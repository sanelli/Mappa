// <copyright file="MappaGenerator.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Mappa.Generator.Algorithm;
using Mappa.Generator.Extensions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mappa.Generator;

/// <summary>
/// The mappa incremental generator.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class MappaGenerator
    : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Step 1. Identify the classes exposing the [Mappa] attribute.
        var classDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
                IsPartialClassDeclarationWithAttributes,
                GetSemanticTargetForGeneration)
            .Where(CommonExtensions.IsNotNull);

        // Step 2.Get the analizer options, the compilation provider and the classes
        var analizerConfigOptionsCompilationAndClasses = context
            .AnalyzerConfigOptionsProvider
            .Combine(context
                .CompilationProvider
                .Combine(classDeclarations.Collect()));

        // Step 3. Register the source output to generate the code.
        context.RegisterSourceOutput(analizerConfigOptionsCompilationAndClasses, Execute);
    }

    private static bool IsPartialClassDeclarationWithAttributes(SyntaxNode syntaxNode, CancellationToken cancellationToken)
        => syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
            classDeclarationSyntax.AttributeLists.Any() &&
            classDeclarationSyntax.IsPartial();

    private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        => context.Node is ClassDeclarationSyntax classDeclarationSyntax &&
            classDeclarationSyntax.HasMappaAttribute(context.SemanticModel, cancellationToken)
                ? classDeclarationSyntax
                : null;

    private static void Execute(SourceProductionContext context, (AnalyzerConfigOptionsProvider AnalyzerConfigOptionsProvider, (Compilation Compilation, ImmutableArray<ClassDeclarationSyntax?> ClassDeclarationSyntaxes) Right) settings)
        => Execute(context, settings.AnalyzerConfigOptionsProvider, settings.Right.Compilation, settings.Right.ClassDeclarationSyntaxes);

    private static void Execute(SourceProductionContext context, AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider, Compilation compilation, ImmutableArray<ClassDeclarationSyntax?> classDeclarationSyntaxes)
        => new MappaGeneratorClassAlgorithm(context, analyzerConfigOptionsProvider, compilation, classDeclarationSyntaxes)
            .Execute();
}