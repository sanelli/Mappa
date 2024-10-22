// <copyright file="ClassDeclarationSyntaxExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="ClassDeclarationSyntax"/>.
/// </summary>
internal static class ClassDeclarationSyntaxExtensions
{
    /// <summary>
    /// Returns <c>true</c> if the <paramref name="classDeclarationSyntax"/> references
    /// a partial class.
    /// </summary>
    /// <param name="classDeclarationSyntax">The class decalaration syntax.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="classDeclarationSyntax"/> is <c>partial</c> class,
    /// <c>false</c> otherwise.
    /// </returns>
    internal static bool IsPartial(this ClassDeclarationSyntax classDeclarationSyntax)
        => classDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);

    /// <summary>
    /// Obtain the <see cref="MappaAttribute"/>.
    /// </summary>
    /// <param name="classDeclarationSyntax">The class syntax to query for the list of attributes.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="MappaAttribute"/> attribute, or <c>null</c> if the attribute does not exist.</returns>
    internal static AttributeSyntax? GetMappaAttribute(this ClassDeclarationSyntax classDeclarationSyntax, SemanticModel semanticModel, CancellationToken cancellationToken)
        => classDeclarationSyntax.AttributeLists.GetMappaAttributeSyntax(semanticModel, cancellationToken);

    /// <summary>
    /// Check if the class contains the <see cref="MappaAttribute"/>.
    /// </summary>
    /// <param name="classDeclarationSyntax">The class syntax to query for the list of attributes.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if the class contains the <see cref="MappaAttribute"/>, <c>false</c> otherwise.</returns>
    internal static bool HasMappaAttribute(this ClassDeclarationSyntax classDeclarationSyntax, SemanticModel semanticModel, CancellationToken cancellationToken)
        => classDeclarationSyntax.GetMappaAttribute(semanticModel, cancellationToken) is not null;
}