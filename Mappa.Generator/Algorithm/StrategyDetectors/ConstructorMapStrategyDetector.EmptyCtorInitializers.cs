// <copyright file="ConstructorMapStrategyDetector.EmptyCtorInitializers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Empty-constructor property initializer mapping for <see cref="ConstructorMapStrategyDetector"/>.
/// </summary>
internal sealed partial class ConstructorMapStrategyDetector
{
    private static string GetExpectedSourcePropertyNameFromMultipleUsePropertyAttributes(
        MappaUsePropertyAttribute[] usePropertyAttributes,
        string targetPropertyName,
        out bool useExactNameFromAttribute)
    {
        useExactNameFromAttribute = false;
        var distinctSourceRoots = usePropertyAttributes
            .Select(attribute => PropertyPath.Parse(attribute.SourcePropertyName).GetFirstSegment())
            .Where(segment => segment is not null)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        if (distinctSourceRoots.Length != 1)
        {
            return targetPropertyName;
        }

        var sourceRoot = distinctSourceRoots[0];
        if (sourceRoot is null)
        {
            return targetPropertyName;
        }

        useExactNameFromAttribute = true;
        return sourceRoot;
    }

    private PropertyMapStrategy TryCreateEmptyCtorPropertyMapStrategy(
        IPropertySymbol targetProperty,
        IPropertySymbol[] allTargetProperties,
        IPropertySymbol[] sourceProperties,
        NoMapStrategy noMapStrategy)
    {
        var sourceResolution = this.ResolveEmptyCtorPropertySourceNaming(targetProperty, noMapStrategy);
        if (sourceResolution.FailureStrategy is PropertyMapStrategy failureStrategy)
        {
            return failureStrategy;
        }

        var nestedPropertyPathContext = this.ApplyNestedPathContextForEmptyCtorTarget(
            targetProperty.Name,
            sourceResolution.NestedPropertyPathContext);

        var sourceProperty = this.TryResolveEmptyCtorSourceProperty(
            sourceProperties,
            sourceResolution,
            out var chainedSourcePropertyPath);

        var attributeStrategy = this.TryCreateEmptyCtorPropertyMapStrategyFromAttributes(
            targetProperty,
            allTargetProperties,
            sourceProperties,
            noMapStrategy,
            chainedSourcePropertyPath,
            ref sourceProperty);
        if (attributeStrategy is not null)
        {
            return attributeStrategy;
        }

        var chainedStrategy = this.TryCreateEmptyCtorPropertyMapStrategyFromChainedSource(
            targetProperty,
            allTargetProperties,
            chainedSourcePropertyPath);
        if (chainedStrategy is not null)
        {
            return chainedStrategy;
        }

        var canWriteTargetProperty = this.TryIsTargetPropertyWritable(targetProperty, out var requiresUnsafeAccessorOnTargetSetter);
        var readonlyGetterStrategy = this.TryCreateReadonlyGetterBackedEmptyCtorPropertyMapStrategy(
            targetProperty,
            sourceProperty,
            canWriteTargetProperty,
            noMapStrategy);
        if (readonlyGetterStrategy is not null)
        {
            return readonlyGetterStrategy;
        }

        return this.TryCreateWritableEmptyCtorPropertyMapStrategy(
            targetProperty,
            allTargetProperties,
            sourceProperties,
            sourceProperty,
            nestedPropertyPathContext,
            canWriteTargetProperty,
            requiresUnsafeAccessorOnTargetSetter,
            noMapStrategy);
    }

    private EmptyCtorPropertySourceResolution ResolveEmptyCtorPropertySourceNaming(
        IPropertySymbol targetProperty,
        NoMapStrategy noMapStrategy)
    {
        var usePropertyAttributes = this.GetMatchingUsePropertyAttributes(
            targetProperty.Name,
            StringComparison.Ordinal);

        return usePropertyAttributes.Length switch
        {
            0 => new EmptyCtorPropertySourceResolution(
                targetProperty.Name,
                UseExactNameFromAttribute: false,
                NestedPropertyPathContext: null,
                ChainedSourcePropertyPath: null,
                FailureStrategy: null),
            1 => this.ResolveEmptyCtorPropertySourceNamingFromSingleUsePropertyAttribute(
                targetProperty,
                usePropertyAttributes[0]),
            _ => this.ResolveEmptyCtorPropertySourceNamingFromMultipleUsePropertyAttributes(
                targetProperty,
                usePropertyAttributes,
                noMapStrategy),
        };
    }

