// <copyright file="ValueTransformerResolver.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Discovers the value transformers (<c>[MappaTransformer]</c> and <c>[MappaApplyTransformer]</c>)
/// available to the map methods of a mapper class.
/// </summary>
/// <remarks>
/// Methods marked with <c>[MappaTransformer]</c> are discovered (and validated) once per mapper class so that
/// invalid signatures are reported only once. The resulting catalog is computed for each map method.
/// </remarks>
internal sealed class ValueTransformerResolver
{
    private readonly Compilation compilation;
    private readonly MappaClassGeneratorContext classContext;
    private readonly INamedTypeSymbol mapperClass;
    private List<ValueTransformer>? ownTransformers;
    private List<ValueTransformer>? dependencyTransformers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueTransformerResolver"/> class.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <param name="classContext">The mapper class generator context.</param>
    internal ValueTransformerResolver(Compilation compilation, MappaClassGeneratorContext classContext)
    {
        this.compilation = compilation;
        this.classContext = classContext;
        this.mapperClass = classContext.ClassSymbol;
    }

    /// <summary>
    /// Resolves the transformers available to <paramref name="mapMethod"/>.
    /// </summary>
    /// <param name="mapMethod">The root map method.</param>
    /// <param name="mappaUserSettings">The effective settings of the map method.</param>
    /// <returns>
    /// The catalog of transformers. Queryable projection map methods never use transformers
    /// so their catalog is always empty.
    /// </returns>
    internal ValueTransformerCatalog Resolve(MapMethod mapMethod, IMappaUserSettings mappaUserSettings)
    {
        if (mapMethod.MethodSymbol?.IsQueryableProjectionMapMethod(this.compilation) == true)
        {
            return ValueTransformerCatalog.Empty;
        }

        var applyTransformers = this.ResolveApplyTransformers(mapMethod, mappaUserSettings);

        var classTransformers = new List<ValueTransformer>();
        AddUsableTransformers(classTransformers, this.GetOwnTransformers(), mapMethod);
        if (mappaUserSettings.MappaTransformerFromDependency is BooleanSetting.Enable)
        {
            AddUsableTransformers(classTransformers, this.GetDependencyTransformers(), mapMethod);
        }

        return applyTransformers.Length == 0 && classTransformers.Count == 0
            ? ValueTransformerCatalog.Empty
            : new ValueTransformerCatalog(applyTransformers, classTransformers);
    }

    private static bool IsStaticCompatible(ValueTransformer transformer, MapMethod mapMethod)
        => !mapMethod.CanBeUsedByStaticMethod
           || transformer.Method.IsStatic
           || transformer.FieldOrProperty?.IsStatic == true;

    private static bool IsContextCompatible(ValueTransformer transformer, MapMethod mapMethod)
        => !transformer.RequiresContext || mapMethod.ProvideMappaContextWhenInvoked();

    private static bool Contains(List<ValueTransformer> transformers, ValueTransformer transformer)
        => transformers.Exists(existing => existing.IsSameAs(transformer));

    private static void AddUsableTransformers(
        List<ValueTransformer> target,
        IEnumerable<ValueTransformer> candidates,
        MapMethod mapMethod)
    {
        // The predicate is evaluated lazily, so duplicates are detected against the items added so far.
        foreach (var candidate in candidates.Where(candidate =>
                     IsStaticCompatible(candidate, mapMethod)
                     && IsContextCompatible(candidate, mapMethod)
                     && !Contains(target, candidate)))
        {
            target.Add(candidate);
        }
    }

    private List<ValueTransformer> GetOwnTransformers()
    {
        if (this.ownTransformers is not null)
        {
            return this.ownTransformers;
        }

        var transformers = new List<ValueTransformer>();
        var overriddenMethods = new HashSet<IMethodSymbol>(SymbolEqualityComparer.Default);

        // Local tier first (declaration order), then the base hierarchy, most-derived type first.
        INamedTypeSymbol? currentType = this.mapperClass;
        while (currentType is not null && currentType.SpecialType != SpecialType.System_Object)
        {
            foreach (var method in currentType.GetMembers().OfType<IMethodSymbol>())
            {
                if (method.OverriddenMethod is { } overriddenMethod)
                {
                    overriddenMethods.Add(overriddenMethod.OriginalDefinition);
                }

                if (!overriddenMethods.Contains(method.OriginalDefinition)
                    && this.CreateDiscoveredTransformer(method, null, null) is { } transformer)
                {
                    transformers.Add(transformer);
                }
            }

            currentType = currentType.BaseType;
        }

        this.ownTransformers = transformers;
        return transformers;
    }

