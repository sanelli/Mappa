// <copyright file="MappaGeneratorClassAlgorithm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Mappa.Generator.Builders;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Diagnostics.Debug;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

            // Rebuild options
            var options = new MappaGlobalOptions(this.AnalyzerConfigOptionsProvider, classDeclarationSyntax.SyntaxTree);
            var mappaDebug = new MappaDebug(options, this.Context.ReportDiagnostic);

            // Execute for a single class.
            this.ExecuteForSingleClass(classDeclarationSyntax, options, mappaDebug, cancellationToken);
        }
    }

    private void GenerateStrategyForEachMethod(
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
                this.Compilation,
                cancellationToken);
            var strategy = typeIdentifierAlgorithm.GetStrategy();
            var methodParameterMapStrategy = new MethodParameterMapStrategy(strategy);
            mapMethod.SetStrategy(methodParameterMapStrategy);
            mapMethod.MarkMapped();
        }
    }

    private void ExecuteForSingleClass(
        ClassDeclarationSyntax classDeclarationSyntax,
        MappaGlobalOptions options,
        MappaDebug mappaDebug,
        CancellationToken cancellationToken)
    {
        mappaDebug.Debug(
            $"Started addressing class \"{classDeclarationSyntax.Identifier.ToFullString()}\".",
            classDeclarationSyntax);

        // Build the class generator context.
        var classContext =
            new MappaClassGeneratorContext(options, mappaDebug, this.Compilation, classDeclarationSyntax);

        // Gather all the methods that require a mapping.
        foreach (var methodDeclarationSyntax in classDeclarationSyntax.ChildNodes().OfType<MethodDeclarationSyntax>())
        {
            mappaDebug.Debug(
                $"Started deciding if method class \"{classDeclarationSyntax.Identifier.ToFullString()}.{methodDeclarationSyntax.Identifier.ToString()}\" can be mapped.",
                classDeclarationSyntax);

            cancellationToken.ThrowIfCancellationRequested();

            // Try to add a method as either a method that define a mapping from Mappa
            // or as a method with already code that can be used for the mapping.
            if (!this.AcceptMapMethod(methodDeclarationSyntax, classContext, cancellationToken))
            {
                this.AcceptMapMethodAlreadyMapped(methodDeclarationSyntax, classContext, cancellationToken);
            }
        }

        // Get all accessible properties that:
        // - have a getter method
        // - have MappaDependency attribute
        foreach (var propertyDeclarationSyntax in classDeclarationSyntax.ChildNodes().OfType<PropertyDeclarationSyntax>())
        {
            if (propertyDeclarationSyntax.AccessorList is not null && propertyDeclarationSyntax.AccessorList.Accessors.Any(accessor => accessor.Kind() == SyntaxKind.GetAccessorDeclaration))
            {
                if (propertyDeclarationSyntax.AttributeLists.GetMappaDependencyAttributeSyntax(classContext.SemanticModel, cancellationToken) is null)
                {
                    continue;
                }

                var propertySymbol = classContext.SemanticModel.GetDeclaredSymbol(propertyDeclarationSyntax, cancellationToken);
                if (propertySymbol is not null)
                {
                    var accessFieldName = propertyDeclarationSyntax.Identifier.ToString();
                    if (!propertySymbol.IsStatic)
                    {
                        accessFieldName = $"this.{accessFieldName}";
                    }

                    var methods = propertySymbol.Type.GetMembers().OfType<IMethodSymbol>().ToArray();
                    foreach (var method in methods)
                    {
                        this.AcceptMapMethodFromDependency(
                            propertyDeclarationSyntax,
                            method,
                            accessFieldName,
                            classContext);
                    }
                }
            }
        }

        // Get all accessible properties that have MappaDependency attribute
        foreach (var fieldDeclarationSyntax in classDeclarationSyntax.ChildNodes().OfType<FieldDeclarationSyntax>())
        {
            if (fieldDeclarationSyntax.AttributeLists.GetMappaDependencyAttributeSyntax(classContext.SemanticModel, cancellationToken) is null)
            {
                continue;
            }

            // We only take one declaration because all variable would have anyway
            // the same type and won't make sense having multiple dependencies against
            // the same type (it would be ignored anyway when trying to add the mapping method).
            foreach (var variableDeclarationSyntax in fieldDeclarationSyntax.Declaration.Variables.Take(1))
            {
                if (classContext.SemanticModel.GetDeclaredSymbol(variableDeclarationSyntax, cancellationToken) is IFieldSymbol fieldSymbol)
                {
                    var accessFieldName = variableDeclarationSyntax.Identifier.ToString();
                    if (!fieldDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        accessFieldName = $"this.{accessFieldName}";
                    }

                    var methods = fieldSymbol.Type.GetMembers().OfType<IMethodSymbol>().ToArray();
                    foreach (var method in methods)
                    {
                        this.AcceptMapMethodFromDependency(
                            fieldDeclarationSyntax,
                            method,
                            accessFieldName,
                            classContext);
                    }
                }
            }
        }

        // Identify the strategy for each method.
        // While generating strategies new methods might be found or requested to be generated.
        while (!classContext.AreAllMethodsMapped())
        {
            this.GenerateStrategyForEachMethod(classContext, cancellationToken);
        }

        // Build the source code (only if there is something to generate)
        this.GenerateSourceCode(classContext, options);

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

    private void GenerateSourceCode(
        MappaClassGeneratorContext classContext,
        MappaGlobalOptions options)
    {
        if (!classContext.MapMethods.Any(mapMethod => mapMethod.HasStrategy))
        {
            return;
        }

        var builder = new MappaFileBuilder(classContext);
        var hintName = builder.HintName;
        var sourceFile = builder.BuildSource(new(this.Compilation), options);
        this.Context.AddSource(hintName, sourceFile);
    }

    private bool AcceptMapMethod(
        MethodDeclarationSyntax methodDeclarationSyntax,
        MappaClassGeneratorContext classContext,
        CancellationToken cancellationToken)
    {
        if (methodDeclarationSyntax.AttributeLists
            .GetMappaIgnoreAttributeSyntax(classContext.SemanticModel, cancellationToken) is not null)
        {
            return false;
        }

        if (!methodDeclarationSyntax.IsPartial())
        {
            return false;
        }

        // TODO [#55] Accept method with MappaContext.
        if (!methodDeclarationSyntax.HasArity(1))
        {
            classContext.ReportDiagnostic(MappaDiagnostics.MethodHasInvalidNumberOfParameters(methodDeclarationSyntax));
            return false;
        }

        var mapMethod = new MapMethod(
            methodDeclarationSyntax,
            classContext.SemanticModel,
            classContext.IsNullableEnabled(methodDeclarationSyntax),
            cancellationToken);

        if (mapMethod.MethodSymbol.IsVoid())
        {
            classContext.ReportDiagnostic(MappaDiagnostics.MethodIsVoid(methodDeclarationSyntax));
            return false;
        }

        if (mapMethod.MethodSymbol.ReturnsAnyTaskType(this.Compilation))
        {
            classContext.ReportDiagnostic(MappaDiagnostics.MethodReturnsTaskType(methodDeclarationSyntax));
            return false;
        }

        var added = classContext.TryAddMethod(mapMethod);
        if (!added)
        {
            classContext.ReportDiagnostic(MappaDiagnostics.DuplicatedMapping(methodDeclarationSyntax));
            return false;
        }

        return true;
    }

    private void AcceptMapMethodFromDependency(
        SyntaxNode referenceSyntaxNode,
        IMethodSymbol method,
        string accessFieldName,
        MappaClassGeneratorContext classContext)
    {
        // TODO [#55] Accept method with MappaContext.
        if (method.GetMappaIgnoreAttribute(this.Compilation) is not null)
        {
            return;
        }

        if (!this.Compilation.IsSymbolAccessibleWithin(method, classContext.ClassSymbol))
        {
            return;
        }

        if (method.Parameters.Length != 1)
        {
            return;
        }

        if (method.IsVoid())
        {
            return;
        }

        if (method.ReturnsAnyTaskType(this.Compilation))
        {
            return;
        }

        var mapMethod = new MapMethod(
            method,
            accessFieldName,
            classContext.IsNullableEnabled(referenceSyntaxNode));

        // If the method cannot be added it is OK:
        // method defined in the class takes precedence if they
        // declare the very same mapping.
        classContext.TryAddMethod(mapMethod);
    }

    private void AcceptMapMethodAlreadyMapped(
        MethodDeclarationSyntax methodDeclarationSyntax,
        MappaClassGeneratorContext classContext,
        CancellationToken cancellationToken)
    {
        if (methodDeclarationSyntax.AttributeLists
                .GetMappaIgnoreAttributeSyntax(classContext.SemanticModel, cancellationToken) is not null)
        {
            return;
        }

        if (methodDeclarationSyntax.IsPartial())
        {
            return;
        }

        // TODO [#55] Accept method with MappaContext.
        if (!methodDeclarationSyntax.HasArity(1))
        {
            return;
        }

        var mapMethod = new MapMethod(
            methodDeclarationSyntax,
            classContext.SemanticModel,
            classContext.IsNullableEnabled(methodDeclarationSyntax),
            cancellationToken);

        if (mapMethod.MethodSymbol.IsVoid())
        {
            return;
        }

        if (mapMethod.MethodSymbol.ReturnsAnyTaskType(this.Compilation))
        {
            return;
        }

        var added = classContext.TryAddMethod(mapMethod);
        if (added)
        {
            mapMethod.MarkMapped();
        }
    }
}