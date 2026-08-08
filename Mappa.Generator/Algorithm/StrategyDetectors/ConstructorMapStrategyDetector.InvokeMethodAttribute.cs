// <copyright file="ConstructorMapStrategyDetector.InvokeMethodAttribute.cs" company="Stefano Anelli">
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
/// MappaInvokeMethod attribute handling for <see cref="ConstructorMapStrategyDetector"/>.
/// </summary>
internal sealed partial class ConstructorMapStrategyDetector
{
    private static ITypeSymbol GetFieldOrPropertyType(ISymbol fieldOrProperty)
        => fieldOrProperty switch
        {
            IFieldSymbol field => field.Type,
            IPropertySymbol property => property.Type,
            _ => throw new MappaGeneratorException($"Unexpected symbol kind '{fieldOrProperty.Kind}' for field or property '{fieldOrProperty.Name}'."),
        };

    private MapStrategy TryGetStrategyFromSingleTargetPropertyAttribute(
        string targetName,
        ITypeSymbol targetType,
        ITypeSymbol sourceClassType,
        ref IPropertySymbol? sourceProperty,
        StringComparison stringComparison,
        bool isConstructorParameterPath,
        IMappaTargetPropertyNameAttribute attribute)
    {
        switch (attribute)
        {
            case MappaInvokeMethodAttribute mappaInvokeMethodAttribute:
                return this.TryGetStrategyFromMappaInvokeMethodAttribute(
                    targetName,
                    targetType,
                    sourceClassType,
                    ref sourceProperty,
                    mappaInvokeMethodAttribute,
                    stringComparison,
                    isConstructorParameterPath);
            case MappaAssignFromContextAttribute mappaAssignFromContextAttribute:
                return this.TryGetStrategyFromMappaAssignFromContextAttribute(
                    targetName,
                    targetType,
                    mappaAssignFromContextAttribute,
                    ref sourceProperty,
                    stringComparison);
            case MappaAssignFromConstantAttribute mappaAssignFromConstantAttribute:
                return this.TryGetStrategyFromMappaAssignFromConstantAttribute(
                    targetType,
                    mappaAssignFromConstantAttribute,
                    ref sourceProperty,
                    targetName,
                    stringComparison);
            default:
                return new NoMapStrategy(targetType, sourceClassType);
        }
    }

    private MapStrategy TryGetStrategyFromMappaInvokeMethodAttribute(
        string targetName,
        ITypeSymbol targetType,
        ITypeSymbol sourceClassType,
        ref IPropertySymbol? sourceProperty,
        MappaInvokeMethodAttribute mappaInvokeMethodAttribute,
        StringComparison stringComparison,
        bool isConstructorParameterPath)
    {
        if (!string.IsNullOrWhiteSpace(mappaInvokeMethodAttribute.SourcePropertyName))
        {
            this.ReportMappaUsePropertySourcePropertyWillNotBeUsedIfPresent(
                targetName,
                stringComparison,
                nameof(MappaInvokeMethodAttribute));
            this.TryResolveSourcePropertyForMappaInvokeMethodAttribute(
                mappaInvokeMethodAttribute,
                sourceClassType,
                isConstructorParameterPath,
                ref sourceProperty);
        }

        this.TryGetStrategyUsingMappaInvokeMethodAttribute(
            targetName,
            targetType,
            sourceClassType,
            sourceProperty,
            mappaInvokeMethodAttribute,
            stringComparison,
            out var strategy);
        return strategy;
    }

    private MapStrategy TryGetStrategyFromMappaAssignFromContextAttribute(
        string targetName,
        ITypeSymbol targetType,
        MappaAssignFromContextAttribute mappaAssignFromContextAttribute,
        ref IPropertySymbol? sourceProperty,
        StringComparison stringComparison)
    {
        this.TryGetStrategyUsingMappaAssignFromContextAttribute(
            targetName,
            targetType,
            mappaAssignFromContextAttribute,
            ref sourceProperty,
            out var strategy);
        if (strategy is not NoMapStrategy)
        {
            this.ReportMappaUsePropertySourcePropertyWillNotBeUsedIfPresent(
                targetName,
                stringComparison,
                nameof(MappaAssignFromContextAttribute));
        }

        return strategy;
    }

