// <copyright file="MethodSymbolExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="IMethodSymbol"/>.
/// </summary>
internal static class MethodSymbolExtensions
{
    private const string MappaContextTypeFullName = "Mappa.MappaContext";

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
        var attributes = methodSymbol.GetAttributes();

        // Mappa Invoke Method Attributes
        var invokeMethodAttributes = attributes.GetInvokeMethodAttributes(compilation);
        result.AddRange(invokeMethodAttributes);

        // Mappa Assign From Context Attributes
        var assignFromContextAttributes = attributes.GetMappaAssignFromContextAttributes(compilation);
        result.AddRange(assignFromContextAttributes);

        var assignToContextAttributes = attributes.GetMappaAssignToContextAttributes(compilation);
        result.AddRange(assignToContextAttributes);

        // Mappa Setting Attribute
        if (attributes.GetMappaSettingsAttribute(compilation) is { } mappaSettingAttribute)
        {
            result.Add(mappaSettingAttribute);
        }

        // Mappa Use Property Attribute
        var usePropertyAttributes = attributes.GetMappaUsePropertyAttributes(compilation);
        result.AddRange(usePropertyAttributes);

        // Mappa Assign From Constant Attributes
        var assignFromConstantAttributes = attributes.GetMappaAssignFromConstantAttributes(compilation);
        result.AddRange(assignFromConstantAttributes);

        // Mappa Ignore Target Property Attributes
        var ignoreTargetPropertyAttributes = attributes.GetMappaIgnoreTargetPropertyAttributes(compilation);
        result.AddRange(ignoreTargetPropertyAttributes);

        // Mappa Must Map Target Property Attribute
        if (attributes.GetMappaMustMapTargetPropertyAttribute(compilation) is { } mustMapTargetPropertyAttribute)
        {
            result.Add(mustMapTargetPropertyAttribute);
        }

        // Mappa Allow Inaccessible Source/Target Members Attributes
        if (attributes.GetMappaAllowInaccessibleSourceMembersAttribute(compilation) is { } allowInaccessibleSourceMembersAttribute)
        {
            result.Add(allowInaccessibleSourceMembersAttribute);
        }

        if (attributes.GetMappaAllowInaccessibleTargetMembersAttribute(compilation) is { } allowInaccessibleTargetMembersAttribute)
        {
            result.Add(allowInaccessibleTargetMembersAttribute);
        }

        // Mappa type mapping attributes
        var typeMappingAttributes = attributes.GetTypeMappingAttributes(compilation);
        result.AddRange(typeMappingAttributes);

        // Mappa type mapping default attribute
        if (attributes.GetMappaTypeMappingDefaultAttribute(compilation) is { } mappaTypeMappingDefaultAttribute)
        {
            result.Add(mappaTypeMappingDefaultAttribute);
        }

        // Mappa enum mapping configuration attributes
        result.AddRange(attributes.GetEnumMapMemberAttributes(compilation));
        result.AddRange(attributes.GetEnumMapIgnoreAttributes(compilation));
        result.AddRange(attributes.GetEnumMapDefaultAttributes(compilation));