    private EmptyCtorPropertySourceResolution ResolveEmptyCtorPropertySourceNamingFromSingleUsePropertyAttribute(
        IPropertySymbol targetProperty,
        MappaUsePropertyAttribute usePropertyAttribute)
    {
        var isLeafTargetMapping = this.IsLeafTargetMappingForAttribute(usePropertyAttribute.TargetPropertyName);
        if (this.TryResolveExpectedSourcePropertyName(
                usePropertyAttribute,
                isLeafTargetMapping,
                out var expectedSourcePropertyName,
                out var useExactNameFromAttribute,
                out var nestedPropertyPathContext,
                out var chainedSourcePropertyPath))
        {
            return new EmptyCtorPropertySourceResolution(
                expectedSourcePropertyName,
                useExactNameFromAttribute,
                nestedPropertyPathContext,
                chainedSourcePropertyPath,
                FailureStrategy: null);
        }

        return new EmptyCtorPropertySourceResolution(
            targetProperty.Name,
            UseExactNameFromAttribute: true,
            nestedPropertyPathContext,
            chainedSourcePropertyPath,
            FailureStrategy: null);
    }

    private EmptyCtorPropertySourceResolution ResolveEmptyCtorPropertySourceNamingFromMultipleUsePropertyAttributes(
        IPropertySymbol targetProperty,
        MappaUsePropertyAttribute[] usePropertyAttributes,
        NoMapStrategy noMapStrategy)
    {
        var distinctTargetPaths = usePropertyAttributes
            .Select(attribute => attribute.TargetPropertyName)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        if (distinctTargetPaths.Length == 1)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.TooManyUsePropertyAttributesForTheSameTargetProperty(
                this.context.GetRootMapMethod().MethodDeclarationSyntax,
                this.context.GetRootMapMethod().MethodName,
                targetProperty.Name));
            return new EmptyCtorPropertySourceResolution(
                string.Empty,
                false,
                null,
                null,
                new PropertyMapStrategy(targetProperty, null, noMapStrategy, false));
        }

        var expectedSourcePropertyName = GetExpectedSourcePropertyNameFromMultipleUsePropertyAttributes(
            usePropertyAttributes,
            targetProperty.Name,
            out var useExactNameFromAttribute);

        return new EmptyCtorPropertySourceResolution(
            expectedSourcePropertyName,
            useExactNameFromAttribute,
            PropertyPathContext.CreateNestedAttributeScope(targetProperty.Name),
            null,
            FailureStrategy: null);
    }

    private PropertyPathContext? ApplyNestedPathContextForEmptyCtorTarget(
        string targetPropertyName,
        PropertyPathContext? nestedPropertyPathContext)
    {
        if (this.context.PropertyPathContext is not null)
        {
            return nestedPropertyPathContext;
        }

        if (!this.HasNestedPathAttributesForTargetMember(targetPropertyName, StringComparison.Ordinal))
        {
            return nestedPropertyPathContext;
        }

        if (nestedPropertyPathContext is not null
            && this.CountNestedPathAttributesForTargetMember(targetPropertyName, StringComparison.Ordinal) <= 1)
        {
            return nestedPropertyPathContext;
        }

        return PropertyPathContext.CreateNestedAttributeScope(targetPropertyName);
    }

    private IPropertySymbol? TryResolveEmptyCtorSourceProperty(
        IPropertySymbol[] sourceProperties,
        EmptyCtorPropertySourceResolution sourceResolution,
        out ChainedSourcePropertyPathInfo? chainedSourcePropertyPath)
    {
        chainedSourcePropertyPath = sourceResolution.ChainedSourcePropertyPath;
        if (chainedSourcePropertyPath is null)
        {
            PropertyMapNameMatcher.TryFindSourceProperty(
                sourceProperties,
                sourceResolution.ExpectedSourcePropertyName,
                this.context.MappaUserSettings.CaseInsensitivePropertyMap,
                this.context.MappaUserSettings.IgnoreUnderscoreForPropertyMap,
                isConstructorParameterPath: false,
                sourceResolution.UseExactNameFromAttribute,
                out var sourceProperty);
            return sourceProperty;
        }

        if (!this.TryResolveChainedSourceProperty(
                chainedSourcePropertyPath,
                out var resolvedSourceProperties,
                out _))
        {
            return null;
        }

        return resolvedSourceProperties[0];
    }

    private PropertyMapStrategy? TryCreateEmptyCtorPropertyMapStrategyFromAttributes(
        IPropertySymbol targetProperty,
        IPropertySymbol[] allTargetProperties,
        IPropertySymbol[] sourceProperties,
        NoMapStrategy noMapStrategy,
        ChainedSourcePropertyPathInfo? chainedSourcePropertyPath,
        ref IPropertySymbol? sourceProperty)
    {
        if (this.context.MapMethod is null && this.context.PropertyPathContext is null)
        {
            return null;
        }

        if (!this.TryGetStrategyForPropertyOrArgumentUsingAttributesOnMethod(
                targetProperty.Name,
                targetProperty.Type,
                this.context.SourceType,
                ref sourceProperty,
                StringComparison.Ordinal,
                isConstructorParameterPath: false,
                out var propertyStrategyFromAttribute))
        {
            return null;
        }

        if (!this.TryIsTargetPropertyWritable(targetProperty, out var requiresUnsafeAccessorOnTargetFromAttribute))
        {
            this.context.ReportDiagnostic(MappaDiagnostics.PropertySetterIsNotAccessible(
                this.context.GetRootMapMethod().MethodDeclarationSyntax,
                this.context.TargetType,
                targetProperty));
            return new PropertyMapStrategy(targetProperty, null, noMapStrategy, false);
        }

        propertyStrategyFromAttribute = this.EncapsulateMapStrategyForSourceOptional(
            sourceProperty,
            sourceProperties,
            propertyStrategyFromAttribute);
        propertyStrategyFromAttribute = this.EncapsulateMapStrategyForTargetOptional(
            targetProperty,
            allTargetProperties,
            propertyStrategyFromAttribute,
            out var postConstructorInitializer);
        var requiresUnsafeAccessorOnSourceFromAttribute = sourceProperty is not null
            && this.TryIsSourcePropertyReadable(sourceProperty, out var sourceRequiresUnsafeFromAttribute)
            && sourceRequiresUnsafeFromAttribute;
        return new PropertyMapStrategy(
            targetProperty,
            sourceProperty,
            propertyStrategyFromAttribute,
            postConstructorInitializer,
            chainedSourcePropertyPath,
            requiresUnsafeAccessorOnSourceFromAttribute,
            requiresUnsafeAccessorOnTargetFromAttribute);
    }

    private PropertyMapStrategy? TryCreateEmptyCtorPropertyMapStrategyFromChainedSource(
        IPropertySymbol targetProperty,
        IPropertySymbol[] allTargetProperties,
        ChainedSourcePropertyPathInfo? chainedSourcePropertyPath)
    {
        if (chainedSourcePropertyPath is null
            || !this.TryResolveChainedSourceProperty(
                chainedSourcePropertyPath,
                out var chainedSourceProperties,
                out _))
        {
            return null;
        }

        var innerSourceType = chainedSourceProperties[chainedSourceProperties.Length - 1].Type;
        MapStrategy chainedPropertyStrategy = new IdentityMapStrategy(targetProperty.Type, innerSourceType);
        chainedPropertyStrategy = this.EncapsulateMapStrategyForTargetOptional(
            targetProperty,
            allTargetProperties,
            chainedPropertyStrategy,
            out var chainedPostConstructorInitializer);
        this.TryIsTargetPropertyWritable(targetProperty, out var requiresUnsafeAccessorOnTargetFromChain);
        return new PropertyMapStrategy(
            targetProperty,
            null,
            chainedPropertyStrategy,
            chainedPostConstructorInitializer,
            chainedSourcePropertyPath,
            requiresUnsafeAccessorOnSource: false,
            requiresUnsafeAccessorOnTarget: requiresUnsafeAccessorOnTargetFromChain);
    }

    private PropertyMapStrategy? TryCreateReadonlyGetterBackedEmptyCtorPropertyMapStrategy(
        IPropertySymbol targetProperty,
        IPropertySymbol? sourceProperty,
        bool canWriteTargetProperty,
        NoMapStrategy noMapStrategy)
    {
        if (sourceProperty is null
            || canWriteTargetProperty
            || !this.TryIsTargetPropertyGetterReadable(targetProperty, out var requiresUnsafeAccessorOnTargetGetter))
        {
            return null;
        }

        if (this.TryCreateReadonlyDictionaryEmptyCtorPropertyMapStrategy(
                targetProperty,
                sourceProperty,
                requiresUnsafeAccessorOnTargetGetter,
                out var dictionaryStrategy))
        {
            return dictionaryStrategy;
        }

        if (this.TryCreateReadonlyStackEmptyCtorPropertyMapStrategy(
                targetProperty,
                sourceProperty,
                requiresUnsafeAccessorOnTargetGetter,
                out var stackStrategy))
        {
            return stackStrategy;
        }

        if (this.TryCreateReadonlyQueueEmptyCtorPropertyMapStrategy(
                targetProperty,
                sourceProperty,
                requiresUnsafeAccessorOnTargetGetter,
                out var queueStrategy))
        {
            return queueStrategy;
        }

        if (this.TryCreateReadonlyAddCollectionEmptyCtorPropertyMapStrategy(
                targetProperty,
                sourceProperty,
                requiresUnsafeAccessorOnTargetGetter,
                out var addCollectionStrategy))
        {
            return addCollectionStrategy;
        }

        if (this.TryCreateReadonlyCollectionEmptyCtorPropertyMapStrategy(
                targetProperty,
                sourceProperty,
                requiresUnsafeAccessorOnTargetGetter,
                out var collectionStrategy))
        {
            return collectionStrategy;
        }

        return new PropertyMapStrategy(targetProperty, null, noMapStrategy, false);
    }

    private bool TryCreateReadonlyDictionaryEmptyCtorPropertyMapStrategy(
        IPropertySymbol targetProperty,
        IPropertySymbol sourceProperty,
        bool requiresUnsafeAccessorOnTargetGetter,
        out PropertyMapStrategy? propertyMapStrategy)
    {
        propertyMapStrategy = null;
        if (!targetProperty.Type.IsOrImplementIDictionary(this.compilation)
            || !sourceProperty.Type.IsOrImplementIDictionary(this.compilation)
            || !this.context.TryGetKeyAndValueStrategy(
                targetProperty.Type,
                sourceProperty.Type,
                this.compilation,
                out var keyStrategy,
                out var valueStrategy,
                this.cancellationToken))
        {
            return false;
        }

        var dictionaryPropertyStrategy = new ReadonlyDictionaryPropertyMapStrategy(
            targetProperty,
            sourceProperty,
            keyStrategy,
            valueStrategy,
            DictionaryAssignmentSettingHelper.GetEffective(this.context.MappaUserSettings.DictionaryAssignment));
        this.TryIsSourcePropertyReadable(sourceProperty, out var requiresUnsafeAccessorOnSourceDictionary);
        propertyMapStrategy = new PropertyMapStrategy(
            targetProperty,
            sourceProperty,
            dictionaryPropertyStrategy,
            true,
            requiresUnsafeAccessorOnSource: requiresUnsafeAccessorOnSourceDictionary,
            requiresUnsafeAccessorOnTarget: requiresUnsafeAccessorOnTargetGetter);
        return true;
    }

    private bool TryCreateReadonlyStackEmptyCtorPropertyMapStrategy(
        IPropertySymbol targetProperty,
        IPropertySymbol sourceProperty,
        bool requiresUnsafeAccessorOnTargetGetter,
        out PropertyMapStrategy? propertyMapStrategy)
    {
        propertyMapStrategy = null;
        if (!(targetProperty.Type.IsOrDerivedFromStack(this.compilation)
              || targetProperty.Type.IsOrDerivedFromConcurrentStack(this.compilation))
            || !(sourceProperty.Type.IsArray() || sourceProperty.Type.IsOrImplementIEnumerable())
            || !this.context.TryGetElementStrategy(
                targetProperty.Type,
                sourceProperty.Type,
                this.compilation,
                out var stackElementStrategy,
                this.cancellationToken))
        {
            return false;
        }

        var stackPropertyStrategy = new ReadonlyStackPropertyMapStrategy(targetProperty, sourceProperty, stackElementStrategy);
        this.TryIsSourcePropertyReadable(sourceProperty, out var requiresUnsafeAccessorOnSourceStack);
        propertyMapStrategy = new PropertyMapStrategy(
            targetProperty,
            sourceProperty,
            stackPropertyStrategy,
            true,
            requiresUnsafeAccessorOnSource: requiresUnsafeAccessorOnSourceStack,
            requiresUnsafeAccessorOnTarget: requiresUnsafeAccessorOnTargetGetter);
        return true;
    }

    private bool TryCreateReadonlyQueueEmptyCtorPropertyMapStrategy(
        IPropertySymbol targetProperty,
        IPropertySymbol sourceProperty,
        bool requiresUnsafeAccessorOnTargetGetter,
        out PropertyMapStrategy? propertyMapStrategy)
    {
        propertyMapStrategy = null;
        if (!(targetProperty.Type.IsOrDerivedFromQueue(this.compilation)
              || targetProperty.Type.IsOrImplementConcurrentQueue(this.compilation))
            || !(sourceProperty.Type.IsArray() || sourceProperty.Type.IsOrImplementIEnumerable())
            || !this.context.TryGetElementStrategy(
                targetProperty.Type,
                sourceProperty.Type,
                this.compilation,
                out var queueElementStrategy,
                this.cancellationToken))
        {
            return false;
        }

        var queuePropertyStrategy = new ReadonlyQueuePropertyMapStrategy(targetProperty, sourceProperty, queueElementStrategy);
        this.TryIsSourcePropertyReadable(sourceProperty, out var requiresUnsafeAccessorOnSourceQueue);
        propertyMapStrategy = new PropertyMapStrategy(
            targetProperty,
            sourceProperty,
            queuePropertyStrategy,
            true,
            requiresUnsafeAccessorOnSource: requiresUnsafeAccessorOnSourceQueue,
            requiresUnsafeAccessorOnTarget: requiresUnsafeAccessorOnTargetGetter);
        return true;
    }

    private bool TryCreateReadonlyAddCollectionEmptyCtorPropertyMapStrategy(
        IPropertySymbol targetProperty,
        IPropertySymbol sourceProperty,
        bool requiresUnsafeAccessorOnTargetGetter,
        out PropertyMapStrategy? propertyMapStrategy)
    {
        propertyMapStrategy = null;
        if (!(targetProperty.Type.IsOrDerivedFromConcurrentBag(this.compilation)
              || targetProperty.Type.IsOrDerivedFromBlockingCollection(this.compilation))
            || !(sourceProperty.Type.IsArray() || sourceProperty.Type.IsOrImplementIEnumerable())
            || !this.context.TryGetElementStrategy(
                targetProperty.Type,
                sourceProperty.Type,
                this.compilation,
                out var addCollectionElementStrategy,
                this.cancellationToken))
        {
            return false;
        }

        var addCollectionPropertyStrategy = new ReadonlyAddCollectionPropertyMapStrategy(
            targetProperty,
            sourceProperty,
            addCollectionElementStrategy);
        this.TryIsSourcePropertyReadable(sourceProperty, out var requiresUnsafeAccessorOnSourceAddCollection);
        propertyMapStrategy = new PropertyMapStrategy(
            targetProperty,
            sourceProperty,
            addCollectionPropertyStrategy,
            true,
            requiresUnsafeAccessorOnSource: requiresUnsafeAccessorOnSourceAddCollection,
            requiresUnsafeAccessorOnTarget: requiresUnsafeAccessorOnTargetGetter);
        return true;
    }

    private bool TryCreateReadonlyCollectionEmptyCtorPropertyMapStrategy(
        IPropertySymbol targetProperty,
        IPropertySymbol sourceProperty,
        bool requiresUnsafeAccessorOnTargetGetter,
        out PropertyMapStrategy? propertyMapStrategy)
    {
        propertyMapStrategy = null;
        if (!targetProperty.Type.IsOrImplementICollection()
            || !(sourceProperty.Type.IsArray() || sourceProperty.Type.IsOrImplementIEnumerable())
            || !this.context.TryGetElementStrategy(
                targetProperty.Type,
                sourceProperty.Type,
                this.compilation,
                out var elementStrategy,
                this.cancellationToken))
        {
            return false;
        }

        var collectionPropertyStrategy = new ReadonlyCollectionPropertyMapStrategy(
            targetProperty,
            sourceProperty,
            elementStrategy);
        this.TryIsSourcePropertyReadable(sourceProperty, out var requiresUnsafeAccessorOnSourceCollection);
        propertyMapStrategy = new PropertyMapStrategy(
            targetProperty,
            sourceProperty,
            collectionPropertyStrategy,
            true,
            requiresUnsafeAccessorOnSource: requiresUnsafeAccessorOnSourceCollection,
            requiresUnsafeAccessorOnTarget: requiresUnsafeAccessorOnTargetGetter);
        return true;
    }

    private PropertyMapStrategy TryCreateWritableEmptyCtorPropertyMapStrategy(
        IPropertySymbol targetProperty,
        IPropertySymbol[] allTargetProperties,
        IPropertySymbol[] sourceProperties,
        IPropertySymbol? sourceProperty,
        PropertyPathContext? nestedPropertyPathContext,
        bool canWriteTargetProperty,
        bool requiresUnsafeAccessorOnTargetSetter,
        NoMapStrategy noMapStrategy)
    {
        if (sourceProperty is null || !canWriteTargetProperty)
        {
            return new PropertyMapStrategy(targetProperty, sourceProperty, noMapStrategy, false);
        }

        if (!this.TryGetStrategyBetweenTypes(
                targetProperty.Type,
                sourceProperty.Type,
                true,
                ConstructorMapStrategyDetector.GetNestedTypeMappingPropertyPathContext(
                    targetProperty.Name,
                    nestedPropertyPathContext),
                out var propertyStrategy))
        {
            return new PropertyMapStrategy(targetProperty, null, noMapStrategy, false);
        }

        propertyStrategy = this.EncapsulateMapStrategyForSourceOptional(sourceProperty, sourceProperties, propertyStrategy);
        propertyStrategy = this.EncapsulateMapStrategyForTargetOptional(
            targetProperty,
            allTargetProperties,
            propertyStrategy,
            out var postConstructorInitializer);
        this.TryIsSourcePropertyReadable(sourceProperty, out var requiresUnsafeAccessorOnSource);
        return new PropertyMapStrategy(
            targetProperty,
            sourceProperty,
            propertyStrategy,
            postConstructorInitializer,
            requiresUnsafeAccessorOnSource: requiresUnsafeAccessorOnSource,
            requiresUnsafeAccessorOnTarget: requiresUnsafeAccessorOnTargetSetter);
    }

    private bool ReportEmptyCtorDiagnosticsForUnmappedProperties(
        PropertyMapStrategy[] propertiesWithStrategies,
        PropertyMapStrategy[] propertiesWithoutStrategy)
    {
        var mustMapAttribute = this.GetMustMapTargetPropertyAttribute();
        var mustMapFailed = false;

        foreach (var propertyWithoutStrategy in propertiesWithoutStrategy
                     .Select(propertyStrategy => propertyStrategy.TargetProperty)
                     .Where(this.IsReportableEmptyCtorUnmappedProperty))
        {
            mustMapFailed |= this.TryReportMustMapOrNonRequiredEmptyCtorDiagnostic(
                propertyWithoutStrategy,
                propertiesWithStrategies,
                mustMapAttribute);
        }

        return mustMapFailed;
    }

    private bool TryReportMustMapOrNonRequiredEmptyCtorDiagnostic(
        IPropertySymbol propertyWithoutStrategy,
        PropertyMapStrategy[] propertiesWithStrategies,
        MappaMustMapTargetPropertyAttribute? mustMapAttribute)
    {
        if (IsMustMapEmptyCtorUnmappedProperty(propertyWithoutStrategy, mustMapAttribute))
        {
            this.context.ReportDiagnostic(MappaDiagnostics.MustMapTargetPropertyWasNotMapped(
                this.context.GetRootMapMethod().MethodDeclarationSyntax,
                this.context.TargetType,
                propertyWithoutStrategy));
            return true;
        }

        if (propertiesWithStrategies.Length > 0)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.CannotMapNonRequiredProperty(
                this.context.GetRootMapMethod().MethodDeclarationSyntax,
                this.context.TargetType,
                propertyWithoutStrategy));
        }

        return false;
    }

    private bool IsReportableEmptyCtorUnmappedProperty(IPropertySymbol propertyWithoutStrategy)
    {
        var targetCollections = propertyWithoutStrategy.Type.IsPostInitializationCollectionType(this.compilation);
        var hasSetter = this.TryIsTargetPropertyWritable(propertyWithoutStrategy, out _);
        return hasSetter || targetCollections;
    }

    private readonly record struct EmptyCtorPropertySourceResolution(
        string ExpectedSourcePropertyName,
        bool UseExactNameFromAttribute,
        PropertyPathContext? NestedPropertyPathContext,
        ChainedSourcePropertyPathInfo? ChainedSourcePropertyPath,
        PropertyMapStrategy? FailureStrategy);
}