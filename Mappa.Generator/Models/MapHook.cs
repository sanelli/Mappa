// <copyright file="MapHook.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describes a resolved before-map or after-map hook.
/// </summary>
internal sealed class MapHook
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapHook"/> class.
    /// </summary>
    /// <param name="method">The resolved hook method.</param>
    /// <param name="fieldOrProperty">The mapper field or property used to locate the hook, if any.</param>
    /// <param name="explicitType">The explicit type supplied by the attribute, if any.</param>
    /// <param name="attributeLocation">The hook attribute location.</param>
    internal MapHook(
        IMethodSymbol method,
        ISymbol? fieldOrProperty,
        ITypeSymbol? explicitType,
        Location? attributeLocation)
    {
        this.Method = method;
        this.FieldOrProperty = fieldOrProperty;
        this.ExplicitType = explicitType;
        this.AttributeLocation = attributeLocation;
    }

    /// <summary>
    /// Gets the resolved hook method.
    /// </summary>
    internal IMethodSymbol Method { get; }

    /// <summary>
    /// Gets the mapper field or property used to locate the hook, if any.
    /// </summary>
    internal ISymbol? FieldOrProperty { get; }

    /// <summary>
    /// Gets the explicit type supplied by the attribute, if any.
    /// </summary>
    internal ITypeSymbol? ExplicitType { get; }

    /// <summary>
    /// Gets the hook attribute location.
    /// </summary>
    internal Location? AttributeLocation { get; }
}