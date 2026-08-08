// <copyright file="MapMethodMappingAttributesValidator.TargetNames.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Target-name validation for <see cref="MapMethodMappingAttributesValidator"/>.
/// </summary>
internal static partial class MapMethodMappingAttributesValidator
{
    private static void ValidateUsePropertyTargetNames(
        MappaMapAlgorithmContext context,
        TargetNamesValidationContext validationContext,
        MappaUsePropertyAttribute[] attributes)
    {
        foreach (var attribute in attributes)
        {
            ValidateTargetPropertyPath(
                context,
                validationContext.MethodDeclarationSyntax,
                validationContext.MethodName,
                validationContext.TargetTypeName,
                nameof(MappaUsePropertyAttribute),
                attribute.TargetPropertyName,
                validationContext.PropertyNames,
                validationContext.ConstructorParameterNames,
                validationContext.TargetType,
                validationContext.PropertyPathContext);
            ValidateSourcePropertyPath(
                context,
                validationContext.MethodDeclarationSyntax,
                validationContext.MethodName,
                validationContext.SourceTypeName,
                nameof(MappaUsePropertyAttribute),
                attribute.TargetPropertyName,
                attribute.SourcePropertyName,
                validationContext.SourceType,
                validationContext.PropertyPathContext);
        }
    }

    private static void ValidateInvokeMethodTargetNames(
        MappaMapAlgorithmContext context,
        TargetNamesValidationContext validationContext,
        MappaInvokeMethodAttribute[] attributes)
    {
        foreach (var attribute in attributes)
        {
            ValidateTargetPropertyPath(
                context,
                validationContext.MethodDeclarationSyntax,
                validationContext.MethodName,
                validationContext.TargetTypeName,
                nameof(MappaInvokeMethodAttribute),
                attribute.TargetPropertyName,
                validationContext.PropertyNames,
                validationContext.ConstructorParameterNames,
                validationContext.TargetType,
                validationContext.PropertyPathContext);

            if (attribute.SourcePropertyName is not string sourcePropertyName
                || string.IsNullOrWhiteSpace(sourcePropertyName))
            {
                continue;
            }

            ValidateSourcePropertyPath(
                context,
                validationContext.MethodDeclarationSyntax,
                validationContext.MethodName,
                validationContext.SourceTypeName,
                nameof(MappaInvokeMethodAttribute),
                attribute.TargetPropertyName,
                sourcePropertyName,
                validationContext.SourceType,
                validationContext.PropertyPathContext);
        }
    }

    private static void ValidateTargetOnlyAttributeNames<TAttribute>(
        MappaMapAlgorithmContext context,
        TargetNamesValidationContext validationContext,
        TAttribute[] attributes,
        string attributeName,
        Func<TAttribute, string> getTargetPropertyName)
        where TAttribute : notnull
    {
        foreach (var attribute in attributes)
        {
            ValidateTargetPropertyPath(
                context,
                validationContext.MethodDeclarationSyntax,
                validationContext.MethodName,
                validationContext.TargetTypeName,
                attributeName,
                getTargetPropertyName(attribute),
                validationContext.PropertyNames,
                validationContext.ConstructorParameterNames,
                validationContext.TargetType,
                validationContext.PropertyPathContext);
        }
    }

    private sealed class TargetNamesValidationContext
    {
        internal TargetNamesValidationContext(
            MethodDeclarationSyntax methodDeclarationSyntax,
            string methodName,
            string targetTypeName,
            string sourceTypeName,
            HashSet<string> propertyNames,
            string[] constructorParameterNames,
            ITypeSymbol targetType,
            ITypeSymbol sourceType,
            PropertyPathContext? propertyPathContext)
        {
            this.MethodDeclarationSyntax = methodDeclarationSyntax;
            this.MethodName = methodName;
            this.TargetTypeName = targetTypeName;
            this.SourceTypeName = sourceTypeName;
            this.PropertyNames = propertyNames;
            this.ConstructorParameterNames = constructorParameterNames;
            this.TargetType = targetType;
            this.SourceType = sourceType;
            this.PropertyPathContext = propertyPathContext;
        }

        internal MethodDeclarationSyntax MethodDeclarationSyntax { get; }

        internal string MethodName { get; }

        internal string TargetTypeName { get; }

        internal string SourceTypeName { get; }

        internal HashSet<string> PropertyNames { get; }

        internal string[] ConstructorParameterNames { get; }

        internal ITypeSymbol TargetType { get; }

        internal ITypeSymbol SourceType { get; }

        internal PropertyPathContext? PropertyPathContext { get; }
    }
}