// <copyright file="ConstructorMapStrategyDetector.NullSubstitute.cs" company="Stefano Anelli">
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
/// Null-substitute attribute handling for <see cref="ConstructorMapStrategyDetector"/>.
/// </summary>
internal sealed partial class ConstructorMapStrategyDetector
{
    private MappaNullSubstituteAttribute[] GetMatchingNullSubstituteAttributes(
        string targetMemberName,
        StringComparison stringComparison)
    {
        if (this.context.MapMethod is null && this.context.PropertyPathContext is null)
        {
            return [];
        }

        return this.GetAttributeMapMethod()
            .GetAttributes<MappaNullSubstituteAttribute>()
            .Where(attribute => PropertyPathAttributeMatching.MatchesTargetMember(
                attribute.TargetPropertyName,
                targetMemberName,
                this.context.PropertyPathContext,
                stringComparison))
            .Where(attribute => this.IsMappingAttributeActiveAtCurrentLevel(attribute.TargetPropertyName))
            .ToArray();
    }

    private bool HasMatchingAssignFromConstantAttribute(
        string targetMemberName,
        StringComparison stringComparison)
    {
        if (this.context.MapMethod is null && this.context.PropertyPathContext is null)
        {
            return false;
        }

        return this.GetAttributeMapMethod()
            .GetAttributes<MappaAssignFromConstantAttribute>()
            .Any(attribute => PropertyPathAttributeMatching.MatchesTargetMember(
                attribute.TargetPropertyName,
                targetMemberName,
                this.context.PropertyPathContext,
                stringComparison)
                && this.IsMappingAttributeActiveAtCurrentLevel(attribute.TargetPropertyName));
    }

    private MapStrategy TryWrapWithNullSubstituteAssignValue(
        string targetName,
        StringComparison stringComparison,
        IPropertySymbol? sourceProperty,
        MapStrategy strategy)
    {
        if (strategy is NoMapStrategy
            or MappaAssignFromConstantAttributeStrategy
            or MappaInvokeMethodAttributeStrategy
            or MappaAssignFromContextAttributeStrategy)
        {
            return strategy;
        }

        if (this.HasMatchingAssignFromConstantAttribute(targetName, stringComparison))
        {
            return strategy;
        }

        var nullSubstituteAttributes = this.GetMatchingNullSubstituteAttributes(targetName, stringComparison);
        if (nullSubstituteAttributes.Length != 1)
        {
            return strategy;
        }

        var nullSubstituteAttribute = nullSubstituteAttributes[0];
        if (!nullSubstituteAttribute.HasAssignValue)
        {
            // InvokeMethod mode is handled in a later step.
            return strategy;
        }

        var sourceType = sourceProperty?.Type ?? strategy.SourceType;
        return new NullSubstituteMapStrategy(
            strategy.TargetType,
            sourceType,
            nullSubstituteAttribute.AssignValue,
            strategy);
    }

    private bool TryResolveNullSubstituteSourceNaming(
        MappaNullSubstituteAttribute nullSubstituteAttribute,
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

        var targetPath = PropertyPath.Parse(nullSubstituteAttribute.TargetPropertyName);
        var sourcePath = PropertyPath.Parse(nullSubstituteAttribute.SourcePropertyName);
        var activePropertyPathContext = this.GetActivePropertyPathContextForSourcePath(
            nullSubstituteAttribute.TargetPropertyName,
            nullSubstituteAttribute.SourcePropertyName);

        if (this.TryCreateChainedSourceFromRootNestedSourcePath(
                nullSubstituteAttribute.SourcePropertyName,
                targetPath,
                sourcePath,
                out chainedSourcePropertyPath))
        {
            return false;
        }

        if (this.TryCreateChainedSourceForLeafTargetMappingPath(
                nullSubstituteAttribute.SourcePropertyName,
                sourcePath,
                activePropertyPathContext,
                isLeafTargetMapping,
                out chainedSourcePropertyPath))
        {
            return false;
        }

        var expectedSegment = PropertyPathAttributeMatching.GetExpectedSourcePropertyNameForCurrentLevel(
            nullSubstituteAttribute.SourcePropertyName,
            activePropertyPathContext,
            isLeafTargetMapping);

        if (expectedSegment is null)
        {
            return false;
        }

        expectedSourcePropertyName = expectedSegment;
        nestedPropertyPathContext = targetPath.IsNested
            ? PropertyPathAttributeMatching.CreatePropertyPathContext(
                nullSubstituteAttribute.TargetPropertyName,
                nullSubstituteAttribute.SourcePropertyName)
            : null;
        return true;
    }

    private PropertyPathContext? GetActivePropertyPathContextForSourcePath(
        string targetPropertyName,
        string sourcePropertyName)
    {
        var activePropertyPathContext = this.context.PropertyPathContext;
        if (activePropertyPathContext?.IsNestedAttributeScope == true)
        {
            return PropertyPathAttributeMatching.CreatePropertyPathContext(
                targetPropertyName,
                sourcePropertyName);
        }

        return activePropertyPathContext;
    }

    private bool TryCreateChainedSourceFromRootNestedSourcePath(
        string sourcePropertyName,
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
            sourcePropertyName,
            sourcePath.Segments,
            this.context.GetRootSourceType(),
            this.context.GetRootMapMethod().SourceParameterName);
        return true;
    }

    private bool TryCreateChainedSourceForLeafTargetMappingPath(
        string sourcePropertyName,
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
            sourcePropertyName,
            remainingSourceSegments,
            this.context.SourceType,
            string.Empty);
        return true;
    }
}