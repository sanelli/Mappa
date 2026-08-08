// <copyright file="ConstructorMapStrategyDetector.PropertyPaths.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Property-path support for <see cref="ConstructorMapStrategyDetector"/>.
/// </summary>
internal sealed partial class ConstructorMapStrategyDetector
{
    /// <summary>
    /// Computes the property-path context to pass into a nested type mapping for <paramref name="targetMemberName"/>.
    /// </summary>
    /// <param name="targetMemberName">The target member currently being mapped.</param>
    /// <param name="nestedPropertyPathContext">The property-path context from the parent mapping, if any.</param>
    /// <returns>The context for the nested mapping, or <see langword="null"/> when nested-path handling does not apply.</returns>
    internal static PropertyPathContext? GetNestedTypeMappingPropertyPathContext(
        string targetMemberName,
        PropertyPathContext? nestedPropertyPathContext)
    {
        if (nestedPropertyPathContext is null)
        {
            return null;
        }

        if (nestedPropertyPathContext.IsNestedAttributeScope)
        {
            return nestedPropertyPathContext;
        }

        var fromRemainingSegments = TryGetNestedTypeMappingFromRemainingSegments(targetMemberName, nestedPropertyPathContext);
        if (fromRemainingSegments is not null)
        {
            return fromRemainingSegments;
        }

        return TryGetNestedTypeMappingFromOriginalPath(targetMemberName, nestedPropertyPathContext);
    }

    private static PropertyPathContext? TryGetNestedTypeMappingFromRemainingSegments(
        string targetMemberName,
        PropertyPathContext nestedPropertyPathContext)
    {
        if (nestedPropertyPathContext.RemainingTargetSegments.Length == 0)
        {
            return null;
        }

        if (nestedPropertyPathContext.RemainingTargetSegments[0].Equals(targetMemberName, StringComparison.Ordinal)
            && nestedPropertyPathContext.RemainingTargetSegments.Length > 1)
        {
            return nestedPropertyPathContext.DescendOneLevel();
        }

        return nestedPropertyPathContext;
    }

    private static PropertyPathContext? TryGetNestedTypeMappingFromOriginalPath(
        string targetMemberName,
        PropertyPathContext nestedPropertyPathContext)
    {
        var originalTargetPath = PropertyPath.Parse(nestedPropertyPathContext.OriginalTargetPath);
        if (!originalTargetPath.IsNested)
        {
            return null;
        }

        var originalTargetFirstSegment = originalTargetPath.GetFirstSegment();
        if (originalTargetFirstSegment is not null
            && originalTargetFirstSegment.Equals(targetMemberName, StringComparison.Ordinal))
        {
            return PropertyPathContext.CreateNestedAttributeScope(targetMemberName);
        }

        return nestedPropertyPathContext;
    }

    private static bool IsMustMapEmptyCtorUnmappedProperty(
        IPropertySymbol propertyWithoutStrategy,
        MappaMustMapTargetPropertyAttribute? mustMapAttribute)
    {
        return mustMapAttribute is not null
               && !propertyWithoutStrategy.IsRequired
               && (mustMapAttribute.TargetPropertyNames.Length == 0
                   || mustMapAttribute.TargetPropertyNames.Contains(propertyWithoutStrategy.Name, StringComparer.Ordinal));
    }

    private MapMethod GetAttributeMapMethod()
        => this.context.MapMethod ?? this.context.GetRootMapMethod();

    private bool ShouldIgnoreTargetPropertyAtCurrentLevel(string targetMemberName)
    {
        var mapMethod = this.context.MapMethod ?? this.context.GetRootMapMethod();
        if (this.context.AlgorithmSettings.UseAttributesForConstructorDetectorSettings
            .Equals(MappaMapAlgorithmContextSettings.MappaAttributesForConstructorDetectorSettings.Disable))
        {
            return false;
        }

        return mapMethod.GetAttributes<MappaIgnoreTargetPropertyAttribute>()
            .Any(ignoreAttribute => PropertyPathAttributeMatching.MatchesTargetMember(
                ignoreAttribute.TargetPropertyName,
                targetMemberName,
                this.context.PropertyPathContext,
                StringComparison.Ordinal)
                && this.IsIgnoreAttributeActiveAtCurrentLevel(ignoreAttribute.TargetPropertyName));
    }

