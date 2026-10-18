// <copyright file="PreferredInterfaceSession.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Tracks preferred-interface attributes and resolution for a single root map method.
/// </summary>
internal sealed class PreferredInterfaceSession
{
    private readonly HashSet<(INamedTypeSymbol Type, PreferredInterfaceFamily Family, PreferredInterfaceSide Side)> resolutionStack =
        new(PreferredInterfaceResolutionStackComparer.Instance);

    private readonly HashSet<PreferredInterfaceImplementationAttributeData> neededAttributes = new(PreferredInterfaceAttributeReferenceComparer.Instance);

    private PreferredInterfaceImplementationAttributeData[] attributes = [];
    private bool attributesValidated;

    /// <summary>
    /// Initializes preferred-interface state from method attributes.
    /// </summary>
    /// <param name="methodAttributes">The method attributes.</param>
    /// <param name="compilation">The compilation.</param>
    internal void Initialize(ImmutableArray<AttributeData> methodAttributes, Compilation compilation)
    {
        this.attributes = methodAttributes.GetPreferredInterfaceImplementationAttributes(compilation);
    }

    /// <summary>
    /// Validates parsed attributes and reports diagnostics once per session.
    /// </summary>
    /// <param name="context">The algorithm context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="methodName">The map method name.</param>
    internal void ValidateAttributes(MappaMapAlgorithmContext context, Compilation compilation, string methodName)
    {
        if (this.attributesValidated)
        {
            return;
        }

        this.attributesValidated = true;
        var duplicateKeys = new HashSet<PreferredInterfaceAttributeKey>(PreferredInterfaceAttributeKeyComparer.Instance);

        foreach (var attribute in this.attributes)
        {
            ValidateSingleAttribute(context, compilation, methodName, attribute, duplicateKeys);
        }
    }

    /// <summary>
    /// Reports MP00082 for attributes that were never needed during successful mapping.
    /// </summary>
    /// <param name="context">The algorithm context.</param>
    internal void ReportNotNeededWarnings(MappaMapAlgorithmContext context)
    {
        foreach (var attribute in this.attributes)
        {
            if (this.neededAttributes.Contains(attribute))
            {
                continue;
            }

            if (attribute.ImplementingType is not { } implementingType
                || attribute.PreferredClosedInterface is not { } preferredInterface)
            {
                continue;
            }

            context.ReportDiagnostic(
                MappaDiagnostics.PreferredInterfaceAttributeNotNeeded(
                    attribute.Location,
                    implementingType,
                    preferredInterface));
        }
    }

    /// <summary>
    /// Marks an attribute as needed for the mapping.
    /// </summary>
    /// <param name="attribute">The attribute data.</param>
    internal void MarkAttributeNeeded(PreferredInterfaceImplementationAttributeData attribute)
        => this.neededAttributes.Add(attribute);

    /// <summary>
    /// Tries to get a matching attribute for resolution.
    /// </summary>
    /// <param name="implementingType">The implementing type.</param>
    /// <param name="family">The interface family being resolved.</param>
    /// <param name="side">The mapping side.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="attribute">The matching attribute when found.</param>
    /// <returns><c>true</c> when a valid matching attribute exists.</returns>
    internal bool TryGetMatchingAttribute(
        INamedTypeSymbol implementingType,
        PreferredInterfaceFamily family,
        PreferredInterfaceSide side,
        Compilation compilation,
        out PreferredInterfaceImplementationAttributeData? attribute)
    {
        foreach (var candidate in this.attributes)
        {
            if (candidate.Side != side
                || candidate.ImplementingType is not { } attributeImplementingType
                || candidate.PreferredClosedInterface is not { } preferredClosedInterface)
            {
                continue;
            }

            if (!SymbolEqualityComparer.Default.Equals(attributeImplementingType, implementingType))
            {
                continue;
            }

            if (PreferredInterfaceResolver.TryGetFamily(preferredClosedInterface, compilation) != family)
            {
                continue;
            }

            attribute = candidate;
            return true;
        }

        attribute = null;
        return false;
    }

    /// <summary>
    /// Pushes a resolution frame; returns <c>false</c> when re-entering the same triple.
    /// </summary>
    /// <param name="type">The implementing type.</param>
    /// <param name="family">The interface family.</param>
    /// <param name="side">The mapping side.</param>
    /// <returns><c>true</c> when the frame was pushed.</returns>
    internal bool TryEnterResolution(INamedTypeSymbol type, PreferredInterfaceFamily family, PreferredInterfaceSide side)
        => this.resolutionStack.Add((type, family, side));

    /// <summary>
    /// Pops a resolution frame pushed by <see cref="TryEnterResolution"/>.
    /// </summary>
    /// <param name="type">The implementing type.</param>
    /// <param name="family">The interface family.</param>
    /// <param name="side">The mapping side.</param>
    internal void ExitResolution(INamedTypeSymbol type, PreferredInterfaceFamily family, PreferredInterfaceSide side)
        => this.resolutionStack.Remove((type, family, side));

