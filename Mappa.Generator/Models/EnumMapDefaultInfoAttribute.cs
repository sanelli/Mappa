// <copyright file="EnumMapDefaultInfoAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describes a <see cref="MappaMapEnumDefaultAttribute{TEnum}"/> declaration parsed from Roslyn symbols.
/// </summary>
/// <remarks>
/// Generic attributes cannot be reconstructed as CLR instances by the generator therefore the parsed
/// data is carried by this type which still derives from <see cref="Attribute"/> so that it can be
/// stored alongside the other mapping attributes of a <see cref="MapMethod"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
internal sealed class EnumMapDefaultInfoAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumMapDefaultInfoAttribute"/> class.
    /// </summary>
    /// <param name="enumType">The enum type declared as type argument.</param>
    /// <param name="behavior">The configured fallback behaviour.</param>
    /// <param name="enumDefaultMemberName">The name of the fallback member of <paramref name="enumType"/>, when the enum constructor was used.</param>
    /// <param name="integerDefaultValue">The fallback integral value, when the integral constructor was used.</param>
    /// <param name="stringDefaultValue">The fallback string value, when the string constructor was used.</param>
    internal EnumMapDefaultInfoAttribute(
        INamedTypeSymbol enumType,
        MappaMapEnumDefaultBehavior behavior,
        string? enumDefaultMemberName,
        int? integerDefaultValue,
        string? stringDefaultValue)
    {
        this.EnumType = enumType;
        this.Behavior = behavior;
        this.EnumDefaultMemberName = enumDefaultMemberName;
        this.IntegerDefaultValue = integerDefaultValue;
        this.StringDefaultValue = stringDefaultValue;
    }

    /// <summary>
    /// Gets the enum type declared as type argument.
    /// </summary>
    internal INamedTypeSymbol EnumType { get; }

    /// <summary>
    /// Gets the configured fallback behaviour.
    /// </summary>
    internal MappaMapEnumDefaultBehavior Behavior { get; }

    /// <summary>
    /// Gets the name of the fallback member of <see cref="EnumType"/>,
    /// or <c>null</c> when the enum constructor was not used.
    /// </summary>
    internal string? EnumDefaultMemberName { get; }

    /// <summary>
    /// Gets the fallback integral value, or <c>null</c> when the integral constructor was not used.
    /// </summary>
    internal int? IntegerDefaultValue { get; }

    /// <summary>
    /// Gets the fallback string value, or <c>null</c> when the string constructor was not used.
    /// </summary>
    internal string? StringDefaultValue { get; }

    /// <summary>
    /// Gets a value indicating whether a fallback default value was provided.
    /// </summary>
    internal bool HasDefaultValue =>
        this.EnumDefaultMemberName is not null
        || this.IntegerDefaultValue.HasValue
        || this.StringDefaultValue is not null;
}