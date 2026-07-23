// <copyright file="MappaMapEnumDefaultAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Configures the fallback behaviour when a value of <typeparamref name="TEnum"/> cannot be mapped
/// (for example because it was excluded via <see cref="MappaMapEnumIgnoreAttribute{TEnum}"/>
/// or because no pairing exists for that member).
/// <para>
/// Multiple instances may be applied when mapping classes or structs that contain several enum
/// properties, provided each instance targets a distinct <typeparamref name="TEnum"/>.
/// On a mapping method whose source or return type is itself an enum, at most one
/// <see cref="MappaMapEnumDefaultAttribute{TEnum}"/> may be present.
/// </para>
/// </summary>
/// <typeparam name="TEnum">The enum type whose fallback behaviour is being configured.</typeparam>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[CLSCompliant(false)]
public sealed class MappaMapEnumDefaultAttribute<TEnum> : Attribute
    where TEnum : struct, Enum
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaMapEnumDefaultAttribute{TEnum}"/> class
    /// without a fallback value. Use with <see cref="MappaMapEnumDefaultBehavior.Throw"/>, or provide
    /// a default value via another constructor when using <see cref="MappaMapEnumDefaultBehavior.UseDefaultValue"/>.
    /// </summary>
    /// <param name="behavior">The fallback behaviour.</param>
    public MappaMapEnumDefaultAttribute(MappaMapEnumDefaultBehavior behavior)
    {
        this.Behavior = behavior;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaMapEnumDefaultAttribute{TEnum}"/> class
    /// with an enum fallback value. Valid when the mapping target type is <typeparamref name="TEnum"/>.
    /// </summary>
    /// <param name="behavior">The fallback behaviour.</param>
    /// <param name="enumDefaultValue">The enum value to use when <paramref name="behavior"/> is <see cref="MappaMapEnumDefaultBehavior.UseDefaultValue"/>.</param>
    public MappaMapEnumDefaultAttribute(MappaMapEnumDefaultBehavior behavior, TEnum enumDefaultValue)
    {
        this.Behavior = behavior;
        this.EnumDefaultValue = enumDefaultValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaMapEnumDefaultAttribute{TEnum}"/> class
    /// with an integral fallback value. Valid when the mapping target type is an integral type.
    /// </summary>
    /// <param name="behavior">The fallback behaviour.</param>
    /// <param name="integerDefaultValue">The integral value to use when <paramref name="behavior"/> is <see cref="MappaMapEnumDefaultBehavior.UseDefaultValue"/>.</param>
    public MappaMapEnumDefaultAttribute(MappaMapEnumDefaultBehavior behavior, int integerDefaultValue)
    {
        this.Behavior = behavior;
        this.IntegerDefaultValue = integerDefaultValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaMapEnumDefaultAttribute{TEnum}"/> class
    /// with a string fallback value. Valid when the mapping target type is <see cref="string"/>.
    /// </summary>
    /// <param name="behavior">The fallback behaviour.</param>
    /// <param name="stringDefaultValue">The string value to use when <paramref name="behavior"/> is <see cref="MappaMapEnumDefaultBehavior.UseDefaultValue"/>.</param>
    public MappaMapEnumDefaultAttribute(MappaMapEnumDefaultBehavior behavior, string stringDefaultValue)
    {
        this.Behavior = behavior;
        this.StringDefaultValue = stringDefaultValue;
    }

    /// <summary>
    /// Gets the fallback behaviour.
    /// </summary>
    public MappaMapEnumDefaultBehavior Behavior { get; }

    /// <summary>
    /// Gets the enum fallback value when the target type is <typeparamref name="TEnum"/>,
    /// or <see langword="null"/> when no enum default was provided.
    /// </summary>
    public TEnum? EnumDefaultValue { get; }

    /// <summary>
    /// Gets the integral fallback value when the target type is an integral type,
    /// or <see langword="null"/> when no integral default was provided.
    /// </summary>
    public int? IntegerDefaultValue { get; }

    /// <summary>
    /// Gets the string fallback value when the target type is <see cref="string"/>,
    /// or <see langword="null"/> when no string default was provided.
    /// </summary>
    public string? StringDefaultValue { get; }

    /// <summary>
    /// Gets a value indicating whether a fallback default value was provided.
    /// </summary>
    public bool HasDefaultValue =>
        this.EnumDefaultValue.HasValue
        || this.IntegerDefaultValue.HasValue
        || this.StringDefaultValue is not null;
}