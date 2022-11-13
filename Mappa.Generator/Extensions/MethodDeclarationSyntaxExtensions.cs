// <copyright file="MethodDeclarationSyntaxExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="MethodDeclarationSyntax"/>.
/// </summary>
internal static class MethodDeclarationSyntaxExtensions
{
    /// <summary>
    /// Check if a method is static.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <returns><c>true</c> is the method is static.</returns>
    internal static bool IsStatic(this MethodDeclarationSyntax methodDeclarationSyntax)
        => methodDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword);

    /// <summary>
    /// Check if a method is partial.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <returns><c>true</c> is the method is partial.</returns>
    internal static bool IsPartial(this MethodDeclarationSyntax methodDeclarationSyntax)
        => methodDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);

    /// <summary>
    /// Check method has the given number of parameters.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="arity">The number of parameters.</param>
    /// <returns><c>true</c> is the method is partial.</returns>
    internal static bool HasArity(this MethodDeclarationSyntax methodDeclarationSyntax, int arity)
        => methodDeclarationSyntax.ParameterList.Parameters.Count == arity;
}