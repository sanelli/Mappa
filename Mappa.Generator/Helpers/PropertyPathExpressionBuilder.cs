// <copyright file="PropertyPathExpressionBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Builds C# member-access expressions for nested property paths.
/// </summary>
internal static class PropertyPathExpressionBuilder
{
    /// <summary>
    /// Builds a chained member-access expression for the specified path.
    /// </summary>
    /// <param name="receiverExpression">The starting receiver expression.</param>
    /// <param name="receiverPathPrefix">The dotted path prefix already consumed, used for diagnostics.</param>
    /// <param name="pathSegments">The remaining path segments to access.</param>
    /// <param name="startingType">The type of the receiver expression.</param>
    /// <param name="nullableEnabled">Whether nullable reference types are enabled.</param>
    /// <param name="targetType">The final target member type.</param>
    /// <param name="resolvedProperties">The resolved properties for each segment.</param>
    /// <returns>The chained access expression.</returns>
    internal static string BuildChainedAccessExpression(
        string receiverExpression,
        string receiverPathPrefix,
        string[] pathSegments,
        ITypeSymbol startingType,
        bool nullableEnabled,
        ITypeSymbol targetType,
        out IPropertySymbol[] resolvedProperties)
    {
        var path = PropertyPath.FromRemainingSegments(pathSegments);
        if (!PropertyPathSymbolResolver.TryResolvePropertyPath(startingType, path, out resolvedProperties, out _))
        {
            return receiverExpression;
        }

        var expression = receiverExpression;
        var diagnosticPath = string.IsNullOrWhiteSpace(receiverPathPrefix)
            ? receiverExpression
            : receiverPathPrefix;

        for (var index = 0; index < pathSegments.Length; index++)
        {
            var segment = pathSegments[index];
            var segmentType = resolvedProperties[index].Type;
            var accessOperator = ShouldUseConditionalAccess(segmentType, nullableEnabled) ? "?." : ".";
            expression = $"{expression}{accessOperator}{segment}";
            diagnosticPath = string.IsNullOrWhiteSpace(diagnosticPath)
                ? segment
                : $"{diagnosticPath}.{segment}";
        }

        if (!targetType.IsNullable())
        {
            expression = $"{expression} ?? throw new System.NullReferenceException({CSharpLiteralHelper.ToStringLiteral($"\"{diagnosticPath}\" is null.")})";
        }

        return expression;
    }

    /// <summary>
    /// Builds a chained member-access expression for assign-to-context target reads.
    /// </summary>
    /// <param name="receiverExpression">The starting receiver expression.</param>
    /// <param name="memberPath">The dot-separated member path.</param>
    /// <returns>The member access expression.</returns>
    internal static string BuildTargetMemberAccessExpression(string receiverExpression, string memberPath)
    {
        var path = PropertyPath.Parse(memberPath);
        if (!path.IsNested)
        {
            return $"{receiverExpression}.{memberPath}";
        }

        return $"{receiverExpression}.{path.ToDotSeparatedString()}";
    }

    private static bool ShouldUseConditionalAccess(ITypeSymbol segmentType, bool nullableEnabled)
    {
        if (segmentType.IsValueTypeNullable())
        {
            return true;
        }

        if (!segmentType.IsReferenceType)
        {
            return false;
        }

        return nullableEnabled
            ? segmentType.NullableAnnotation is NullableAnnotation.Annotated
            : segmentType.IsReferenceType;
    }
}