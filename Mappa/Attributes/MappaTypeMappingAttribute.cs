// <copyright file="MappaTypeMappingAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Attribute used to support polymorphism in mapper methods by defining
/// for each possible input type what is the expected target type.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MappaTypeMappingAttribute(Type targetType, Type sourceType)
    : Attribute
{
    /// <summary>
    /// Gets the target type.
    /// </summary>
    public Type TargetType { get; } = targetType;

    /// <summary>
    /// Gets the source type.
    /// </summary>
    public Type SourceType { get; } = sourceType;
}