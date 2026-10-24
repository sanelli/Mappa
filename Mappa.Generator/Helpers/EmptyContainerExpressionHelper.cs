// <copyright file="EmptyContainerExpressionHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Resolves empty-container expressions for <c>ReplaceNullWithEmptyContainer</c>.
/// </summary>
internal static class EmptyContainerExpressionHelper
{
    /// <summary>
    /// Returns <c>true</c> when the setting is enabled.
    /// </summary>
    /// <param name="setting">The user setting.</param>
    /// <returns><c>true</c> when empty-container replacement is enabled.</returns>
    internal static bool IsEnabled(BooleanSetting setting)
        => setting is BooleanSetting.Enable;

    /// <summary>
    /// Returns <c>true</c> when a container/dictionary strategy should handle null via an empty target
    /// instead of <see cref="NullableStrategy"/>.
    /// </summary>
    /// <param name="strategy">The candidate element strategy.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="mapMethodSymbol">The map method symbol, used for constructor accessibility.</param>
    /// <returns><c>true</c> when null-to-empty applies.</returns>
    internal static bool ShouldReplaceNullWithEmpty(
        MapStrategy strategy,
        Compilation compilation,
        IMethodSymbol? mapMethodSymbol)
    {
        return strategy switch
        {
            CollectionToCollectionMapStrategy collection
                => IsEnabled(collection.ReplaceNullWithEmptyContainer)
                   && TryGetEmptyExpression(collection.TargetType, compilation, mapMethodSymbol, out _),
            DictionaryToDictionaryMapStrategy dictionary
                => IsEnabled(dictionary.ReplaceNullWithEmptyContainer)
                   && TryGetEmptyExpression(dictionary.TargetType, compilation, mapMethodSymbol, out _),
            _ => false,
        };
    }

    /// <summary>
    /// Attempts to build a C# expression that yields an empty instance of <paramref name="targetType"/>.
    /// </summary>
    /// <param name="targetType">The target container or dictionary type.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="mapMethodSymbol">The map method symbol, used for constructor accessibility.</param>
    /// <param name="emptyExpression">The empty expression when successful.</param>
    /// <returns><c>true</c> when an empty expression is available for the target.</returns>
    internal static bool TryGetEmptyExpression(
        ITypeSymbol targetType,
        Compilation compilation,
        IMethodSymbol? mapMethodSymbol,
        out string emptyExpression)
    {
        emptyExpression = string.Empty;
        var nonNullTarget = targetType.GetTypeInsideNullable();

        if (IsExcludedFromEmptyContainer(nonNullTarget, compilation))
        {
            return false;
        }

        if (TryGetWellKnownEmptyExpression(nonNullTarget, compilation, out emptyExpression))
        {
            return true;
        }

        if (TryGetAllowListedInterfaceOrArrayEmptyExpression(nonNullTarget, compilation, out emptyExpression))
        {
            return true;
        }

        if (nonNullTarget.TypeKind is TypeKind.Class or TypeKind.Struct
            && nonNullTarget is not INamedTypeSymbol { IsAbstract: true }
            && nonNullTarget.TypeKind is not TypeKind.Interface
            && nonNullTarget.HasSymbolAccessibleZeroParametersConstructor(compilation, mapMethodSymbol))
        {
            emptyExpression = $"new {nonNullTarget.ToDisplayNameWithoutNullableAnnotation()}()";
            return true;
        }

        return false;
    }

    private static bool IsExcludedFromEmptyContainer(ITypeSymbol targetType, Compilation compilation)
        => targetType.IsSpan(compilation)
           || targetType.IsReadOnlySpan(compilation)
           || targetType.IsMemory(compilation)
           || targetType.IsReadOnlyMemory(compilation)
           || targetType.IsIProducerConsumerCollection(compilation);

