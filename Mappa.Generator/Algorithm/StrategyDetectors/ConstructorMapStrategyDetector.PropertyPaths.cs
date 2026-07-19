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

        // Remaining segments are already relative to the nested type being mapped.
        // Descend when the current member is an intermediate (non-leaf) remaining segment.
        if (nestedPropertyPathContext.RemainingTargetSegments.Length > 0)
        {
            if (nestedPropertyPathContext.RemainingTargetSegments[0].Equals(targetMemberName, StringComparison.Ordinal)
                && nestedPropertyPathContext.RemainingTargetSegments.Length > 1)
            {
                return nestedPropertyPathContext.DescendOneLevel();
            }

            return nestedPropertyPathContext;
        }

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
        var activePropertyPathContext = this.context.PropertyPathContext;
        if (activePropertyPathContext?.IsNestedAttributeScope == true)
        {
            activePropertyPathContext = PropertyPathAttributeMatching.CreatePropertyPathContext(
                usePropertyAttribute.TargetPropertyName,
                usePropertyAttribute.SourcePropertyName);
        }

        if (this.context.PropertyPathContext is null && !targetPath.IsNested && sourcePath.IsNested)
        {
            chainedSourcePropertyPath = new ChainedSourcePropertyPathInfo(
                usePropertyAttribute.SourcePropertyName,
                sourcePath.Segments,
                this.context.GetRootSourceType(),
                this.context.GetRootMapMethod().MethodSymbol.Parameters[0].Name);
            return false;
        }

        if (isLeafTargetMapping && (sourcePath.IsNested || activePropertyPathContext is not null))
        {
            // Prefer reading remaining segments from the current nested source receiver
            // so intermediate temps are reused instead of rebuilding the chain from the root.
            if (this.context.PropertyPathContext is not null)
            {
                string[] remainingSourceSegments;
                if (this.context.PropertyPathContext.IsNestedAttributeScope)
                {
                    remainingSourceSegments = activePropertyPathContext?.RemainingSourceSegments ?? sourcePath.Segments;
                }
                else
                {
                    remainingSourceSegments = this.context.PropertyPathContext.RemainingSourceSegments.Length > 0
                        ? this.context.PropertyPathContext.RemainingSourceSegments
                        : sourcePath.Segments;
                }

                if (remainingSourceSegments.Length > 0)
                {
                    chainedSourcePropertyPath = new ChainedSourcePropertyPathInfo(
                        usePropertyAttribute.SourcePropertyName,
                        remainingSourceSegments,
                        this.context.SourceType,
                        string.Empty);
                    return false;
                }
            }

            var receiverPrefix = this.context.GetRootMapMethod().MethodSymbol.Parameters[0].Name;
            if (activePropertyPathContext is not null
                && sourcePath.Segments.Length > activePropertyPathContext.RemainingSourceSegments.Length)
            {
                var consumedCount = sourcePath.Segments.Length - activePropertyPathContext.RemainingSourceSegments.Length;
                receiverPrefix = string.Join(
                    ".",
                    new[] { this.context.GetRootMapMethod().MethodSymbol.Parameters[0].Name }
                        .Concat(sourcePath.Segments.Take(consumedCount)));
            }

            chainedSourcePropertyPath = new ChainedSourcePropertyPathInfo(
                usePropertyAttribute.SourcePropertyName,
                activePropertyPathContext?.RemainingSourceSegments ?? sourcePath.Segments,
                this.context.GetRootSourceType(),
                receiverPrefix);
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
            var rootReceiverExpression = this.context.GetRootMapMethod().MethodSymbol.Parameters[0].Name;
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