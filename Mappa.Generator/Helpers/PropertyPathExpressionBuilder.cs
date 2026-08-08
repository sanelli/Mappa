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
    /// <param name="diagnosticPathOverride">
    /// Optional full path used in null-reference diagnostics (for example the original attribute source path).
    /// </param>
    /// <returns>The chained access expression.</returns>
    internal static string BuildChainedAccessExpression(
        string receiverExpression,
        string receiverPathPrefix,
        string[] pathSegments,
        ITypeSymbol startingType,
        bool nullableEnabled,
        ITypeSymbol targetType,
        out IPropertySymbol[] resolvedProperties,
        string? diagnosticPathOverride = null)
    {
        var path = PropertyPath.FromRemainingSegments(pathSegments);
        var chainExpression = string.IsNullOrWhiteSpace(receiverPathPrefix)
            ? receiverExpression
            : receiverPathPrefix;

        if (!PropertyPathSymbolResolver.TryGetReceiverTypeForPathPrefix(
                startingType,
                receiverExpression,
                chainExpression,
                out var pathStartingType))
        {
            resolvedProperties = [];
            return receiverExpression;
        }

        if (!PropertyPathSymbolResolver.TryResolvePropertyPath(pathStartingType, path, out resolvedProperties, out _))
        {
            return chainExpression;
        }

        var segmentChain = BuildSegmentAccessChain(
            chainExpression,
            pathSegments,
            pathStartingType,
            nullableEnabled,
            resolvedProperties,
            out var diagnosticPath,
            out var receiverTypeForAccess,
            out var usedConditionalAccess);

        return MaybeAppendNullCoalescingThrow(
            segmentChain,
            usedConditionalAccess,
            receiverTypeForAccess,
            nullableEnabled,
            targetType,
            diagnosticPath,
            diagnosticPathOverride);
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

    private static string BuildSegmentAccessChain(
        string chainExpression,
        string[] pathSegments,
        ITypeSymbol pathStartingType,
        bool nullableEnabled,
        IPropertySymbol[] resolvedProperties,
        out string diagnosticPath,
        out ITypeSymbol receiverTypeForAccess,
        out bool usedConditionalAccess)
    {
        var expression = chainExpression;
        diagnosticPath = string.Empty;
        usedConditionalAccess = false;
        var receiverTypeForAccessLocal = pathStartingType;

        for (var index = 0; index < pathSegments.Length; index++)
        {
            var segment = pathSegments[index];
            var useConditionalAccess = ShouldUseConditionalAccess(receiverTypeForAccessLocal, nullableEnabled);
            usedConditionalAccess = usedConditionalAccess || useConditionalAccess;
            var accessOperator = useConditionalAccess ? "?." : ".";
            expression = $"{expression}{accessOperator}{segment}";
            diagnosticPath = string.IsNullOrWhiteSpace(diagnosticPath)
                ? segment
                : $"{diagnosticPath}.{segment}";
            receiverTypeForAccessLocal = resolvedProperties[index].Type;
        }

        receiverTypeForAccess = receiverTypeForAccessLocal;
        return expression;
    }

    private static string MaybeAppendNullCoalescingThrow(
        string expression,
        bool usedConditionalAccess,
        ITypeSymbol receiverTypeForAccess,
        bool nullableEnabled,
        ITypeSymbol targetType,
        string diagnosticPath,
        string? diagnosticPathOverride)
    {
        var expressionCanBeNull = usedConditionalAccess
            || IsNullableCapableType(receiverTypeForAccess, nullableEnabled);
        if (!expressionCanBeNull || targetType.IsNullable())
        {
            return expression;
        }

        var pathForDiagnostic = string.IsNullOrWhiteSpace(diagnosticPathOverride)
            ? diagnosticPath
            : diagnosticPathOverride;
        return $"{expression} ?? throw new System.NullReferenceException({CSharpLiteralHelper.ToStringLiteral($"\"{pathForDiagnostic}\" is null.")})";
    }

    private static bool ShouldUseConditionalAccess(ITypeSymbol segmentType, bool nullableEnabled)
        => IsNullableCapableType(segmentType, nullableEnabled);

    private static bool IsNullableCapableType(ITypeSymbol typeSymbol, bool nullableEnabled)
    {
        if (typeSymbol.IsValueTypeNullable())
        {
            return true;
        }

        if (!typeSymbol.IsReferenceType)
        {
            return false;
        }

        return nullableEnabled
            ? typeSymbol.NullableAnnotation is NullableAnnotation.Annotated
            : typeSymbol.IsReferenceType;
    }
}