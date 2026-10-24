// <copyright file="PropertyPathSymbolResolver.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Resolves dot-separated property/field paths against type symbols.
/// </summary>
internal static class PropertyPathSymbolResolver
{
    /// <summary>
    /// Tries to resolve a property path on a type.
    /// </summary>
    /// <param name="typeSymbol">The starting type.</param>
    /// <param name="path">The property path.</param>
    /// <param name="resolvedProperties">The resolved properties in path order.</param>
    /// <param name="missingSegment">The first segment that could not be resolved, if any.</param>
    /// <returns><c>true</c> when the full path resolves; otherwise, <c>false</c>.</returns>
    internal static bool TryResolvePropertyPath(
        ITypeSymbol typeSymbol,
        PropertyPath path,
        out IPropertySymbol[] resolvedProperties,
        out string? missingSegment)
    {
        if (!TryResolveMemberPath(typeSymbol, path, includeFields: false, out var resolvedMembers, out missingSegment))
        {
            resolvedProperties = [];
            return false;
        }

        resolvedProperties = resolvedMembers
            .Select(member => member.Property!)
            .ToArray();
        return true;
    }

    /// <summary>
    /// Tries to resolve a member path on a type, preferring properties over fields per segment.
    /// </summary>
    /// <param name="typeSymbol">The starting type.</param>
    /// <param name="path">The member path.</param>
    /// <param name="includeFields">Whether fields may resolve a segment when no property matches.</param>
    /// <param name="resolvedMembers">The resolved members in path order.</param>
    /// <param name="missingSegment">The first segment that could not be resolved, if any.</param>
    /// <returns><c>true</c> when the full path resolves; otherwise, <c>false</c>.</returns>
    internal static bool TryResolveMemberPath(
        ITypeSymbol typeSymbol,
        PropertyPath path,
        bool includeFields,
        out MappableMember[] resolvedMembers,
        out string? missingSegment)
    {
        List<MappableMember> members = new();
        var currentType = typeSymbol;
        foreach (var segment in path.Segments)
        {
            var property = currentType
                .GetTypeProperties()
                .FirstOrDefault(candidate => candidate.Name.Equals(segment, StringComparison.Ordinal)
                                             || candidate.GetMappablePropertyName().Equals(segment, StringComparison.Ordinal));

            if (property is not null)
            {
                members.Add(MappableMember.FromProperty(property, property.GetMappablePropertyName()));
                currentType = property.Type;
                continue;
            }

            if (includeFields)
            {
                var field = currentType
                    .GetTypeFields()
                    .FirstOrDefault(candidate => candidate.Name.Equals(segment, StringComparison.Ordinal));
                if (field is not null)
                {
                    members.Add(MappableMember.FromField(field));
                    currentType = field.Type;
                    continue;
                }
            }

            resolvedMembers = [];
            missingSegment = segment;
            return false;
        }

        resolvedMembers = [.. members];
        missingSegment = null;
        return true;
    }

    /// <summary>
    /// Resolves the receiver type reached by a dotted path prefix from the root source receiver.
    /// </summary>
    /// <param name="rootSourceType">The root source type.</param>
    /// <param name="rootReceiverExpression">The root receiver expression.</param>
    /// <param name="receiverPathPrefix">The dotted receiver path prefix.</param>
    /// <param name="receiverType">The resolved receiver type.</param>
    /// <returns><c>true</c> when the prefix resolves; otherwise, <c>false</c>.</returns>
    internal static bool TryGetReceiverTypeForPathPrefix(
        ITypeSymbol rootSourceType,
        string rootReceiverExpression,
        string receiverPathPrefix,
        out ITypeSymbol receiverType)
        => TryGetReceiverTypeForPathPrefix(
            rootSourceType,
            rootReceiverExpression,
            receiverPathPrefix,
            includeFields: false,
            out receiverType);

    /// <summary>
    /// Resolves the receiver type reached by a dotted path prefix from the root source receiver.
    /// </summary>
    /// <param name="rootSourceType">The root source type.</param>
    /// <param name="rootReceiverExpression">The root receiver expression.</param>
    /// <param name="receiverPathPrefix">The dotted receiver path prefix.</param>
    /// <param name="includeFields">Whether fields may resolve a path segment.</param>
    /// <param name="receiverType">The resolved receiver type.</param>
    /// <returns><c>true</c> when the prefix resolves; otherwise, <c>false</c>.</returns>
    internal static bool TryGetReceiverTypeForPathPrefix(
        ITypeSymbol rootSourceType,
        string rootReceiverExpression,
        string receiverPathPrefix,
        bool includeFields,
        out ITypeSymbol receiverType)
    {
        receiverType = rootSourceType;
        if (string.IsNullOrWhiteSpace(receiverPathPrefix)
            || receiverPathPrefix.Equals(rootReceiverExpression, StringComparison.Ordinal))
        {
            return true;
        }

        var prefixSuffix = $"{rootReceiverExpression}.";
        if (!receiverPathPrefix.StartsWith(prefixSuffix, StringComparison.Ordinal))
        {
            return false;
        }

        var suffixPath = PropertyPath.Parse(receiverPathPrefix.Substring(prefixSuffix.Length));
        if (!TryResolveMemberPath(rootSourceType, suffixPath, includeFields, out var resolvedMembers, out _))
        {
            return false;
        }

        receiverType = resolvedMembers[resolvedMembers.Length - 1].Type;
        return true;
    }

    /// <summary>
    /// Tries to resolve a property path on a type, including fields for assign-to-context targets.
    /// </summary>
    /// <param name="typeSymbol">The starting type.</param>
    /// <param name="path">The property path.</param>
    /// <param name="memberAccessExpression">The member access expression for the resolved path.</param>
    /// <param name="missingSegment">The first segment that could not be resolved, if any.</param>
    /// <returns><c>true</c> when the full path resolves; otherwise, <c>false</c>.</returns>
    internal static bool TryResolveTargetMemberPath(
        ITypeSymbol typeSymbol,
        PropertyPath path,
        out string memberAccessExpression,
        out string? missingSegment)
    {
        if (path.Segments.Length == 0)
        {
            memberAccessExpression = string.Empty;
            missingSegment = string.Empty;
            return false;
        }

        if (path.Segments.Length == 1)
        {
            memberAccessExpression = path.Segments[0];
            missingSegment = null;
            return true;
        }

        if (!TryResolveMemberPath(typeSymbol, path, includeFields: true, out var resolvedMembers, out missingSegment))
        {
            memberAccessExpression = string.Empty;
            return false;
        }

        memberAccessExpression = string.Join(".", resolvedMembers.Select(member => member.Name));
        missingSegment = null;
        return true;
    }
}