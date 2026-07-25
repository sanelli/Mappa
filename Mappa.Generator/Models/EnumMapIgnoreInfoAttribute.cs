// <copyright file="EnumMapIgnoreInfoAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describes a <see cref="MappaMapEnumIgnoreAttribute{TEnum}"/> declaration parsed from Roslyn symbols.
/// </summary>
/// <remarks>
/// Generic attributes cannot be reconstructed as CLR instances by the generator therefore the parsed
/// data is carried by this type which still derives from <see cref="Attribute"/> so that it can be
/// stored alongside the other mapping attributes of a <see cref="MapMethod"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
internal sealed class EnumMapIgnoreInfoAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumMapIgnoreInfoAttribute"/> class.
    /// </summary>
    /// <param name="enumType">The enum type declared as type argument.</param>
    /// <param name="enumMemberName">The name of the excluded member of <paramref name="enumType"/>.</param>
    internal EnumMapIgnoreInfoAttribute(INamedTypeSymbol enumType, string enumMemberName)
    {
        this.EnumType = enumType;
        this.EnumMemberName = enumMemberName;
    }

    /// <summary>
    /// Gets the enum type declared as type argument.
    /// </summary>
    internal INamedTypeSymbol EnumType { get; }

    /// <summary>
    /// Gets the name of the excluded member of <see cref="EnumType"/>.
    /// </summary>
    internal string EnumMemberName { get; }
}