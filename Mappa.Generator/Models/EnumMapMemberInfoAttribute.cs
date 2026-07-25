// <copyright file="EnumMapMemberInfoAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describes a <see cref="MappaMapEnumMemberAttribute{TEnum}"/> or
/// <see cref="MappaMapEnumMemberAttribute{TEnum, TOtherEnum}"/> declaration parsed from Roslyn symbols.
/// </summary>
/// <remarks>
/// Generic attributes cannot be reconstructed as CLR instances by the generator therefore the parsed
/// data is carried by this type which still derives from <see cref="Attribute"/> so that it can be
/// stored alongside the other mapping attributes of a <see cref="MapMethod"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
internal sealed class EnumMapMemberInfoAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumMapMemberInfoAttribute"/> class.
    /// </summary>
    /// <param name="enumType">The enum type declared as first type argument.</param>
    /// <param name="enumMemberName">The name of the member of <paramref name="enumType"/> being configured.</param>
    /// <param name="integerValue">The paired integral value, when the integral constructor was used.</param>
    /// <param name="stringValue">The paired string value, when the string constructor was used.</param>
    /// <param name="otherEnumType">The enum type declared as second type argument, when present.</param>
    /// <param name="otherEnumMemberName">The name of the member of <paramref name="otherEnumType"/>, when present.</param>
    internal EnumMapMemberInfoAttribute(
        INamedTypeSymbol enumType,
        string enumMemberName,
        int? integerValue,
        string? stringValue,
        INamedTypeSymbol? otherEnumType,
        string? otherEnumMemberName)
    {
        this.EnumType = enumType;
        this.EnumMemberName = enumMemberName;
        this.IntegerValue = integerValue;
        this.StringValue = stringValue;
        this.OtherEnumType = otherEnumType;
        this.OtherEnumMemberName = otherEnumMemberName;
    }

    /// <summary>
    /// Gets the enum type declared as first type argument.
    /// </summary>
    internal INamedTypeSymbol EnumType { get; }

    /// <summary>
    /// Gets the name of the member of <see cref="EnumType"/> being configured.
    /// </summary>
    internal string EnumMemberName { get; }

    /// <summary>
    /// Gets the paired integral value, or <c>null</c> when the integral constructor was not used.
    /// </summary>
    internal int? IntegerValue { get; }

    /// <summary>
    /// Gets the paired string value, or <c>null</c> when the string constructor was not used.
    /// </summary>
    internal string? StringValue { get; }

    /// <summary>
    /// Gets the enum type declared as second type argument, or <c>null</c> when the attribute has a single type argument.
    /// </summary>
    internal INamedTypeSymbol? OtherEnumType { get; }

    /// <summary>
    /// Gets the name of the member of <see cref="OtherEnumType"/>, or <c>null</c> when the attribute has a single type argument.
    /// </summary>
    internal string? OtherEnumMemberName { get; }
}