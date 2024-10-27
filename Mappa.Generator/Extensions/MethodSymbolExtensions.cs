// <copyright file="MethodSymbolExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="IMethodSymbol"/>.
/// </summary>
internal static class MethodSymbolExtensions
{
    private static readonly string MappaContextTypeFullName = typeof(MappaContext).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} from {typeof(MappaContext)}.");

    /// <summary>
    /// Returns <c>true</c> if the method returns <c>void</c>.
    /// </summary>
    /// <param name="methodSymbol">The method to validate.</param>
    /// <returns><c>true</c> if the method returns <c>void</c>.</returns>
    internal static bool IsVoid(this IMethodSymbol methodSymbol)
        => methodSymbol.ReturnType.IsVoid();

    /// <summary>
    /// Returns <c>true</c> if the method returns either <see cref="Task"/>,
    /// <see cref="Task{T}"/>, <see cref="ValueTask"/> or <see cref="ValueTask{TResult}"/>.
    /// </summary>
    /// <param name="methodSymbol">The method to validate.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the method returns <c>void</c>.</returns>
    internal static bool ReturnsAnyTaskType(this IMethodSymbol methodSymbol, Compilation compilation)
        => methodSymbol.ReturnType.IsAnyTaskType(compilation);

    /// <summary>
    /// Return a list of non-inherited attributes applied to the method.
    /// </summary>
    /// <param name="methodSymbol">The method symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>A list of mappa attributes applied to the method that can impact the mapping.</returns>
    internal static Attribute[] GetMethodMappaAttributes(this IMethodSymbol methodSymbol, Compilation compilation)
    {
        var result = new List<Attribute>();

        // Mappa Invoke Method Attributes
        var invokeMethodAttributes = methodSymbol.GetInvokeMethodAttributes(compilation);
        result.AddRange(invokeMethodAttributes);

        // Mappa Assign From Context Attributes
        var assignFromContextAttributes = methodSymbol.GetMappaAssignFromContextAttributes(compilation);
        result.AddRange(assignFromContextAttributes);

        // TODO [#56] Allow to read the MappaSettingsAttribute.

        // All done.
        return [.. result];
    }

    /// <summary>
    /// Returns <c>true</c> if the second parameter of the method is
    /// of type <see cref="MappaContext"/>.
    /// </summary>
    /// <param name="methodSymbol">The method to validate.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the second parameter of the method is
    /// of type <see cref="MappaContext"/>, <c>false</c> otherwise.</returns>
    internal static bool SecondParameterIsMappaContext(
        this IMethodSymbol methodSymbol,
        Compilation compilation)
    {
        var secondParameterType = methodSymbol.Parameters[1].Type;
        var mappaContextType = compilation.GetTypeByMetadataName(MappaContextTypeFullName);
        return SymbolEqualityComparer.Default.Equals(mappaContextType, secondParameterType);
    }
}