    private List<ValueTransformer> GetDependencyTransformers()
    {
        if (this.dependencyTransformers is not null)
        {
            return this.dependencyTransformers;
        }

        var transformers = new List<ValueTransformer>();

        // Same relative discovery order used to collect map methods from dependencies:
        // static dependencies, then [MappaDependency] properties, then [MappaDependency] fields.
        foreach (var dependencyType in this.mapperClass.GetAttributes().GetMappaStaticDependencies(this.compilation))
        {
            foreach (var method in dependencyType.GetMembers().OfType<IMethodSymbol>().Where(method => method.IsStatic))
            {
                if (this.CreateDiscoveredTransformer(method, null, dependencyType) is { } transformer)
                {
                    transformers.Add(transformer);
                }
            }
        }

        foreach (var dependencyMember in this.GetMappaDependencyMembers())
        {
            foreach (var method in ValueTransformer.GetFieldOrPropertyType(dependencyMember).GetMethodsInTypeHierarchy())
            {
                if (this.CreateDiscoveredTransformer(method, dependencyMember, null) is { } transformer)
                {
                    transformers.Add(transformer);
                }
            }
        }

        this.dependencyTransformers = transformers;
        return transformers;
    }

    private IEnumerable<ISymbol> GetMappaDependencyMembers()
    {
        var processedNames = new HashSet<string>(StringComparer.Ordinal);
        var members = this.mapperClass.GetMembers();

        var localProperties = members.OfType<IPropertySymbol>().Where(property =>
            property.GetMethod is not null
            && property.GetAttributes().HasMappaDependencyAttribute(this.compilation));
        foreach (var property in localProperties.Where(property => processedNames.Add(property.Name)))
        {
            yield return property;
        }

        var inheritedProperties = this.compilation.GetMappaDependencyPropertiesInMapperBaseTypeHierarchy(this.mapperClass);
        foreach (var property in inheritedProperties.Where(property => processedNames.Add(property.Name)))
        {
            yield return property;
        }

        var localFields = members.OfType<IFieldSymbol>().Where(field =>
            field.GetAttributes().HasMappaDependencyAttribute(this.compilation));
        foreach (var field in localFields.Where(field => processedNames.Add(field.Name)))
        {
            yield return field;
        }

        var inheritedFields = this.compilation.GetMappaDependencyFieldsInMapperBaseTypeHierarchy(this.mapperClass);
        foreach (var field in inheritedFields.Where(field => processedNames.Add(field.Name)))
        {
            yield return field;
        }
    }

    private ValueTransformer? CreateDiscoveredTransformer(
        IMethodSymbol method,
        ISymbol? fieldOrProperty,
        ITypeSymbol? explicitType)
    {
        if (!method.GetAttributes().HasMappaTransformerAttribute(this.compilation)
            || !this.compilation.IsSymbolAccessibleWithin(method, this.mapperClass))
        {
            return null;
        }

        if (!ValueTransformerSignature.TryValidate(method, this.compilation, out var reason))
        {
            this.classContext.ReportDiagnostic(MappaDiagnostics.InvalidTransformerSignature(
                method.Locations.FirstOrDefault(),
                method.ToDisplayString(),
                reason));
            return null;
        }

        return new ValueTransformer(method, fieldOrProperty, explicitType, null, null);
    }

    private ValueTransformer[] ResolveApplyTransformers(MapMethod mapMethod, IMappaUserSettings mappaUserSettings)
    {
        if (mapMethod.MethodSymbol is null)
        {
            return [];
        }

        var attributes = mapMethod.MethodSymbol.GetAttributes().GetMappaApplyTransformerAttributes(this.compilation);
        var transformers = new List<ValueTransformer>();
        foreach (var attribute in attributes)
        {
            if (this.TryResolveApplyTransformer(mapMethod, attribute, mappaUserSettings, out var resolved))
            {
                transformers.AddRange(resolved);
            }
        }

        return [.. transformers];
    }

    private bool TryResolveApplyTransformer(
        MapMethod mapMethod,
        MappaApplyTransformerAttributeData attribute,
        IMappaUserSettings mappaUserSettings,
        out ValueTransformer[] resolved)
    {
        resolved = [];

        if (attribute.ClassType is not null && attribute.FieldName is not null)
        {
            this.classContext.ReportDiagnostic(MappaDiagnostics.ApplyTransformerClassTypeAndFieldNameConflict(
                attribute.Location,
                mapMethod.MethodName,
                attribute.MethodName));
            return false;
        }

        var sourcePathIsValid = this.ValidateSourcePropertyName(mapMethod, attribute, mappaUserSettings);
        if (!this.TryBuildLookup(mapMethod, attribute, out var lookup))
        {
            return false;
        }

        var transformers = this.FindTransformerMethods(mapMethod, attribute, lookup);
        if (transformers.Length == 0)
        {
            return false;
        }

        if (!sourcePathIsValid)
        {
            return false;
        }

        resolved = transformers;
        return true;
    }

    private bool ValidateSourcePropertyName(
        MapMethod mapMethod,
        MappaApplyTransformerAttributeData attribute,
        IMappaUserSettings mappaUserSettings)
    {
        if (attribute.SourcePropertyName is not { } sourcePropertyName)
        {
            return true;
        }

        var path = PropertyPath.Parse(sourcePropertyName);
        var includeFields = mappaUserSettings.MapSourceFields is BooleanSetting.Enable;
        if (path.Segments.Length > 0
            && PropertyPathSymbolResolver.TryResolveMemberPath(mapMethod.SourceType, path, includeFields, out _, out _))
        {
            return true;
        }

        this.classContext.ReportDiagnostic(MappaDiagnostics.ApplyTransformerSourceMemberNotFound(
            attribute.Location,
            mapMethod.MethodName,
            sourcePropertyName,
            mapMethod.SourceType.ToDisplayString()));
        return false;
    }

