// <copyright file="FieldSymbolExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods over <see cref="IFieldSymbol"/>.
/// </summary>
internal static class FieldSymbolExtensions
{
    /// <summary>
    /// Gets a value indicating whether <paramref name="field"/> is eligible for instance member mapping
    /// (excludes static, const, ref, fixed, and compiler-generated backing fields).
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns><c>true</c> when the field may be considered for mapping.</returns>
    internal static bool IsEligibleForInstanceMemberMapping(this IFieldSymbol field)
    {
        if (field.IsStatic
            || field.IsConst
            || field.RefKind != RefKind.None
            || field.IsFixedSizeBuffer
            || field.IsImplicitlyDeclared)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Gets a value indicating whether <paramref name="field"/> can be assigned as a mapping target
    /// (eligible instance field that is not <c>readonly</c>).
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns><c>true</c> when the field may be written by mapping.</returns>
    internal static bool IsEligibleForTargetMemberAssignment(this IFieldSymbol field)
        => field.IsEligibleForInstanceMemberMapping() && !field.IsReadOnly;

    /// <summary>
    /// Check if a field is accessible from inside a map method.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="method">The method that will access the field.</param>
    /// <returns><c>true</c> when the field is accessible from <paramref name="method"/>.</returns>
    internal static bool IsFieldAccessible(
        this IFieldSymbol field,
        Compilation compilation,
        MapMethod method)
        => compilation.IsSymbolAccessibleWithin(field, method.ContainingType);
}