    private MapStrategy TryGetStrategyFromMappaAssignFromConstantAttribute(
        ITypeSymbol targetType,
        MappaAssignFromConstantAttribute mappaAssignFromConstantAttribute,
        ref IPropertySymbol? sourceProperty,
        string targetName,
        StringComparison stringComparison)
    {
        TryGetStrategyUsingMappaAssignFromConstantAttribute(
            targetType,
            mappaAssignFromConstantAttribute,
            out var strategy);
        if (strategy is not NoMapStrategy)
        {
            sourceProperty = null;
            this.ReportMappaUsePropertySourcePropertyWillNotBeUsedIfPresent(
                targetName,
                stringComparison,
                nameof(MappaAssignFromConstantAttribute));
        }

        return strategy;
    }

    private void TryGetStrategyUsingMappaInvokeMethodAttribute(
        string targetName,
        ITypeSymbol targetType,
        ITypeSymbol sourceClassType,
        IPropertySymbol? sourceProperty,
        MappaInvokeMethodAttribute mappaInvokeMethodAttribute,
        StringComparison stringComparison,
        out MapStrategy strategy)
    {
        strategy = new NoMapStrategy(targetType, sourceClassType);

        var mapMethod = this.GetAttributeMapMethod();
        var mapMethodMethodDeclarationSyntax = mapMethod.MethodDeclarationSyntax ?? throw new MappaGeneratorException("Method declaration syntax has not been defined.");
        var mapMethodClass = mapMethod.ContainingType;
        var rootMethod = this.context.GetRootMapMethod();

        if (!this.TryResolveMappaInvokeMethodSymbol(
                targetName,
                mappaInvokeMethodAttribute,
                mapMethodClass,
                rootMethod,
                mapMethodMethodDeclarationSyntax,
                targetType,
                sourceClassType,
                sourceProperty,
                out var fieldOrProperty,
                out var method)
            || method is null)
        {
            return;
        }

        var contextParameterName = method.MethodHasMappaContextParameter(this.compilation)
            ? rootMethod.MaybeGetMappaContextParameterName()
            : null;

        strategy = new MappaInvokeMethodAttributeStrategy(
            targetType,
            sourceClassType,
            mappaInvokeMethodAttribute,
            fieldOrProperty,
            method,
            sourceProperty,
            this.GetAttributeMapMethod().NullableEnabled,
            contextParameterName);

        this.ReportMappaUsePropertyNotUsedByInvokeMethodIfNeeded(
            targetName,
            sourceProperty,
            sourceClassType,
            mappaInvokeMethodAttribute,
            method,
            stringComparison,
            mapMethodMethodDeclarationSyntax);
    }

    private bool TryResolveMappaInvokeMethodSymbol(
        string targetName,
        MappaInvokeMethodAttribute mappaInvokeMethodAttribute,
        INamedTypeSymbol mapMethodClass,
        MapMethod rootMethod,
        MethodDeclarationSyntax mapMethodMethodDeclarationSyntax,
        ITypeSymbol targetType,
        ITypeSymbol sourceClassType,
        IPropertySymbol? sourceProperty,
        out ISymbol? fieldOrProperty,
        out IMethodSymbol? method)
    {
        fieldOrProperty = null;

        if (mappaInvokeMethodAttribute.FieldName is not null)
        {
            return this.TryResolveMappaInvokeMethodFromField(
                targetName,
                mappaInvokeMethodAttribute,
                mapMethodClass,
                rootMethod,
                mapMethodMethodDeclarationSyntax,
                targetType,
                sourceClassType,
                sourceProperty,
                out fieldOrProperty,
                out method);
        }

        if (mappaInvokeMethodAttribute.ClassType is not null)
        {
            return this.TryResolveMappaInvokeMethodFromClassType(
                targetName,
                mappaInvokeMethodAttribute,
                mapMethodClass,
                rootMethod,
                mapMethodMethodDeclarationSyntax,
                targetType,
                sourceClassType,
                sourceProperty,
                out method);
        }

        return this.TryResolveMappaInvokeMethodFromMapMethodClass(
            targetName,
            mappaInvokeMethodAttribute,
            mapMethodClass,
            rootMethod,
            mapMethodMethodDeclarationSyntax,
            targetType,
            sourceClassType,
            sourceProperty,
            out method);
    }

