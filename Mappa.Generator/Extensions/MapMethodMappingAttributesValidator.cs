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
        var mapMethod = context.MapMethod;
        if (mapMethod is null)
        {
            // Derived nested mappings omit MapMethod; use the root method when a property path context is active.
            if (context.PropertyPathContext is null)
            {
                return;
            }

            mapMethod = context.GetRootMapMethod();
        }

        if (context.AlgorithmSettings.UseAttributesForConstructorDetectorSettings
            .Equals(MappaMapAlgorithmContextSettings.MappaAttributesForConstructorDetectorSettings.Disable))
        {
            return;
        }

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
                targetType,
                context.PropertyPathContext);
            ValidateSourcePropertyPath(
                context,
                methodDeclarationSyntax,
                methodName,
                sourceTypeName,
                nameof(MappaUsePropertyAttribute),
                attribute.TargetPropertyName,
                attribute.SourcePropertyName,
                sourceType,
                context.PropertyPathContext);
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
                targetType,
                context.PropertyPathContext);

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
                    sourceType,
                    context.PropertyPathContext);
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
                targetType,
                context.PropertyPathContext);
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
                targetType,
                context.PropertyPathContext);
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
                targetType,
                context.PropertyPathContext);
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

    /// <summary>
    /// Reports diagnostics for <see cref="MappaMustMapTargetPropertyAttribute"/> declarations
    /// that list missing or required properties, or conflict with ignore attributes.
    /// </summary>
    /// <param name="context">The mapping algorithm context.</param>
    internal static void ValidateMappaMustMapTargetPropertyAttributes(this MappaMapAlgorithmContext context)
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

        var mustMapAttribute = mapMethod.GetAttribute<MappaMustMapTargetPropertyAttribute>();
        if (mustMapAttribute is null)
        {
            return;
        }

        var methodName = context.GetRootMapMethod().MethodName;
        var targetType = context.TargetType;
        var targetTypeName = targetType.ToDisplayString();
        var targetProperties = targetType.GetTypeProperties().ToArray();
        var ignoredPropertyNames = new HashSet<string>(
            mapMethod.GetAttributes<MappaIgnoreTargetPropertyAttribute>()
                .Select(attribute => attribute.TargetPropertyName),
            StringComparer.Ordinal);

        foreach (var propertyName in mustMapAttribute.TargetPropertyNames.Distinct(StringComparer.Ordinal))
        {
            if (ignoredPropertyNames.Contains(propertyName))
            {
                context.ReportDiagnostic(MappaDiagnostics.MultipleAttributesTargetTheSamePropertyOrParameter(
                    methodDeclarationSyntax,
                    propertyName));
            }

            var targetProperty = Array.Find(
                targetProperties,
                property => property.Name.Equals(propertyName, StringComparison.Ordinal));
            if (targetProperty is null)
            {
                context.ReportDiagnostic(MappaDiagnostics.MappingAttributeTargetPropertyOrParameterDoesNotExist(
                    methodDeclarationSyntax,
                    methodName,
                    nameof(MappaMustMapTargetPropertyAttribute),
                    propertyName,
                    targetTypeName));
                continue;
            }

            if (targetProperty.IsRequired)
            {
                context.ReportDiagnostic(MappaDiagnostics.MappaMustMapTargetPropertyListsRequiredProperty(
                    methodDeclarationSyntax,
                    methodName,
                    propertyName,
                    targetTypeName));
            }
        }
    }

    /// <summary>
    /// Reports diagnostics for inaccessible-member attribute declarations
    /// (missing whitelist names, disabled target flags, unsupported TFM).
    /// </summary>
    /// <param name="context">The mapping algorithm context.</param>
    /// <param name="compilation">The compilation.</param>
    internal static void ValidateMappaAllowInaccessibleMembersAttributes(
        this MappaMapAlgorithmContext context,
        Compilation compilation)
    {
        if (context.MapMethod is null)
        {
            return;
        }

        var mapMethod = context.MapMethod;
        var methodDeclarationSyntax = mapMethod.MethodDeclarationSyntax;
        if (methodDeclarationSyntax is null)
        {
            return;
        }

        var sourceAttribute = mapMethod.GetAttribute<MappaAllowInaccessibleSourceMembersAttribute>();
        var targetAttribute = mapMethod.GetAttribute<MappaAllowInaccessibleTargetMembersAttribute>();
        if (sourceAttribute is null && targetAttribute is null)
        {
            return;
        }

        var methodName = context.GetRootMapMethod().MethodName;

        if (!compilation.IsUnsafeAccessorSupported())
        {
            context.ReportDiagnostic(MappaDiagnostics.UnsafeAccessorNotSupported(
                methodDeclarationSyntax,
                methodName));
        }

        if (targetAttribute is { AllowProperties: false, AllowConstructors: false })
        {
            context.ReportDiagnostic(MappaDiagnostics.AllowInaccessibleTargetMembersDisabledAll(
                methodDeclarationSyntax,
                methodName));
        }

        if (sourceAttribute is not null)
        {
            ValidateMemberNamesExist(
                context,
                methodDeclarationSyntax,
                methodName,
                nameof(MappaAllowInaccessibleSourceMembersAttribute),
                sourceAttribute.MemberNames,
                context.SourceType);
        }

        if (targetAttribute is not null)
        {
            ValidateMemberNamesExist(
                context,
                methodDeclarationSyntax,
                methodName,
                nameof(MappaAllowInaccessibleTargetMembersAttribute),
                targetAttribute.MemberNames,
                context.TargetType);
        }
    }

    /// <summary>
    /// Validates that each segment in <paramref name="segmentsToValidate"/> exists when resolved from <paramref name="targetType"/>.
    /// </summary>
    /// <param name="context">The mapping algorithm context.</param>
    /// <param name="methodDeclarationSyntax">The map method declaration.</param>
    /// <param name="methodName">The map method name.</param>
    /// <param name="targetTypeName">The display name of the target type.</param>
    /// <param name="attributeName">The attribute type name.</param>
    /// <param name="targetPropertyPath">The full target property path from the attribute.</param>
    /// <param name="segmentsToValidate">The remaining target path segments to validate at the current type.</param>
    /// <param name="targetType">The type from which segments are resolved.</param>
    internal static void ValidateTargetPathSegments(
        MappaMapAlgorithmContext context,
        MethodDeclarationSyntax methodDeclarationSyntax,
        string methodName,
        string targetTypeName,
        string attributeName,
        string targetPropertyPath,
        string[] segmentsToValidate,
        ITypeSymbol targetType)
    {
        if (segmentsToValidate.Length == 0)
        {
            return;
        }

        if (segmentsToValidate.Length == 1)
        {
            var segment = segmentsToValidate[0];
            if (!targetType.GetTypeProperties().Any(property => property.Name.Equals(segment, StringComparison.Ordinal)))
            {
                context.ReportDiagnostic(MappaDiagnostics.MappingAttributeTargetPropertyOrParameterDoesNotExist(
                    methodDeclarationSyntax,
                    methodName,
                    attributeName,
                    targetPropertyPath,
                    targetTypeName));
            }

            return;
        }

        if (!PropertyPathSymbolResolver.TryResolvePropertyPath(
                targetType,
                PropertyPath.FromRemainingSegments(segmentsToValidate),
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

    /// <summary>
    /// Returns whether the attribute target path should be validated against the type at the current nesting level.
    /// </summary>
    /// <param name="attributeTargetPath">The attribute target path.</param>
    /// <param name="propertyPathContext">The active property path context.</param>
    /// <returns><see langword="true"/> when validation applies at the current level; otherwise, <see langword="false"/>.</returns>
    internal static bool ShouldValidateAttributeTargetPathAtCurrentLevel(
        PropertyPath attributeTargetPath,
        PropertyPathContext propertyPathContext)
    {
        if (propertyPathContext.IsNestedAttributeScope)
        {
            return propertyPathContext.OuterTargetSegment is string outerTargetSegment
                   && attributeTargetPath.Segments.Length >= 2
                   && attributeTargetPath.Segments[0].Equals(outerTargetSegment, StringComparison.Ordinal);
        }

        return attributeTargetPath.EndsWith(propertyPathContext.RemainingTargetSegments);
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
        ITypeSymbol targetType,
        PropertyPathContext? propertyPathContext)
    {
        var parsedTargetPath = PropertyPath.Parse(targetPropertyPath);
        if (parsedTargetPath.Segments.Length == 0)
        {
            return;
        }

        if (propertyPathContext is not null)
        {
            if (!ShouldValidateAttributeTargetPathAtCurrentLevel(parsedTargetPath, propertyPathContext))
            {
                return;
            }

            ValidateTargetPathSegments(
                context,
                methodDeclarationSyntax,
                methodName,
                targetTypeName,
                attributeName,
                targetPropertyPath,
                propertyPathContext.RemainingTargetSegments,
                targetType);
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
        ITypeSymbol sourceType,
        PropertyPathContext? propertyPathContext)
    {
        if (propertyPathContext is not null)
        {
            return;
        }

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

    private static void ValidateMemberNamesExist(
        MappaMapAlgorithmContext context,
        MethodDeclarationSyntax methodDeclarationSyntax,
        string methodName,
        string attributeName,
        string[] memberNames,
        ITypeSymbol type)
    {
        var typeName = type.ToDisplayString();
        var propertyNames = new HashSet<string>(
            type.GetTypeProperties().Select(property => property.Name),
            StringComparer.Ordinal);

        foreach (var memberName in memberNames
                     .Distinct(StringComparer.Ordinal)
                     .Where(name => !propertyNames.Contains(name)))
        {
            context.ReportDiagnostic(MappaDiagnostics.MappingAttributeTargetPropertyOrParameterDoesNotExist(
                methodDeclarationSyntax,
                methodName,
                attributeName,
                memberName,
                typeName));
        }
    }
}