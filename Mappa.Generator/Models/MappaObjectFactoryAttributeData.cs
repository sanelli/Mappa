// <copyright file="MappaObjectFactoryAttributeData.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describes a parsed <see cref="Mappa.Attributes.MappaObjectFactoryAttribute"/>.
/// </summary>
internal sealed class MappaObjectFactoryAttributeData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaObjectFactoryAttributeData"/> class.
    /// </summary>
    /// <param name="targetType">The target type for which the factory is registered.</param>
    /// <param name="methodName">The factory method name.</param>
    /// <param name="classType">The explicit factory type, if any.</param>
    /// <param name="fieldName">The mapper field or property name, if any.</param>
    /// <param name="location">The attribute location.</param>
    internal MappaObjectFactoryAttributeData(
        INamedTypeSymbol targetType,
        string methodName,
        Type? classType,
        string? fieldName,
        Location? location)
    {
        this.TargetType = targetType;
        this.MethodName = methodName;
        this.ClassType = classType;
        this.FieldName = fieldName;
        this.Location = location;
    }

    /// <summary>
    /// Gets the target type for which the factory is registered.
    /// </summary>
    internal INamedTypeSymbol TargetType { get; }

    /// <summary>
    /// Gets the factory method name.
    /// </summary>
    internal string MethodName { get; }

    /// <summary>
    /// Gets the explicit factory type, if any.
    /// </summary>
    internal Type? ClassType { get; }

    /// <summary>
    /// Gets the mapper field or property name, if any.
    /// </summary>
    internal string? FieldName { get; }

    /// <summary>
    /// Gets the attribute location.
    /// </summary>
    internal Location? Location { get; }
}