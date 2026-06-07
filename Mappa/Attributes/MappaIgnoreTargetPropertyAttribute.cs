// <copyright file="MappaIgnoreTargetPropertyAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Attribute that can be used to exclude a target property from empty-constructor property mapping.
/// </summary>
/// <param name="targetPropertyName">The name of the target property to ignore.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MappaIgnoreTargetPropertyAttribute(string targetPropertyName)
        : Attribute
{
    /// <summary>
    /// Gets the target property name.
    /// </summary>
    public string TargetPropertyName { get; } = targetPropertyName;
}