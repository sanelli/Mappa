// <copyright file="MappaMapEnumIgnoreAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Excludes a specific member of <typeparamref name="TEnum"/> from enum mapping.
/// <para>
/// When the excluded member is encountered at runtime, the fallback configured by
/// <see cref="MappaMapEnumDefaultAttribute{TEnum}"/> is applied (throw by default).
/// </para>
/// <para>
/// Multiple instances may be applied to the same method. When the map method maps classes or structs,
/// the attribute is used while mapping nested enum properties whose type is <typeparamref name="TEnum"/>.
/// </para>
/// </summary>
/// <typeparam name="TEnum">The enum type being configured.</typeparam>
/// <param name="enumValue">The enum member to exclude from mapping.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[CLSCompliant(false)]
public sealed class MappaMapEnumIgnoreAttribute<TEnum>(TEnum enumValue) : Attribute
    where TEnum : struct, Enum
{
    /// <summary>
    /// Gets the enum member excluded from mapping.
    /// </summary>
    public TEnum EnumValue { get; } = enumValue;
}