// <copyright file="MappaInterceptorAttributeSyntaxMaterializer.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Attributes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Materializes Mappa attribute / settings instances from <c>new</c> object-creation syntax
/// used in interceptor <c>With</c> arguments.
/// </summary>
internal static class MappaInterceptorAttributeSyntaxMaterializer
{
    private const string GlobalPrefix = "global::";

    /// <summary>
    /// Tries to materialize a <see cref="MappaSettingsAttribute"/> from an object creation expression.
    /// </summary>
    /// <param name="creation">The object creation.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The settings instance, or <c>null</c> when materialization fails.</returns>
    internal static MappaSettingsAttribute? TryMaterializeSettings(
        ObjectCreationExpressionSyntax creation,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        var settings = new MappaSettingsAttribute();
        if (creation.Initializer is null)
        {
            return settings;
        }

        foreach (var expression in creation.Initializer.Expressions)
        {
            if (expression is not AssignmentExpressionSyntax assignment
                || assignment.Left is not IdentifierNameSyntax propertyName)
            {
                continue;
            }

            ApplySettingsProperty(
                settings,
                propertyName.Identifier.Text,
                assignment.Right,
                semanticModel,
                cancellationToken);
        }

        return settings;
    }

    /// <summary>
    /// Tries to resolve the dependency type from <c>new MappaStaticDependencyAttribute(typeof(T))</c>.
    /// </summary>
    /// <param name="creation">The object creation.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The dependency type, or <c>null</c> when it cannot be resolved.</returns>
    internal static INamedTypeSymbol? TryMaterializeStaticDependencyType(
        ObjectCreationExpressionSyntax creation,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        if (creation.ArgumentList?.Arguments.Count != 1)
        {
            return null;
        }

        var argument = creation.ArgumentList.Arguments[0].Expression;
        if (argument is not TypeOfExpressionSyntax typeOfExpression)
        {
            return null;
        }

        return semanticModel.GetTypeInfo(typeOfExpression.Type, cancellationToken).Type as INamedTypeSymbol;
    }

