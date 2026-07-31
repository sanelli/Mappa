// <copyright file="ObjectFactory.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describes how an object factory method is invoked.
/// </summary>
internal enum ObjectFactoryInvocationKind
{
    /// <summary>
    /// The factory fully produces the target; no property assignment is performed.
    /// </summary>
    FullyProduced,

    /// <summary>
    /// The factory is treated like an empty constructor; property fills may apply.
    /// </summary>
    EmptyCtorLike,

    /// <summary>
    /// The factory parameters are mapped like a parameterized constructor.
    /// </summary>
    ParameterizedLike,
}

/// <summary>
/// Describes a resolved object factory.
/// </summary>
internal sealed class ObjectFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectFactory"/> class.
    /// </summary>
    /// <param name="method">The resolved factory method.</param>
    /// <param name="fieldOrProperty">The mapper field or property used to locate the factory, if any.</param>
    /// <param name="explicitType">The explicit type supplied by the attribute, if any.</param>
    /// <param name="invocationKind">The invocation signature tier.</param>
    /// <param name="attributeLocation">The factory attribute location.</param>
    internal ObjectFactory(
        IMethodSymbol method,
        ISymbol? fieldOrProperty,
        ITypeSymbol? explicitType,
        ObjectFactoryInvocationKind invocationKind,
        Location? attributeLocation)
    {
        this.Method = method;
        this.FieldOrProperty = fieldOrProperty;
        this.ExplicitType = explicitType;
        this.InvocationKind = invocationKind;
        this.AttributeLocation = attributeLocation;
    }

    /// <summary>
    /// Gets the resolved factory method.
    /// </summary>
    internal IMethodSymbol Method { get; }

    /// <summary>
    /// Gets the mapper field or property used to locate the factory, if any.
    /// </summary>
    internal ISymbol? FieldOrProperty { get; }

    /// <summary>
    /// Gets the explicit type supplied by the attribute, if any.
    /// </summary>
    internal ITypeSymbol? ExplicitType { get; }

    /// <summary>
    /// Gets the invocation signature tier.
    /// </summary>
    internal ObjectFactoryInvocationKind InvocationKind { get; }

    /// <summary>
    /// Gets the factory attribute location.
    /// </summary>
    internal Location? AttributeLocation { get; }
}