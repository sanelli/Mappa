// <copyright file="MappaInterceptorGeneratorAlgorithm.cs" company="Stefano Anelli">
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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mappa.Generator.Algorithm;

/// <summary>
/// Algorithm that generates interceptable mapping stubs and interceptor methods for classes marked with
/// <see cref="Mappa.Attributes.MappaInterceptorAttribute"/>.
/// </summary>
internal sealed class MappaInterceptorGeneratorAlgorithm
{
    private const string TypedSourceParameterName = "__mappaTypedSource";
    private const string ContextParameterName = "context";

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaInterceptorGeneratorAlgorithm"/> class.
    /// </summary>
    /// <param name="context">The source production context.</param>
    /// <param name="analyzerConfigOptionsProvider">The analyzer config options provider.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="classDeclarationSyntaxes">The candidate interceptor class declarations.</param>
    /// <param name="classSymbolResolver">
    /// Optional resolver used by unit tests to simulate <c>GetDeclaredSymbol</c> failures.
    /// </param>
    public MappaInterceptorGeneratorAlgorithm(
        SourceProductionContext context,
        AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider,
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax?> classDeclarationSyntaxes,
        Func<Compilation, ClassDeclarationSyntax, CancellationToken, INamedTypeSymbol?>? classSymbolResolver = null)
    {
        this.Context = context;
        this.AnalyzerConfigOptionsProvider = analyzerConfigOptionsProvider;
        this.Compilation = compilation;
        this.ClassDeclarationSyntaxes = classDeclarationSyntaxes;
        this.ClassSymbolResolver = classSymbolResolver;
    }

    private SourceProductionContext Context { get; }

    private AnalyzerConfigOptionsProvider AnalyzerConfigOptionsProvider { get; }

    private Compilation Compilation { get; }

    private ImmutableArray<ClassDeclarationSyntax?> ClassDeclarationSyntaxes { get; }

    private Func<Compilation, ClassDeclarationSyntax, CancellationToken, INamedTypeSymbol?>? ClassSymbolResolver { get; }

