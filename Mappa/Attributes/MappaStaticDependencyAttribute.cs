// <copyright file="MappaStaticDependencyAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Attribute that can be used to provide mapping methods that can be used
/// as dependencies when mapping a method via <see cref="Mappa"/> source generator.
/// Type defined by <see cref="Dependency"/> must be a static class.
/// </summary>
/// <param name="dependency">The type exposing the methods to use as dependency.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class MappaStaticDependencyAttribute(Type dependency)
        : Attribute
{
    /// <summary>
    /// Gets the type containing the methods to be used as dependencies while mapping.
    /// </summary>
    public Type Dependency { get; } = dependency;
}