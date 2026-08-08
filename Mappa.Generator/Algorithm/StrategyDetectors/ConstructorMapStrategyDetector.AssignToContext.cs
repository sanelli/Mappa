// <copyright file="ConstructorMapStrategyDetector.AssignToContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// MappaAssignToContext enrichment for <see cref="ConstructorMapStrategyDetector"/>.
/// </summary>
internal sealed partial class ConstructorMapStrategyDetector
{
    private static HashSet<string> GetDuplicateAssignToContextKeys(MappaAssignToContextAttribute[] attributes)
        => new(
            attributes
                .GroupBy(attribute => attribute.ContextKey, StringComparer.Ordinal)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key),
            StringComparer.Ordinal);

    private bool TryBuildAssignToContextEnrichment(
        bool attributesEnabled,
        out MappaAssignToContextEntry[] entries,
        out string? contextParameterName)
    {
        entries = [];
        contextParameterName = null;

        if (!attributesEnabled || this.context.MapMethod is null)
        {
            return false;
        }

        var attributes = this.context.MapMethod.GetAttributes<MappaAssignToContextAttribute>();
        if (attributes.Length == 0)
        {
            return false;
        }

        var methodDeclarationSyntax = this.context.MapMethod.MethodDeclarationSyntax
            ?? throw new MappaGeneratorException("Method declaration syntax has not been defined.");
        var rootMapMethod = this.context.GetRootMapMethod();
        var methodName = rootMapMethod.MethodName;
        var targetTypeName = this.context.TargetType.ToDisplayString();
        var providesContext = rootMapMethod.ProvideMappaContextWhenInvoked();

        var duplicateContextKeys = GetDuplicateAssignToContextKeys(attributes);
        this.ReportDuplicateAssignToContextKeys(methodDeclarationSyntax, methodName, duplicateContextKeys);

        List<MappaAssignToContextEntry> assignToContextEntries = new();

        foreach (var attribute in attributes)
        {
            if (duplicateContextKeys.Contains(attribute.ContextKey))
            {
                continue;
            }

            if (!this.TryAppendValidAssignToContextEntry(
                    attribute,
                    providesContext,
                    methodDeclarationSyntax,
                    methodName,
                    targetTypeName,
                    assignToContextEntries,
                    ref contextParameterName))
            {
                continue;
            }
        }

        if (assignToContextEntries.Count == 0)
        {
            return false;
        }

        entries = [.. assignToContextEntries];
        return true;
    }

    private void ReportDuplicateAssignToContextKeys(
        MethodDeclarationSyntax methodDeclarationSyntax,
        string methodName,
        HashSet<string> duplicateContextKeys)
    {
        foreach (var duplicateContextKey in duplicateContextKeys)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.MultipleMappaAssignToContextAttributesUseTheSameContextKey(
                methodDeclarationSyntax,
                methodName,
                duplicateContextKey));
        }
    }

    private bool TryAppendValidAssignToContextEntry(
        MappaAssignToContextAttribute attribute,
        bool providesContext,
        MethodDeclarationSyntax methodDeclarationSyntax,
        string methodName,
        string targetTypeName,
        List<MappaAssignToContextEntry> assignToContextEntries,
        ref string? contextParameterName)
    {
        if (!providesContext)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.CannotUseMappaAssignToContextAttributeWithoutContextParameter(
                methodDeclarationSyntax,
                methodName,
                attribute.ContextKey));
            return false;
        }

        if (!this.TryResolveAssignToContextTargetMember(attribute.TargetPropertyName))
        {
            this.context.ReportDiagnostic(MappaDiagnostics.MappaAssignToContextTargetMemberDoesNotExistOrIsNotAccessible(
                methodDeclarationSyntax,
                methodName,
                attribute.ContextKey,
                attribute.TargetPropertyName,
                targetTypeName));
            return false;
        }

        assignToContextEntries.Add(new MappaAssignToContextEntry(attribute.ContextKey, attribute.TargetPropertyName));
        contextParameterName ??= this.context.GetRootMapMethod().GetMappaContextParameterName();
        return true;
    }
}