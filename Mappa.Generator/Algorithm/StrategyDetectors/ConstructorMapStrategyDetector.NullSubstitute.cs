// <copyright file="ConstructorMapStrategyDetector.NullSubstitute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Null-substitute attribute handling for <see cref="ConstructorMapStrategyDetector"/>.
/// </summary>
internal sealed partial class ConstructorMapStrategyDetector
{
    private static MapStrategy UnwrapNullableStrategies(MapStrategy strategy)
    {
        while (strategy is NullableStrategy nullableStrategy)
        {
            strategy = nullableStrategy.ElementStrategy;
        }

        return strategy;
    }

    private MappaNullSubstituteAttribute[] GetMatchingNullSubstituteAttributes(
        string targetMemberName,
        StringComparison stringComparison)
    {
        // Use the root map method when mapping in a derived context (e.g. projection element types).
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
        if (this.ShouldSkipNullSubstituteWrap(strategy, targetName, stringComparison))
        {
            return strategy;
        }

        var nullSubstituteAttribute = this.GetMatchingNullSubstituteAttributes(targetName, stringComparison)[0];
        var sourceType = sourceProperty?.Type ?? strategy.SourceType;
        var elementStrategy = UnwrapNullableStrategies(strategy);

        if (nullSubstituteAttribute.HasAssignValue)
        {
            return NullSubstituteMapStrategy.ForAssignValue(
                strategy.TargetType,
                sourceType,
                nullSubstituteAttribute.AssignValue,
                elementStrategy);
        }

        return this.TryWrapWithNullSubstituteInvokeMethod(
            targetName,
            sourceType,
            nullSubstituteAttribute,
            elementStrategy);
    }

    private bool ShouldSkipNullSubstituteWrap(
        MapStrategy strategy,
        string targetName,
        StringComparison stringComparison)
    {
        if (strategy is NoMapStrategy
            or MappaAssignFromConstantAttributeStrategy
            or MappaInvokeMethodAttributeStrategy
            or MappaAssignFromContextAttributeStrategy
            || this.HasMatchingAssignFromConstantAttribute(targetName, stringComparison))
        {
            return true;
        }

        var nullSubstituteAttributes = this.GetMatchingNullSubstituteAttributes(targetName, stringComparison);
        if (nullSubstituteAttributes.Length != 1)
        {
            return true;
        }

        var nullSubstituteAttribute = nullSubstituteAttributes[0];
        var hasInvokeMethod = !string.IsNullOrWhiteSpace(nullSubstituteAttribute.InvokeMethod);

        // Exactly one mode must be set; validation reports the diagnostic when neither or both are present.
        return nullSubstituteAttribute.HasAssignValue == hasInvokeMethod;
    }

    private MapStrategy TryWrapWithNullSubstituteInvokeMethod(
        string targetName,
        ITypeSymbol sourceType,
        MappaNullSubstituteAttribute nullSubstituteAttribute,
        MapStrategy strategy)
    {
        var mapMethod = this.GetAttributeMapMethod();
        var mapMethodMethodDeclarationSyntax = mapMethod.MethodDeclarationSyntax
            ?? throw new MappaGeneratorException("Method declaration syntax has not been defined.");
        var mapMethodClass = mapMethod.ContainingType;
        var rootMethod = this.context.GetRootMapMethod();
        var methodName = nullSubstituteAttribute.InvokeMethod!;

        if (!this.TryResolveNullSubstituteInvokeMethodSymbol(
                targetName,
                nullSubstituteAttribute,
                methodName,
                mapMethodClass,
                rootMethod,
                mapMethodMethodDeclarationSyntax,
                sourceType,
                out var fieldOrProperty,
                out var method)
            || method is null)
        {
            return strategy;
        }

        var contextParameterName = method.MethodHasMappaContextParameter(this.compilation)
            ? rootMethod.MaybeGetMappaContextParameterName()
            : null;

        return NullSubstituteMapStrategy.ForInvokeMethod(
            strategy.TargetType,
            sourceType,
            method,
            fieldOrProperty,
            nullSubstituteAttribute.InvokeMethodType,
            mapMethod.NullableEnabled,
            contextParameterName,
            strategy);
    }