    private bool TryResolveMappaInvokeMethodFromField(
        string targetName,
        MappaInvokeMethodAttribute mappaInvokeMethodAttribute,
        INamedTypeSymbol mapMethodClass,
        MapMethod rootMethod,
        MethodDeclarationSyntax mapMethodMethodDeclarationSyntax,
        ITypeSymbol targetType,
        ITypeSymbol sourceClassType,
        IPropertySymbol? sourceProperty,
        out ISymbol? fieldOrProperty,
        out IMethodSymbol? method)
    {
        if (mappaInvokeMethodAttribute.FieldName is not { Length: > 0 } fieldName || string.IsNullOrWhiteSpace(fieldName))
        {
            fieldOrProperty = null;
            method = null;
            return false;
        }

        fieldOrProperty = this.compilation.LocateAccessibleFieldOrPropertyInTypeHierarchy(
            mapMethodClass,
            fieldName,
            mapMethodClass);

        if (fieldOrProperty is null)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.CannotFindFieldOrProperty(
                mapMethodMethodDeclarationSyntax,
                fieldName));
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
        var resolutionResult = this.TryResolveInvokeMethodForAttribute(
            mapMethodClass,
            fieldOrPropertyType.LocateMethods(mappaInvokeMethodAttribute.MethodName),
            mappaInvokeMethodAttribute.MethodName,
            targetType,
            sourceClassType,
            sourceProperty,
            InvokeMethodStaticRequirement.NotStatic,
            rootMethod,
            mapMethodMethodDeclarationSyntax,
            out var resolvedMethod);
        method = resolvedMethod;

