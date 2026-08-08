// <copyright file="ProjectionMapMethodEligibilityValidator.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Validates root map methods that project <see cref="System.Linq.IQueryable{T}"/>.
/// </summary>
internal static class ProjectionMapMethodEligibilityValidator
{
    /// <summary>
    /// Validates that a queryable projection map method does not use unsupported features.
    /// </summary>
    /// <param name="mapMethod">The map method.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="classContext">The class generator context.</param>
    /// <param name="classBeforeMapAttributes">The class-level before-map hook attributes.</param>
    /// <param name="classAfterMapAttributes">The class-level after-map hook attributes.</param>
    /// <param name="classObjectFactoryAttributes">The class-level object factory attributes.</param>
    /// <param name="methodObjectFactoryAttributes">The method-level object factory attributes.</param>
    /// <param name="mappaUserSettings">The effective user settings for the method.</param>
    /// <returns><c>true</c> when the method is eligible or not a projection method; otherwise <c>false</c>.</returns>
    internal static bool TryValidate(
        MapMethod mapMethod,
        Compilation compilation,
        MappaClassGeneratorContext classContext,
        MapHookAttributeData[] classBeforeMapAttributes,
        MapHookAttributeData[] classAfterMapAttributes,
        MappaObjectFactoryAttributeData[] classObjectFactoryAttributes,
        MappaObjectFactoryAttributeData[] methodObjectFactoryAttributes,
        IMappaUserSettings mappaUserSettings)
    {
        if (mapMethod.IsSynthetic || mapMethod.MethodSymbol is null)
        {
            return true;
        }

        if (!mapMethod.MethodSymbol.IsQueryableProjectionMapMethod(compilation))
        {
            return true;
        }

        return ValidateProjectionQueryableMethod(
            mapMethod,
            compilation,
            classContext,
            classBeforeMapAttributes,
            classAfterMapAttributes,
            classObjectFactoryAttributes,
            methodObjectFactoryAttributes,
            mappaUserSettings);
    }

    private static bool ValidateProjectionQueryableMethod(
        MapMethod mapMethod,
        Compilation compilation,
        MappaClassGeneratorContext classContext,
        MapHookAttributeData[] classBeforeMapAttributes,
        MapHookAttributeData[] classAfterMapAttributes,
        MappaObjectFactoryAttributeData[] classObjectFactoryAttributes,
        MappaObjectFactoryAttributeData[] methodObjectFactoryAttributes,
        IMappaUserSettings mappaUserSettings)
    {
        var methodDeclarationSyntax = mapMethod.MethodDeclarationSyntax;
        var methodName = mapMethod.MethodName;

        if (mapMethod.MethodSymbol is not IMethodSymbol methodSymbol)
        {
            return true;
        }

        if (!ValidateProjectionQueryableHasNoMappaContextParameter(
                methodSymbol,
                compilation,
                classContext,
                methodDeclarationSyntax,
                methodName))
        {
            return false;
        }

        if (!ValidateProjectionQueryableHasNoReferenceHandling(
                mappaUserSettings,
                classContext,
                methodDeclarationSyntax,
                methodName))
        {
            return false;
        }

        if (!ValidateProjectionQueryableHasNoBeforeOrAfterMapHooks(
                mapMethod,
                compilation,
                classContext,
                classBeforeMapAttributes,
                classAfterMapAttributes,
                methodDeclarationSyntax,
                methodName))
        {
            return false;
        }

        if (!ValidateProjectionQueryableHasNoObjectFactory(
                classObjectFactoryAttributes,
                methodObjectFactoryAttributes,
                classContext,
                methodDeclarationSyntax,
                methodName))
        {
            return false;
        }

        return ValidateProjectionQueryableHasNoAllowInaccessibleMembers(mapMethod, classContext, methodDeclarationSyntax, methodName);
    }

    private static bool ValidateProjectionQueryableHasNoMappaContextParameter(
        IMethodSymbol methodSymbol,
        Compilation compilation,
        MappaClassGeneratorContext classContext,
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName)
    {
        if (methodSymbol.Parameters.Length <= 1
            || !methodSymbol.SecondParameterIsMappaContext(compilation))
        {
            return true;
        }

        classContext.ReportDiagnostic(
            MappaDiagnostics.ProjectionMethodHasMappaContextParameter(methodDeclarationSyntax, methodName));
        return false;
    }

    private static bool ValidateProjectionQueryableHasNoReferenceHandling(
        IMappaUserSettings mappaUserSettings,
        MappaClassGeneratorContext classContext,
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName)
    {
        if (!ReferenceHandlingCodeGenerator.IsReferenceHandlingRequested(mappaUserSettings))
        {
            return true;
        }

        classContext.ReportDiagnostic(
            MappaDiagnostics.ProjectionMappingNotSupported(
                methodDeclarationSyntax?.GetLocation(),
                methodName,
                "reference handling"));
        return false;
    }

    private static bool ValidateProjectionQueryableHasNoBeforeOrAfterMapHooks(
        MapMethod mapMethod,
        Compilation compilation,
        MappaClassGeneratorContext classContext,
        MapHookAttributeData[] classBeforeMapAttributes,
        MapHookAttributeData[] classAfterMapAttributes,
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName)
    {
        if (!HasBeforeOrAfterMapHooks(
                mapMethod,
                compilation,
                classBeforeMapAttributes,
                classAfterMapAttributes))
        {
            return true;
        }

        classContext.ReportDiagnostic(
            MappaDiagnostics.ProjectionMethodHasBeforeOrAfterMapHooks(methodDeclarationSyntax, methodName));
        return false;
    }

    private static bool ValidateProjectionQueryableHasNoObjectFactory(
        MappaObjectFactoryAttributeData[] classObjectFactoryAttributes,
        MappaObjectFactoryAttributeData[] methodObjectFactoryAttributes,
        MappaClassGeneratorContext classContext,
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName)
    {
        if (classObjectFactoryAttributes.Length == 0 && methodObjectFactoryAttributes.Length == 0)
        {
            return true;
        }

        classContext.ReportDiagnostic(
            MappaDiagnostics.ProjectionMethodHasObjectFactory(methodDeclarationSyntax, methodName));
        return false;
    }

    private static bool ValidateProjectionQueryableHasNoAllowInaccessibleMembers(
        MapMethod mapMethod,
        MappaClassGeneratorContext classContext,
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName)
    {
        if (mapMethod.GetAttribute<MappaAllowInaccessibleSourceMembersAttribute>() is null
            && mapMethod.GetAttribute<MappaAllowInaccessibleTargetMembersAttribute>() is null)
        {
            return true;
        }

        classContext.ReportDiagnostic(
            MappaDiagnostics.ProjectionMethodHasAllowInaccessibleMembers(methodDeclarationSyntax, methodName));
        return false;
    }

    private static bool HasBeforeOrAfterMapHooks(
        MapMethod mapMethod,
        Compilation compilation,
        MapHookAttributeData[] classBeforeMapAttributes,
        MapHookAttributeData[] classAfterMapAttributes)
    {
        if (classBeforeMapAttributes.Length > 0 || classAfterMapAttributes.Length > 0)
        {
            return true;
        }

        var methodAttributes = mapMethod.MethodSymbol?.GetAttributes();
        if (methodAttributes is null)
        {
            return false;
        }

        var attributes = methodAttributes.GetValueOrDefault();
        return attributes.GetMappaBeforeMapAttributes(compilation).Length > 0
               || attributes.GetMappaAfterMapAttributes(compilation).Length > 0;
    }
}