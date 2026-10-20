// <copyright file="MappaUseConstructorAttributeData.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describes a parsed <see cref="Mappa.Attributes.MappaUseConstructorAttribute"/>.
/// </summary>
internal sealed class MappaUseConstructorAttributeData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaUseConstructorAttributeData"/> class.
    /// </summary>
    /// <param name="declaredTargetType">
    /// The explicit target type from the attribute, or <see langword="null"/> for the map method return type.
    /// </param>
    /// <param name="constructorType">The constructor kind.</param>
    /// <param name="parameterTypes">The exact constructor parameter types, when specified.</param>
    /// <param name="location">The attribute location.</param>
    internal MappaUseConstructorAttributeData(
        ITypeSymbol? declaredTargetType,
        Mappa.ConstructorType constructorType,
        ITypeSymbol[] parameterTypes,
        Location? location)
    {
        this.DeclaredTargetType = declaredTargetType;
        this.ConstructorType = constructorType;
        this.ParameterTypes = parameterTypes;
        this.Location = location;
    }

    /// <summary>
    /// Gets the explicit target type from the attribute, or <see langword="null"/> for the map method return type.
    /// </summary>
    internal ITypeSymbol? DeclaredTargetType { get; }

    /// <summary>
    /// Gets the constructor kind.
    /// </summary>
    internal Mappa.ConstructorType ConstructorType { get; }

    /// <summary>
    /// Gets the exact constructor parameter types, when specified.
    /// </summary>
    internal ITypeSymbol[] ParameterTypes { get; }

    /// <summary>
    /// Gets the attribute location.
    /// </summary>
    internal Location? Location { get; }

    /// <summary>
    /// Gets or sets the effective target type after resolving a missing declared type to the root map target.
    /// </summary>
    internal ITypeSymbol? EffectiveTargetType { get; set; }
}