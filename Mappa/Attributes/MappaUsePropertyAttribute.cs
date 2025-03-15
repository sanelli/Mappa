// <copyright file="MappaUsePropertyAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Attribute that can be used to define name of the source property for a specific property.
/// </summary>
/// <param name="targetPropertyName">The name of the target property or constructor parameter.</param>
/// <param name="sourcePropertyName">The name of the source property.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MappaUsePropertyAttribute(string targetPropertyName, string sourcePropertyName)
        : Attribute
{
    /// <summary>
    /// Gets the target property name.
    /// </summary>
    public string TargetPropertyName { get; } = targetPropertyName;

    /// <summary>
    /// Gets the source property name.
    /// </summary>
    public string SourcePropertyName { get; } = sourcePropertyName;
}