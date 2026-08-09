// <copyright file="ClassDeclarationSyntaxExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

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
    /// <param name="classDeclarationSyntax">The class declaration syntax.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="classDeclarationSyntax"/> is <c>partial</c> class,
    /// <c>false</c> otherwise.
    /// </returns>
    internal static bool IsPartial(this ClassDeclarationSyntax classDeclarationSyntax)
        => classDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);
}