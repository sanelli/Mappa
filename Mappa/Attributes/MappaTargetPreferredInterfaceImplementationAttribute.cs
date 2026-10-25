// <copyright file="MappaTargetPreferredInterfaceImplementationAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type. Non-generic and generic overloads share this file by design.
#pragma warning disable CA1813 // Unsealed so MappaTargetPreferredInterfaceImplementationAttribute<TImplementingType, TClosedInterface> can derive from this attribute.

namespace Mappa.Attributes;

/// <summary>
/// Forces the Mappa source generator to use a specific closed collection or dictionary
/// interface when resolving a <strong>target</strong> type that implements multiple closed
/// versions of the same open-generic interface family (for example both
/// <c>ICollection&lt;int&gt;</c> and <c>ICollection&lt;string&gt;</c>).
/// <para>
/// Apply this attribute to a mapping method only. The first constructor argument is the
/// concrete implementing type being disambiguated (exact symbol match in the mapping tree,
/// including nested properties and collection elements). The second argument is the preferred
/// closed generic interface for that type. The preference applies only when the generator is
/// resolving that open-generic family (for example an <c>IEnumerable&lt;string&gt;</c>
/// preference does not apply when resolving <c>ICollection&lt;&gt;</c>).
/// </para>
/// <para>
/// When a matching attribute is present, the preferred interface is used whether it is
/// implemented implicitly or explicitly. If that interface cannot be used for the mapping,
/// generation fails with an error and does not fall through to other candidates. Without a
/// matching attribute, the generator prefers a unique usable implicit implementation, then a
/// unique usable explicit implementation (with a cast); multiple usable candidates of the same
/// kind, or none, result in the usual silent fall-through.
/// </para>
/// <para>
/// Target preference is narrower than source preference: it is not needed when the declared
/// target is already a closed interface or closed generic with known type arguments, or when
/// the only choice is how to materialize a known element type (for example via
/// <c>EnumerableConcreteType</c>). It matters when a concrete multi-interface target (root or
/// nested) requires discovering element, key, or value types and which <c>Add</c> or indexer
/// to call. Multiple attributes may target different implementing types or interface families
/// on the same method.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MappaTargetPreferredInterfaceImplementationAttribute(Type implementingType, Type preferredClosedInterface)
    : Attribute, IMappaMethodAttribute
{
    /// <summary>
    /// Gets the concrete type being disambiguated when it appears as a target type in the mapping tree.
    /// </summary>
    public Type ImplementingType { get; } = implementingType;

    /// <summary>
    /// Gets the preferred closed generic interface for <see cref="ImplementingType"/>.
    /// </summary>
    public Type PreferredClosedInterface { get; } = preferredClosedInterface;
}

/// <summary>
/// Generic form of <see cref="MappaTargetPreferredInterfaceImplementationAttribute"/> that pairs
/// <typeparamref name="TImplementingType"/> with <typeparamref name="TClosedInterface"/>.
/// Equivalent to
/// <c>[MappaTargetPreferredInterfaceImplementation(typeof(TImplementingType), typeof(TClosedInterface))]</c>.
/// </summary>
/// <typeparam name="TImplementingType">
/// The concrete type that implements multiple closed interfaces of the same open-generic family.
/// </typeparam>
/// <typeparam name="TClosedInterface">
/// The closed generic interface to prefer when resolving that family for <typeparamref name="TImplementingType"/>.
/// </typeparam>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[CLSCompliant(false)]
public sealed class MappaTargetPreferredInterfaceImplementationAttribute<TImplementingType, TClosedInterface>()
    : MappaTargetPreferredInterfaceImplementationAttribute(typeof(TImplementingType), typeof(TClosedInterface));