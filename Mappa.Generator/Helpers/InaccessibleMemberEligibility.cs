// <copyright file="InaccessibleMemberEligibility.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Helpers for deciding when inaccessible members can participate in mapping.
/// </summary>
internal static class InaccessibleMemberEligibility
{
    /// <summary>
    /// Checks whether a source property can be read either because it is accessible
    /// or because inaccessible source members are opted in and the property is eligible.
    /// </summary>
    /// <param name="property">The source property.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="mapMethod">The root map method.</param>
    /// <param name="options">The inaccessible source options, if any.</param>
    /// <param name="requiresUnsafeAccessor">
    /// Set to <c>true</c> when the property is only readable via an unsafe accessor.
    /// </param>
    /// <returns><c>true</c> when the property can be used as a mapping source.</returns>
    internal static bool TryIsSourcePropertyReadable(
        IPropertySymbol property,
        Compilation compilation,
        MapMethod mapMethod,
        InaccessibleSourceMemberOptions? options,
        out bool requiresUnsafeAccessor)
    {
        requiresUnsafeAccessor = false;

        if (property.IsIndexer || property.GetMethod is null)
        {
            return false;
        }

        if (property.IsGetterAccessible(compilation, mapMethod))
        {
            return true;
        }

        if (options is null
            || !compilation.IsUnsafeAccessorSupported()
            || !options.IsMemberAllowed(property.Name))
        {
            return false;
        }

        requiresUnsafeAccessor = true;
        return true;
    }

    /// <summary>
    /// Checks whether a target property setter can be written either because it is accessible
    /// or because inaccessible target properties are opted in and the property is eligible.
    /// </summary>
    /// <param name="property">The target property.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="mapMethod">The root map method.</param>
    /// <param name="options">The inaccessible target options, if any.</param>
    /// <param name="requiresUnsafeAccessor">
    /// Set to <c>true</c> when the property is only writable via an unsafe accessor.
    /// </param>
    /// <returns><c>true</c> when the property setter can be used.</returns>
    internal static bool TryIsTargetPropertyWritable(
        IPropertySymbol property,
        Compilation compilation,
        MapMethod mapMethod,
        InaccessibleTargetMemberOptions? options,
        out bool requiresUnsafeAccessor)
    {
        requiresUnsafeAccessor = false;

        if (property.IsIndexer || property.SetMethod is null)
        {
            return false;
        }

        if (property.IsSetterAccessible(compilation, mapMethod))
        {
            return true;
        }

        if (options is null
            || !compilation.IsUnsafeAccessorSupported()
            || !options.IsPropertyAllowed(property.Name))
        {
            return false;
        }

        requiresUnsafeAccessor = true;
        return true;
    }

    /// <summary>
    /// Checks whether a constructor can be invoked either because it is accessible
    /// or because inaccessible constructors are opted in.
    /// </summary>
    /// <param name="constructor">The constructor.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="accessibleWithin">The symbol from which accessibility is evaluated.</param>
    /// <param name="options">The inaccessible target options, if any.</param>
    /// <param name="requiresUnsafeAccessor">
    /// Set to <c>true</c> when the constructor is only invokable via an unsafe accessor.
    /// </param>
    /// <returns><c>true</c> when the constructor can be used.</returns>
    internal static bool TryIsConstructorInvokable(
        IMethodSymbol constructor,
        Compilation compilation,
        ISymbol accessibleWithin,
        InaccessibleTargetMemberOptions? options,
        out bool requiresUnsafeAccessor)
    {
        requiresUnsafeAccessor = false;

        if (compilation.IsSymbolAccessibleWithin(constructor, accessibleWithin))
        {
            return true;
        }

        if (options is null
            || !options.AllowConstructors
            || !compilation.IsUnsafeAccessorSupported())
        {
            return false;
        }

        requiresUnsafeAccessor = true;
        return true;
    }
}