    private bool IsIgnoreAttributeActiveAtCurrentLevel(string targetPropertyPath)
    {
        var parsedPath = PropertyPath.Parse(targetPropertyPath);
        if (this.context.PropertyPathContext is null)
        {
            return parsedPath.Segments.Length == 1;
        }

        if (this.context.PropertyPathContext.IsNestedAttributeScope)
        {
            return parsedPath.IsNested;
        }

        return this.context.PropertyPathContext.IsLeafTargetMapping;
    }

    private bool HasNestedPathAttributesForTargetMember(
        string targetMemberName,
        StringComparison stringComparison)
        => this.CountNestedPathAttributesForTargetMember(targetMemberName, stringComparison) > 0;

    private int CountNestedPathAttributesForTargetMember(
        string targetMemberName,
        StringComparison stringComparison)
    {
        var mapMethod = this.GetAttributeMapMethod();
        var nestedTargetNameAttributeCount = mapMethod
            .GetAttributes<Attribute>()
            .OfType<IMappaTargetPropertyNameAttribute>()
            .Where(attribute => PropertyPath.Parse(attribute.TargetPropertyName).IsNested)
            .Count(attribute => PropertyPathAttributeMatching.MatchesTargetMember(
                attribute.TargetPropertyName,
                targetMemberName,
                null,
                stringComparison));

        // MappaUsePropertyAttribute does not implement IMappaTargetPropertyNameAttribute.
        var nestedUsePropertyAttributeCount = mapMethod
            .GetAttributes<MappaUsePropertyAttribute>()
            .Where(attribute => PropertyPath.Parse(attribute.TargetPropertyName).IsNested)
            .Count(attribute => PropertyPathAttributeMatching.MatchesTargetMember(
                attribute.TargetPropertyName,
                targetMemberName,
                null,
                stringComparison));

        // Ignore does not implement IMappaTargetPropertyNameAttribute.
        var nestedIgnoreAttributeCount = mapMethod
            .GetAttributes<MappaIgnoreTargetPropertyAttribute>()
            .Where(attribute => PropertyPath.Parse(attribute.TargetPropertyName).IsNested)
            .Count(attribute => PropertyPathAttributeMatching.MatchesTargetMember(
                attribute.TargetPropertyName,
                targetMemberName,
                null,
                stringComparison));

        return nestedTargetNameAttributeCount + nestedUsePropertyAttributeCount + nestedIgnoreAttributeCount;
    }

    private MappaUsePropertyAttribute[] GetMatchingUsePropertyAttributes(
        string targetMemberName,
        StringComparison stringComparison)
    {
        return this.GetAttributeMapMethod()
            .GetAttributes<MappaUsePropertyAttribute>()
            .Where(attribute => PropertyPathAttributeMatching.MatchesTargetMember(
                attribute.TargetPropertyName,
                targetMemberName,
                this.context.PropertyPathContext,
                stringComparison))
            .ToArray();
    }

