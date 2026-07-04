// <copyright file="IdentityMapNestedFieldStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Describes nested field mapping for <see cref="IdentityMapStrategy"/> in nested deep copy mode.
/// </summary>
/// <param name="field">The instance field to copy.</param>
/// <param name="fieldStrategy">The strategy used to map the field value.</param>
internal sealed class IdentityMapNestedFieldStrategy(IFieldSymbol field, MapStrategy fieldStrategy)
{
    /// <summary>
    /// Gets the instance field to copy.
    /// </summary>
    public IFieldSymbol Field { get; } = field;

    /// <summary>
    /// Gets the strategy used to map the field value.
    /// </summary>
    public MapStrategy FieldStrategy { get; } = fieldStrategy;
}