// <copyright file="MapHookResolver.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Resolves before-map and after-map hook attributes for a generated mapping method.
/// </summary>
internal sealed class MapHookResolver
{
    private const string AfterMapHookKind = "after-map";
    private const string BeforeMapHookKind = "before-map";

    private readonly Compilation compilation;
    private readonly MappaClassGeneratorContext context;
    private readonly MapMethod mapMethod;
    private readonly INamedTypeSymbol mapClass;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapHookResolver"/> class.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <param name="context">The mapper class generator context.</param>
    /// <param name="mapMethod">The mapping method.</param>
    internal MapHookResolver(
        Compilation compilation,
        MappaClassGeneratorContext context,
        MapMethod mapMethod)
    {
        this.compilation = compilation;
        this.context = context;
        this.mapMethod = mapMethod;
        this.mapClass = mapMethod.ContainingType;
    }

    /// <summary>
    /// Resolves before-map hooks in class-level then method-level order.
    /// </summary>
    /// <param name="classAttributes">The class-level attributes.</param>
    /// <param name="methodAttributes">The method-level attributes.</param>
    /// <returns>The resolved hooks.</returns>
    internal MapHook[] ResolveBeforeMapHooks(
        MapHookAttributeData[] classAttributes,
        MapHookAttributeData[] methodAttributes)
    {
        var classHooks = this.ResolveHooks(
            classAttributes,
            this.mapMethod.SourceType,
            BeforeMapHookKind);
        var methodHooks = this.ResolveHooks(
            methodAttributes,
            this.mapMethod.SourceType,
            BeforeMapHookKind);

        return this.MergeScopes(classHooks, methodHooks, BeforeMapHookKind);
    }

    /// <summary>
    /// Resolves after-map hooks in method-level then class-level order.
    /// </summary>
    /// <param name="classAttributes">The class-level attributes.</param>
    /// <param name="methodAttributes">The method-level attributes.</param>
    /// <returns>The resolved hooks.</returns>
    internal MapHook[] ResolveAfterMapHooks(
        MapHookAttributeData[] classAttributes,
        MapHookAttributeData[] methodAttributes)
    {
        var methodHooks = this.ResolveHooks(
            methodAttributes,
            this.mapMethod.TargetType,
            AfterMapHookKind);
        var classHooks = this.ResolveHooks(
            classAttributes,
            this.mapMethod.TargetType,
            AfterMapHookKind);

        return this.MergeScopes(methodHooks, classHooks, AfterMapHookKind);
    }

    private static ITypeSymbol GetFieldOrPropertyType(ISymbol fieldOrProperty)
        => fieldOrProperty switch
        {
            IFieldSymbol field => field.Type,
            IPropertySymbol property => property.Type,
            _ => throw new MappaGeneratorException($"Unexpected symbol kind '{fieldOrProperty.Kind}' for field or property '{fieldOrProperty.Name}'."),
        };

    private MapHook[] ResolveHooks(
        IEnumerable<MapHookAttributeData> attributes,
        ITypeSymbol mappedValueType,
        string hookKind)
    {
        var hooks = new List<MapHook>();
        foreach (var attribute in attributes)
        {
            var hook = this.ResolveHook(
                attribute,
                mappedValueType,
                hookKind);
            if (hook is not null)
            {
                hooks.Add(hook);
            }
        }

        return [.. hooks];
    }

