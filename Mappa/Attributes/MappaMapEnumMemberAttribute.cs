// <copyright file="MappaMapEnumMemberAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type. One- and two-type-parameter overloads share this file by design.

namespace Mappa.Attributes;

/// <summary>
/// Configures an explicit mapping between a member of <typeparamref name="TEnum"/> and either
/// an integral value or a string value.
/// <para>
/// The attribute is applied to a mapping method and is bidirectional: the same declaration is used
/// when mapping from <typeparamref name="TEnum"/> to the paired value and from the paired value to
/// <typeparamref name="TEnum"/>.
/// </para>
/// <para>
/// Multiple instances may be applied to the same method. When the map method maps classes or structs,
/// the attribute is used while mapping nested enum properties whose type is <typeparamref name="TEnum"/>.
/// </para>
/// </summary>
/// <typeparam name="TEnum">The enum type being configured.</typeparam>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[CLSCompliant(false)]
public sealed class MappaMapEnumMemberAttribute<TEnum> : Attribute
    where TEnum : struct, Enum
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaMapEnumMemberAttribute{TEnum}"/> class
    /// that pairs an enum member with an integral value.
    /// </summary>
    /// <param name="enumValue">The enum member.</param>
    /// <param name="integerValue">The integral value paired with <paramref name="enumValue"/>.</param>
    public MappaMapEnumMemberAttribute(TEnum enumValue, int integerValue)
    {
        this.EnumValue = enumValue;
        this.IntegerValue = integerValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaMapEnumMemberAttribute{TEnum}"/> class
    /// that pairs an enum member with a string value.
    /// </summary>
    /// <param name="enumValue">The enum member.</param>
    /// <param name="stringValue">The string value paired with <paramref name="enumValue"/>.</param>
    public MappaMapEnumMemberAttribute(TEnum enumValue, string stringValue)
    {
        this.EnumValue = enumValue;
        this.StringValue = stringValue;
    }

    /// <summary>
    /// Gets the enum member being mapped.
    /// </summary>
    public TEnum EnumValue { get; }

    /// <summary>
    /// Gets the integral value paired with <see cref="EnumValue"/>, or <see langword="null"/>
    /// when this attribute was constructed with a string pairing.
    /// </summary>
    public int? IntegerValue { get; }

    /// <summary>
    /// Gets the string value paired with <see cref="EnumValue"/>, or <see langword="null"/>
    /// when this attribute was constructed with an integral pairing.
    /// </summary>
    public string? StringValue { get; }
}

/// <summary>
/// Configures an explicit mapping between a member of <typeparamref name="TEnum"/> and a member of
/// <typeparamref name="TOtherEnum"/>.
/// <para>
/// The attribute is applied to a mapping method and is bidirectional: the same declaration is used
/// when mapping from <typeparamref name="TEnum"/> to <typeparamref name="TOtherEnum"/> and in the
/// opposite direction.
/// </para>
/// <para>
/// Multiple instances may be applied to the same method. When the map method maps classes or structs,
/// the attribute is used while mapping nested enum properties involving these enum types.
/// </para>
/// </summary>
/// <typeparam name="TEnum">The first enum type being configured.</typeparam>
/// <typeparam name="TOtherEnum">The second enum type being configured.</typeparam>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[CLSCompliant(false)]
public sealed class MappaMapEnumMemberAttribute<TEnum, TOtherEnum> : Attribute
    where TEnum : struct, Enum
    where TOtherEnum : struct, Enum
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaMapEnumMemberAttribute{TEnum, TOtherEnum}"/> class.
    /// </summary>
    /// <param name="enumValue">The member of <typeparamref name="TEnum"/>.</param>
    /// <param name="otherEnumValue">The member of <typeparamref name="TOtherEnum"/> paired with <paramref name="enumValue"/>.</param>
    public MappaMapEnumMemberAttribute(TEnum enumValue, TOtherEnum otherEnumValue)
    {
        this.EnumValue = enumValue;
        this.OtherEnumValue = otherEnumValue;
    }

    /// <summary>
    /// Gets the member of <typeparamref name="TEnum"/> being mapped.
    /// </summary>
    public TEnum EnumValue { get; }

    /// <summary>
    /// Gets the member of <typeparamref name="TOtherEnum"/> paired with <see cref="EnumValue"/>.
    /// </summary>
    public TOtherEnum OtherEnumValue { get; }
}