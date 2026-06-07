// <copyright file="MapMethodMappingAttributesValidator.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods that validate mapping attributes declared on a map method.
/// </summary>
internal static class MapMethodMappingAttributesValidator
{
    /// <summary>
    /// Reports a warning for each mapping attribute whose <c>TargetPropertyName</c>
    /// does not match a property or constructor parameter on the target type.
    /// </summary>
    /// <param name="context">The mapping algorithm context.</param>
    /// <param name="compilation">The compilation.</param>
    internal static void ValidateTargetNamesExist(
        this MappaMapAlgorithmContext context,
        Compilation compilation)
    {
        if (context.MapMethod is null)
        {
            return;
        }

        if (context.AlgorithmSettings.UseAttributesForConstructorDetectorSettings
            .Equals(MappaMapAlgorithmContextSettings.MappaAttributesForConstructorDetectorSettings.Disable))
        {
            return;
        }

        var mapMethod = context.MapMethod;
        var methodDeclarationSyntax = mapMethod.MethodDeclarationSyntax;
        if (methodDeclarationSyntax is null)
        {
            return;
        }

        var targetType = context.TargetType;
        var propertyNames = new HashSet<string>(
            targetType.GetTypeProperties().Select(property => property.Name),
            StringComparer.Ordinal);
        var constructorParameterNames = targetType
            .GetAccessibleConstructors(compilation, context.ParentSymbol)
            .SelectMany(constructor => constructor.Parameters)
            .Select(parameter => parameter.Name)
            .ToArray();

        var methodName = context.GetRootMapMethod().MethodName;
        var targetTypeName = targetType.ToDisplayString();

        var attributeTargets = mapMethod.GetAttributes<MappaUsePropertyAttribute>()
            .Select(attribute => (AttributeName: nameof(MappaUsePropertyAttribute), TargetName: attribute.TargetPropertyName))
            .Concat(mapMethod.GetAttributes<MappaInvokeMethodAttribute>()
                .Select(attribute => (AttributeName: nameof(MappaInvokeMethodAttribute), TargetName: attribute.TargetPropertyName)))
            .Concat(mapMethod.GetAttributes<MappaAssignFromContextAttribute>()
                .Select(attribute => (AttributeName: nameof(MappaAssignFromContextAttribute), TargetName: attribute.TargetPropertyName)))
            .Concat(mapMethod.GetAttributes<MappaAssignFromConstantAttribute>()
                .Select(attribute => (AttributeName: nameof(MappaAssignFromConstantAttribute), TargetName: attribute.TargetPropertyName)));

        foreach (var (attributeName, targetName) in attributeTargets.Where(
                     attributeTarget => !IsValidTargetName(
                         attributeTarget.TargetName,
                         propertyNames,
                         constructorParameterNames)))
        {
            context.ReportDiagnostic(MappaDiagnostics.MappingAttributeTargetPropertyOrParameterDoesNotExist(
                methodDeclarationSyntax,
                methodName,
                attributeName,
                targetName,
                targetTypeName));
        }
    }

    private static bool IsValidTargetName(
        string targetName,
        HashSet<string> propertyNames,
        string[] constructorParameterNames)
    {
        if (propertyNames.Contains(targetName))
        {
            return true;
        }

        return constructorParameterNames.Any(
            parameterName => parameterName.Equals(targetName, StringComparison.OrdinalIgnoreCase));
    }
}