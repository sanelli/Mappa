// <copyright file="IMappaTargetPropertyNameAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa;

/// <summary>
/// Describe a mappa attribute with a field
/// mapping to a property.
/// </summary>
#pragma warning disable CA1711
public interface IMappaTargetPropertyNameAttribute
#pragma warning restore CA1711
{
    /// <summary>
    /// Gets the target property name.
    /// </summary>
    string TargetPropertyName { get; }
}