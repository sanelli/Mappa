// <copyright file="IdentityMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Describe an identity type map strategy.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
/// <param name="identityMapDeepCopySetting">The effective identity deep copy setting.</param>
/// <param name="requiresMemberwiseClone">Whether generated code must call <see cref="object.MemberwiseClone"/>.</param>
/// <param name="isStructRoot">Whether the identity mapping root is a struct type.</param>
/// <param name="nestedFieldStrategies">Nested field strategies for nested deep copy mode.</param>
internal sealed class IdentityMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    IdentityMapDeepCopySetting identityMapDeepCopySetting = IdentityMapDeepCopySetting.ShallowCopy,
    bool requiresMemberwiseClone = false,
    bool isStructRoot = false,
    IReadOnlyList<IdentityMapNestedFieldStrategy>? nestedFieldStrategies = null)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the effective identity deep copy setting.
    /// </summary>
    public IdentityMapDeepCopySetting IdentityMapDeepCopySetting { get; } = identityMapDeepCopySetting;

    /// <summary>
    /// Gets a value indicating whether generated code must call <see cref="object.MemberwiseClone"/>.
    /// </summary>
    public bool RequiresMemberwiseClone { get; } = requiresMemberwiseClone;

    /// <summary>
    /// Gets a value indicating whether the identity mapping root is a struct type.
    /// </summary>
    public bool IsStructRoot { get; } = isStructRoot;

    /// <summary>
    /// Gets nested field strategies for nested deep copy mode.
    /// </summary>
    public IReadOnlyList<IdentityMapNestedFieldStrategy> NestedFieldStrategies { get; } = nestedFieldStrategies ?? [];

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new IdentityMapStrategyBuilder(this);
}