    private bool TryResolveExpectedSourcePropertyName(
        MappaUsePropertyAttribute usePropertyAttribute,
        bool isLeafTargetMapping,
        out string expectedSourcePropertyName,
        out bool useExactNameFromAttribute,
        out PropertyPathContext? nestedPropertyPathContext,
        out ChainedSourcePropertyPathInfo? chainedSourcePropertyPath)
    {
        expectedSourcePropertyName = string.Empty;
        useExactNameFromAttribute = true;
        nestedPropertyPathContext = null;
        chainedSourcePropertyPath = null;

        var targetPath = PropertyPath.Parse(usePropertyAttribute.TargetPropertyName);
        var sourcePath = PropertyPath.Parse(usePropertyAttribute.SourcePropertyName);
        var activePropertyPathContext = this.GetActivePropertyPathContextForUseProperty(usePropertyAttribute);

        if (this.TryCreateChainedSourceFromRootNestedSource(usePropertyAttribute, targetPath, sourcePath, out chainedSourcePropertyPath))
        {
            return false;
        }

        if (this.TryCreateChainedSourceForLeafTargetMapping(
                usePropertyAttribute,
                sourcePath,
                activePropertyPathContext,
                isLeafTargetMapping,
                out chainedSourcePropertyPath))
        {
            return false;
        }

        var expectedSegment = PropertyPathAttributeMatching.GetExpectedSourcePropertyNameForCurrentLevel(
            usePropertyAttribute.SourcePropertyName,
            activePropertyPathContext,
            isLeafTargetMapping);

        if (expectedSegment is null)
        {
            return false;
        }

        expectedSourcePropertyName = expectedSegment;
        nestedPropertyPathContext = targetPath.IsNested
            ? PropertyPathAttributeMatching.CreatePropertyPathContext(
                usePropertyAttribute.TargetPropertyName,
                usePropertyAttribute.SourcePropertyName)
            : null;
        return true;
    }

    private PropertyPathContext? GetActivePropertyPathContextForUseProperty(MappaUsePropertyAttribute usePropertyAttribute)
    {
        var activePropertyPathContext = this.context.PropertyPathContext;
        if (activePropertyPathContext?.IsNestedAttributeScope == true)
        {
            return PropertyPathAttributeMatching.CreatePropertyPathContext(
                usePropertyAttribute.TargetPropertyName,
                usePropertyAttribute.SourcePropertyName);
        }

        return activePropertyPathContext;
    }

    private bool TryCreateChainedSourceFromRootNestedSource(
        MappaUsePropertyAttribute usePropertyAttribute,
        PropertyPath targetPath,
        PropertyPath sourcePath,
        out ChainedSourcePropertyPathInfo? chainedSourcePropertyPath)
    {
        chainedSourcePropertyPath = null;
        if (this.context.PropertyPathContext is not null || targetPath.IsNested || !sourcePath.IsNested)
        {
            return false;
        }

        chainedSourcePropertyPath = new ChainedSourcePropertyPathInfo(
            usePropertyAttribute.SourcePropertyName,
            sourcePath.Segments,
            this.context.GetRootSourceType(),
            this.context.GetRootMapMethod().SourceParameterName);
        return true;
    }

    private bool TryCreateChainedSourceForLeafTargetMapping(
        MappaUsePropertyAttribute usePropertyAttribute,
        PropertyPath sourcePath,
        PropertyPathContext? activePropertyPathContext,
        bool isLeafTargetMapping,
        out ChainedSourcePropertyPathInfo? chainedSourcePropertyPath)
    {
        chainedSourcePropertyPath = null;
        if (!isLeafTargetMapping
            || (!sourcePath.IsNested && activePropertyPathContext is null)
            || this.context.PropertyPathContext is null)
        {
            return false;
        }

        var remainingSourceSegments = this.GetRemainingSourceSegmentsForLeafMapping(
            sourcePath,
            activePropertyPathContext);
        if (remainingSourceSegments.Length == 0)
        {
            return false;
        }

        chainedSourcePropertyPath = new ChainedSourcePropertyPathInfo(
            usePropertyAttribute.SourcePropertyName,
            remainingSourceSegments,
            this.context.SourceType,
            string.Empty);
        return true;
    }

    private string[] GetRemainingSourceSegmentsForLeafMapping(
        PropertyPath sourcePath,
        PropertyPathContext? activePropertyPathContext)
    {
        if (this.context.PropertyPathContext is null)
        {
            return [];
        }

        if (this.context.PropertyPathContext.IsNestedAttributeScope)
        {
            return activePropertyPathContext?.RemainingSourceSegments ?? sourcePath.Segments;
        }

        return this.context.PropertyPathContext.RemainingSourceSegments.Length > 0
            ? this.context.PropertyPathContext.RemainingSourceSegments
            : sourcePath.Segments;
    }