    private static void ValidateSingleAttribute(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        string methodName,
        PreferredInterfaceImplementationAttributeData attribute,
        HashSet<PreferredInterfaceAttributeKey> duplicateKeys)
    {
        if (!attribute.HasImplementingType || attribute.ImplementingType is null)
        {
            context.ReportDiagnostic(MappaDiagnostics.PreferredInterfaceImplementingTypeInvalid(attribute.Location));
            return;
        }

        if (!PreferredInterfaceResolver.IsConcreteImplementingType(attribute.ImplementingType))
        {
            context.ReportDiagnostic(
                MappaDiagnostics.PreferredInterfaceImplementingTypeNotConcrete(
                    attribute.Location,
                    attribute.ImplementingTypeDisplay));
            return;
        }

        if (!attribute.HasPreferredInterface || attribute.PreferredClosedInterface is null)
        {
            context.ReportDiagnostic(
                MappaDiagnostics.PreferredInterfaceNotClosedGenericInterface(
                    attribute.Location,
                    attribute.PreferredInterfaceDisplay));
            return;
        }

        if (!PreferredInterfaceResolver.IsClosedGenericInterface(attribute.PreferredClosedInterface))
        {
            context.ReportDiagnostic(
                MappaDiagnostics.PreferredInterfaceNotClosedGenericInterface(
                    attribute.Location,
                    attribute.PreferredInterfaceDisplay));
            return;
        }

        var family = PreferredInterfaceResolver.TryGetFamily(attribute.PreferredClosedInterface, compilation);
        if (family is null)
        {
            context.ReportDiagnostic(
                MappaDiagnostics.PreferredInterfaceNotClosedGenericInterface(
                    attribute.Location,
                    attribute.PreferredInterfaceDisplay));
            return;
        }

        var duplicateKey = new PreferredInterfaceAttributeKey(attribute.Side, attribute.ImplementingType, family.Value);
        if (!duplicateKeys.Add(duplicateKey))
        {
            context.ReportDiagnostic(
                MappaDiagnostics.DuplicatePreferredInterfaceAttribute(
                    attribute.Location,
                    attribute.ImplementingType,
                    methodName));
            return;
        }

        if (!attribute.ImplementingType.AllInterfaces.Any(@interface =>
                SymbolEqualityComparer.Default.Equals(@interface, attribute.PreferredClosedInterface)))
        {
            context.ReportDiagnostic(
                MappaDiagnostics.PreferredInterfaceNotImplemented(
                    attribute.Location,
                    attribute.ImplementingType,
                    attribute.PreferredClosedInterface));
        }
    }

    private readonly struct PreferredInterfaceAttributeKey(
        PreferredInterfaceSide side,
        INamedTypeSymbol implementingType,
        PreferredInterfaceFamily family)
    {
        internal PreferredInterfaceSide Side { get; } = side;

        internal INamedTypeSymbol ImplementingType { get; } = implementingType;

        internal PreferredInterfaceFamily Family { get; } = family;
    }

    private sealed class PreferredInterfaceAttributeKeyComparer : IEqualityComparer<PreferredInterfaceAttributeKey>
    {
        internal static PreferredInterfaceAttributeKeyComparer Instance { get; } = new();

        public bool Equals(PreferredInterfaceAttributeKey x, PreferredInterfaceAttributeKey y)
            => x.Side == y.Side
               && x.Family == y.Family
               && SymbolEqualityComparer.Default.Equals(x.ImplementingType, y.ImplementingType);

        public int GetHashCode(PreferredInterfaceAttributeKey obj)
        {
            unchecked
            {
                var hash = (int)obj.Side;
                hash = (hash * 397) ^ (int)obj.Family;
                hash = (hash * 397) ^ SymbolEqualityComparer.Default.GetHashCode(obj.ImplementingType);
                return hash;
            }
        }
    }

    private sealed class PreferredInterfaceResolutionStackComparer
        : IEqualityComparer<(INamedTypeSymbol Type, PreferredInterfaceFamily Family, PreferredInterfaceSide Side)>
    {
        internal static PreferredInterfaceResolutionStackComparer Instance { get; } = new();

        public bool Equals(
            (INamedTypeSymbol Type, PreferredInterfaceFamily Family, PreferredInterfaceSide Side) x,
            (INamedTypeSymbol Type, PreferredInterfaceFamily Family, PreferredInterfaceSide Side) y)
            => x.Family == y.Family
               && x.Side == y.Side
               && SymbolEqualityComparer.Default.Equals(x.Type, y.Type);

        public int GetHashCode((INamedTypeSymbol Type, PreferredInterfaceFamily Family, PreferredInterfaceSide Side) obj)
        {
            unchecked
            {
                var hash = SymbolEqualityComparer.Default.GetHashCode(obj.Type);
                hash = (hash * 397) ^ (int)obj.Family;
                hash = (hash * 397) ^ (int)obj.Side;
                return hash;
            }
        }
    }

    private sealed class PreferredInterfaceAttributeReferenceComparer : IEqualityComparer<PreferredInterfaceImplementationAttributeData>
    {
        internal static PreferredInterfaceAttributeReferenceComparer Instance { get; } = new();

        public bool Equals(PreferredInterfaceImplementationAttributeData? x, PreferredInterfaceImplementationAttributeData? y)
            => ReferenceEquals(x, y);

        public int GetHashCode(PreferredInterfaceImplementationAttributeData obj)
            => RuntimeHelpers.GetHashCode(obj);
    }
}