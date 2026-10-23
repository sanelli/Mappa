// <copyright file="MappaApplyTransformerAttributeData.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describes a parsed <c>[MappaApplyTransformer]</c> attribute.
/// </summary>
internal sealed class MappaApplyTransformerAttributeData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaApplyTransformerAttributeData"/> class.
    /// </summary>
    /// <param name="methodName">The transformer method name.</param>
    /// <param name="classType">The explicit type declaring the static transformer, if any.</param>
    /// <param name="sourcePropertyName">The source property name or dotted path restricting the transformer, if any.</param>
    /// <param name="fieldName">The mapper field or property name used to locate the transformer, if any.</param>
    /// <param name="location">The attribute location.</param>
    internal MappaApplyTransformerAttributeData(
        string methodName,
        INamedTypeSymbol? classType,
        string? sourcePropertyName,
        string? fieldName,
        Location? location)
    {
        this.MethodName = methodName;
        this.ClassType = classType;
        this.SourcePropertyName = sourcePropertyName;
        this.FieldName = fieldName;
        this.Location = location;
    }

    /// <summary>
    /// Gets the transformer method name.
    /// </summary>
    internal string MethodName { get; }

    /// <summary>
    /// Gets the explicit type declaring the static transformer, if any.
    /// </summary>
    internal INamedTypeSymbol? ClassType { get; }

    /// <summary>
    /// Gets the source property name or dotted path restricting the transformer, if any.
    /// </summary>
    internal string? SourcePropertyName { get; }

    /// <summary>
    /// Gets the mapper field or property name used to locate the transformer, if any.
    /// </summary>
    internal string? FieldName { get; }

    /// <summary>
    /// Gets the attribute location.
    /// </summary>
    internal Location? Location { get; }
}