    private MapHook? ResolveHook(
        MapHookAttributeData attribute,
        ITypeSymbol mappedValueType,
        string hookKind)
    {
        if (!this.TryBuildHookLookup(attribute, out var lookup))
        {
            return null;
        }

        var methods = lookup.LookupType.LocateMethodsIncludingInheritedInterfaces(attribute.MethodName);
        var resolutionResult = this.TryResolveHook(
            lookup.LookupType,
            methods,
            attribute.MethodName,
            mappedValueType,
            lookup.StaticRequirement,
            attribute.Location,
            reportAmbiguity: true,
            out var method);

        if (this.ShouldReportFieldMustBeStatic(
                resolutionResult,
                lookup.FieldOrProperty,
                lookup.LookupType,
                methods,
                attribute,
                mappedValueType))
        {
            return null;
        }

        if (resolutionResult is InvokeMethodResolutionResult.NotFound)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.HookMethodNotFound(
                attribute.Location,
                this.mapMethod.MethodName,
                hookKind,
                attribute.MethodName));
            return null;
        }

        return resolutionResult is InvokeMethodResolutionResult.Success && method is not null
            ? new MapHook(method, lookup.FieldOrProperty, lookup.ExplicitType, attribute.Location)
            : null;
    }

    private bool TryBuildHookLookup(MapHookAttributeData attribute, out HookLookup lookup)
    {
        if (attribute.FieldName is not null)
        {
            return this.TryBuildFieldHookLookup(attribute, out lookup);
        }

        if (attribute.ClassType is not null)
        {
            return this.TryBuildClassTypeHookLookup(attribute, out lookup);
        }

        lookup = new HookLookup(
            this.mapClass,
            this.mapMethod.CanBeUsedByStaticMethod ? InvokeMethodStaticRequirement.Static : InvokeMethodStaticRequirement.StaticOrNotStatic,
            null,
            null);
        return true;
    }

    private bool TryBuildFieldHookLookup(MapHookAttributeData attribute, out HookLookup lookup)
    {
        if (attribute.FieldName is not { Length: > 0 } fieldName || string.IsNullOrWhiteSpace(fieldName))
        {
            lookup = default;
            return false;
        }

        var fieldOrProperty = this.compilation.LocateAccessibleFieldOrPropertyInTypeHierarchy(
            this.mapClass,
            fieldName,
            this.mapClass);
        if (fieldOrProperty is null)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.CannotFindFieldOrProperty(
                attribute.Location,
                fieldName));
            lookup = default;
            return false;
        }

        var staticRequirement = this.mapMethod.CanBeUsedByStaticMethod && !fieldOrProperty.IsStatic
            ? InvokeMethodStaticRequirement.Static
            : InvokeMethodStaticRequirement.StaticOrNotStatic;
        lookup = new HookLookup(GetFieldOrPropertyType(fieldOrProperty), staticRequirement, fieldOrProperty, null);
        return true;
    }

    private bool TryBuildClassTypeHookLookup(MapHookAttributeData attribute, out HookLookup lookup)
    {
        var classTypeFullName = attribute.ClassType?.FullName;
        if (classTypeFullName is not { Length: > 0 } metadataName || string.IsNullOrWhiteSpace(metadataName))
        {
            throw new MappaGeneratorException($"Cannot detect the full name for hook type '{attribute.ClassType}'.");
        }

        var explicitType = this.compilation.GetTypeByMetadataName(metadataName);
        if (explicitType is null)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.CannotDetectType(
                attribute.Location,
                classTypeFullName));
            lookup = default;
            return false;
        }

        lookup = new HookLookup(explicitType, InvokeMethodStaticRequirement.Static, null, explicitType);
        return true;
    }

    private bool ShouldReportFieldMustBeStatic(
        InvokeMethodResolutionResult resolutionResult,
        ISymbol? fieldOrProperty,
        ITypeSymbol lookupType,
        IMethodSymbol[] methods,
        MapHookAttributeData attribute,
        ITypeSymbol mappedValueType)
    {
        if (resolutionResult is not InvokeMethodResolutionResult.NotFound
            || fieldOrProperty is null
            || !this.mapMethod.CanBeUsedByStaticMethod
            || fieldOrProperty.IsStatic)
        {
            return false;
        }

        var instanceResolutionResult = this.TryResolveHook(
            lookupType,
            methods,
            attribute.MethodName,
            mappedValueType,
            InvokeMethodStaticRequirement.NotStatic,
            attribute.Location,
            reportAmbiguity: false,
            out _);
        if (instanceResolutionResult is InvokeMethodResolutionResult.NotFound)
        {
            return false;
        }

        this.context.ReportDiagnostic(MappaDiagnostics.FieldOrPropertyMustBeStatic(
            fieldOrProperty.Name,
            attribute.Location));
        return true;
    }

    private InvokeMethodResolutionResult TryResolveHook(
        ITypeSymbol lookupType,
        IMethodSymbol[] methods,
        string methodName,
        ITypeSymbol mappedValueType,
        InvokeMethodStaticRequirement staticRequirement,
        Location? location,
        bool reportAmbiguity,
        out IMethodSymbol? method)
    {
        var resolutionResult = InvokeMethodResolution.TryResolveMapHook(
            this.compilation,
            this.mapClass,
            lookupType,
            methods,
            methodName,
            mappedValueType,
            this.mapMethod.NullableEnabled,
            staticRequirement,
            this.mapMethod.ProvideMappaContextWhenInvoked(),
            out method,
            out var ambiguityDetails);

        if (reportAmbiguity && resolutionResult is InvokeMethodResolutionResult.Ambiguous)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.AmbiguousInvokeMethodResolution(
                location,
                ambiguityDetails));
        }

        return resolutionResult;
    }

    private MapHook[] MergeScopes(
        MapHook[] firstScopeHooks,
        MapHook[] secondScopeHooks,
        string hookKind)
    {
        var hooks = new List<MapHook>(firstScopeHooks);
        foreach (var hook in secondScopeHooks)
        {
            if (firstScopeHooks.Any(firstScopeHook =>
                    SymbolEqualityComparer.Default.Equals(
                        firstScopeHook.Method.OriginalDefinition,
                        hook.Method.OriginalDefinition)))
            {
                this.context.ReportDiagnostic(MappaDiagnostics.DuplicateMapHookRegistration(
                    hook.AttributeLocation,
                    this.mapMethod.MethodName,
                    hookKind,
                    hook.Method.ToDisplayString()));
                continue;
            }

            hooks.Add(hook);
        }

        return [.. hooks];
    }

    private readonly struct HookLookup
    {
        internal HookLookup(
            ITypeSymbol lookupType,
            InvokeMethodStaticRequirement staticRequirement,
            ISymbol? fieldOrProperty,
            ITypeSymbol? explicitType)
        {
            this.LookupType = lookupType;
            this.StaticRequirement = staticRequirement;
            this.FieldOrProperty = fieldOrProperty;
            this.ExplicitType = explicitType;
        }

        internal ITypeSymbol LookupType { get; }

        internal InvokeMethodStaticRequirement StaticRequirement { get; }

        internal ISymbol? FieldOrProperty { get; }

        internal ITypeSymbol? ExplicitType { get; }
    }
}