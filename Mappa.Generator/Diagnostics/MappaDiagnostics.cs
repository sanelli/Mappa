// <copyright file="MappaDiagnostics.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

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
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
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
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MethodIsVoid(MethodDeclarationSyntax methodDeclarationSyntax)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MethodIsVoid,
            methodDeclarationSyntax.GetLocation(),
            methodDeclarationSyntax.Identifier.ToFullString());

    /// <summary>
    /// Diagnostic to report the fact that the method described by syntax
    /// <paramref name="methodDeclarationSyntax"/> returns either <see cref="Task"/>,
    /// or <see cref="Task{T}"/>, or <see cref="ValueTask"/>, or <see cref="ValueTask{T}"/>.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MethodReturnsTaskType(MethodDeclarationSyntax methodDeclarationSyntax)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MethodReturnsTaskType,
            methodDeclarationSyntax.GetLocation(),
            methodDeclarationSyntax.Identifier.ToFullString());

    /// <summary>
    /// Diagnostic to report the fact that a method cannot be generated because a
    /// mapping between two types has already been defined.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic DuplicatedMapping(MethodDeclarationSyntax methodDeclarationSyntax)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.DuplicatedMapping,
            methodDeclarationSyntax.GetLocation(),
            methodDeclarationSyntax.Identifier.ToFullString(),
            methodDeclarationSyntax.ParameterList.Parameters.First().Type?.ToFullString() ?? "unknown",
            methodDeclarationSyntax.ReturnType.ToFullString());

    /// <summary>
    /// Diagnostic to report the fact that a mapping cannot be identifier.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="location">The location of the mapping.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic CannotIdentifyStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        Location? location)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.CannotIdentifyStrategy,
            location,
            sourceType.ToDisplayString(),
            targetType.ToDisplayString());

    /// <summary>
    /// Diagnostic to report the fact that multiple attributes are targeting the same property.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="propertyOrParameterName">The name of the property or constructor parameter for which multiple mapping attributes have been defined.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MultipleAttributesTargetTheSamePropertyOrParameter(MethodDeclarationSyntax methodDeclarationSyntax, string propertyOrParameterName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MultipleAttributesTargetTheSamePropertyOrParameter,
            methodDeclarationSyntax.GetLocation(),
            methodDeclarationSyntax.Identifier.ToFullString(),
            propertyOrParameterName);

    /// <summary>
    /// Diagnostic to report the fact that it is not possible identify a suitable method to invoke.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="propertyOrParameterName">The name of the property or constructor parameter for which multiple mapping attributes have been defined.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="typeName">The type on which the method is being looked for.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic CannotDetectSuitableMethodToInvoke(
        MethodDeclarationSyntax methodDeclarationSyntax,
        string propertyOrParameterName,
        string methodName,
        string typeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.CannotDetectSuitableMethodToInvoke,
            methodDeclarationSyntax.GetLocation(),
            methodName,
            typeName,
            propertyOrParameterName);

    /// <summary>
    /// Diagnostic to report the fact that it is not possible identify a suitable method to invoke.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="type">The type that cannot be found.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic CannotDetectType(
        MethodDeclarationSyntax methodDeclarationSyntax,
        Type type)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.CannotDetectType,
            methodDeclarationSyntax.GetLocation(),
            type);
}