    private bool TryResolveNullSubstituteInvokeMethodSymbol(
        string targetName,
        MappaNullSubstituteAttribute nullSubstituteAttribute,
        string methodName,
        INamedTypeSymbol mapMethodClass,
        MapMethod rootMethod,
        MethodDeclarationSyntax mapMethodMethodDeclarationSyntax,
        ITypeSymbol sourceMemberType,
        out ISymbol? fieldOrProperty,
        out IMethodSymbol? method)
    {
        fieldOrProperty = null;

        if (!string.IsNullOrWhiteSpace(nullSubstituteAttribute.InvokeMethodSource))
        {
            return this.TryResolveNullSubstituteInvokeMethodFromSource(
                targetName,
                nullSubstituteAttribute.InvokeMethodSource!,
                methodName,
                mapMethodClass,
                rootMethod,
                mapMethodMethodDeclarationSyntax,
                sourceMemberType,
                out fieldOrProperty,
                out method);
        }

        if (nullSubstituteAttribute.InvokeMethodType is not null)
        {
            return this.TryResolveNullSubstituteInvokeMethodFromType(
                targetName,
                nullSubstituteAttribute.InvokeMethodType,
                methodName,
                mapMethodClass,
                rootMethod,
                mapMethodMethodDeclarationSyntax,
                sourceMemberType,
                out method);
        }

        return this.TryResolveNullSubstituteInvokeMethodFromMapper(
            targetName,
            methodName,
            mapMethodClass,
            rootMethod,
            mapMethodMethodDeclarationSyntax,
            sourceMemberType,
            out method);
    }

