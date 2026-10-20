// <copyright file="MappaUseConstructorSession.cs" company="Stefano Anelli">
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
/// Tracks <see cref="Mappa.Attributes.MappaUseConstructorAttribute"/> state for a single root map method.
/// </summary>
internal sealed class MappaUseConstructorSession
{
    private readonly HashSet<MappaUseConstructorAttributeData> usedAttributes = new(AttributeReferenceComparer.Instance);
    private readonly HashSet<MappaUseConstructorAttributeData> excludedFromUnusedWarning = new(AttributeReferenceComparer.Instance);

    private MappaUseConstructorAttributeData[] attributes = [];
    private bool attributesValidated;

    /// <summary>
    /// Initializes state from method attributes.
    /// </summary>
    /// <param name="methodAttributes">The method attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="rootTargetType">The root map method target type.</param>
    internal void Initialize(
        ImmutableArray<AttributeData> methodAttributes,
        Compilation compilation,
        ITypeSymbol rootTargetType)
    {
        this.attributes = methodAttributes.GetMappaUseConstructorAttributes(compilation);
        foreach (var attribute in this.attributes)
        {
            attribute.EffectiveTargetType = attribute.DeclaredTargetType ?? rootTargetType;
        }
    }

    /// <summary>
    /// Validates parsed attributes and reports diagnostics once per session.
    /// </summary>
    /// <param name="context">The algorithm context.</param>
    /// <param name="methodName">The map method name.</param>
    internal void ValidateAttributes(MappaMapAlgorithmContext context, string methodName)
    {
        if (this.attributesValidated)
        {
            return;
        }

        this.attributesValidated = true;
        var firstByTarget = new Dictionary<ITypeSymbol, MappaUseConstructorAttributeData>(SymbolEqualityComparer.Default);

        foreach (var attribute in this.attributes)
        {
            if (attribute.EffectiveTargetType is null)
            {
                this.excludedFromUnusedWarning.Add(attribute);
                continue;
            }

            if (!IsConfigurationValid(attribute, out var configurationError))
            {
                this.excludedFromUnusedWarning.Add(attribute);
                context.ReportDiagnostic(
                    MappaDiagnostics.MappaUseConstructorInvalidConfiguration(
                        attribute.Location,
                        methodName,
                        attribute.EffectiveTargetType,
                        configurationError));
                continue;
            }

            if (firstByTarget.TryGetValue(attribute.EffectiveTargetType, out var first))
            {
                this.excludedFromUnusedWarning.Add(first);
                this.excludedFromUnusedWarning.Add(attribute);
                context.ReportDiagnostic(
                    MappaDiagnostics.DuplicateMappaUseConstructorForTargetType(
                        attribute.Location,
                        methodName,
                        attribute.EffectiveTargetType));
                continue;
            }

            firstByTarget[attribute.EffectiveTargetType] = attribute;
        }
    }

    /// <summary>
    /// Tries to get a valid attribute for the given target type.
    /// </summary>
    /// <param name="targetType">The target type being constructed.</param>
    /// <param name="attribute">The matching attribute when found.</param>
    /// <returns><c>true</c> when a valid matching attribute exists.</returns>
    internal bool TryGetForTargetType(ITypeSymbol targetType, out MappaUseConstructorAttributeData? attribute)
    {
        foreach (var candidate in this.attributes)
        {
            if (this.excludedFromUnusedWarning.Contains(candidate)
                || candidate.EffectiveTargetType is null
                || !SymbolEqualityComparer.Default.Equals(candidate.EffectiveTargetType, targetType)
                || !IsConfigurationValid(candidate, out _))
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
    /// Marks an attribute as used for the mapping.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    internal void MarkUsed(MappaUseConstructorAttributeData attribute)
        => this.usedAttributes.Add(attribute);

    /// <summary>
    /// Reports MP00092 for attributes that were never used during successful mapping.
    /// </summary>
    /// <param name="context">The algorithm context.</param>
    /// <param name="methodName">The map method name.</param>
    internal void ReportUnusedWarnings(MappaMapAlgorithmContext context, string methodName)
    {
        foreach (var attribute in this.attributes)
        {
            if (this.usedAttributes.Contains(attribute)
                || this.excludedFromUnusedWarning.Contains(attribute)
                || attribute.EffectiveTargetType is null)
            {
                continue;
            }

            context.ReportDiagnostic(
                MappaDiagnostics.MappaUseConstructorAttributeNotUsed(
                    attribute.Location,
                    methodName,
                    attribute.EffectiveTargetType));
        }
    }

    private static bool IsConfigurationValid(MappaUseConstructorAttributeData attribute, out string error)
    {
        switch (attribute.ConstructorType)
        {
            case Mappa.ConstructorType.NoParameters when attribute.ParameterTypes.Length > 0:
                error = "ConstructorType.NoParameters must not specify parameter types";
                return false;
            case Mappa.ConstructorType.WithParameters when attribute.ParameterTypes.Length == 0:
                error = "ConstructorType.WithParameters must specify parameter types";
                return false;
            default:
                error = string.Empty;
                return true;
        }
    }

    private sealed class AttributeReferenceComparer : IEqualityComparer<MappaUseConstructorAttributeData>
    {
        internal static AttributeReferenceComparer Instance { get; } = new();

        public bool Equals(MappaUseConstructorAttributeData? x, MappaUseConstructorAttributeData? y)
            => ReferenceEquals(x, y);

        public int GetHashCode(MappaUseConstructorAttributeData obj)
            => RuntimeHelpers.GetHashCode(obj);
    }
}