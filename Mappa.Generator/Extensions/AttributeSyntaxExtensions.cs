// <copyright file="AttributeSyntaxExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="AttributeSyntax"/>
/// and <see cref="AttributeListSyntax"/>.
/// </summary>
internal static class AttributeSyntaxExtensions
{
    /// <summary>
    /// Obtain the <see cref="MappaAttribute"/>.
    /// </summary>
    /// <param name="attributeLists">The attributes list to query.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="MappaAttribute"/> attribute, or <c>null</c> if the attribute does not exist.</returns>
    internal static AttributeSyntax? GetMappaAttributeSyntax(this SyntaxList<AttributeListSyntax> attributeLists, SemanticModel semanticModel, CancellationToken cancellationToken)
        => attributeLists.GetAttributes<MappaAttribute>(semanticModel, cancellationToken).SingleOrDefault();

    /// <summary>
    /// Obtain the <see cref="MappaDependencyAttribute"/>.
    /// </summary>
    /// <param name="attributeLists">The attributes list to query.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="MappaDependencyAttribute"/> attribute, or <c>null</c> if the attribute does not exist.</returns>
    internal static AttributeSyntax? GetMappaDependencyAttributeSyntax(this SyntaxList<AttributeListSyntax> attributeLists, SemanticModel semanticModel, CancellationToken cancellationToken)
        => attributeLists.GetAttributes<MappaDependencyAttribute>(semanticModel, cancellationToken).SingleOrDefault();

    /// <summary>
    /// Obtain the attributes with the specified type.
    /// </summary>
    /// <param name="attributeLists">The attributes list to query.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <returns>The attributes of type <typeparamref name="TAttribute"/>.</returns>
    private static List<AttributeSyntax> GetAttributes<TAttribute>(this SyntaxList<AttributeListSyntax> attributeLists, SemanticModel semanticModel, CancellationToken cancellationToken)
        where TAttribute : Attribute
    {
        var attributeTypeFullName = typeof(TAttribute).FullName ?? throw new ArgumentException($"Cannot obtain {nameof(Type.FullName)} for type '{typeof(TAttribute)}'");
        List<AttributeSyntax> attributes = new();
        foreach (var attributeSyntax in attributeLists.SelectMany(attributeList => attributeList.Attributes))
        {
            if (semanticModel.GetSymbolInfo(attributeSyntax, cancellationToken).Symbol is IMethodSymbol symbolInfo)
            {
                var attributeTypeName = symbolInfo.ContainingType.ToDisplayString();
                if (attributeTypeFullName.Equals(attributeTypeName, StringComparison.Ordinal))
                {
                    attributes.Add(attributeSyntax);
                }
            }
        }

        return attributes;
    }
}