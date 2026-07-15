// <copyright file="MapMethodMappingAttributesValidator.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        var sourceType = context.SourceType;
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
        var sourceTypeName = sourceType.ToDisplayString();

        foreach (var attribute in mapMethod.GetAttributes<MappaUsePropertyAttribute>())
        {
            ValidateTargetPropertyPath(
                context,
                methodDeclarationSyntax,
                methodName,
                targetTypeName,
                nameof(MappaUsePropertyAttribute),
                attribute.TargetPropertyName,
                propertyNames,
                constructorParameterNames,
                targetType);
            ValidateSourcePropertyPath(
                context,
                methodDeclarationSyntax,
                methodName,
                sourceTypeName,
                nameof(MappaUsePropertyAttribute),
                attribute.TargetPropertyName,
                attribute.SourcePropertyName,
                sourceType);
        }

        foreach (var attribute in mapMethod.GetAttributes<MappaInvokeMethodAttribute>())
        {
            ValidateTargetPropertyPath(
                context,
                methodDeclarationSyntax,
                methodName,
                targetTypeName,
                nameof(MappaInvokeMethodAttribute),
                attribute.TargetPropertyName,
                propertyNames,
                constructorParameterNames,
                targetType);

            if (!string.IsNullOrWhiteSpace(attribute.SourcePropertyName))
            {
                ValidateSourcePropertyPath(
                    context,
                    methodDeclarationSyntax,
                    methodName,
                    sourceTypeName,
                    nameof(MappaInvokeMethodAttribute),
                    attribute.TargetPropertyName,
                    attribute.SourcePropertyName!,
                    sourceType);
            }
        }

        foreach (var attribute in mapMethod.GetAttributes<MappaAssignFromContextAttribute>())
        {
            ValidateTargetPropertyPath(
                context,
                methodDeclarationSyntax,
                methodName,
                targetTypeName,
                nameof(MappaAssignFromContextAttribute),
                attribute.TargetPropertyName,
                propertyNames,
                constructorParameterNames,
                targetType);
        }

        foreach (var attribute in mapMethod.GetAttributes<MappaAssignFromConstantAttribute>())
        {
            ValidateTargetPropertyPath(
                context,
                methodDeclarationSyntax,
                methodName,
                targetTypeName,
                nameof(MappaAssignFromConstantAttribute),
                attribute.TargetPropertyName,
                propertyNames,
                constructorParameterNames,
                targetType);
        }

        foreach (var ignoreAttribute in mapMethod.GetAttributes<MappaIgnoreTargetPropertyAttribute>())
        {
            ValidateTargetPropertyPath(
                context,
                methodDeclarationSyntax,
                methodName,
                targetTypeName,
                nameof(MappaIgnoreTargetPropertyAttribute),
                ignoreAttribute.TargetPropertyName,
                propertyNames,
                constructorParameterNames,
                targetType);
        }
    }

    /// <summary>
    /// Reports errors when <see cref="MappaIgnoreTargetPropertyAttribute"/> declarations
    /// are duplicated or conflict with other target-name mapping attributes.
    /// </summary>
    /// <param name="context">The mapping algorithm context.</param>
    internal static void ValidateMappaIgnoreTargetPropertyAttributes(this MappaMapAlgorithmContext context)
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

        var ignoreAttributes = mapMethod.GetAttributes<MappaIgnoreTargetPropertyAttribute>();
        if (ignoreAttributes.Length == 0)
        {
            return;
        }

        var methodName = context.GetRootMapMethod().MethodName;

        foreach (var duplicateTarget in ignoreAttributes
                     .GroupBy(attribute => attribute.TargetPropertyName, StringComparer.Ordinal)
                     .Where(group => group.Count() > 1)
                     .Select(group => group.Key))
        {
            context.ReportDiagnostic(MappaDiagnostics.TooManyMappaIgnoreTargetPropertyAttributesForTheSameTargetProperty(
                methodDeclarationSyntax,
                methodName,
                duplicateTarget));
        }

        var otherTargetPropertyNames = mapMethod.GetAttributes<MappaUsePropertyAttribute>()
            .Select(attribute => attribute.TargetPropertyName)
            .Concat(mapMethod.GetAttributes<MappaInvokeMethodAttribute>()
                .Select(attribute => attribute.TargetPropertyName))
            .Concat(mapMethod.GetAttributes<MappaAssignFromContextAttribute>()
                .Select(attribute => attribute.TargetPropertyName))
            .Concat(mapMethod.GetAttributes<MappaAssignFromConstantAttribute>()
                .Select(attribute => attribute.TargetPropertyName))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        foreach (var ignoredPropertyName in ignoreAttributes
                     .Select(attribute => attribute.TargetPropertyName)
                     .Distinct(StringComparer.Ordinal)
                     .Where(ignoredPropertyName => otherTargetPropertyNames.Any(
                         targetName => targetName.Equals(ignoredPropertyName, StringComparison.Ordinal))))
        {
            context.ReportDiagnostic(MappaDiagnostics.MultipleAttributesTargetTheSamePropertyOrParameter(
                methodDeclarationSyntax,
                ignoredPropertyName));
        }
    }

    private static void ValidateTargetPropertyPath(
        MappaMapAlgorithmContext context,
        MethodDeclarationSyntax methodDeclarationSyntax,
        string methodName,
        string targetTypeName,
        string attributeName,
        string targetPropertyPath,
        HashSet<string> propertyNames,
        string[] constructorParameterNames,
        ITypeSymbol targetType)
    {
        var parsedTargetPath = PropertyPath.Parse(targetPropertyPath);
        if (parsedTargetPath.Segments.Length == 0)
        {
            return;
        }

        var firstSegment = parsedTargetPath.GetFirstSegment();
        if (firstSegment is null || !IsValidTargetName(firstSegment, propertyNames, constructorParameterNames))
        {
            context.ReportDiagnostic(MappaDiagnostics.MappingAttributeTargetPropertyOrParameterDoesNotExist(
                methodDeclarationSyntax,
                methodName,
                attributeName,
                targetPropertyPath,
                targetTypeName));
            return;
        }

        if (parsedTargetPath.IsNested
            && !PropertyPathSymbolResolver.TryResolvePropertyPath(
                targetType,
                parsedTargetPath,
                out _,
                out _))
        {
            context.ReportDiagnostic(MappaDiagnostics.MappingAttributeTargetPropertyOrParameterDoesNotExist(
                methodDeclarationSyntax,
                methodName,
                attributeName,
                targetPropertyPath,
                targetTypeName));
        }
    }

    private static void ValidateSourcePropertyPath(
        MappaMapAlgorithmContext context,
        MethodDeclarationSyntax methodDeclarationSyntax,
        string methodName,
        string sourceTypeName,
        string attributeName,
        string targetPropertyPath,
        string sourcePropertyPath,
        ITypeSymbol sourceType)
    {
        var parsedTargetPath = PropertyPath.Parse(targetPropertyPath);
        var parsedSourcePath = PropertyPath.Parse(sourcePropertyPath);
        if (parsedSourcePath.Segments.Length == 0)
        {
            return;
        }

        if (parsedSourcePath.Segments.Length == 0 || !parsedSourcePath.IsNested)
        {
            return;
        }

        if (parsedSourcePath.Segments.Length < parsedTargetPath.Segments.Length)
        {
            context.ReportDiagnostic(MappaDiagnostics.MappingAttributeSourcePropertyPathIsShorterThanTargetPropertyPath(
                methodDeclarationSyntax,
                methodName,
                attributeName,
                sourcePropertyPath,
                targetPropertyPath));
            return;
        }

        if (!PropertyPathSymbolResolver.TryResolvePropertyPath(
                sourceType,
                parsedSourcePath,
                out _,
                out var missingSegment))
        {
            context.ReportDiagnostic(MappaDiagnostics.MappingAttributeSourcePropertyPathSegmentDoesNotExist(
                methodDeclarationSyntax,
                methodName,
                attributeName,
                sourcePropertyPath,
                missingSegment ?? string.Empty,
                sourceTypeName));
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