    /// <summary>
    /// Tries to materialize a method-level Mappa attribute from an object creation expression.
    /// </summary>
    /// <param name="creation">The object creation.</param>
    /// <param name="attributeType">The attribute type symbol.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The attribute instance, or <c>null</c> when materialization is not supported yet.</returns>
    internal static Attribute? TryMaterializeMethodAttribute(
        ObjectCreationExpressionSyntax creation,
        INamedTypeSymbol attributeType,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        var metadataName = StripGlobalPrefix(
            attributeType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        var ctorArgs = GetConstructorArgumentConstants(creation, semanticModel, cancellationToken);

        switch (metadataName)
        {
            case "Mappa.Attributes.MappaIgnoreTargetPropertyAttribute"
                when ctorArgs.Length == 1 && ctorArgs[0] is string ignoreTarget:
                return new MappaIgnoreTargetPropertyAttribute(ignoreTarget);
            case "Mappa.Attributes.MappaUsePropertyAttribute"
                when ctorArgs.Length == 2 && ctorArgs[0] is string useTarget && ctorArgs[1] is string useSource:
                return new MappaUsePropertyAttribute(useTarget, useSource);
            case "Mappa.Attributes.MappaMustMapTargetPropertyAttribute"
                when ctorArgs.Length == 0:
                return new MappaMustMapTargetPropertyAttribute();
            case "Mappa.Attributes.MappaMustMapTargetPropertyAttribute"
                when ctorArgs.Length >= 1 && ctorArgs.All(static arg => arg is string):
                return new MappaMustMapTargetPropertyAttribute(ctorArgs.Cast<string>().ToArray());
            case "Mappa.Attributes.MappaAssignFromContextAttribute"
                when ctorArgs.Length == 2 && ctorArgs[0] is string contextTarget && ctorArgs[1] is string contextKey:
                return new MappaAssignFromContextAttribute(contextTarget, contextKey);
            case "Mappa.Attributes.MappaAssignFromConstantAttribute"
                when ctorArgs.Length == 2 && ctorArgs[0] is string constantTarget:
                return new MappaAssignFromConstantAttribute(constantTarget, ctorArgs[1]);
            case "Mappa.Attributes.MappaAssignToContextAttribute"
                when ctorArgs.Length == 2 && ctorArgs[0] is string contextKey && ctorArgs[1] is string contextTarget:
                return new MappaAssignToContextAttribute(contextKey, contextTarget);
            case "Mappa.Attributes.MappaNullSubstituteAttribute"
                when ctorArgs.Length == 1 && ctorArgs[0] is string nullSubstituteTarget:
                return MaterializeNullSubstituteAttribute(
                    creation,
                    nullSubstituteTarget,
                    semanticModel,
                    cancellationToken);
            case "Mappa.Attributes.MappaInvokeMethodAttribute"
                when ctorArgs.Length == 2 && ctorArgs[0] is string invokeTarget && ctorArgs[1] is string invokeMethod:
                return MaterializeInvokeMethodAttribute(
                    creation,
                    invokeTarget,
                    invokeMethod,
                    fieldName: null,
                    semanticModel,
                    cancellationToken);
            case "Mappa.Attributes.MappaInvokeMethodAttribute"
                when ctorArgs.Length == 3
                     && ctorArgs[0] is string invokeTargetWithField
                     && ctorArgs[1] is string invokeField
                     && ctorArgs[2] is string invokeMethodWithField:
                return MaterializeInvokeMethodAttribute(
                    creation,
                    invokeTargetWithField,
                    invokeMethodWithField,
                    invokeField,
                    semanticModel,
                    cancellationToken);
            case "Mappa.Attributes.MappaBeforeMapAttribute"
                when ctorArgs.Length == 1 && ctorArgs[0] is string beforeMapMethod:
                return new MappaBeforeMapAttribute(beforeMapMethod);
            case "Mappa.Attributes.MappaBeforeMapAttribute"
                when ctorArgs.Length == 2 && ctorArgs[0] is string beforeField && ctorArgs[1] is string beforeMethod:
                return new MappaBeforeMapAttribute(beforeField, beforeMethod);
            case "Mappa.Attributes.MappaAfterMapAttribute"
                when ctorArgs.Length == 1 && ctorArgs[0] is string afterMapMethod:
                return new MappaAfterMapAttribute(afterMapMethod);
            case "Mappa.Attributes.MappaAfterMapAttribute"
                when ctorArgs.Length == 2 && ctorArgs[0] is string afterField && ctorArgs[1] is string afterMethod:
                return new MappaAfterMapAttribute(afterField, afterMethod);
            case "Mappa.Attributes.MappaApplyTransformerAttribute"
                when ctorArgs.Length == 1 && ctorArgs[0] is string applyTransformerMethod:
                return new MappaApplyTransformerAttribute(applyTransformerMethod);
            case "Mappa.Attributes.MappaIgnoreAttribute":
                return new MappaIgnoreAttribute();
            case "Mappa.Attributes.MappaTransformerAttribute":
                return new MappaTransformerAttribute();
            case "Mappa.Attributes.MappaAllowInaccessibleSourceMembersAttribute"
                when ctorArgs.Length == 0:
                return new MappaAllowInaccessibleSourceMembersAttribute();
            case "Mappa.Attributes.MappaAllowInaccessibleSourceMembersAttribute"
                when ctorArgs.All(static arg => arg is string):
                return new MappaAllowInaccessibleSourceMembersAttribute(ctorArgs.Cast<string>().ToArray());
            case "Mappa.Attributes.MappaAllowInaccessibleTargetMembersAttribute"
                when ctorArgs.Length == 0:
                return MaterializeAllowInaccessibleTargetMembersAttribute(creation, semanticModel, cancellationToken);
            case "Mappa.Attributes.MappaAllowInaccessibleTargetMembersAttribute"
                when ctorArgs.All(static arg => arg is string):
                return MaterializeAllowInaccessibleTargetMembersAttribute(
                    creation,
                    semanticModel,
                    cancellationToken,
                    ctorArgs.Cast<string>().ToArray());
            case "Mappa.Attributes.MappaProjectionAttribute":
                return MaterializeProjectionAttribute(creation, semanticModel, cancellationToken);
            default:
                return null;
        }
    }

    /// <summary>
    /// Returns whether <paramref name="attributeType"/> allows multiple instances.
    /// </summary>
    /// <param name="attributeType">The attribute type.</param>
    /// <returns><c>true</c> when <see cref="AttributeUsageAttribute.AllowMultiple"/> is <c>true</c>.</returns>
    internal static bool AllowsMultiple(INamedTypeSymbol attributeType)
    {
        foreach (var attributeData in attributeType.GetAttributes())
        {
            if (attributeData.AttributeClass?.Name is not "AttributeUsageAttribute")
            {
                continue;
            }

            foreach (var namedArgument in attributeData.NamedArguments)
            {
                if (namedArgument.Key.Equals(nameof(AttributeUsageAttribute.AllowMultiple), StringComparison.Ordinal)
                    && namedArgument.Value.Value is bool allowMultiple)
                {
                    return allowMultiple;
                }
            }

            if (attributeData.ConstructorArguments.Length >= 2
                && attributeData.ConstructorArguments[1].Value is bool constructorAllowMultiple)
            {
                return constructorAllowMultiple;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns whether <paramref name="attributeType"/> implements <c>IMappaMethodAttribute</c>.
    /// </summary>
    /// <param name="attributeType">The attribute type.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> when the marker interface is implemented.</returns>
    internal static bool ImplementsIMappaMethodAttribute(INamedTypeSymbol attributeType, Compilation compilation)
    {
        var marker = compilation.GetTypeByMetadataName("Mappa.IMappaMethodAttribute");
        if (marker is null)
        {
            return false;
        }

        return attributeType.AllInterfaces.Any(
            @interface => SymbolEqualityComparer.Default.Equals(@interface, marker));
    }

    /// <summary>
    /// Strips a leading <c>global::</c> prefix from a fully qualified type name.
    /// </summary>
    /// <param name="metadataName">The display string.</param>
    /// <returns>The name without the global alias prefix.</returns>
    internal static string StripGlobalPrefix(string metadataName)
    {
        if (metadataName.StartsWith(GlobalPrefix, StringComparison.Ordinal))
        {
            return metadataName.Substring(GlobalPrefix.Length);
        }

        return metadataName;
    }

    private static MappaProjectionAttribute MaterializeProjectionAttribute(
        ObjectCreationExpressionSyntax creation,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        var attribute = new MappaProjectionAttribute();
        if (creation.Initializer is null)
        {
            return attribute;
        }

        foreach (var expression in creation.Initializer.Expressions)
        {
            if (expression is not AssignmentExpressionSyntax assignment
                || assignment.Left is not IdentifierNameSyntax propertyName
                || !propertyName.Identifier.Text.Equals(nameof(MappaProjectionAttribute.Enabled), StringComparison.Ordinal))
            {
                continue;
            }

            if (TryGetConstant(assignment.Right, semanticModel, cancellationToken) is int enabled)
            {
                attribute.Enabled = (BooleanSetting)enabled;
            }
        }

        return attribute;
    }

    private static MappaNullSubstituteAttribute MaterializeNullSubstituteAttribute(
        ObjectCreationExpressionSyntax creation,
        string targetPropertyName,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        var attribute = new MappaNullSubstituteAttribute(targetPropertyName);
        if (creation.Initializer is null)
        {
            return attribute;
        }

        foreach (var expression in creation.Initializer.Expressions)
        {
            if (expression is not AssignmentExpressionSyntax assignment
                || assignment.Left is not IdentifierNameSyntax propertyName)
            {
                continue;
            }

            var name = propertyName.Identifier.Text;
            if (name.Equals(nameof(MappaNullSubstituteAttribute.AssignValue), StringComparison.Ordinal))
            {
                attribute.AssignValue = TryGetConstant(assignment.Right, semanticModel, cancellationToken);
                attribute.HasAssignValue = true;
            }
            else if (name.Equals(nameof(MappaNullSubstituteAttribute.SourcePropertyName), StringComparison.Ordinal)
                     && TryGetConstant(assignment.Right, semanticModel, cancellationToken) is string sourcePropertyName)
            {
                attribute.SourcePropertyName = sourcePropertyName;
            }
            else if (name.Equals(nameof(MappaNullSubstituteAttribute.InvokeMethod), StringComparison.Ordinal)
                     && TryGetConstant(assignment.Right, semanticModel, cancellationToken) is string invokeMethod)
            {
                attribute.InvokeMethod = invokeMethod;
            }
        }

        return attribute;
    }

    private static MappaInvokeMethodAttribute MaterializeInvokeMethodAttribute(
        ObjectCreationExpressionSyntax creation,
        string targetPropertyName,
        string methodName,
        string? fieldName,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        var attribute = fieldName is null
            ? new MappaInvokeMethodAttribute(targetPropertyName, methodName)
            : new MappaInvokeMethodAttribute(targetPropertyName, fieldName, methodName);

        if (creation.Initializer is null)
        {
            return attribute;
        }

        foreach (var expression in creation.Initializer.Expressions)
        {
            if (expression is not AssignmentExpressionSyntax assignment
                || assignment.Left is not IdentifierNameSyntax propertyName
                || !propertyName.Identifier.Text.Equals(
                    nameof(MappaInvokeMethodAttribute.SourcePropertyName),
                    StringComparison.Ordinal))
            {
                continue;
            }

            if (TryGetConstant(assignment.Right, semanticModel, cancellationToken) is string sourcePropertyName)
            {
                attribute.SourcePropertyName = sourcePropertyName;
            }
        }

        return attribute;
    }

    private static MappaAllowInaccessibleTargetMembersAttribute MaterializeAllowInaccessibleTargetMembersAttribute(
        ObjectCreationExpressionSyntax creation,
        SemanticModel semanticModel,
        CancellationToken cancellationToken,
        string[]? memberNames = null)
    {
        var attribute = memberNames is null
            ? new MappaAllowInaccessibleTargetMembersAttribute()
            : new MappaAllowInaccessibleTargetMembersAttribute(memberNames);

        if (creation.Initializer is null)
        {
            return attribute;
        }

        foreach (var expression in creation.Initializer.Expressions)
        {
            if (expression is not AssignmentExpressionSyntax assignment
                || assignment.Left is not IdentifierNameSyntax identifier)
            {
                continue;
            }

            var propertyName = identifier.Identifier.Text;
            if (propertyName.Equals(nameof(MappaAllowInaccessibleTargetMembersAttribute.AllowFields), StringComparison.Ordinal)
                && TryGetConstant(assignment.Right, semanticModel, cancellationToken) is bool allowFields)
            {
                attribute.AllowFields = allowFields;
            }
            else if (propertyName.Equals(nameof(MappaAllowInaccessibleTargetMembersAttribute.AllowProperties), StringComparison.Ordinal)
                     && TryGetConstant(assignment.Right, semanticModel, cancellationToken) is bool allowProperties)
            {
                attribute.AllowProperties = allowProperties;
            }
            else if (propertyName.Equals(nameof(MappaAllowInaccessibleTargetMembersAttribute.AllowConstructors), StringComparison.Ordinal)
                     && TryGetConstant(assignment.Right, semanticModel, cancellationToken) is bool allowConstructors)
            {
                attribute.AllowConstructors = allowConstructors;
            }
        }

        return attribute;
    }

    private static void ApplySettingsProperty(
        MappaSettingsAttribute settings,
        string propertyName,
        ExpressionSyntax valueExpression,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        var constant = TryGetConstant(valueExpression, semanticModel, cancellationToken);
        var property = typeof(MappaSettingsAttribute).GetProperty(propertyName);
        if (property is null || !property.CanWrite || constant is null)
        {
            return;
        }

        try
        {
            var converted = ConvertConstantToPropertyType(constant, property.PropertyType);
            if (converted is not null || property.PropertyType.IsClass || IsNullableValueType(property.PropertyType))
            {
                property.SetValue(settings, converted);
            }
        }
        catch (ArgumentException)
        {
            // Ignore values that cannot be converted for this property.
        }
        catch (InvalidCastException)
        {
            // Ignore values that cannot be converted for this property.
        }
    }

    private static object? ConvertConstantToPropertyType(object constant, Type propertyType)
    {
        var underlying = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        if (underlying.IsEnum)
        {
            return Enum.ToObject(underlying, constant);
        }

        if (underlying == typeof(string))
        {
            return Convert.ToString(constant, CultureInfo.InvariantCulture);
        }

        if (underlying == typeof(bool))
        {
            return Convert.ToBoolean(constant, CultureInfo.InvariantCulture);
        }

        if (underlying == typeof(int))
        {
            return Convert.ToInt32(constant, CultureInfo.InvariantCulture);
        }

        if (underlying == typeof(short))
        {
            return Convert.ToInt16(constant, CultureInfo.InvariantCulture);
        }

        if (underlying == typeof(DateTimeStyles) && constant is int dateTimeStyles)
        {
            return (DateTimeStyles)dateTimeStyles;
        }

        if (underlying == typeof(NumberStyles) && constant is int numberStyles)
        {
            return (NumberStyles)numberStyles;
        }

        return Convert.ChangeType(constant, underlying, CultureInfo.InvariantCulture);
    }

    private static bool IsNullableValueType(Type type)
        => Nullable.GetUnderlyingType(type) is not null;

    private static object?[] GetConstructorArgumentConstants(
        ObjectCreationExpressionSyntax creation,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        if (creation.ArgumentList is null)
        {
            return Array.Empty<object?>();
        }

        var values = new List<object?>();
        foreach (var argument in creation.ArgumentList.Arguments)
        {
            values.Add(TryGetConstant(argument.Expression, semanticModel, cancellationToken));
        }

        return values.ToArray();
    }

    private static object? TryGetConstant(
        ExpressionSyntax expression,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        var constant = semanticModel.GetConstantValue(expression, cancellationToken);
        return constant.HasValue ? constant.Value : null;
    }
}