// <copyright file="MappaTypeMappingAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type. Non-generic and generic overloads share this file by design.
#pragma warning disable CA1813 // Unsealed so MappaTypeMappingAttribute<TTarget, TSource> can derive from this attribute.

namespace Mappa.Attributes;

/// <summary>
/// Attribute used to support polymorphism in mapper methods by defining
/// for each possible input type what is the expected target type.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MappaTypeMappingAttribute(Type targetType, Type sourceType)
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

/// <summary>
/// Generic form of <see cref="MappaTypeMappingAttribute"/> that pairs <typeparamref name="TTarget"/>
/// with <typeparamref name="TSource"/>. Equivalent to
/// <c>[MappaTypeMapping(typeof(TTarget), typeof(TSource))]</c>.
/// </summary>
/// <typeparam name="TTarget">The target type for this polymorphic mapping entry.</typeparam>
/// <typeparam name="TSource">The source type for this polymorphic mapping entry.</typeparam>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[CLSCompliant(false)]
public sealed class MappaTypeMappingAttribute<TTarget, TSource>()
    : MappaTypeMappingAttribute(typeof(TTarget), typeof(TSource));