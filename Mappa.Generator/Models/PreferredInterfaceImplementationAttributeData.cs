// <copyright file="PreferredInterfaceImplementationAttributeData.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describes a parsed preferred-interface attribute on a map method.
/// </summary>
internal sealed class PreferredInterfaceImplementationAttributeData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PreferredInterfaceImplementationAttributeData"/> class.
    /// </summary>
    /// <param name="side">The mapping side the attribute applies to.</param>
    /// <param name="implementingType">The concrete implementing type, when valid.</param>
    /// <param name="preferredClosedInterface">The preferred closed interface, when valid.</param>
    /// <param name="location">The attribute location.</param>
    /// <param name="implementingTypeDisplay">Display string for the implementing type argument.</param>
    /// <param name="preferredInterfaceDisplay">Display string for the preferred interface argument.</param>
    /// <param name="hasImplementingType">Whether an implementing type argument was provided.</param>
    /// <param name="hasPreferredInterface">Whether a preferred interface argument was provided.</param>
    internal PreferredInterfaceImplementationAttributeData(
        PreferredInterfaceSide side,
        INamedTypeSymbol? implementingType,
        INamedTypeSymbol? preferredClosedInterface,
        Location? location,
        string implementingTypeDisplay,
        string preferredInterfaceDisplay,
        bool hasImplementingType,
        bool hasPreferredInterface)
    {
        this.Side = side;
        this.ImplementingType = implementingType;
        this.PreferredClosedInterface = preferredClosedInterface;
        this.Location = location;
        this.ImplementingTypeDisplay = implementingTypeDisplay;
        this.PreferredInterfaceDisplay = preferredInterfaceDisplay;
        this.HasImplementingType = hasImplementingType;
        this.HasPreferredInterface = hasPreferredInterface;
    }

    /// <summary>
    /// Gets the mapping side.
    /// </summary>
    internal PreferredInterfaceSide Side { get; }

    /// <summary>
    /// Gets the concrete implementing type when valid.
    /// </summary>
    internal INamedTypeSymbol? ImplementingType { get; }

    /// <summary>
    /// Gets the preferred closed interface when valid.
    /// </summary>
    internal INamedTypeSymbol? PreferredClosedInterface { get; }

    /// <summary>
    /// Gets the attribute location.
    /// </summary>
    internal Location? Location { get; }

    /// <summary>
    /// Gets the implementing type display string.
    /// </summary>
    internal string ImplementingTypeDisplay { get; }

    /// <summary>
    /// Gets the preferred interface display string.
    /// </summary>
    internal string PreferredInterfaceDisplay { get; }

    /// <summary>
    /// Gets a value indicating whether an implementing type argument was provided.
    /// </summary>
    internal bool HasImplementingType { get; }

    /// <summary>
    /// Gets a value indicating whether a preferred interface argument was provided.
    /// </summary>
    internal bool HasPreferredInterface { get; }
}