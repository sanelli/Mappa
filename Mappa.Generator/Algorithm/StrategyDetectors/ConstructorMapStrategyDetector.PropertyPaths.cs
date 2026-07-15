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
    private static PropertyPathContext? GetNestedTypeMappingPropertyPathContext(
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

        var originalTargetFirstSegment = PropertyPath.Parse(nestedPropertyPathContext.OriginalTargetPath).GetFirstSegment();
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

        return this.context.PropertyPathContext.IsLeafTargetMapping;
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
        nestedPropertyPathContext = PropertyPathAttributeMatching.CreatePropertyPathContext(
            usePropertyAttribute.TargetPropertyName,
            usePropertyAttribute.SourcePropertyName);
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

        var rootSourceType = this.context.GetRootSourceType();
        var rootReceiverExpression = this.context.GetRootMapMethod().MethodSymbol.Parameters[0].Name;
        if (!PropertyPathSymbolResolver.TryGetReceiverTypeForPathPrefix(
                rootSourceType,
                rootReceiverExpression,
                chainedSourcePropertyPath.ReceiverPathPrefix,
                out var chainReceiverType))
        {
            chainReceiverType = chainedSourcePropertyPath.StartingSourceType;
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