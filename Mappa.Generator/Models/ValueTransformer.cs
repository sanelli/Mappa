// <copyright file="ValueTransformer.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describes a resolved value transformer: a <c>T → T</c> method applied around mapping sites.
/// </summary>
internal sealed class ValueTransformer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueTransformer"/> class.
    /// </summary>
    /// <param name="method">The transformer method.</param>
    /// <param name="fieldOrProperty">The mapper field or property used to locate the transformer, if any.</param>
    /// <param name="explicitType">The explicit type declaring the static transformer, if any.</param>
    /// <param name="sourcePropertyName">The source property name or dotted path restricting the transformer, if any.</param>
    /// <param name="attributeLocation">The location of the attribute that introduced the transformer, if any.</param>
    internal ValueTransformer(
        IMethodSymbol method,
        ISymbol? fieldOrProperty,
        ITypeSymbol? explicitType,
        string? sourcePropertyName,
        Location? attributeLocation)
    {
        this.Method = method;
        this.FieldOrProperty = fieldOrProperty;
        this.ExplicitType = explicitType;
        this.SourcePropertyName = sourcePropertyName;
        this.AttributeLocation = attributeLocation;
    }

    /// <summary>
    /// Gets the transformer method.
    /// </summary>
    internal IMethodSymbol Method { get; }

    /// <summary>
    /// Gets the mapper field or property used to locate the transformer, if any.
    /// </summary>
    internal ISymbol? FieldOrProperty { get; }

    /// <summary>
    /// Gets the explicit type declaring the static transformer, if any.
    /// </summary>
    internal ITypeSymbol? ExplicitType { get; }

    /// <summary>
    /// Gets the source property name or dotted path restricting the transformer, if any.
    /// When <c>null</c> the transformer applies to every matching site of the map method.
    /// </summary>
    internal string? SourcePropertyName { get; }

    /// <summary>
    /// Gets the location of the attribute that introduced the transformer, if any.
    /// </summary>
    internal Location? AttributeLocation { get; }

    /// <summary>
    /// Gets a value indicating whether the transformer receives the <c>MappaContext</c> as second argument.
    /// </summary>
    internal bool RequiresContext => this.Method.Parameters.Length == 2;

    /// <summary>
    /// Gets the type of a field or property used to locate transformers.
    /// </summary>
    /// <param name="fieldOrProperty">The field or property symbol.</param>
    /// <returns>The type of the field or property.</returns>
    internal static ITypeSymbol GetFieldOrPropertyType(ISymbol fieldOrProperty)
        => fieldOrProperty is IFieldSymbol field
            ? field.Type
            : ((IPropertySymbol)fieldOrProperty).Type;

    /// <summary>
    /// Determines whether <paramref name="other"/> invokes the very same method through the very same accessor.
    /// </summary>
    /// <param name="other">The other transformer.</param>
    /// <returns><c>true</c> when both transformers are invoked identically.</returns>
    internal bool IsSameAs(ValueTransformer other)
        => SymbolEqualityComparer.Default.Equals(this.Method.OriginalDefinition, other.Method.OriginalDefinition)
           && SymbolEqualityComparer.Default.Equals(this.FieldOrProperty, other.FieldOrProperty)
           && SymbolEqualityComparer.Default.Equals(this.ExplicitType, other.ExplicitType);
}