// <copyright file="MethodSymbolExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Models;

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
        var attributeDatas = methodSymbol.GetAttributes();

        // Mappa Invoke Method Attributes
        var invokeMethodAttributes = attributeDatas.GetInvokeMethodAttributes(compilation);
        result.AddRange(invokeMethodAttributes);

        // Mappa Assign From Context Attributes
        var assignFromContextAttributes = attributeDatas.GetMappaAssignFromContextAttributes(compilation);
        result.AddRange(assignFromContextAttributes);

        // Mappa Setting Attribute
        if (attributeDatas.GetMappaSettingsAttribute(compilation) is { } mappaSettingAttribute)
        {
            result.Add(mappaSettingAttribute);
        }

        // Mappa Assign From Context Attributes
        var usePropertyAttributes = attributeDatas.GetMappaUsePropertyAttributes(compilation);
        result.AddRange(usePropertyAttributes);

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
        this IMethodSymbol? methodSymbol,
        Compilation compilation)
    {
        if (methodSymbol is null)
        {
            return true;
        }

        var secondParameterType = methodSymbol.Parameters[1].Type;
        var mappaContextType = compilation.GetTypeByMetadataName(MappaContextTypeFullName);
        return SymbolEqualityComparer.Default.Equals(mappaContextType, secondParameterType);
    }

    /// <summary>
    /// Check if a method can be invoked from inside another method.
    /// </summary>
    /// <param name="calleeMethod">The symbol of the method being invoked.</param>
    /// <param name="callerMethod">The method that is calling <paramref name="calleeMethod"/>.</param>
    /// <returns><c>true</c> if <paramref name="calleeMethod"/> can be invoked by <paramref name="callerMethod"/>, <c>false</c> otherwise.</returns>
    internal static bool IsAccessibleFromMethod(this IMethodSymbol calleeMethod, IMethodSymbol callerMethod)
    {
        return calleeMethod.DeclaredAccessibility switch
        {
            Accessibility.Public => true,
            Accessibility.Internal => calleeMethod.ContainingAssembly.Equals(callerMethod.ContainingAssembly, SymbolEqualityComparer.Default),
            _ => false,
        };
    }

    /// <summary>
    /// Check if a method can be accessed directly or can be accessed
    /// via explicit interface.
    /// </summary>
    /// <param name="targetTypeSymbol">The type containing the method.</param>
    /// <param name="methodName">The name of the method (e.g. <c>"Add"</c>).</param>
    /// <param name="fullInterfaceName">The full name interface (e.g. <c>"System.Collections.Generic.ICollection"</c>).</param>
    /// <param name="elementTypeName">The type of the element.</param>
    /// <param name="returnTypeCheck">The check on the return type of the method.</param>
    /// <param name="parameterTypes">The list of type of parameters.</param>
    /// <returns>An enumeration indicating how the method can be accessed.</returns>
    internal static InterfaceMethodAccessMode GetInterfaceMethodAccessMode(
        this ITypeSymbol targetTypeSymbol,
        string methodName,
        string fullInterfaceName,
        string elementTypeName,
        Predicate<ITypeSymbol> returnTypeCheck,
        ITypeSymbol[] parameterTypes)
    {
        // Look up for accessible method in the type and its hierarchy.
        var currentSymbol = targetTypeSymbol;
        while (currentSymbol is not null)
        {
            if (HasExpectedMethod(currentSymbol, methodName))
            {
                return InterfaceMethodAccessMode.Direct;
            }

            currentSymbol = currentSymbol.BaseType;
        }

        string explicitName = $"{fullInterfaceName}<{elementTypeName}>.{methodName}";
        currentSymbol = targetTypeSymbol;
        while (currentSymbol is not null)
        {
            if (HasExpectedMethod(currentSymbol, explicitName))
            {
                return InterfaceMethodAccessMode.InterfaceExplicit;
            }

            currentSymbol = currentSymbol.BaseType;
        }

        // Look up for the generic variant of the method name.
        if (targetTypeSymbol is INamedTypeSymbol
            { OriginalDefinition.TypeArguments.Length: > 0 } namedTypeSymbol)
        {
            var typeArgumentName = namedTypeSymbol.OriginalDefinition.TypeArguments[0].Name;
            string genericName = $"{fullInterfaceName}<{typeArgumentName}>.{methodName}";
            currentSymbol = targetTypeSymbol;
            while (currentSymbol is not null)
            {
                if (HasExpectedMethod(currentSymbol, genericName))
                {
                    return InterfaceMethodAccessMode.InterfaceExplicit;
                }

                currentSymbol = currentSymbol.BaseType;
            }
        }

        // The method cannot be found.
        return InterfaceMethodAccessMode.None;

        bool EqualParameters(IMethodSymbol methodSymbol)
        {
            for (int index = 0; index < parameterTypes.Length; index++)
            {
                if (!SymbolEqualityComparer.Default.Equals(methodSymbol.Parameters[index].Type, parameterTypes[index]))
                {
                    return false;
                }
            }

            return true;
        }

        bool HasExpectedMethod(ITypeSymbol typeSymbol, string name)
            => typeSymbol
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Any(method => method.Name.Equals(name, StringComparison.Ordinal)
                     && returnTypeCheck(method.ReturnType)
                     && method.Parameters.Length == parameterTypes.Length
                     && EqualParameters(method));
    }
}