    private static bool TryGetWellKnownEmptyExpression(
        ITypeSymbol targetType,
        Compilation compilation,
        out string emptyExpression)
    {
        emptyExpression = string.Empty;
        if (targetType.IsIImmutableQueue(compilation) || targetType.IsImmutableQueue(compilation))
        {
            var elementType = targetType.GetElementType().ToDisplayString();
            emptyExpression = $"global::System.Collections.Immutable.ImmutableQueue<{elementType}>.Empty";
            return true;
        }

        if (targetType.IsIImmutableStack(compilation) || targetType.IsImmutableStack(compilation))
        {
            var elementType = targetType.GetElementType().ToDisplayString();
            emptyExpression = $"global::System.Collections.Immutable.ImmutableStack<{elementType}>.Empty";
            return true;
        }

        if (targetType.IsImmutableList(compilation))
        {
            var elementType = targetType.GetElementType().ToDisplayString();
            emptyExpression = $"global::System.Collections.Immutable.ImmutableList<{elementType}>.Empty";
            return true;
        }

        if (targetType.IsImmutableArray(compilation))
        {
            var elementType = targetType.GetElementType().ToDisplayString();
            emptyExpression = $"global::System.Collections.Immutable.ImmutableArray<{elementType}>.Empty";
            return true;
        }

        if (targetType.IsImmutableHashSet(compilation))
        {
            var elementType = targetType.GetElementType().ToDisplayString();
            emptyExpression = $"global::System.Collections.Immutable.ImmutableHashSet<{elementType}>.Empty";
            return true;
        }

        if (targetType.IsImmutableSortedSet(compilation))
        {
            var elementType = targetType.GetElementType().ToDisplayString();
            emptyExpression = $"global::System.Collections.Immutable.ImmutableSortedSet<{elementType}>.Empty";
            return true;
        }

        if (targetType.IsImmutableDictionary(compilation) || targetType.IsIImmutableDictionary(compilation))
        {
            var (keyType, valueType) = targetType.GetKeyAndValueTypes(compilation);
            emptyExpression =
                $"global::System.Collections.Immutable.ImmutableDictionary<{keyType.ToDisplayString()}, {valueType.ToDisplayString()}>.Empty";
            return true;
        }

        if (targetType.IsImmutableSortedDictionary(compilation))
        {
            var (keyType, valueType) = targetType.GetKeyAndValueTypes(compilation);
            emptyExpression =
                $"global::System.Collections.Immutable.ImmutableSortedDictionary<{keyType.ToDisplayString()}, {valueType.ToDisplayString()}>.Empty";
            return true;
        }

        if (targetType.IsFrozenSet(compilation))
        {
            var elementType = targetType.GetElementType().ToDisplayString();
            emptyExpression = $"global::System.Collections.Frozen.FrozenSet<{elementType}>.Empty";
            return true;
        }

        if (targetType.IsFrozenDictionary(compilation))
        {
            var (keyType, valueType) = targetType.GetKeyAndValueTypes(compilation);
            emptyExpression =
                $"global::System.Collections.Frozen.FrozenDictionary<{keyType.ToDisplayString()}, {valueType.ToDisplayString()}>.Empty";
            return true;
        }

        if (targetType.IsReadOnlyCollection(compilation))
        {
            var elementType = targetType.GetElementType().ToDisplayString();
            emptyExpression =
                $"new global::System.Collections.ObjectModel.ReadOnlyCollection<{elementType}>(global::System.Array.Empty<{elementType}>())";
            return true;
        }

        if (targetType.IsReadOnlySet(compilation))
        {
            var elementType = targetType.GetElementType().ToDisplayString();
            emptyExpression =
                $"new global::System.Collections.ObjectModel.ReadOnlySet<{elementType}>(global::System.Collections.Immutable.ImmutableHashSet<{elementType}>.Empty)";
            return true;
        }

        if (targetType.IsReadOnlyDictionary(compilation))
        {
            var (keyType, valueType) = targetType.GetKeyAndValueTypes(compilation);
            emptyExpression =
                $"new global::System.Collections.ObjectModel.ReadOnlyDictionary<{keyType.ToDisplayString()}, {valueType.ToDisplayString()}>(new global::System.Collections.Generic.Dictionary<{keyType.ToDisplayString()}, {valueType.ToDisplayString()}>())";
            return true;
        }

        return false;
    }

    private static bool TryGetAllowListedInterfaceOrArrayEmptyExpression(
        ITypeSymbol targetType,
        Compilation compilation,
        out string emptyExpression)
    {
        emptyExpression = string.Empty;
        if (targetType.IsArray()
            || targetType.IsIEnumerable()
            || targetType.IsICollection()
            || targetType.IsIReadOnlyCollection()
            || targetType.IsIList()
            || targetType.IsIReadOnlyList()
            || targetType.IsISet(compilation)
            || targetType.IsIReadOnlySet(compilation)
            || targetType.IsIImmutableList(compilation)
            || targetType.IsIImmutableSet(compilation)
            || targetType.IsIDictionary(compilation)
            || targetType.IsIReadOnlyDictionary(compilation)
            || targetType.IsIEnumerableOfKeyValuePairs(compilation))
        {
            emptyExpression = "[]";
            return true;
        }

        return false;
    }
}