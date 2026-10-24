// <copyright file="InaccessibleMemberAccessHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Builds member-access and construction expressions that may use generated <c>UnsafeAccessor</c> methods
/// or explicit interface casts.
/// </summary>
internal static class InaccessibleMemberAccessHelper
{
    /// <summary>
    /// Builds an expression that reads <paramref name="property"/> from <paramref name="receiverExpression"/>.
    /// </summary>
    /// <param name="receiverExpression">The instance expression.</param>
    /// <param name="property">The property to read.</param>
    /// <param name="requiresUnsafeAccessor"><c>true</c> when an unsafe accessor must be used.</param>
    /// <param name="requiresInterfaceCast"><c>true</c> when an interface cast must be used.</param>
    /// <param name="context">The builder context.</param>
    /// <returns>The read expression.</returns>
    internal static string BuildPropertyReadExpression(
        string receiverExpression,
        IPropertySymbol property,
        bool requiresUnsafeAccessor,
        bool requiresInterfaceCast,
        MappaBuilderContext context)
    {
        if (requiresInterfaceCast
            && InaccessibleMemberEligibility.TryGetExplicitInterfaceType(property, out var interfaceType))
        {
            var interfacePropertyName = property.ExplicitInterfaceImplementations[0].Name;
            return $"(({interfaceType.ToDisplayNameWithoutNullableAnnotation()}){receiverExpression}).{interfacePropertyName}";
        }

        if (!requiresUnsafeAccessor)
        {
            return $"{receiverExpression}.{property.Name}";
        }

        var accessor = context.InaccessibleAccessors.GetOrAddPropertyGetter(
            property.ContainingType,
            property,
            context);
        return $"{accessor}({receiverExpression})";
    }

    /// <summary>
    /// Builds an expression that reads <paramref name="field"/> from <paramref name="receiverExpression"/>.
    /// </summary>
    /// <param name="receiverExpression">The instance expression.</param>
    /// <param name="field">The field to read.</param>
    /// <param name="requiresUnsafeAccessor"><c>true</c> when an unsafe accessor must be used.</param>
    /// <param name="context">The builder context.</param>
    /// <returns>The read expression.</returns>
    internal static string BuildFieldReadExpression(
        string receiverExpression,
        IFieldSymbol field,
        bool requiresUnsafeAccessor,
        MappaBuilderContext context)
    {
        if (!requiresUnsafeAccessor)
        {
            return $"{receiverExpression}.{field.Name}";
        }

        var accessor = context.InaccessibleAccessors.GetOrAddField(
            field.ContainingType,
            field,
            context);
        return $"{accessor}({receiverExpression})";
    }

    /// <summary>
    /// Builds a statement that assigns <paramref name="valueExpression"/> to <paramref name="property"/>
    /// on <paramref name="receiverExpression"/>.
    /// </summary>
    /// <param name="receiverExpression">The instance expression.</param>
    /// <param name="property">The property to write.</param>
    /// <param name="valueExpression">The value expression.</param>
    /// <param name="requiresUnsafeAccessor"><c>true</c> when an unsafe accessor must be used.</param>
    /// <param name="requiresInterfaceCast"><c>true</c> when an interface cast must be used.</param>
    /// <param name="context">The builder context.</param>
    /// <returns>The assignment statement without a trailing newline.</returns>
    internal static string BuildPropertyAssignmentStatement(
        string receiverExpression,
        IPropertySymbol property,
        string valueExpression,
        bool requiresUnsafeAccessor,
        bool requiresInterfaceCast,
        MappaBuilderContext context)
    {
        if (requiresInterfaceCast
            && InaccessibleMemberEligibility.TryGetExplicitInterfaceType(property, out var interfaceType))
        {
            var interfacePropertyName = property.ExplicitInterfaceImplementations[0].Name;
            return $"(({interfaceType.ToDisplayNameWithoutNullableAnnotation()}){receiverExpression}).{interfacePropertyName} = {valueExpression};";
        }

        if (!requiresUnsafeAccessor)
        {
            return $"{receiverExpression}.{property.Name} = {valueExpression};";
        }

        var accessor = context.InaccessibleAccessors.GetOrAddPropertySetter(
            property.ContainingType,
            property,
            context);
        return $"{accessor}({receiverExpression}, {valueExpression});";
    }

    /// <summary>
    /// Builds a statement that assigns <paramref name="valueExpression"/> to <paramref name="field"/>
    /// on <paramref name="receiverExpression"/>.
    /// </summary>
    /// <param name="receiverExpression">The instance expression.</param>
    /// <param name="field">The field to write.</param>
    /// <param name="valueExpression">The value expression.</param>
    /// <param name="requiresUnsafeAccessor"><c>true</c> when an unsafe accessor must be used.</param>
    /// <param name="context">The builder context.</param>
    /// <returns>The assignment statement without a trailing newline.</returns>
    internal static string BuildFieldAssignmentStatement(
        string receiverExpression,
        IFieldSymbol field,
        string valueExpression,
        bool requiresUnsafeAccessor,
        MappaBuilderContext context)
    {
        if (!requiresUnsafeAccessor)
        {
            return $"{receiverExpression}.{field.Name} = {valueExpression};";
        }

        var accessor = context.InaccessibleAccessors.GetOrAddField(
            field.ContainingType,
            field,
            context);
        return $"{accessor}({receiverExpression}) = {valueExpression};";
    }

    /// <summary>
    /// Builds an expression that constructs an instance using <paramref name="constructor"/>.
    /// </summary>
    /// <param name="constructor">The constructor.</param>
    /// <param name="argumentExpressions">The argument expressions.</param>
    /// <param name="requiresUnsafeAccessor"><c>true</c> when an unsafe accessor must be used.</param>
    /// <param name="context">The builder context.</param>
    /// <returns>The construction expression.</returns>
    internal static string BuildConstructorInvocationExpression(
        IMethodSymbol constructor,
        IReadOnlyList<string> argumentExpressions,
        bool requiresUnsafeAccessor,
        MappaBuilderContext context)
    {
        var arguments = string.Join(", ", argumentExpressions);
        if (!requiresUnsafeAccessor)
        {
            return $"new {constructor.ContainingType.ToDisplayNameWithoutNullableAnnotation()}({arguments})";
        }

        var accessor = context.InaccessibleAccessors.GetOrAddConstructor(constructor, context);
        return $"{accessor}({arguments})";
    }

    /// <summary>
    /// Builds a target property read expression, honouring the current builder-context access flags
    /// when the property matches the active target property.
    /// </summary>
    /// <param name="receiverExpression">The instance expression.</param>
    /// <param name="property">The property to read.</param>
    /// <param name="context">The builder context.</param>
    /// <returns>The read expression.</returns>
    internal static string BuildTargetPropertyReadExpression(
        string receiverExpression,
        IPropertySymbol property,
        MappaBuilderContext context)
    {
        var isCurrentTarget = SymbolEqualityComparer.Default.Equals(context.CurrentTargetPropertyForUnsafeAccess, property);
        var requiresUnsafeAccessor = context.RequiresUnsafeAccessorOnCurrentTargetProperty && isCurrentTarget;
        var requiresInterfaceCast = context.RequiresInterfaceCastOnCurrentTargetProperty && isCurrentTarget;
        return BuildPropertyReadExpression(
            receiverExpression,
            property,
            requiresUnsafeAccessor,
            requiresInterfaceCast,
            context);
    }
}