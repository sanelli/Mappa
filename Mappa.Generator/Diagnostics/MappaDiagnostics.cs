// <copyright file="MappaDiagnostics.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Diagnostics;

/// <summary>
/// Diagnostics reported by the the Mappa generator.
/// </summary>
internal static class MappaDiagnostics
{
    /// <summary>
    /// Diagnostic to report the fact that the method described by syntax
    /// <paramref name="methodDeclarationSyntax"/> has an invalid number of
    /// parameters.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method with the incorrect number of parameters.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MethodHasInvalidNumberOfParameters(MethodDeclarationSyntax methodDeclarationSyntax)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MethodHasInvalidNumberOfParameters,
            methodDeclarationSyntax.GetLocation(),
            methodDeclarationSyntax.Identifier.ToFullString());

    /// <summary>
    /// Diagnostic to report the fact that the method described by syntax
    /// <paramref name="methodDeclarationSyntax"/> returns <c>void</c>.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method with the incorrect number of parameters.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MethodIsVoid(MethodDeclarationSyntax methodDeclarationSyntax)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MethodIsVoid,
            methodDeclarationSyntax.GetLocation(),
            methodDeclarationSyntax.Identifier.ToFullString());
}