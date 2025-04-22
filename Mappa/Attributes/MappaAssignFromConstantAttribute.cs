// <copyright file="MappaAssignFromConstantAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Attribute that can be used to assign a constant to target property.
/// </summary>
/// <param name="targetPropertyName">The name of the target property or constructor parameter.</param>
/// <param name="value">The name of the context.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MappaAssignFromConstantAttribute(string targetPropertyName, object? value)
        : Attribute, IMappaTargetPropertyNameAttribute
{
    /// <summary>
    /// Gets the target property name.
    /// </summary>
    public string TargetPropertyName { get; } = targetPropertyName;

    /// <summary>
    /// Gets the source property name.
    /// </summary>
    public object? Value { get; } = value;
}