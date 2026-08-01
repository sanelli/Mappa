// <copyright file="ProjectionMapMethodEligibilityValidator.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

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
    /// <returns><c>true</c> when the method is eligible or not a projection method; otherwise <c>false</c>.</returns>
    internal static bool TryValidate(
        MapMethod mapMethod,
        Compilation compilation,
        MappaClassGeneratorContext classContext,
        MapHookAttributeData[] classBeforeMapAttributes,
        MapHookAttributeData[] classAfterMapAttributes,
        MappaObjectFactoryAttributeData[] classObjectFactoryAttributes,
        MappaObjectFactoryAttributeData[] methodObjectFactoryAttributes)
    {
        if (!mapMethod.MethodSymbol.IsQueryableProjectionMapMethod(compilation))
        {
            return true;
        }

        var methodDeclarationSyntax = mapMethod.MethodDeclarationSyntax;
        var methodName = mapMethod.MethodSymbol.Name;

        if (mapMethod.MethodSymbol.Parameters.Length > 1
            && mapMethod.MethodSymbol.SecondParameterIsMappaContext(compilation))
        {
            classContext.ReportDiagnostic(
                MappaDiagnostics.ProjectionMethodHasMappaContextParameter(methodDeclarationSyntax, methodName));
            return false;
        }

        var methodAttributes = mapMethod.MethodSymbol.GetAttributes();
        var hasMethodBeforeMapHooks = methodAttributes.GetMappaBeforeMapAttributes(compilation).Length > 0;
        var hasMethodAfterMapHooks = methodAttributes.GetMappaAfterMapAttributes(compilation).Length > 0;
        if (classBeforeMapAttributes.Length > 0
            || classAfterMapAttributes.Length > 0
            || hasMethodBeforeMapHooks
            || hasMethodAfterMapHooks)
        {
            classContext.ReportDiagnostic(
                MappaDiagnostics.ProjectionMethodHasBeforeOrAfterMapHooks(methodDeclarationSyntax, methodName));
            return false;
        }

        if (classObjectFactoryAttributes.Length > 0
            || methodObjectFactoryAttributes.Length > 0)
        {
            classContext.ReportDiagnostic(
                MappaDiagnostics.ProjectionMethodHasObjectFactory(methodDeclarationSyntax, methodName));
            return false;
        }

        if (mapMethod.GetAttribute<MappaAllowInaccessibleSourceMembersAttribute>() is not null
            || mapMethod.GetAttribute<MappaAllowInaccessibleTargetMembersAttribute>() is not null)
        {
            classContext.ReportDiagnostic(
                MappaDiagnostics.ProjectionMethodHasAllowInaccessibleMembers(methodDeclarationSyntax, methodName));
            return false;
        }

        return true;
    }
}