        return this.IsSuccessfulMappaInvokeMethodResolution(
            resolutionResult,
            method,
            mappaInvokeMethodAttribute,
            mapMethodClass,
            mapMethodMethodDeclarationSyntax,
            targetName);
    }

    private bool TryResolveMappaInvokeMethodFromClassType(
        string targetName,
        MappaInvokeMethodAttribute mappaInvokeMethodAttribute,
        INamedTypeSymbol mapMethodClass,
        MapMethod rootMethod,
        MethodDeclarationSyntax mapMethodMethodDeclarationSyntax,
        ITypeSymbol targetType,
        ITypeSymbol sourceClassType,
        IPropertySymbol? sourceProperty,
        out IMethodSymbol? method)
    {
        var classTypeFullName = mappaInvokeMethodAttribute.ClassType?.FullName
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

        var resolutionResult = this.TryResolveInvokeMethodForAttribute(
            mapMethodClass,
            className.LocateMethods(mappaInvokeMethodAttribute.MethodName),
            mappaInvokeMethodAttribute.MethodName,
            targetType,
            sourceClassType,
            sourceProperty,
            InvokeMethodStaticRequirement.Static,
            rootMethod,
            mapMethodMethodDeclarationSyntax,
            out method);

        return this.IsSuccessfulMappaInvokeMethodResolution(
            resolutionResult,
            method,
            mappaInvokeMethodAttribute,
            mapMethodClass,
            mapMethodMethodDeclarationSyntax,
            targetName);
    }

    private bool TryResolveMappaInvokeMethodFromMapMethodClass(
        string targetName,
        MappaInvokeMethodAttribute mappaInvokeMethodAttribute,
        INamedTypeSymbol mapMethodClass,
        MapMethod rootMethod,
        MethodDeclarationSyntax mapMethodMethodDeclarationSyntax,
        ITypeSymbol targetType,
        ITypeSymbol sourceClassType,
        IPropertySymbol? sourceProperty,
        out IMethodSymbol? method)
    {
        var staticRequirement = rootMethod.CanBeUsedByStaticMethod
            ? InvokeMethodStaticRequirement.Static
            : InvokeMethodStaticRequirement.StaticOrNotStatic;

        var resolutionResult = this.TryResolveInvokeMethodForAttribute(
            mapMethodClass,
            mapMethodClass.LocateMethods(mappaInvokeMethodAttribute.MethodName),
            mappaInvokeMethodAttribute.MethodName,
            targetType,
            sourceClassType,
            sourceProperty,
            staticRequirement,
            rootMethod,
            mapMethodMethodDeclarationSyntax,
            out method);

        return this.IsSuccessfulMappaInvokeMethodResolution(
            resolutionResult,
            method,
            mappaInvokeMethodAttribute,
            mapMethodClass,
            mapMethodMethodDeclarationSyntax,
            targetName);
    }

    private bool IsSuccessfulMappaInvokeMethodResolution(
        InvokeMethodResolutionResult resolutionResult,
        IMethodSymbol? method,
        MappaInvokeMethodAttribute mappaInvokeMethodAttribute,
        INamedTypeSymbol mapMethodClass,
        MethodDeclarationSyntax mapMethodMethodDeclarationSyntax,
        string targetName)
    {
        if (resolutionResult is InvokeMethodResolutionResult.Ambiguous)
        {
            return false;
        }

        if (resolutionResult is InvokeMethodResolutionResult.Success && method is not null)
        {
            return true;
        }

        var displayClassName = mappaInvokeMethodAttribute.ClassType is not null
            ? mappaInvokeMethodAttribute.ClassType.FullName ?? "unknown"
            : mapMethodClass.ToDisplayString();
        this.context.ReportDiagnostic(MappaDiagnostics.CannotDetectSuitableMethodToInvokeForParameter(
            mapMethodMethodDeclarationSyntax,
            targetName,
            mappaInvokeMethodAttribute.MethodName,
            displayClassName));
        return false;
    }

    private void ReportMappaUsePropertyNotUsedByInvokeMethodIfNeeded(
        string targetName,
        IPropertySymbol? sourceProperty,
        ITypeSymbol sourceClassType,
        MappaInvokeMethodAttribute mappaInvokeMethodAttribute,
        IMethodSymbol method,
        StringComparison stringComparison,
        MethodDeclarationSyntax mapMethodMethodDeclarationSyntax)
    {
        var explicitSourcePropertyName = this.GetExplicitSourcePropertyNameForInvokeMethod(
            targetName,
            mappaInvokeMethodAttribute,
            stringComparison);
        if (explicitSourcePropertyName is null)
        {
            return;
        }

        if (method.UsesSourceProperty(
                this.compilation,
                sourceProperty,
                sourceClassType,
                this.context.IsNullableEnabled()))
        {
            return;
        }

        this.context.ReportDiagnostic(MappaDiagnostics.MappaUsePropertyNotUsedByInvokeMethod(
            mapMethodMethodDeclarationSyntax,
            this.context.GetRootMapMethod().MethodName,
            targetName,
            explicitSourcePropertyName,
            mappaInvokeMethodAttribute.MethodName));
    }

    private string? GetExplicitSourcePropertyNameForInvokeMethod(
        string targetName,
        MappaInvokeMethodAttribute mappaInvokeMethodAttribute,
        StringComparison stringComparison)
    {
        if (!string.IsNullOrWhiteSpace(mappaInvokeMethodAttribute.SourcePropertyName))
        {
            return mappaInvokeMethodAttribute.SourcePropertyName;
        }

        var usePropertyAttributes = this.GetAttributeMapMethod()
            .GetAttributes<MappaUsePropertyAttribute>()
            .Where(attribute => this.AttributeTargetPathMatches(attribute.TargetPropertyName, targetName, stringComparison))
            .ToArray();

        return usePropertyAttributes.Length == 1
            ? usePropertyAttributes[0].SourcePropertyName
            : null;
    }
}