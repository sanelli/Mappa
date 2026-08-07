// <copyright file="MappaGeneratorClassAlgorithm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Mappa.Attributes;
using Mappa.Generator.Builders;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Diagnostics.Debug;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
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
        MappaUserSettings mappaUserSettings,
        MapHookAttributeData[] classBeforeMapAttributes,
        MapHookAttributeData[] classAfterMapAttributes,
        MappaObjectFactoryAttributeData[] classObjectFactoryAttributes,
        CancellationToken cancellationToken)
    {
        foreach (var mapMethod in classContext.MapMethods)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (mapMethod.Mapped)
            {
                continue;
            }

            var methodAttributes = mapMethod.MethodSymbol.GetAttributes();
            var methodObjectFactoryAttributes = methodAttributes.GetMappaObjectFactoryAttributes(this.Compilation);

            if (!ProjectionMapMethodEligibilityValidator.TryValidate(
                    mapMethod,
                    this.Compilation,
                    classContext,
                    classBeforeMapAttributes,
                    classAfterMapAttributes,
                    classObjectFactoryAttributes,
                    methodObjectFactoryAttributes))
            {
                mapMethod.MarkMapped();
                continue;
            }

            if (!ObjectFactoryDuplicateValidator.TryValidate(
                    mapMethod,
                    classObjectFactoryAttributes,
                    methodObjectFactoryAttributes,
                    classContext))
            {
                mapMethod.MarkMapped();
                continue;
            }

            var mappaSettingsAttribute = mapMethod.GetAttribute<MappaSettingsAttribute>();
            using (mappaUserSettings.Apply(mappaSettingsAttribute))
            {
                mapMethod.SetPragmaWarning(mappaUserSettings.PragmaWarning);
                mapMethod.SetMaxRuntimeDepth(mappaUserSettings.MaxRuntimeDepth);
                var methodContext = new MappaMethodGeneratorContext(classContext, mappaUserSettings, mapMethod);
                var typeIdentifierAlgorithm = new TypeMapIdentifierAlgorithm(
                    methodContext,
                    this.Compilation,
                    cancellationToken);
                var strategy = typeIdentifierAlgorithm.GetStrategy();
                var mapHookResolver = new MapHookResolver(this.Compilation, classContext, mapMethod);
                var beforeMapHooks = mapHookResolver.ResolveBeforeMapHooks(
                    classBeforeMapAttributes,
                    methodAttributes.GetMappaBeforeMapAttributes(this.Compilation));
                var afterMapHooks = mapHookResolver.ResolveAfterMapHooks(
                    classAfterMapAttributes,
                    methodAttributes.GetMappaAfterMapAttributes(this.Compilation));
                var methodParameterMapStrategy = new MethodParameterMapStrategy(
                    strategy,
                    beforeMapHooks,
                    afterMapHooks);
                mapMethod.SetStrategy(methodParameterMapStrategy);
                mapMethod.MarkMapped();
            }
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

        // [Mappa] and [MappaDependencyInjection] cannot be applied to the same class.
        // The DI pipeline reports MP00071; skip mapper generation here without a duplicate diagnostic.
        if (classContext.ClassSymbol.GetAttributes().HasMappaDependencyInjectionAttribute(this.Compilation))
        {
            return;
        }

        // Gather all the methods that require a mapping.
        foreach (var methodDeclarationSyntax in classDeclarationSyntax.ChildNodes().OfType<MethodDeclarationSyntax>())
        {
            mappaDebug.Debug(
                $"Started deciding if method class \"{classDeclarationSyntax.Identifier.ToFullString()}.{methodDeclarationSyntax.Identifier.ToString()}\" can be mapped.",
                classDeclarationSyntax);

            cancellationToken.ThrowIfCancellationRequested();

            // Try to add a method as either a method that defines a mapping from Mappa
            // or as a method with already code that can be used for the mapping.
            if (!this.AcceptMapMethod(methodDeclarationSyntax, classContext, cancellationToken))
            {
                this.AcceptMapMethodAlreadyMapped(methodDeclarationSyntax, classContext, cancellationToken);
            }
        }

        // List methods also from base classes of the mapper.
        foreach (var method in this.Compilation.GetMethodsInTypeHierarchyFromMetadata(classContext.ClassSymbol))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var accessFieldName = method.IsStatic
                ? $"global::{method.ContainingType.ToDisplayString()}"
                : "this";
            this.AcceptMapMethodFromDependency(
                classDeclarationSyntax,
                method,
                accessFieldName,
                methodMustBeStatic: method.IsStatic,
                canBeInvokedByStaticMethod: method.IsStatic,
                classContext);
        }

        // Get all the static types on the class listed as static dependencies
        foreach (var dependencyType in classContext
                     .ClassSymbol
                     .GetAttributes()
                     .GetMappaStaticDependencies(this.Compilation))
        {
            var anyMethodCanBeUsed = false;
            var methods = dependencyType.GetMembers().OfType<IMethodSymbol>().ToArray();
            var accessFieldName = $"global::{dependencyType.ToDisplayString()}";
            foreach (var method in methods)
            {
                // Skip non-static methods
                if (!method.IsStatic)
                {
                    continue;
                }

                var added = this.AcceptMapMethodFromDependency(
                    classContext.ClassDeclarationSyntax,
                    method,
                    accessFieldName,
                    methodMustBeStatic: true,
                    canBeInvokedByStaticMethod: true,
                    classContext);
                anyMethodCanBeUsed |= added;
            }

            if (!anyMethodCanBeUsed)
            {
                this.Context.ReportDiagnostic(MappaDiagnostics.DependencyDoesNotProvideAnyViableMethod(classContext.ClassDeclarationSyntax, dependencyType.ToDisplayString()));
            }
        }

        // Get all accessible properties that:
        // - have a getter method
        // - have MappaDependency attribute
        var processedDependencyMemberNames = new HashSet<string>(StringComparer.Ordinal);
