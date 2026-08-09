// <copyright file="MappaClassGeneratorContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics.Debug;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Models;

/// <summary>
/// The context of the generator for a single class.
/// </summary>
internal sealed class MappaClassGeneratorContext
{
    private readonly List<MapMethod> mapMethods = [];
    private readonly List<Diagnostic> diagnostics = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaClassGeneratorContext"/> class.
    /// </summary>
    /// <param name="options">The mappa global options.</param>
    /// <param name="mappaDebug">Mappa debug helper.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="classDeclarationSyntax">The class declaration syntax.</param>
    public MappaClassGeneratorContext(
        MappaGlobalOptions options,
        MappaDebug mappaDebug,
        Compilation compilation,
        ClassDeclarationSyntax classDeclarationSyntax)
    {
        this.Options = options;
        this.MappaDebug = mappaDebug;
        this.Compilation = compilation;
        this.ClassDeclarationSyntax = classDeclarationSyntax;
        this.SemanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
        this.ClassSymbol = this.SemanticModel.GetDeclaredSymbol(this.ClassDeclarationSyntax) as INamedTypeSymbol
                           ?? throw new MappaGeneratorException(
                               "Cannot obtain semantic model",
                               this.ClassDeclarationSyntax.GetLocation());
    }

    /// <summary>
    /// Gets the global mappa options.
    /// </summary>
    internal MappaGlobalOptions Options { get; }

    /// <summary>
    /// Gets the debugging tool.
    /// </summary>
    internal MappaDebug MappaDebug { get; }

    /// <summary>
    /// Gets the compilation.
    /// </summary>
    internal Compilation Compilation { get; }

    /// <summary>
    /// Gets the semantic model.
    /// </summary>
    internal SemanticModel SemanticModel { get; }

    /// <summary>
    /// Gets the class declaration syntax of the mapper class begin processed.
    /// </summary>
    internal ClassDeclarationSyntax ClassDeclarationSyntax { get; }

    /// <summary>
    /// Gets the declared symbol.
    /// </summary>
    internal INamedTypeSymbol ClassSymbol { get; }

    /// <summary>
    /// Gets the list of map methods associated to class defined by <see cref="ClassDeclarationSyntax"/>.
    /// </summary>
    internal IReadOnlyCollection<MapMethod> MapMethods => this.mapMethods;

    /// <summary>
    /// Gets the diagnostics associated with the mapping.
    /// </summary>
    internal IReadOnlyCollection<Diagnostic> Diagnostics => this.diagnostics;