    private bool TryBuildLookup(
        MapMethod mapMethod,
        MappaApplyTransformerAttributeData attribute,
        out TransformerLookup lookup)
    {
        if (attribute.FieldName is { } fieldName)
        {
            return this.TryBuildFieldLookup(mapMethod, attribute, fieldName, out lookup);
        }

        if (attribute.ClassType is { } classType)
        {
            lookup = new TransformerLookup(classType, true, null, classType);
            return true;
        }

        lookup = new TransformerLookup(this.mapperClass, mapMethod.CanBeUsedByStaticMethod, null, null);
        return true;
    }

    private bool TryBuildFieldLookup(
        MapMethod mapMethod,
        MappaApplyTransformerAttributeData attribute,
        string fieldName,
        out TransformerLookup lookup)
    {
        lookup = default;
        var fieldOrProperty = string.IsNullOrWhiteSpace(fieldName)
            ? null
            : this.compilation.LocateAccessibleFieldOrPropertyInTypeHierarchy(
                this.mapperClass,
                fieldName,
                this.mapperClass);
        if (fieldOrProperty is null)
        {
            this.classContext.ReportDiagnostic(MappaDiagnostics.CannotFindFieldOrProperty(
                attribute.Location,
                fieldName));
            return false;
        }

        lookup = new TransformerLookup(
            ValueTransformer.GetFieldOrPropertyType(fieldOrProperty),
            mapMethod.CanBeUsedByStaticMethod && !fieldOrProperty.IsStatic,
            fieldOrProperty,
            null);
        return true;
    }

    private ValueTransformer[] FindTransformerMethods(
        MapMethod mapMethod,
        MappaApplyTransformerAttributeData attribute,
        TransformerLookup lookup)
    {
        var transformers = this.LocateTransformerMethods(mapMethod, attribute, lookup, lookup.MustBeStatic);
        if (transformers.Length > 0)
        {
            return transformers;
        }

        // A static map cannot use an instance field: tell the user when an instance transformer would otherwise match.
        if (lookup.FieldOrProperty is { } fieldOrProperty
            && lookup.MustBeStatic
            && this.LocateTransformerMethods(mapMethod, attribute, lookup, false).Length > 0)
        {
            this.classContext.ReportDiagnostic(MappaDiagnostics.FieldOrPropertyMustBeStatic(
                fieldOrProperty.Name,
                attribute.Location));
            return [];
        }

        this.classContext.ReportDiagnostic(MappaDiagnostics.ApplyTransformerMethodNotFound(
            attribute.Location,
            mapMethod.MethodName,
            attribute.MethodName));
        return [];
    }

    private ValueTransformer[] LocateTransformerMethods(
        MapMethod mapMethod,
        MappaApplyTransformerAttributeData attribute,
        TransformerLookup lookup,
        bool mustBeStatic)
    {
        var candidates = lookup.LookupType
            .LocateMethodsIncludingInheritedInterfaces(attribute.MethodName)
            .Where(method => this.compilation.IsSymbolAccessibleWithin(method, this.mapperClass)
                             && (!mustBeStatic || method.IsStatic)
                             && ValueTransformerSignature.TryValidate(method, this.compilation, out _))
            .ToArray();

        // Overridden methods are replaced by their overrides.
        var overriddenMethods = new HashSet<IMethodSymbol>(SymbolEqualityComparer.Default);
        foreach (var directlyOverridden in candidates.Select(candidate => candidate.OverriddenMethod))
        {
            var overridden = directlyOverridden;
            while (overridden is not null)
            {
                overriddenMethods.Add(overridden.OriginalDefinition);
                overridden = overridden.OverriddenMethod;
            }
        }

        return candidates
            .Where(method => !overriddenMethods.Contains(method.OriginalDefinition))
            .Select(method => new ValueTransformer(
                method,
                lookup.FieldOrProperty,
                lookup.ExplicitType,
                attribute.SourcePropertyName,
                attribute.Location))
            .Where(transformer => IsContextCompatible(transformer, mapMethod))
            .ToArray();
    }

    private readonly struct TransformerLookup
    {
        internal TransformerLookup(
            ITypeSymbol lookupType,
            bool mustBeStatic,
            ISymbol? fieldOrProperty,
            ITypeSymbol? explicitType)
        {
            this.LookupType = lookupType;
            this.MustBeStatic = mustBeStatic;
            this.FieldOrProperty = fieldOrProperty;
            this.ExplicitType = explicitType;
        }

        internal ITypeSymbol LookupType { get; }

        internal bool MustBeStatic { get; }

        internal ISymbol? FieldOrProperty { get; }

        internal ITypeSymbol? ExplicitType { get; }
    }
}