    /// <summary>
    /// Execute the algorithm and produce the sources.
    /// </summary>
    internal void Execute()
    {
        var cancellationToken = this.Context.CancellationToken;
        foreach (var classDeclarationSyntax in this.ClassDeclarationSyntaxes)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (classDeclarationSyntax is null)
            {
                continue;
            }

            this.ExecuteForSingleClass(classDeclarationSyntax, cancellationToken);
        }
    }

    private static INamedTypeSymbol[] CollectStaticDependencyTypes(
        ImmutableArray<AttributeData> classAttributes,
        ImmutableArray<MappaInterceptorCallSite> callSites,
        Compilation compilation)
    {
        var seen = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        var results = new List<INamedTypeSymbol>();

        foreach (var dependency in classAttributes.GetMappaStaticDependencies(compilation)
                     .Where(dependency => seen.Add(dependency)))
        {
            results.Add(dependency);
        }

        foreach (var dependency in callSites
                     .SelectMany(static callSite => callSite.StaticDependencies)
                     .Where(dependency => seen.Add(dependency)))
        {
            results.Add(dependency);
        }

        return [.. results];
    }

    private static string AllocateInterceptorMethodName(MappaClassGeneratorContext classContext, int index)
    {
        var baseName = $"__MappaIntercept_{index}";
        if (!IsNameTaken(classContext, baseName))
        {
            return baseName;
        }

        var suffix = 1;
        while (IsNameTaken(classContext, $"{baseName}_{suffix}"))
        {
            suffix++;
        }

        return $"{baseName}_{suffix}";
    }

    private static bool IsNameTaken(MappaClassGeneratorContext classContext, string methodName)
        => classContext.MapMethods.Any(mapMethod =>
               string.Equals(mapMethod.MethodName, methodName, StringComparison.Ordinal))
           || classContext.ClassSymbol.GetMembers(methodName).Length > 0;

    private static MapHookAttributeData[] GetMethodBeforeMapAttributes(MapMethod mapMethod)
    {
        // Interceptor synthetics carry materialised attributes; dependency MapMethods are already Mapped.
        return
        [
            .. mapMethod.GetAttributes<MappaBeforeMapAttribute>()
                .Select(attribute => new MapHookAttributeData(
                    attribute.MethodName,
                    attribute.ClassType,
                    attribute.FieldName,
                    mapMethod.Location)),
        ];
    }

    private static MapHookAttributeData[] GetMethodAfterMapAttributes(MapMethod mapMethod)
    {
        // Interceptor synthetics carry materialised attributes; dependency MapMethods are already Mapped.
        return
        [
            .. mapMethod.GetAttributes<MappaAfterMapAttribute>()
                .Select(attribute => new MapHookAttributeData(
                    attribute.MethodName,
                    attribute.ClassType,
                    attribute.FieldName,
                    mapMethod.Location)),
        ];
    }

    private static ImmutableArray<(MappaInterceptorCallSite CallSite, MapMethod MapMethod)> RegisterSyntheticInterceptorMethods(
        MappaClassGeneratorContext classContext,
        ImmutableArray<MappaInterceptorCallSite> callSites)
    {
        var results = ImmutableArray.CreateBuilder<(MappaInterceptorCallSite CallSite, MapMethod MapMethod)>();
        var interceptorIndex = 0;

        foreach (var callSite in callSites)
        {
            if (callSite.HasErrors || callSite.InterceptableLocation is null)
            {
                continue;
            }

            var methodName = AllocateInterceptorMethodName(classContext, interceptorIndex++);
            var mapMethod = MapMethod.CreateSynthetic(
                methodName,
                callSite.SourceType,
                callSite.TargetType,
                classContext.ClassSymbol,
                classContext.IsNullableEnabled(callSite.Invocation),
                isStatic: false,
                sourceParameterName: TypedSourceParameterName,
                mappaContextParameterName: callSite.HasContext ? ContextParameterName : null,
                location: callSite.Invocation.GetLocation(),
                attributes: callSite.MethodAttributes.ToArray());

            // Interceptor synthetics always register (see TryAddMethod); uniqueness is by call site, not type pair.
            classContext.TryAddMethod(mapMethod);
            results.Add((callSite, mapMethod));
        }

        return results.ToImmutable();
    }

    private void ExecuteForSingleClass(ClassDeclarationSyntax classDeclarationSyntax, CancellationToken cancellationToken)
    {
        var classSymbol = this.ResolveClassSymbol(classDeclarationSyntax, cancellationToken);
        if (classSymbol is null)
        {
            return;
        }

        var className = classDeclarationSyntax.Identifier.Text;
        var attributes = classSymbol.GetAttributes();
        if (!attributes.HasMappaInterceptorAttribute(this.Compilation))
        {
            return;
        }

        if (!classDeclarationSyntax.IsPartial())
        {
            this.Context.ReportDiagnostic(MappaDiagnostics.MappaInterceptorClassIsNotPartial(classDeclarationSyntax, className));
            return;
        }

        if (classSymbol.IsStatic)
        {
            this.Context.ReportDiagnostic(MappaDiagnostics.MappaInterceptorClassIsStatic(classDeclarationSyntax, className));
            return;
        }

        var methodName = attributes.GetMappaInterceptorMethodName(this.Compilation);
        var options = new MappaGlobalOptions(this.AnalyzerConfigOptionsProvider, classDeclarationSyntax.SyntaxTree);
        var mappaDebug = new MappaDebug(options, this.Context.ReportDiagnostic);
        var classContext = new MappaClassGeneratorContext(options, mappaDebug, this.Compilation, classDeclarationSyntax);

        var callSites = MappaInterceptorCallSiteAnalyzer.Analyze(
            classDeclarationSyntax,
            classSymbol,
            methodName,
            this.Compilation,
            this.Context.ReportDiagnostic,
            cancellationToken);

        var hasMappaAttribute = attributes.HasMappaAttribute(this.Compilation);
        var staticDependencyTypes = CollectStaticDependencyTypes(attributes, callSites, this.Compilation);

        // Register interceptor synthetics before dependency collection so same-signature
        // helpers (static deps / dual-[Mappa] wrappers) cannot block TryAddMethod.
        var pendingInterceptorMethods = RegisterSyntheticInterceptorMethods(classContext, callSites);

        MappaInterceptorDependencyCollector.Collect(
            classContext,
            this.Compilation,
            hasMappaAttribute,
            staticDependencyTypes,
            this.Context.ReportDiagnostic,
            cancellationToken);

        var interceptorMethods = this.BuildInterceptorMethods(
            classContext,
            options,
            pendingInterceptorMethods,
            cancellationToken);

        foreach (var diagnostic in classContext.Diagnostics)
        {
            this.Context.ReportDiagnostic(diagnostic);
        }

        var builder = new MappaInterceptorFileBuilder(
            classDeclarationSyntax,
            classSymbol,
            methodName,
            interceptorMethods,
            this.Compilation,
            options);
        this.Context.AddSource(builder.HintName, builder.BuildSource());
    }

    private ImmutableArray<MappaInterceptorGeneratedMethod> BuildInterceptorMethods(
        MappaClassGeneratorContext classContext,
        MappaGlobalOptions options,
        ImmutableArray<(MappaInterceptorCallSite CallSite, MapMethod MapMethod)> pendingInterceptorMethods,
        CancellationToken cancellationToken)
    {
        var results = ImmutableArray.CreateBuilder<MappaInterceptorGeneratedMethod>();
        var classAttributes = classContext.ClassSymbol.GetAttributes();
        var classBeforeMapAttributes = classAttributes.GetMappaBeforeMapAttributes(this.Compilation);
        var classAfterMapAttributes = classAttributes.GetMappaAfterMapAttributes(this.Compilation);
        var classObjectFactoryAttributes = classAttributes.GetMappaObjectFactoryAttributes(this.Compilation);
        var classSettings = classAttributes.GetMappaSettingsAttribute(this.Compilation);
        var mappaUserSettings = new MappaUserSettings(options);
        var valueTransformerResolver = new ValueTransformerResolver(this.Compilation, classContext);

        using (mappaUserSettings.Apply(classSettings))
        {
            foreach (var (callSite, mapMethod) in pendingInterceptorMethods)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var withSettingsDisposables = callSite.Settings
                    .Select(mappaUserSettings.Apply)
                    .ToArray();
                try
                {
                    this.GenerateStrategiesUntilStable(
                        classContext,
                        mappaUserSettings,
                        classBeforeMapAttributes,
                        classAfterMapAttributes,
                        classObjectFactoryAttributes,
                        valueTransformerResolver,
                        cancellationToken);

                    if (!mapMethod.HasStrategy || mapMethod.Strategy.Strategy is NoMapStrategy)
                    {
                        continue;
                    }

                    results.Add(new MappaInterceptorGeneratedMethod(
                        mapMethod,
                        callSite.InterceptableLocation!,
                        mapMethod.NullableEnabled));
                }
                finally
                {
                    for (var index = withSettingsDisposables.Length - 1; index >= 0; index--)
                    {
                        withSettingsDisposables[index].Dispose();
                    }
                }
            }
        }

        return results.ToImmutable();
    }

    private void GenerateStrategiesUntilStable(
        MappaClassGeneratorContext classContext,
        MappaUserSettings mappaUserSettings,
        MapHookAttributeData[] classBeforeMapAttributes,
        MapHookAttributeData[] classAfterMapAttributes,
        MappaObjectFactoryAttributeData[] classObjectFactoryAttributes,
        ValueTransformerResolver valueTransformerResolver,
        CancellationToken cancellationToken)
    {
        while (!classContext.AreAllMethodsMapped())
        {
            cancellationToken.ThrowIfCancellationRequested();
            MapMethod[] mapMethodsSnapshot = [.. classContext.MapMethods];
            foreach (var mapMethod in mapMethodsSnapshot)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (mapMethod.Mapped)
                {
                    continue;
                }

                var methodAttributes = mapMethod.MethodSymbol?.GetAttributes() ?? [];
                var methodObjectFactoryAttributes = methodAttributes.GetMappaObjectFactoryAttributes(this.Compilation);
                var methodBeforeMapAttributes = GetMethodBeforeMapAttributes(mapMethod);
                var methodAfterMapAttributes = GetMethodAfterMapAttributes(mapMethod);
                var mappaSettingsAttribute = mapMethod.GetAttribute<MappaSettingsAttribute>();
                using (mappaUserSettings.Apply(mappaSettingsAttribute))
                {
                    mapMethod.SetPragmaWarning(mappaUserSettings.PragmaWarning);
                    mapMethod.SetMaxRuntimeDepth(mappaUserSettings.MaxRuntimeDepth);
                    mapMethod.SetReferenceReusing(mappaUserSettings.ReferenceReusing);

                    // Projection eligibility always passes for interceptor synthetics (IsSynthetic).
                    if (!ObjectFactoryDuplicateValidator.TryValidate(
                            mapMethod,
                            classObjectFactoryAttributes,
                            methodObjectFactoryAttributes,
                            classContext))
                    {
                        mapMethod.MarkMapped();
                        continue;
                    }

                    var methodContext = new MappaMethodGeneratorContext(classContext, mappaUserSettings, mapMethod);
                    methodContext.SetValueTransformerCatalog(valueTransformerResolver.Resolve(mapMethod, mappaUserSettings));
                    methodContext.ValidatePreferredInterfaceAttributes(this.Compilation);
                    methodContext.ValidateMappaUseConstructorAttributes(this.Compilation);
                    var typeIdentifierAlgorithm = new TypeMapIdentifierAlgorithm(
                        methodContext,
                        this.Compilation,
                        cancellationToken);
                    var strategy = typeIdentifierAlgorithm.GetStrategy();
                    if (strategy is not NoMapStrategy && !methodContext.HasErrorDiagnostics)
                    {
                        methodContext.ReportPreferredInterfaceNotNeededWarnings();
                        methodContext.ReportMappaUseConstructorUnusedWarnings();
                    }

                    var mapHookResolver = new MapHookResolver(this.Compilation, classContext, mapMethod);
                    var beforeMapHooks = mapHookResolver.ResolveBeforeMapHooks(
                        classBeforeMapAttributes,
                        methodBeforeMapAttributes);
                    var afterMapHooks = mapHookResolver.ResolveAfterMapHooks(
                        classAfterMapAttributes,
                        methodAfterMapAttributes);
                    var methodParameterMapStrategy = new MethodParameterMapStrategy(
                        strategy,
                        beforeMapHooks,
                        afterMapHooks);
                    mapMethod.SetStrategy(methodParameterMapStrategy);
                    mapMethod.MarkMapped();
                }
            }
        }
    }

    private INamedTypeSymbol? ResolveClassSymbol(ClassDeclarationSyntax classDeclarationSyntax, CancellationToken cancellationToken)
    {
        if (this.ClassSymbolResolver is not null)
        {
            return this.ClassSymbolResolver(this.Compilation, classDeclarationSyntax, cancellationToken);
        }

        var semanticModel = this.Compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
        return semanticModel.GetDeclaredSymbol(classDeclarationSyntax, cancellationToken) as INamedTypeSymbol;
    }
}