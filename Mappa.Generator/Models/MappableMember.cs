// <copyright file="MappableMember.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// A property or field that can participate in member mapping.
/// </summary>
internal sealed class MappableMember
{
    private MappableMember(ISymbol symbol, ITypeSymbol type, string name, bool isProperty)
    {
        this.Symbol = symbol;
        this.Type = type;
        this.Name = name;
        this.IsProperty = isProperty;
    }

    /// <summary>
    /// Gets the underlying Roslyn symbol.
    /// </summary>
    internal ISymbol Symbol { get; }

    /// <summary>
    /// Gets the member type.
    /// </summary>
    internal ITypeSymbol Type { get; }

    /// <summary>
    /// Gets the mappable member name used for pairing.
    /// </summary>
    internal string Name { get; }

    /// <summary>
    /// Gets a value indicating whether this member is a property.
    /// </summary>
    internal bool IsProperty { get; }

    /// <summary>
    /// Gets a value indicating whether this member is a field.
    /// </summary>
    internal bool IsField => !this.IsProperty;

    /// <summary>
    /// Gets the property when <see cref="IsProperty"/> is <c>true</c>; otherwise <c>null</c>.
    /// </summary>
    internal IPropertySymbol? Property => this.Symbol as IPropertySymbol;

    /// <summary>
    /// Gets the field when <see cref="IsField"/> is <c>true</c>; otherwise <c>null</c>.
    /// </summary>
    internal IFieldSymbol? Field => this.Symbol as IFieldSymbol;

    /// <summary>
    /// Creates a mappable member from a property.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="name">The mappable name.</param>
    /// <returns>The mappable member.</returns>
    internal static MappableMember FromProperty(IPropertySymbol property, string name)
        => new(property, property.Type, name, isProperty: true);

    /// <summary>
    /// Creates a mappable member from a field.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>The mappable member.</returns>
    internal static MappableMember FromField(IFieldSymbol field)
        => new(field, field.Type, field.Name, isProperty: false);
}