    private bool IsMappingAttributeActiveAtCurrentLevel(string targetPropertyPath)
    {
        var parsedPath = PropertyPath.Parse(targetPropertyPath);
        if (!parsedPath.IsNested)
        {
            return true;
        }

        if (this.context.PropertyPathContext?.IsNestedAttributeScope == true)
        {
            return true;
        }

        if (this.context.PropertyPathContext is null)
        {
            return false;
        }

        return this.context.PropertyPathContext.IsLeafTargetMapping;
    }

    private bool IsLeafTargetMappingForAttribute(string targetPropertyPath)
    {
        if (this.context.PropertyPathContext?.IsNestedAttributeScope == true)
        {
            return PropertyPath.Parse(targetPropertyPath).IsNested;
        }

        if (this.context.PropertyPathContext is not null)
        {
            return this.context.PropertyPathContext.IsLeafTargetMapping;
        }

        return !PropertyPath.Parse(targetPropertyPath).IsNested;
    }

    private bool TryGetStrategyBetweenTypes(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        bool useConstructorMapStrategyDetector,
        PropertyPathContext? propertyPathContext,
        out MapStrategy elementStrategy)
    {
        using (this.context.AlgorithmSettings.UseConstructorMapStrategyDetector.Apply(useConstructorMapStrategyDetector))
        {
            var attributesSetting = propertyPathContext is not null
                ? MappaMapAlgorithmContextSettings.MappaAttributesForConstructorDetectorSettings.Enable
                : MappaMapAlgorithmContextSettings.MappaAttributesForConstructorDetectorSettings.Disable;

            using (this.context.AlgorithmSettings.UseAttributesForConstructorDetectorSettings.Apply(attributesSetting))
            {
                var derivedContext = new DerivedMappaMapAlgorithmContext(
                    this.context,
                    targetType,
                    sourceType,
                    propertyPathContext);
                var algorithm = new TypeMapIdentifierWithMapMethodAlgorithm(derivedContext, this.compilation, this.cancellationToken);
                elementStrategy = algorithm.GetStrategy();
                return elementStrategy is not NoMapStrategy;
            }
        }
    }

    private bool TryResolveChainedSourceProperty(
        ChainedSourcePropertyPathInfo chainedSourcePropertyPath,
        out IPropertySymbol[] resolvedProperties,
        out IPropertySymbol? firstProperty)
    {
        resolvedProperties = [];
        firstProperty = null;

        ITypeSymbol chainReceiverType;
        if (string.IsNullOrWhiteSpace(chainedSourcePropertyPath.ReceiverPathPrefix))
        {
            chainReceiverType = chainedSourcePropertyPath.StartingSourceType;
        }
        else
        {
            var rootSourceType = this.context.GetRootSourceType();
            var rootReceiverExpression = this.context.GetRootMapMethod().SourceParameterName;
            if (!PropertyPathSymbolResolver.TryGetReceiverTypeForPathPrefix(
                    rootSourceType,
                    rootReceiverExpression,
                    chainedSourcePropertyPath.ReceiverPathPrefix,
                    out chainReceiverType))
            {
                chainReceiverType = chainedSourcePropertyPath.StartingSourceType;
            }
        }

        if (!PropertyPathSymbolResolver.TryResolvePropertyPath(
                chainReceiverType,
                PropertyPath.FromRemainingSegments(chainedSourcePropertyPath.RemainingSourceSegments),
                out resolvedProperties,
                out _))
        {
            return false;
        }

        firstProperty = resolvedProperties[0];
        return true;
    }

    private bool AttributeTargetPathMatches(
        string attributeTargetPath,
        string targetName,
        StringComparison stringComparison)
        => PropertyPathAttributeMatching.MatchesTargetMember(
            attributeTargetPath,
            targetName,
            this.context.PropertyPathContext,
            stringComparison);
}