    private bool TryResolveNullSubstituteInvokeMethodFromSource(
        string targetName,
        string invokeMethodSource,
        string methodName,
        INamedTypeSymbol mapMethodClass,
        MapMethod rootMethod,
        MethodDeclarationSyntax mapMethodMethodDeclarationSyntax,
        ITypeSymbol sourceMemberType,
        out ISymbol? fieldOrProperty,
        out IMethodSymbol? method)
    {
        fieldOrProperty = this.compilation.LocateAccessibleFieldOrPropertyInTypeHierarchy(
            mapMethodClass,
            invokeMethodSource,
            mapMethodClass);

        if (fieldOrProperty is null)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.CannotFindFieldOrProperty(
                mapMethodMethodDeclarationSyntax,
                invokeMethodSource));
            method = null;
            return false;
        }

        if (rootMethod.CanBeUsedByStaticMethod && !fieldOrProperty.IsStatic)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.FieldOrPropertyMustBeStatic(
                fieldOrProperty.Name,
                rootMethod.Location));
            method = null;
            return false;
        }

        var fieldOrPropertyType = GetFieldOrPropertyType(fieldOrProperty);
        var resolutionResult = this.TryResolveNullSubstituteInvokeMethodCandidates(
            mapMethodClass,
            fieldOrPropertyType.ToDisplayString(),
            fieldOrPropertyType.LocateMethods(methodName),
            methodName,
            sourceMemberType,
            InvokeMethodStaticRequirement.NotStatic,
            rootMethod,
            mapMethodMethodDeclarationSyntax,
            out method);

        return this.IsSuccessfulNullSubstituteInvokeMethodResolution(
            resolutionResult,
            method,
            targetName,
            fieldOrPropertyType.ToDisplayString(),
            methodName,
            mapMethodMethodDeclarationSyntax);
    }

    private bool TryResolveNullSubstituteInvokeMethodFromType(
        string targetName,
        Type invokeMethodType,
        string methodName,
        INamedTypeSymbol mapMethodClass,
        MapMethod rootMethod,
        MethodDeclarationSyntax mapMethodMethodDeclarationSyntax,
        ITypeSymbol sourceMemberType,
        out IMethodSymbol? method)
    {
        var classTypeFullName = invokeMethodType.FullName
            ?? throw new MappaGeneratorException("Cannot detect type full name");
        var className = this.compilation.GetTypeByMetadataName(classTypeFullName);
        if (className is null)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.CannotDetectType(
                mapMethodMethodDeclarationSyntax,
                classTypeFullName));
            method = null;
            return false;
        }

        var resolutionResult = this.TryResolveNullSubstituteInvokeMethodCandidates(
            mapMethodClass,
            className.ToDisplayString(),
            className.LocateMethods(methodName),
            methodName,
            sourceMemberType,
            InvokeMethodStaticRequirement.Static,
            rootMethod,
            mapMethodMethodDeclarationSyntax,
            out method);

        return this.IsSuccessfulNullSubstituteInvokeMethodResolution(
            resolutionResult,
            method,
            targetName,
            className.ToDisplayString(),
            methodName,
            mapMethodMethodDeclarationSyntax);
    }

    private bool TryResolveNullSubstituteInvokeMethodFromMapper(
        string targetName,
        string methodName,
        INamedTypeSymbol mapMethodClass,
        MapMethod rootMethod,
        MethodDeclarationSyntax mapMethodMethodDeclarationSyntax,
        ITypeSymbol sourceMemberType,
        out IMethodSymbol? method)
    {
        var staticRequirement = rootMethod.CanBeUsedByStaticMethod
            ? InvokeMethodStaticRequirement.Static
            : InvokeMethodStaticRequirement.StaticOrNotStatic;

        var resolutionResult = this.TryResolveNullSubstituteInvokeMethodCandidates(
            mapMethodClass,
            mapMethodClass.ToDisplayString(),
            mapMethodClass.LocateMethods(methodName),
            methodName,
            sourceMemberType,
            staticRequirement,
            rootMethod,
            mapMethodMethodDeclarationSyntax,
            out method);

        return this.IsSuccessfulNullSubstituteInvokeMethodResolution(
            resolutionResult,
            method,
            targetName,
            mapMethodClass.ToDisplayString(),
            methodName,
            mapMethodMethodDeclarationSyntax);
    }

    private InvokeMethodResolutionResult TryResolveNullSubstituteInvokeMethodCandidates(
        ITypeSymbol mapClass,
        string lookupTypeDisplayName,
        IMethodSymbol[] methods,
        string methodName,
        ITypeSymbol sourceMemberType,
        InvokeMethodStaticRequirement staticRequirement,
        MapMethod rootMapMethod,
        MethodDeclarationSyntax mapMethodMethodDeclarationSyntax,
        out IMethodSymbol? method)
    {
        var resolutionResult = InvokeMethodResolution.TryResolveNullSubstituteInvokeMethod(
            this.compilation,
            mapClass,
            lookupTypeDisplayName,
            methods,
            methodName,
            sourceMemberType,
            this.context.IsNullableEnabled(),
            staticRequirement,
            rootMapMethod,
            out method,
            out var ambiguityDetails);

        if (resolutionResult is InvokeMethodResolutionResult.Ambiguous)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.AmbiguousInvokeMethodResolution(
                mapMethodMethodDeclarationSyntax.GetLocation(),
                ambiguityDetails));
        }

        return resolutionResult;
    }

    private bool IsSuccessfulNullSubstituteInvokeMethodResolution(
        InvokeMethodResolutionResult resolutionResult,
        IMethodSymbol? method,
        string targetName,
        string typeDisplayName,
        string methodName,
        MethodDeclarationSyntax mapMethodMethodDeclarationSyntax)
    {
        if (resolutionResult is InvokeMethodResolutionResult.Ambiguous)
        {
            return false;
        }

        if (resolutionResult is InvokeMethodResolutionResult.Success && method is not null)
        {
            return true;
        }

        this.context.ReportDiagnostic(MappaDiagnostics.CannotDetectSuitableMethodToInvokeForParameter(
            mapMethodMethodDeclarationSyntax,
            targetName,
            methodName,
            typeDisplayName));
        return false;
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

        if (nullSubstituteAttribute.SourcePropertyName is not string sourcePropertyName
            || string.IsNullOrWhiteSpace(sourcePropertyName))
        {
            return false;
        }

        var targetPath = PropertyPath.Parse(nullSubstituteAttribute.TargetPropertyName);
        var sourcePath = PropertyPath.Parse(sourcePropertyName);
        var activePropertyPathContext = this.GetActivePropertyPathContextForSourcePath(
            nullSubstituteAttribute.TargetPropertyName,
            sourcePropertyName);

        if (this.TryCreateChainedSourceFromRootNestedSourcePath(
                sourcePropertyName,
                targetPath,
                sourcePath,
                out chainedSourcePropertyPath))
        {
            return false;
        }

        if (this.TryCreateChainedSourceForLeafTargetMappingPath(
                sourcePropertyName,
                sourcePath,
                activePropertyPathContext,
                isLeafTargetMapping,
                out chainedSourcePropertyPath))
        {
            return false;
        }

        var expectedSegment = PropertyPathAttributeMatching.GetExpectedSourcePropertyNameForCurrentLevel(
            sourcePropertyName,
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
                sourcePropertyName)
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
            this.context.GetRootMapMethod().SourceParameterName,
            this.ShouldIncludeFieldsInSourcePaths());
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
            string.Empty,
            this.ShouldIncludeFieldsInSourcePaths());
        return true;
    }
}