// <copyright file="ObjectFactoryDuplicateValidator.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Validates that class and method object factories do not declare duplicate target types.
/// </summary>
internal static class ObjectFactoryDuplicateValidator
{
    /// <summary>
    /// Validates that the union of class and method object factories has unique target types.
    /// </summary>
    /// <param name="mapMethod">The mapping method.</param>
    /// <param name="classAttributes">The class-level object factory attributes.</param>
    /// <param name="methodAttributes">The method-level object factory attributes.</param>
    /// <param name="classContext">The class generator context.</param>
    /// <returns><c>true</c> when no duplicates are found; otherwise <c>false</c>.</returns>
    internal static bool TryValidate(
        MapMethod mapMethod,
        MappaObjectFactoryAttributeData[] classAttributes,
        MappaObjectFactoryAttributeData[] methodAttributes,
        MappaClassGeneratorContext classContext)
    {
        var union = classAttributes.Concat(methodAttributes).ToArray();
        var duplicateGroups = union
            .GroupBy(attribute => attribute.TargetType, SymbolEqualityComparer.Default)
            .Where(group => group.Count() > 1)
            .ToArray();

        if (duplicateGroups.Length == 0)
        {
            return true;
        }

        var methodName = mapMethod.MethodName;
        foreach (var duplicateGroup in duplicateGroups)
        {
            var targetType = duplicateGroup.Key;
            var targetTypeName = targetType?.ToDisplayString() ?? string.Empty;
            var location = duplicateGroup
                .Select(attribute => attribute.Location)
                .FirstOrDefault(attributeLocation => attributeLocation is not null)
                ?? mapMethod.MethodDeclarationSyntax?.GetLocation();

            classContext.ReportDiagnostic(MappaDiagnostics.DuplicateObjectFactoryForTargetType(
                location,
                methodName,
                targetTypeName));
        }

        return false;
    }
}