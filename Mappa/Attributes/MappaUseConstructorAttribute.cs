// <copyright file="MappaUseConstructorAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type. Non-generic and generic overloads share this file by design.
#pragma warning disable CA1813 // Unsealed so generic MappaUseConstructorAttribute<> variants can derive from this attribute.

namespace Mappa.Attributes;

/// <summary>
/// Forces the Mappa source generator to construct the mapping target (or a nested target type)
/// using a specific constructor kind or exact constructor signature.
/// <para>
/// Apply this attribute to a mapping method only. When <see cref="TargetType"/> is omitted, the
/// attribute applies to the map method's return type. When <see cref="TargetType"/> is set, the
/// preference applies wherever that exact type is constructed in the mapping tree (root or nested).
/// Multiple attributes may target different types on the same method.
/// </para>
/// <para>
/// Use <see cref="ConstructorType.NoParameters"/> to force the empty-constructor path, or
/// <see cref="ConstructorType.WithParameters"/> to force a parameterized constructor. Supply
/// <see cref="ParameterTypes"/> (or a generic overload) to select an exact signature; without
/// parameter types, exactly one invokable parameterized constructor must exist for that target type.
/// </para>
/// <para>
/// Generic forms <c>MappaUseConstructorAttribute&lt;T1&gt;</c> through
/// <c>MappaUseConstructorAttribute&lt;T1, …, T8&gt;</c> are equivalent to
/// <c>[MappaUseConstructor(ConstructorType.WithParameters, typeof(T1), …)]</c> for the method
/// return type. Pass an optional <c>targetType</c> argument to apply the same signature to a nested
/// or otherwise explicit target type (equivalent to
/// <c>[MappaUseConstructor(typeof(TTarget), ConstructorType.WithParameters, typeof(T1), …)]</c>).
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MappaUseConstructorAttribute : Attribute, IMappaMethodAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaUseConstructorAttribute"/> class
    /// for the map method's return type.
    /// </summary>
    /// <param name="constructorType">The constructor kind to use.</param>
    public MappaUseConstructorAttribute(ConstructorType constructorType)
    {
        this.ConstructorType = constructorType;
        this.ParameterTypes = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaUseConstructorAttribute"/> class
    /// for the map method's return type with an exact constructor signature.
    /// </summary>
    /// <param name="constructorType">The constructor kind to use (typically <see cref="ConstructorType.WithParameters"/>).</param>
    /// <param name="parameterTypes">The constructor parameter types, in order.</param>
    public MappaUseConstructorAttribute(ConstructorType constructorType, params Type[] parameterTypes)
    {
        this.ConstructorType = constructorType;
        this.ParameterTypes = parameterTypes ?? [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaUseConstructorAttribute"/> class
    /// for a specific target type in the mapping tree.
    /// </summary>
    /// <param name="targetType">
    /// The target type whose constructor should be selected, or <see langword="null"/> for the map method's return type.
    /// </param>
    /// <param name="constructorType">The constructor kind to use.</param>
    public MappaUseConstructorAttribute(Type? targetType, ConstructorType constructorType)
    {
        this.TargetType = targetType;
        this.ConstructorType = constructorType;
        this.ParameterTypes = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaUseConstructorAttribute"/> class
    /// for a specific target type with an exact constructor signature.
    /// </summary>
    /// <param name="targetType">
    /// The target type whose constructor should be selected, or <see langword="null"/> for the map method's return type.
    /// </param>
    /// <param name="constructorType">The constructor kind to use (typically <see cref="ConstructorType.WithParameters"/>).</param>
    /// <param name="parameterTypes">The constructor parameter types, in order.</param>
    public MappaUseConstructorAttribute(Type? targetType, ConstructorType constructorType, params Type[] parameterTypes)
    {
        this.TargetType = targetType;
        this.ConstructorType = constructorType;
        this.ParameterTypes = parameterTypes ?? [];
    }

    /// <summary>
    /// Gets the target type whose constructor is selected, or <see langword="null"/> when the
    /// attribute applies to the map method's return type.
    /// </summary>
    public Type? TargetType { get; }

    /// <summary>
    /// Gets the constructor kind to use.
    /// </summary>
    public ConstructorType ConstructorType { get; }

    /// <summary>
    /// Gets the constructor parameter types when an exact signature is required; otherwise empty.
    /// </summary>
    public Type[] ParameterTypes { get; }
}

/// <summary>
/// Generic form of <see cref="MappaUseConstructorAttribute"/> that selects the constructor
/// <c>(T1)</c>. Equivalent to
/// <c>[MappaUseConstructor(targetType, ConstructorType.WithParameters, typeof(T1))]</c>
/// (omit <paramref name="targetType"/> for the map method's return type).
/// </summary>
/// <typeparam name="T1">The first constructor parameter type.</typeparam>
/// <param name="targetType">
/// The target type whose constructor should be selected, or <see langword="null"/> for the map method's return type.
/// </param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[CLSCompliant(false)]
public sealed class MappaUseConstructorAttribute<T1>(Type? targetType = null)
    : MappaUseConstructorAttribute(targetType, ConstructorType.WithParameters, typeof(T1));

/// <summary>
/// Generic form of <see cref="MappaUseConstructorAttribute"/> that selects the constructor
/// <c>(T1, T2)</c>. Equivalent to
/// <c>[MappaUseConstructor(targetType, ConstructorType.WithParameters, typeof(T1), typeof(T2))]</c>
/// (omit <paramref name="targetType"/> for the map method's return type).
/// </summary>
/// <typeparam name="T1">The first constructor parameter type.</typeparam>
/// <typeparam name="T2">The second constructor parameter type.</typeparam>
/// <param name="targetType">
/// The target type whose constructor should be selected, or <see langword="null"/> for the map method's return type.
/// </param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[CLSCompliant(false)]
public sealed class MappaUseConstructorAttribute<T1, T2>(Type? targetType = null)
    : MappaUseConstructorAttribute(targetType, ConstructorType.WithParameters, typeof(T1), typeof(T2));

/// <summary>
/// Generic form of <see cref="MappaUseConstructorAttribute"/> that selects the constructor
/// <c>(T1, T2, T3)</c>.
/// </summary>
/// <typeparam name="T1">The first constructor parameter type.</typeparam>
/// <typeparam name="T2">The second constructor parameter type.</typeparam>
/// <typeparam name="T3">The third constructor parameter type.</typeparam>
/// <param name="targetType">
/// The target type whose constructor should be selected, or <see langword="null"/> for the map method's return type.
/// </param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[CLSCompliant(false)]
public sealed class MappaUseConstructorAttribute<T1, T2, T3>(Type? targetType = null)
    : MappaUseConstructorAttribute(targetType, ConstructorType.WithParameters, typeof(T1), typeof(T2), typeof(T3));

/// <summary>
/// Generic form of <see cref="MappaUseConstructorAttribute"/> that selects the constructor
/// <c>(T1, T2, T3, T4)</c>.
/// </summary>
/// <typeparam name="T1">The first constructor parameter type.</typeparam>
/// <typeparam name="T2">The second constructor parameter type.</typeparam>
/// <typeparam name="T3">The third constructor parameter type.</typeparam>
/// <typeparam name="T4">The fourth constructor parameter type.</typeparam>
/// <param name="targetType">
/// The target type whose constructor should be selected, or <see langword="null"/> for the map method's return type.
/// </param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[CLSCompliant(false)]
public sealed class MappaUseConstructorAttribute<T1, T2, T3, T4>(Type? targetType = null)
    : MappaUseConstructorAttribute(targetType, ConstructorType.WithParameters, typeof(T1), typeof(T2), typeof(T3), typeof(T4));

/// <summary>
/// Generic form of <see cref="MappaUseConstructorAttribute"/> that selects the constructor
/// <c>(T1, T2, T3, T4, T5)</c>.
/// </summary>
/// <typeparam name="T1">The first constructor parameter type.</typeparam>
/// <typeparam name="T2">The second constructor parameter type.</typeparam>
/// <typeparam name="T3">The third constructor parameter type.</typeparam>
/// <typeparam name="T4">The fourth constructor parameter type.</typeparam>
/// <typeparam name="T5">The fifth constructor parameter type.</typeparam>
/// <param name="targetType">
/// The target type whose constructor should be selected, or <see langword="null"/> for the map method's return type.
/// </param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[CLSCompliant(false)]
public sealed class MappaUseConstructorAttribute<T1, T2, T3, T4, T5>(Type? targetType = null)
    : MappaUseConstructorAttribute(targetType, ConstructorType.WithParameters, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

/// <summary>
/// Generic form of <see cref="MappaUseConstructorAttribute"/> that selects the constructor
/// <c>(T1, T2, T3, T4, T5, T6)</c>.
/// </summary>
/// <typeparam name="T1">The first constructor parameter type.</typeparam>
/// <typeparam name="T2">The second constructor parameter type.</typeparam>
/// <typeparam name="T3">The third constructor parameter type.</typeparam>
/// <typeparam name="T4">The fourth constructor parameter type.</typeparam>
/// <typeparam name="T5">The fifth constructor parameter type.</typeparam>
/// <typeparam name="T6">The sixth constructor parameter type.</typeparam>
/// <param name="targetType">
/// The target type whose constructor should be selected, or <see langword="null"/> for the map method's return type.
/// </param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[CLSCompliant(false)]
public sealed class MappaUseConstructorAttribute<T1, T2, T3, T4, T5, T6>(Type? targetType = null)
    : MappaUseConstructorAttribute(targetType, ConstructorType.WithParameters, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

/// <summary>
/// Generic form of <see cref="MappaUseConstructorAttribute"/> that selects the constructor
/// <c>(T1, T2, T3, T4, T5, T6, T7)</c>.
/// </summary>
/// <typeparam name="T1">The first constructor parameter type.</typeparam>
/// <typeparam name="T2">The second constructor parameter type.</typeparam>
/// <typeparam name="T3">The third constructor parameter type.</typeparam>
/// <typeparam name="T4">The fourth constructor parameter type.</typeparam>
/// <typeparam name="T5">The fifth constructor parameter type.</typeparam>
/// <typeparam name="T6">The sixth constructor parameter type.</typeparam>
/// <typeparam name="T7">The seventh constructor parameter type.</typeparam>
/// <param name="targetType">
/// The target type whose constructor should be selected, or <see langword="null"/> for the map method's return type.
/// </param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[CLSCompliant(false)]
public sealed class MappaUseConstructorAttribute<T1, T2, T3, T4, T5, T6, T7>(Type? targetType = null)
    : MappaUseConstructorAttribute(targetType, ConstructorType.WithParameters, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));

/// <summary>
/// Generic form of <see cref="MappaUseConstructorAttribute"/> that selects the constructor
/// <c>(T1, T2, T3, T4, T5, T6, T7, T8)</c>.
/// </summary>
/// <typeparam name="T1">The first constructor parameter type.</typeparam>
/// <typeparam name="T2">The second constructor parameter type.</typeparam>
/// <typeparam name="T3">The third constructor parameter type.</typeparam>
/// <typeparam name="T4">The fourth constructor parameter type.</typeparam>
/// <typeparam name="T5">The fifth constructor parameter type.</typeparam>
/// <typeparam name="T6">The sixth constructor parameter type.</typeparam>
/// <typeparam name="T7">The seventh constructor parameter type.</typeparam>
/// <typeparam name="T8">The eighth constructor parameter type.</typeparam>
/// <param name="targetType">
/// The target type whose constructor should be selected, or <see langword="null"/> for the map method's return type.
/// </param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[CLSCompliant(false)]
public sealed class MappaUseConstructorAttribute<T1, T2, T3, T4, T5, T6, T7, T8>(Type? targetType = null)
    : MappaUseConstructorAttribute(targetType, ConstructorType.WithParameters, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));