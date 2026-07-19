// <copyright file="MapHookAttributeData.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describes a parsed before-map or after-map hook attribute.
/// </summary>
internal sealed class MapHookAttributeData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapHookAttributeData"/> class.
    /// </summary>
    /// <param name="methodName">The hook method name.</param>
    /// <param name="classType">The explicit hook type, if any.</param>
    /// <param name="fieldName">The mapper field or property name, if any.</param>
    /// <param name="location">The attribute location.</param>
    internal MapHookAttributeData(
        string methodName,
        Type? classType,
        string? fieldName,
        Location? location)
    {
        this.MethodName = methodName;
        this.ClassType = classType;
        this.FieldName = fieldName;
        this.Location = location;
    }

    /// <summary>
    /// Gets the hook method name.
    /// </summary>
    internal string MethodName { get; }

    /// <summary>
    /// Gets the explicit hook type, if any.
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