        // All done.
        return [.. result];
    }

    /// <summary>
    /// Gets the <see cref="MappaContext"/> type from the compilation.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaContext"/> type, or <c>null</c> if not found.</returns>
    internal static INamedTypeSymbol? GetMappaContextType(this Compilation compilation)
        => compilation.GetTypeByMetadataName(MappaContextTypeFullName);

    /// <summary>
    /// Returns <c>true</c> if <paramref name="type"/> is <see cref="MappaContext"/>.
    /// </summary>
    /// <param name="type">The type to validate.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type is <see cref="MappaContext"/>.</returns>
    internal static bool IsMappaContextType(this ITypeSymbol type, Compilation compilation)
    {
        var mappaContextType = compilation.GetMappaContextType();
        return mappaContextType is not null && SymbolEqualityComparer.Default.Equals(mappaContextType, type);
    }

    /// <summary>
    /// Returns <c>true</c> if the parameter at <paramref name="index"/> is of type <see cref="MappaContext"/>.
    /// </summary>
    /// <param name="methodSymbol">The method to validate.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="index">The parameter index.</param>
    /// <returns><c>true</c> if the parameter is of type <see cref="MappaContext"/>.</returns>
    internal static bool ParameterIsMappaContext(
        this IMethodSymbol methodSymbol,
        Compilation compilation,
        int index)
    {
        if (index < 0 || index >= methodSymbol.Parameters.Length)
        {
            return false;
        }

        return methodSymbol.Parameters[index].Type.IsMappaContextType(compilation);
    }

    /// <summary>
    /// Returns <c>true</c> if the method has at least one parameter of type <see cref="MappaContext"/>.
    /// </summary>
    /// <param name="methodSymbol">The method to validate.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the method has a <see cref="MappaContext"/> parameter.</returns>
    internal static bool MethodHasMappaContextParameter(this IMethodSymbol methodSymbol, Compilation compilation)
    {
        for (var index = 0; index < methodSymbol.Parameters.Length; index++)
        {
            if (methodSymbol.ParameterIsMappaContext(compilation, index))
            {
                return true;
            }
        }

        return false;
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

        return methodSymbol.ParameterIsMappaContext(compilation, 1);
    }

    /// <summary>
    /// Returns <c>true</c> when the method maps <see cref="System.Linq.IQueryable{T}"/>
    /// to <see cref="System.Linq.IQueryable{T}"/>.
    /// </summary>
    /// <param name="methodSymbol">The method to validate.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> when the method is a queryable projection map method.</returns>
    internal static bool IsQueryableProjectionMapMethod(this IMethodSymbol methodSymbol, Compilation compilation)
        => methodSymbol.Parameters.Length > 0
           && methodSymbol.Parameters[0].Type.IsOrImplementIQueryable(compilation)
           && methodSymbol.ReturnType.IsOrImplementIQueryable(compilation);

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
        bool HasExpectedMethod(ITypeSymbol typeSymbol, string name)
            => typeSymbol
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Any(method => method.Name.Equals(name, StringComparison.Ordinal)
                     && returnTypeCheck(method.ReturnType)
                     && method.Parameters.Length == parameterTypes.Length
                     && EqualParameters(method));

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

        if (ExistsInTypeHierarchy(targetTypeSymbol, type => HasExpectedMethod(type, methodName)))
        {
            return InterfaceMethodAccessMode.Direct;
        }

        string explicitName = $"{fullInterfaceName}<{elementTypeName}>.{methodName}";
        if (ExistsInTypeHierarchy(targetTypeSymbol, type => HasExpectedMethod(type, explicitName)))
        {
            return InterfaceMethodAccessMode.InterfaceExplicit;
        }

        if (targetTypeSymbol is INamedTypeSymbol
            { OriginalDefinition.TypeArguments.Length: > 0 } namedTypeSymbol)
        {
            var typeArgumentName = namedTypeSymbol.OriginalDefinition.TypeArguments[0].Name;
            string genericName = $"{fullInterfaceName}<{typeArgumentName}>.{methodName}";
            if (ExistsInTypeHierarchy(targetTypeSymbol, type => HasExpectedMethod(type, genericName)))
            {
                return InterfaceMethodAccessMode.InterfaceExplicit;
            }
        }

        return InterfaceMethodAccessMode.None;
    }

    /// <summary>
    /// Check if <see cref="IDictionary{TKey,TValue}.this[TKey]"/> can be accessed directly
    /// of need an interface because it was implemented explicitly.
    /// </summary>
    /// <param name="targetTypeSymbol">The target type implementing <see cref="IDictionary{TKey,TValue}"/>.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The way the indexer can be accessed.</returns>
    internal static InterfaceMethodAccessMode GetIDictionaryInterfaceIndexerAccessMode(
        this ITypeSymbol targetTypeSymbol,
        Compilation compilation)
    {
        var (keyType, valueType) = targetTypeSymbol.GetKeyAndValueTypes(compilation);

        bool HasIndexer(string name)
            => targetTypeSymbol.GetMembers()
                .OfType<IPropertySymbol>()
                .Any(propertySymbol => propertySymbol.IsIndexer
                                         && propertySymbol.Parameters.Length == 1
                                         && propertySymbol.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                                         && SymbolEqualityComparer.Default.Equals(propertySymbol.Type, valueType)
                                         && SymbolEqualityComparer.Default.Equals(propertySymbol.Parameters[0].Type, keyType));

        if (HasIndexer("this[]"))
        {
            return InterfaceMethodAccessMode.Direct;
        }

        var nonGenericName = $"System.Collections.Generic.IDictionary<{TypeSymbolExtensions.NormalizeType(keyType.ToDisplayString())},{TypeSymbolExtensions.NormalizeType(valueType.ToDisplayString())}>.this[]";
        if (HasIndexer(nonGenericName))
        {
            return InterfaceMethodAccessMode.InterfaceExplicit;
        }

        if (targetTypeSymbol is INamedTypeSymbol { OriginalDefinition.TypeArguments.Length: 2 } namedTypeSymbol)
        {
            var keyTypeArgument = namedTypeSymbol.OriginalDefinition.TypeArguments[0].Name;
            var valueTypeArgument = namedTypeSymbol.OriginalDefinition.TypeArguments[1].Name;
            var genericName = $"System.Collections.Generic.IDictionary<{keyTypeArgument},{valueTypeArgument}>.this[]";
            if (HasIndexer(genericName))
            {
                return InterfaceMethodAccessMode.InterfaceExplicit;
            }
        }

        return InterfaceMethodAccessMode.None;
    }

    /// <summary>
    /// Check if <see cref="IDictionary{TKey,TValue}.Add(TKey, TValue)"/> can be accessed directly
    /// or needs an interface because it was implemented explicitly.
    /// </summary>
    /// <param name="targetTypeSymbol">The target type implementing <see cref="IDictionary{TKey,TValue}"/>.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The way the <c>Add</c> method can be accessed.</returns>
    internal static InterfaceMethodAccessMode GetIDictionaryInterfaceAddAccessMode(
        this ITypeSymbol targetTypeSymbol,
        Compilation compilation)
    {
        var (keyType, valueType) = targetTypeSymbol.GetKeyAndValueTypes(compilation);

        bool HasAdd(string name)
            => targetTypeSymbol.GetMembers()
                .OfType<IMethodSymbol>()
                .Any(methodSymbol => methodSymbol.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                                     && methodSymbol.ReturnType.IsVoid()
                                     && methodSymbol.Parameters.Length == 2
                                     && SymbolEqualityComparer.Default.Equals(methodSymbol.Parameters[0].Type, keyType)
                                     && SymbolEqualityComparer.Default.Equals(methodSymbol.Parameters[1].Type, valueType));

        if (HasAdd("Add"))
        {
            return InterfaceMethodAccessMode.Direct;
        }

        var nonGenericName = $"System.Collections.Generic.IDictionary<{TypeSymbolExtensions.NormalizeType(keyType.ToDisplayString())},{TypeSymbolExtensions.NormalizeType(valueType.ToDisplayString())}>.Add";
        if (HasAdd(nonGenericName))
        {
            return InterfaceMethodAccessMode.InterfaceExplicit;
        }

        if (targetTypeSymbol is INamedTypeSymbol { OriginalDefinition.TypeArguments.Length: 2 } namedTypeSymbol)
        {
            var keyTypeArgument = namedTypeSymbol.OriginalDefinition.TypeArguments[0].Name;
            var valueTypeArgument = namedTypeSymbol.OriginalDefinition.TypeArguments[1].Name;
            var genericName = $"System.Collections.Generic.IDictionary<{keyTypeArgument},{valueTypeArgument}>.Add";
            if (HasAdd(genericName))
            {
                return InterfaceMethodAccessMode.InterfaceExplicit;
            }
        }

        return InterfaceMethodAccessMode.None;
    }

    /// <summary>
    /// Validate that the ref kind for the method parameters are
    /// either <see cref="RefKind.None"/> or <see cref="RefKind.In"/>.
    /// </summary>
    /// <param name="methodSymbol">The method symbol.</param>
    /// <returns><c>true</c> if the ref modifiers are valid, <c>false</c> otherwise.</returns>
    internal static bool AreParametersRefModifiersValid(this IMethodSymbol methodSymbol)
    {
        if (methodSymbol.Parameters[0].RefKind != RefKind.None
            && methodSymbol.Parameters[0].RefKind != RefKind.In)
        {
            return false;
        }

        if (methodSymbol.Parameters.Length == 2
            && methodSymbol.Parameters[1].RefKind != RefKind.None
            && methodSymbol.Parameters[1].RefKind != RefKind.In)
        {
            return false;
        }

        return true;
    }

    private static bool ExistsInTypeHierarchy(ITypeSymbol typeSymbol, Func<ITypeSymbol, bool> predicate)
    {
        var currentSymbol = typeSymbol;
        while (currentSymbol is not null)
        {
            if (predicate(currentSymbol))
            {
                return true;
            }

            currentSymbol = currentSymbol.BaseType;
        }

        return false;
    }
}