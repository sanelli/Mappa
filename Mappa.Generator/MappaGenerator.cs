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
        // Step 1. Identify partial classes exposing the [Mappa] attribute.
        var classDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(
                AttributeDataExtensions.MappaAttributeFullName,
                static (node, _) => node is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.IsPartial(),
                static (attributeContext, _) => attributeContext.TargetNode as ClassDeclarationSyntax)
            .Where(CommonExtensions.IsNotNull);

        // Step 2.Get the analyzer options, the compilation provider and the classes
        var analyzerConfigOptionsCompilationAndClasses = context
            .AnalyzerConfigOptionsProvider
            .Combine(context
                .CompilationProvider
                .Combine(classDeclarations.Collect()));

        // Step 3. Register the source output to generate the code.
        context.RegisterSourceOutput(analyzerConfigOptionsCompilationAndClasses, Execute);

        // Step 4. Identify partial classes exposing [MappaDependencyInjection].
        var dependencyInjectionClassDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(
                AttributeDataExtensions.MappaDependencyInjectionAttributeFullName,
                static (node, _) => node is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.IsPartial(),
                static (attributeContext, _) => attributeContext.TargetNode as ClassDeclarationSyntax)
            .Where(CommonExtensions.IsNotNull);

        // Step 5. Register the source output for dependency injection registration methods.
        var compilationAndDependencyInjectionClasses = context
            .CompilationProvider
            .Combine(dependencyInjectionClassDeclarations.Collect());
        context.RegisterSourceOutput(compilationAndDependencyInjectionClasses, ExecuteDependencyInjection);
    }

    private static void Execute(SourceProductionContext context, (AnalyzerConfigOptionsProvider AnalyzerConfigOptionsProvider, (Compilation Compilation, ImmutableArray<ClassDeclarationSyntax?> ClassDeclarationSyntaxes) Right) settings)
        => Execute(context, settings.AnalyzerConfigOptionsProvider, settings.Right.Compilation, settings.Right.ClassDeclarationSyntaxes);

    private static void Execute(SourceProductionContext context, AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider, Compilation compilation, ImmutableArray<ClassDeclarationSyntax?> classDeclarationSyntaxes)
        => new MappaGeneratorClassAlgorithm(context, analyzerConfigOptionsProvider, compilation, classDeclarationSyntaxes)
            .Execute();

    private static void ExecuteDependencyInjection(
        SourceProductionContext context,
        (Compilation Compilation, ImmutableArray<ClassDeclarationSyntax?> ClassDeclarationSyntaxes) settings)
        => new MappaDependencyInjectionGeneratorAlgorithm(context, settings.Compilation, settings.ClassDeclarationSyntaxes)
            .Execute();
}