    /// <summary>
    /// Gets a value indicating whether a diagnostic with severity error has been reported.
    /// </summary>
    internal bool HasErrorDiagnostics =>
        this.diagnostics.Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);

    /// <summary>
    /// Gets a value indicating whether <c>nullable</c> is enabled for a syntax node.
    /// </summary>
    /// <param name="syntaxNode">The syntax node to investigate.</param>
    /// <returns><c>true</c> if the nullable context is enabled, <c>false</c> otherwise.</returns>
    internal bool IsNullableEnabled(SyntaxNode syntaxNode)
    {
        var methodNullableContext = this.SemanticModel.GetNullableContext(syntaxNode.SpanStart);

        bool contextInherit = (methodNullableContext & NullableContext.ContextInherited) > 0;
        if (contextInherit)
        {
            var projectEnabled = (this.Compilation.Options.NullableContextOptions & NullableContextOptions.Enable) > 0;
            return projectEnabled;
        }

        bool isNullableEnabled = (methodNullableContext & NullableContext.Enabled) > 0;
        return isNullableEnabled;
    }

    /// <summary>
    /// Try getting the method for mapping from <paramref name="sourceType"/> to
    /// <paramref name="targetType"/>.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="nullableEnabled"><c>true</c> if nullable is enabled,<c>false</c> otherwise.</param>
    /// <param name="requireStaticContext"><c>true</c> if the invocation require only method that can invoked in a static context,<c>false</c> otherwise.</param>
    /// <param name="mapMethod">The map method, if it exists.</param>
    /// <param name="allowRelaxedNullability"><c>true</c> to allow relaxed nullability matching when no exact match exists.</param>
    /// <returns><c>true</c> if the method to map from <paramref name="sourceType"/> to
    /// <paramref name="targetType"/>, <c>false</c> otherwise.</returns>
    internal bool TryGetMethod(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        bool nullableEnabled,
        bool requireStaticContext,
        out MapMethod mapMethod,
        bool allowRelaxedNullability = true)
    {
        var foundMethod = this.FindMatchingMethod(
            targetType,
            sourceType,
            nullableEnabled,
            requireStaticContext,
            exactOnly: true);

        if (foundMethod is null && allowRelaxedNullability)
        {
            foundMethod = this.FindMatchingMethod(
                targetType,
                sourceType,
                nullableEnabled,
                requireStaticContext,
                exactOnly: false);
        }

        mapMethod = foundMethod!;
        return foundMethod is not null;
    }

    /// <summary>
    /// Try getting the method for mapping from <paramref name="sourceType"/> to
    /// <paramref name="targetType"/> by checking the polymorphic methods.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="nullableEnabled"><c>true</c> if nullable is enabled,<c>false</c> otherwise.</param>
    /// <param name="requireStaticContext"><c>true</c> if the invocation require only method that can invoked in a static context,<c>false</c> otherwise.</param>
    /// <param name="mappaUserSettings">The user settings applied to the method being mapped.</param>
    /// <param name="mapMethod">The map method, if it exists.</param>
    /// <returns><c>true</c> if the method to map from <paramref name="sourceType"/> to
    /// <paramref name="targetType"/>, <c>false</c> otherwise.</returns>
    internal bool TryGetPolymorphicMethod(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        bool nullableEnabled,
        bool requireStaticContext,
        IMappaUserSettings mappaUserSettings,
        out MapMethod mapMethod)
    {
        if (this.TryFindPolymorphicMethod(
                targetType,
                sourceType,
                nullableEnabled,
                requireStaticContext,
                mappaUserSettings,
                exactOnly: true,
                out mapMethod))
        {
            return true;
        }

        return this.TryFindPolymorphicMethod(
            targetType,
            sourceType,
            nullableEnabled,
            requireStaticContext,
            mappaUserSettings,
            exactOnly: false,
            out mapMethod);
    }

    /// <summary>
    /// Try getting a compatible method for mapping from <paramref name="sourceType"/> to
    /// <paramref name="targetType"/> (base/interface parameter and/or derived return type).
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="requireStaticContext"><c>true</c> if the invocation require only method that can invoked in a static context,<c>false</c> otherwise.</param>
    /// <param name="compilation">The compilation used to resolve implicit conversions.</param>
    /// <param name="mapMethod">The map method, if it exists.</param>
    /// <returns><c>true</c> if a compatible method to map from <paramref name="sourceType"/> to
    /// <paramref name="targetType"/> exists, <c>false</c> otherwise.</returns>
    internal bool TryGetCompatibleMethod(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        bool requireStaticContext,
        Compilation compilation,
        out MapMethod? mapMethod)
    {
        var foundMethod = this.mapMethods.Find(method =>
        {
            if (requireStaticContext && !method.CanBeUsedByStaticMethod)
            {
                return false;
            }

            return method.IsCompatibleMapFor(targetType, sourceType, compilation);
        });

        mapMethod = foundMethod;
        return foundMethod is not null;
    }

    /// <summary>
    /// Adds the map method to the list of methods.
    /// Method is added only if no other method with the same mapping exists.
    /// </summary>
    /// <param name="mapMethod">The method to be added.</param>
    /// <returns><c>true</c> if the method has been added, false otherwise.</returns>
    internal bool TryAddMethod(MapMethod mapMethod)
    {
        if (!this.TryGetMethod(
                mapMethod.TargetType,
                mapMethod.SourceType,
                mapMethod.NullableEnabled,
                false, // Bypass the require static requirements here since I just want to add it.
                out _,
                allowRelaxedNullability: false))
        {
            this.mapMethods.Add(mapMethod);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if all methods have been mapped.
    /// </summary>
    /// <returns>
    /// <c>true</c> if all <see cref="MapMethods"/> have the
    /// <see cref="MapMethod.Mapped"/> flag set to <c>true</c>.</returns>
    internal bool AreAllMethodsMapped()
        => this.mapMethods.TrueForAll(mapMethod => mapMethod.Mapped);

    /// <summary>
    /// Records a new set of diagnostics.
    /// </summary>
    /// <param name="generatedDiagnostics">The diagnostics to be added.</param>
    internal void ReportDiagnostics(IEnumerable<Diagnostic> generatedDiagnostics)
        => this.diagnostics.AddRange(generatedDiagnostics);

    /// <summary>
    /// Record a new diagnostic.
    /// </summary>
    /// <param name="diagnostic">The diagnostic to be added.</param>
    internal void ReportDiagnostic(Diagnostic diagnostic)
        => this.ReportDiagnostics([diagnostic]);

    private static bool TypesMatchForPolymorphicMapping(
        ITypeSymbol requiredSourceType,
        ITypeSymbol attributeSourceType,
        ITypeSymbol requiredTargetType,
        ITypeSymbol attributeTargetType,
        bool nullableEnabled,
        bool exactOnly)
    {
        if (exactOnly)
        {
            return attributeSourceType.IsEqualTo(requiredSourceType, nullableEnabled)
                   && attributeTargetType.IsEqualTo(requiredTargetType, nullableEnabled);
        }

        return requiredSourceType.IsNullabilityMatchOrRelaxed(attributeSourceType, nullableEnabled)
               && requiredTargetType.IsNullabilityMatchOrRelaxed(attributeTargetType, nullableEnabled);
    }

    private MapMethod? FindMatchingMethod(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        bool nullableEnabled,
        bool requireStaticContext,
        bool exactOnly)
    {
        return this.mapMethods.Find(method =>
        {
            if (requireStaticContext && !method.CanBeUsedByStaticMethod)
            {
                return false;
            }

            return exactOnly
                ? method.IsMapFor(targetType, sourceType, nullableEnabled)
                : method.IsRelaxedMapFor(targetType, sourceType, nullableEnabled);
        });
    }

    private bool TryFindPolymorphicMethod(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        bool nullableEnabled,
        bool requireStaticContext,
        IMappaUserSettings mappaUserSettings,
        bool exactOnly,
        out MapMethod mapMethod)
    {
        foreach (var method in this.mapMethods)
        {
            if (requireStaticContext && !method.CanBeUsedByStaticMethod)
            {
                continue;
            }

            var typeMappingAttributes = method.GetAttributes<MappaTypeMappingAttribute>();
            if (typeMappingAttributes.Length <= 0)
            {
                continue;
            }

            if (typeMappingAttributes.Any(typeMappingAttribute =>
                    this.TryMatchPolymorphicTypeMappingAttribute(
                        method,
                        typeMappingAttribute,
                        targetType,
                        sourceType,
                        nullableEnabled,
                        exactOnly)))
            {
                mapMethod = method;
                return true;
            }

            if (mappaUserSettings.PolymorphicMapMethodWithMatchingDefaultAttribute is BooleanSetting.Enable
                && this.TryMatchPolymorphicTypeMappingDefaultAttribute(
                    method,
                    targetType,
                    nullableEnabled,
                    exactOnly))
            {
                mapMethod = method;
                return true;
            }
        }

        mapMethod = null!;
        return false;
    }

    private bool TryMatchPolymorphicTypeMappingAttribute(
        MapMethod method,
        MappaTypeMappingAttribute typeMappingAttribute,
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        bool nullableEnabled,
        bool exactOnly)
    {
        var attributeSourceType =
            this.Compilation.GetTypeByMetadataName(typeMappingAttribute.SourceType.FullName!);
        var attributeTargetType =
            this.Compilation.GetTypeByMetadataName(typeMappingAttribute.TargetType.FullName!);

        var attributeSourceTypeWithNullability =
            attributeSourceType?.WithNullableAnnotation(method.TargetType.NullableAnnotation);
        var attributeTargetTypeWithNullability =
            attributeTargetType?.WithNullableAnnotation(method.TargetType.NullableAnnotation);

        if (attributeSourceTypeWithNullability is null || attributeTargetTypeWithNullability is null)
        {
            return false;
        }

        return TypesMatchForPolymorphicMapping(
            sourceType,
            attributeSourceTypeWithNullability,
            targetType,
            attributeTargetTypeWithNullability,
            nullableEnabled,
            exactOnly);
    }

    private bool TryMatchPolymorphicTypeMappingDefaultAttribute(
        MapMethod method,
        ITypeSymbol targetType,
        bool nullableEnabled,
        bool exactOnly)
    {
        var mappaTypeMappingDefaultAttribute = method.GetAttribute<MappaTypeMappingDefaultAttribute>();
        if (mappaTypeMappingDefaultAttribute is null
            || mappaTypeMappingDefaultAttribute.Behavior is not MappaTypeMappingDefaultBehavior.MapSourceType
            || mappaTypeMappingDefaultAttribute.Type is null)
        {
            return false;
        }

        var attributeTargetType =
            this.Compilation.GetTypeByMetadataName(mappaTypeMappingDefaultAttribute.Type.FullName!);
        var attributeTargetTypeWithNullability =
            attributeTargetType?.WithNullableAnnotation(method.TargetType.NullableAnnotation);

        if (attributeTargetTypeWithNullability is null)
        {
            return false;
        }

        return exactOnly
            ? attributeTargetTypeWithNullability.IsEqualTo(targetType, nullableEnabled)
            : targetType.IsNullabilityMatchOrRelaxed(attributeTargetTypeWithNullability, nullableEnabled);
    }
}