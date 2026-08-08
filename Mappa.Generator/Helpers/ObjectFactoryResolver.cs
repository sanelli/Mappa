// <copyright file="ObjectFactoryResolver.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Resolves <see cref="Mappa.Attributes.MappaObjectFactoryAttribute"/> registrations for a mapping method.
/// </summary>
internal sealed class ObjectFactoryResolver
{
    private readonly Compilation compilation;
    private readonly MappaMapAlgorithmContext context;
    private readonly MapMethod mapMethod;
    private readonly INamedTypeSymbol mapClass;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectFactoryResolver"/> class.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <param name="context">The map algorithm context.</param>
    /// <param name="mapMethod">The root mapping method.</param>
    internal ObjectFactoryResolver(
        Compilation compilation,
        MappaMapAlgorithmContext context,
        MapMethod mapMethod)
    {
        this.compilation = compilation;
        this.context = context;
        this.mapMethod = mapMethod;
        this.mapClass = mapMethod.ContainingType;
    }

    /// <summary>
    /// Tries to resolve an object factory for the current target type.
    /// </summary>
    /// <param name="classAttributes">The class-level object factory attributes.</param>
    /// <param name="methodAttributes">The method-level object factory attributes.</param>
    /// <param name="objectFactory">The resolved factory when successful.</param>
    /// <returns><c>true</c> when a factory is resolved; otherwise <c>false</c>.</returns>
    internal bool TryResolveForTargetType(
        MappaObjectFactoryAttributeData[] classAttributes,
        MappaObjectFactoryAttributeData[] methodAttributes,
        out ObjectFactory? objectFactory)
    {
        objectFactory = null;
        var matchingAttributes = classAttributes
            .Concat(methodAttributes)
            .Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.TargetType, this.context.TargetType))
            .ToArray();

        if (matchingAttributes.Length == 0)
        {
            return false;
        }

        // Duplicates are validated before strategy detection; if more than one remains, take the first.
        var attribute = matchingAttributes[0];
        objectFactory = this.ResolveFactory(attribute);
        return objectFactory is not null;
    }

    private static ITypeSymbol GetFieldOrPropertyType(ISymbol fieldOrProperty)
        => fieldOrProperty switch
        {
            IFieldSymbol field => field.Type,
            IPropertySymbol property => property.Type,
            _ => throw new MappaGeneratorException($"Unexpected symbol kind '{fieldOrProperty.Kind}' for field or property '{fieldOrProperty.Name}'."),
        };

    private static TierPickResult PickFromTier(
        IMethodSymbol[] pool,
        Func<IMethodSymbol, bool> predicate)
    {
        var matches = pool.Where(predicate).ToArray();
        if (matches.Length == 0)
        {
            return new TierPickResult(InvokeMethodResolutionResult.NotFound, null);
        }

        return ResolveUniqueCandidate(matches);
    }

    private static TierPickResult ResolveUniqueCandidate(IMethodSymbol[] candidates)
    {
        if (candidates.Length == 0)
        {
            return new TierPickResult(InvokeMethodResolutionResult.NotFound, null);
        }

        if (candidates.Length == 1)
        {
            return new TierPickResult(InvokeMethodResolutionResult.Success, candidates[0]);
        }

        var preferredContainingType = candidates[0].ContainingType;
        var filteredCandidates = candidates
            .Where(candidate => SymbolEqualityComparer.Default.Equals(candidate.ContainingType, preferredContainingType))
            .ToArray();

        if (filteredCandidates.Length == 1)
        {
            return new TierPickResult(InvokeMethodResolutionResult.Success, filteredCandidates[0]);
        }

        return new TierPickResult(InvokeMethodResolutionResult.Ambiguous, null);
    }

    private static bool ApplyTierPick(
        TierPickResult tierPick,
        ObjectFactoryInvocationKind kind,
        ref IMethodSymbol? method,
        ref ObjectFactoryInvocationKind invocationKind)
    {
        if (tierPick.Status is InvokeMethodResolutionResult.NotFound)
        {
            return false;
        }

        method = tierPick.Method;
        invocationKind = kind;
        return true;
    }

    private ObjectFactory? ResolveFactory(MappaObjectFactoryAttributeData attribute)
    {
        if (!this.TryBuildFactoryLookup(attribute, out var lookup))
        {
            return null;
        }

        var methods = lookup.LookupType.LocateMethodsIncludingInheritedInterfaces(attribute.MethodName);
        var resolution = this.TryResolveFactoryMethod(
            methods,
            attribute.MethodName,
            lookup.StaticRequirement,
            out var method,
            out var invocationKind);

        if (this.ShouldRejectNonStaticFactoryField(
                resolution,
                lookup.FieldOrProperty,
                methods,
                attribute))
        {
            return null;
        }

        if (resolution is not InvokeMethodResolutionResult.Success || method is null)
        {
            this.ReportFactoryNotFound(attribute);
            return null;
        }

        return new ObjectFactory(method, lookup.FieldOrProperty, lookup.ExplicitType, invocationKind, attribute.Location);
    }

    private bool TryBuildFactoryLookup(MappaObjectFactoryAttributeData attribute, out FactoryLookup lookup)
    {
        if (attribute.FieldName is not null)
        {
            return this.TryBuildFieldFactoryLookup(attribute, out lookup);
        }

        if (attribute.ClassType is not null)
        {
            return this.TryBuildClassTypeFactoryLookup(attribute, out lookup);
        }

        lookup = new FactoryLookup(
            this.mapClass,
            this.mapMethod.CanBeUsedByStaticMethod ? InvokeMethodStaticRequirement.Static : InvokeMethodStaticRequirement.StaticOrNotStatic,
            null,
            null);
        return true;
    }

    private bool TryBuildFieldFactoryLookup(MappaObjectFactoryAttributeData attribute, out FactoryLookup lookup)
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
            this.ReportFactoryNotFound(attribute);
            lookup = default;
            return false;
        }

        var staticRequirement = this.mapMethod.CanBeUsedByStaticMethod && !fieldOrProperty.IsStatic
            ? InvokeMethodStaticRequirement.Static
            : InvokeMethodStaticRequirement.StaticOrNotStatic;
        lookup = new FactoryLookup(GetFieldOrPropertyType(fieldOrProperty), staticRequirement, fieldOrProperty, null);
        return true;
    }

    private bool TryBuildClassTypeFactoryLookup(MappaObjectFactoryAttributeData attribute, out FactoryLookup lookup)
    {
        var classTypeFullName = attribute.ClassType?.FullName;
        if (classTypeFullName is not { Length: > 0 } metadataName || string.IsNullOrWhiteSpace(metadataName))
        {
            throw new MappaGeneratorException($"Cannot detect the full name for factory type '{attribute.ClassType}'.");
        }

        var explicitType = this.compilation.GetTypeByMetadataName(metadataName);
        if (explicitType is null)
        {
            this.ReportFactoryNotFound(attribute);
            lookup = default;
            return false;
        }

        lookup = new FactoryLookup(explicitType, InvokeMethodStaticRequirement.Static, null, explicitType);
        return true;
    }

    private bool ShouldRejectNonStaticFactoryField(
        InvokeMethodResolutionResult resolution,
        ISymbol? fieldOrProperty,
        IMethodSymbol[] methods,
        MappaObjectFactoryAttributeData attribute)
    {
        if (resolution is not InvokeMethodResolutionResult.NotFound
            || fieldOrProperty is null
            || !this.mapMethod.CanBeUsedByStaticMethod
            || fieldOrProperty.IsStatic)
        {
            return false;
        }

        var instanceResolution = this.TryResolveFactoryMethod(
            methods,
            attribute.MethodName,
            InvokeMethodStaticRequirement.NotStatic,
            out _,
            out _);
        if (instanceResolution is InvokeMethodResolutionResult.NotFound)
        {
            return false;
        }

        this.ReportFactoryNotFound(attribute);
        return true;
    }

    private InvokeMethodResolutionResult TryResolveFactoryMethod(
        IMethodSymbol[] methods,
        string methodName,
        InvokeMethodStaticRequirement staticRequirement,
        out IMethodSymbol? method,
        out ObjectFactoryInvocationKind invocationKind)
    {
        method = null;
        invocationKind = ObjectFactoryInvocationKind.FullyProduced;

        var candidates = this.FilterFactoryMethodCandidates(methods, methodName, staticRequirement);
        if (candidates.Length == 0)
        {
            return InvokeMethodResolutionResult.NotFound;
        }

        var standardTierResult = this.TryResolveStandardFactoryTiers(candidates, out method, out invocationKind);
        if (standardTierResult is not InvokeMethodResolutionResult.NotFound)
        {
            return standardTierResult;
        }

        return this.TryResolveParameterizedFactoryTier(candidates, out method, out invocationKind);
    }

    private IMethodSymbol[] FilterFactoryMethodCandidates(
        IMethodSymbol[] methods,
        string methodName,
        InvokeMethodStaticRequirement staticRequirement)
    {
        var nullableEnabled = this.mapMethod.NullableEnabled;
        var targetType = this.context.TargetType;

        return methods
            .Where(candidate =>
                candidate.Name.Equals(methodName, StringComparison.Ordinal) &&
                this.compilation.IsSymbolAccessibleWithin(candidate, this.mapClass) &&
                this.IsCompatibleReturnType(candidate.ReturnType, targetType, nullableEnabled) &&
                staticRequirement switch
                {
                    InvokeMethodStaticRequirement.StaticOrNotStatic => true,
                    InvokeMethodStaticRequirement.Static => candidate.IsStatic,
                    InvokeMethodStaticRequirement.NotStatic => !candidate.IsStatic,
                    _ => false,
                })
            .ToArray();
    }

    private InvokeMethodResolutionResult TryResolveStandardFactoryTiers(
        IMethodSymbol[] candidates,
        out IMethodSymbol? method,
        out ObjectFactoryInvocationKind invocationKind)
    {
        method = null;
        invocationKind = ObjectFactoryInvocationKind.FullyProduced;

        var nullableEnabled = this.mapMethod.NullableEnabled;
        var sourceType = this.context.SourceType;
        var mapMethodProvidesContext = this.mapMethod.ProvideMappaContextWhenInvoked();

        bool IsSourceParameter(IParameterSymbol parameter)
            => parameter.RefKind is RefKind.None &&
               parameter.Type.IsEqualTo(sourceType, nullableEnabled);

        bool IsContextParameter(IMethodSymbol candidate, int index)
            => candidate.Parameters[index].RefKind is RefKind.None &&
               candidate.ParameterIsMappaContext(this.compilation, index);

        if (mapMethodProvidesContext)
        {
            var sourceAndContextResult = PickFromTier(
                candidates,
                candidate => candidate.Parameters.Length == 2 &&
                             IsSourceParameter(candidate.Parameters[0]) &&
                             IsContextParameter(candidate, 1));
            if (ApplyTierPick(sourceAndContextResult, ObjectFactoryInvocationKind.FullyProduced, ref method, ref invocationKind))
            {
                return sourceAndContextResult.Status;
            }
        }

        var sourceOnlyResult = PickFromTier(
            candidates,
            candidate => candidate.Parameters.Length == 1 &&
                         IsSourceParameter(candidate.Parameters[0]));
        if (ApplyTierPick(sourceOnlyResult, ObjectFactoryInvocationKind.FullyProduced, ref method, ref invocationKind))
        {
            return sourceOnlyResult.Status;
        }

        if (mapMethodProvidesContext)
        {
            var contextOnlyResult = PickFromTier(
                candidates,
                candidate => candidate.Parameters.Length == 1 &&
                             IsContextParameter(candidate, 0));
            if (ApplyTierPick(contextOnlyResult, ObjectFactoryInvocationKind.EmptyCtorLike, ref method, ref invocationKind))
            {
                return contextOnlyResult.Status;
            }
        }

        var parameterlessResult = PickFromTier(
            candidates,
            candidate => candidate.Parameters.Length == 0);
        if (ApplyTierPick(parameterlessResult, ObjectFactoryInvocationKind.EmptyCtorLike, ref method, ref invocationKind))
        {
            return parameterlessResult.Status;
        }

        return InvokeMethodResolutionResult.NotFound;
    }

    private InvokeMethodResolutionResult TryResolveParameterizedFactoryTier(
        IMethodSymbol[] candidates,
        out IMethodSymbol? method,
        out ObjectFactoryInvocationKind invocationKind)
    {
        method = null;
        invocationKind = ObjectFactoryInvocationKind.ParameterizedLike;

        var nullableEnabled = this.mapMethod.NullableEnabled;
        var sourceType = this.context.SourceType;

        bool IsSourceParameter(IParameterSymbol parameter)
            => parameter.RefKind is RefKind.None &&
               parameter.Type.IsEqualTo(sourceType, nullableEnabled);

        bool IsContextParameter(IMethodSymbol candidate, int index)
            => candidate.Parameters[index].RefKind is RefKind.None &&
               candidate.ParameterIsMappaContext(this.compilation, index);

        var parameterizedCandidates = candidates
            .Where(candidate =>
                !(candidate.Parameters.Length == 2 &&
                  IsSourceParameter(candidate.Parameters[0]) &&
                  IsContextParameter(candidate, 1)) &&
                !(candidate.Parameters.Length == 1 && IsSourceParameter(candidate.Parameters[0])) &&
                !(candidate.Parameters.Length == 1 && IsContextParameter(candidate, 0)) &&
                candidate.Parameters.Length != 0)
            .OrderByDescending(candidate => candidate.Parameters.Length)
            .ToArray();

        if (parameterizedCandidates.Length == 0)
        {
            return InvokeMethodResolutionResult.NotFound;
        }

        var preferredParameterCount = parameterizedCandidates[0].Parameters.Length;
        var preferredCandidates = parameterizedCandidates
            .Where(candidate => candidate.Parameters.Length == preferredParameterCount)
            .ToArray();

        var parameterizedResult = ResolveUniqueCandidate(preferredCandidates);
        method = parameterizedResult.Method;
        return parameterizedResult.Status;
    }

    private bool IsCompatibleReturnType(ITypeSymbol returnType, ITypeSymbol targetType, bool nullableEnabled)
        => returnType.IsEqualTo(targetType, nullableEnabled) ||
           this.compilation.HasImplicitConversion(returnType, targetType);

    private void ReportFactoryNotFound(MappaObjectFactoryAttributeData attribute)
    {
        this.context.ReportDiagnostic(MappaDiagnostics.ObjectFactoryMethodNotFound(
            attribute.Location,
            this.mapMethod.MethodName,
            attribute.TargetType.ToDisplayString(),
            attribute.MethodName));
    }

    private readonly struct FactoryLookup
    {
        internal FactoryLookup(
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

    private sealed class TierPickResult
    {
        internal TierPickResult(InvokeMethodResolutionResult status, IMethodSymbol? method)
        {
            this.Status = status;
            this.Method = method;
        }

        internal InvokeMethodResolutionResult Status { get; }

        internal IMethodSymbol? Method { get; }
    }
}