#pragma warning disable S3267 // Loops should be simplified using the "Where" LINQ method
        foreach (var propertyDeclarationSyntax in classDeclarationSyntax.ChildNodes().OfType<PropertyDeclarationSyntax>())
#pragma warning restore S3267 // Loops should be simplified using the "Where" LINQ method
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
                    var propertyIdentifier = propertyDeclarationSyntax.Identifier.ToString();
                    processedDependencyMemberNames.Add(propertyIdentifier);
                    this.ProcessMappaDependencyProperty(
                        propertyDeclarationSyntax,
                        propertySymbol,
                        propertyIdentifier,
                        classContext);
                }
            }
        }

        foreach (var propertySymbol in this.Compilation.GetMappaDependencyPropertiesInMapperBaseTypeHierarchy(classContext.ClassSymbol))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!processedDependencyMemberNames.Add(propertySymbol.Name))
            {
                continue;
            }

            this.ProcessMappaDependencyProperty(
                classDeclarationSyntax,
                propertySymbol,
                propertySymbol.Name,
                classContext);
        }

        // Get all accessible fields that have MappaDependency attribute
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
                    var fieldIdentifier = variableDeclarationSyntax.Identifier.ToString();
                    processedDependencyMemberNames.Add(fieldIdentifier);
                    this.ProcessMappaDependencyField(
                        fieldDeclarationSyntax,
                        fieldSymbol,
                        fieldIdentifier,
                        classContext);
                }
            }
        }

        foreach (var fieldSymbol in this.Compilation.GetMappaDependencyFieldsInMapperBaseTypeHierarchy(classContext.ClassSymbol))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!processedDependencyMemberNames.Add(fieldSymbol.Name))
            {
                continue;
            }

            this.ProcessMappaDependencyField(
                classDeclarationSyntax,
                fieldSymbol,
                fieldSymbol.Name,
                classContext);
        }

        var classAttributes = classContext.ClassSymbol.GetAttributes();
        var classBeforeMapAttributes = classAttributes.GetMappaBeforeMapAttributes(this.Compilation);
        var classAfterMapAttributes = classAttributes.GetMappaAfterMapAttributes(this.Compilation);
        var classObjectFactoryAttributes = classAttributes.GetMappaObjectFactoryAttributes(this.Compilation);
        var mappaUserSettings = new MappaUserSettings(options);
        var mappaSettingsAttribute = classAttributes.GetMappaSettingsAttribute(this.Compilation);

        using (mappaUserSettings.Apply(mappaSettingsAttribute))
        {
            // Identify the strategy for each method.
            // While generating strategies new methods might be found or requested to be generated.
            while (!classContext.AreAllMethodsMapped())
            {
                this.GenerateStrategyForEachMethod(
                    classContext,
                    mappaUserSettings,
                    classBeforeMapAttributes,
                    classAfterMapAttributes,
                    classObjectFactoryAttributes,
                    cancellationToken);
            }
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
        var mappaBuilderContext = new MappaBuilderContext(this.Compilation);
        var sourceFile = builder.BuildSource(mappaBuilderContext, options);
        classContext.ReportDiagnostics(mappaBuilderContext.Diagnostics);
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

        if (!methodDeclarationSyntax.HasArity(1, 2))
        {
            classContext.ReportDiagnostic(MappaDiagnostics.MethodHasInvalidNumberOfParameters(methodDeclarationSyntax));
            return false;
        }

        var mapMethod = new MapMethod(
            methodDeclarationSyntax,
            classContext.SemanticModel,
            classContext.IsNullableEnabled(methodDeclarationSyntax),
            cancellationToken);

        if (methodDeclarationSyntax.HasArity(2)
            && !mapMethod.MethodSymbol.SecondParameterIsMappaContext(this.Compilation))
        {
            classContext.ReportDiagnostic(MappaDiagnostics.MethodHasInvalidMappaContextParameter(methodDeclarationSyntax));
            return false;
        }

        if (!mapMethod.MethodSymbol.AreParametersRefModifiersValid())
        {
            return false;
        }

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

    private void ProcessMappaDependencyProperty(
        SyntaxNode referenceSyntaxNode,
        IPropertySymbol propertySymbol,
        string propertyIdentifier,
        MappaClassGeneratorContext classContext)
    {
        var staticFieldAccessor = $"global::{propertySymbol.Type.ToDisplayString()}";
        var accessFieldName = propertyIdentifier;
        if (!propertySymbol.IsStatic)
        {
            accessFieldName = $"this.{accessFieldName}";
        }

        var anyMethodCanBeUsed = false;
        var methods = propertySymbol.Type.GetMethodsInTypeHierarchy().ToArray();
        foreach (var method in methods)
        {
            var added = this.AcceptMapMethodFromDependency(
                referenceSyntaxNode,
                method,
                method.IsStatic ? staticFieldAccessor : accessFieldName,
                method.IsStatic,
                canBeInvokedByStaticMethod: method.IsStatic || propertySymbol.IsStatic,
                classContext);
            anyMethodCanBeUsed |= added;
        }

        if (!anyMethodCanBeUsed)
        {
            this.Context.ReportDiagnostic(MappaDiagnostics.DependencyDoesNotProvideAnyViableMethod(referenceSyntaxNode, propertyIdentifier));
        }
    }

    private void ProcessMappaDependencyField(
        SyntaxNode referenceSyntaxNode,
        IFieldSymbol fieldSymbol,
        string fieldIdentifier,
        MappaClassGeneratorContext classContext)
    {
        var staticFieldAccessor = $"global::{fieldSymbol.Type.ToDisplayString()}";
        var accessFieldName = fieldIdentifier;
        if (!fieldSymbol.IsStatic)
        {
            accessFieldName = $"this.{accessFieldName}";
        }

        var anyMethodCanBeUsed = false;
        var methods = fieldSymbol.Type.GetMethodsInTypeHierarchy().ToArray();
        foreach (var method in methods)
        {
            var added = this.AcceptMapMethodFromDependency(
                referenceSyntaxNode,
                method,
                method.IsStatic ? staticFieldAccessor : accessFieldName,
                method.IsStatic,
                canBeInvokedByStaticMethod: method.IsStatic || fieldSymbol.IsStatic,
                classContext);
            anyMethodCanBeUsed |= added;
        }

        if (!anyMethodCanBeUsed)
        {
            this.Context.ReportDiagnostic(MappaDiagnostics.DependencyDoesNotProvideAnyViableMethod(referenceSyntaxNode, fieldIdentifier));
        }
    }

    private bool AcceptMapMethodFromDependency(
        SyntaxNode referenceSyntaxNode,
        IMethodSymbol method,
        string accessFieldName,
        bool methodMustBeStatic,
        bool canBeInvokedByStaticMethod,
        MappaClassGeneratorContext classContext)
    {
        var methodAttributes = method.GetAttributes();
        if (methodAttributes.GetMappaIgnoreAttribute(this.Compilation) is not null)
        {
            return false;
        }

        if (!this.Compilation.IsSymbolAccessibleWithin(method, classContext.ClassSymbol))
        {
            return false;
        }

        if (method.IsStatic != methodMustBeStatic)
        {
            return false;
        }

        if (method.Parameters.Length != 1
            && method.Parameters.Length != 2)
        {
            return false;
        }

        if (method.Parameters.Length == 2
            && !method.SecondParameterIsMappaContext(this.Compilation))
        {
            return false;
        }

        if (!method.AreParametersRefModifiersValid())
        {
            return false;
        }

        if (method.IsVoid())
        {
            return false;
        }

        if (method.ReturnsAnyTaskType(this.Compilation))
        {
            return false;
        }

        // Load all type Mappa related attributes.
        var mappaAttributes = method.GetMethodMappaAttributes(this.Compilation);

        var mapMethod = new MapMethod(
            method,
            accessFieldName,
            classContext.IsNullableEnabled(referenceSyntaxNode),
            canBeInvokedByStaticMethod,
            mappaAttributes);

        // If the method cannot be added, it is OK:
        // method defined in the class takes precedence if they
        // declare the very same mapping.
        return classContext.TryAddMethod(mapMethod);
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

        if (!methodDeclarationSyntax.HasArity(1, 2))
        {
            return;
        }

        var mapMethod = new MapMethod(
            methodDeclarationSyntax,
            classContext.SemanticModel,
            classContext.IsNullableEnabled(methodDeclarationSyntax),
            cancellationToken);

        if (methodDeclarationSyntax.HasArity(2)
            && !mapMethod.MethodSymbol.SecondParameterIsMappaContext(this.Compilation))
        {
            return;
        }

        if (!mapMethod.MethodSymbol.AreParametersRefModifiersValid())
        {
            return;
        }

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