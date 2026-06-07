// <copyright file="CompilationExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Exceptions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="Compilation"/>.
/// </summary>
internal static class CompilationExtensions
{
    /// <summary>
    /// Obtain the <see cref="INamedTypeSymbol"/> for type <paramref name="type"/>.
    /// </summary>
    /// <param name="compilation">The current compilation.</param>
    /// <param name="type">The type.</param>
    /// <returns>The <see cref="INamedTypeSymbol"/> for type <paramref name="type"/>.</returns>
    /// <exception cref="MappaGeneratorException">When <paramref name="type"/> cannot be loaded.</exception>
    internal static INamedTypeSymbol GetTypeSymbol(this Compilation compilation, Type type)
    {
        var namedTypeSymbol = compilation.GetTypeByMetadataName(type.FullName);
        return namedTypeSymbol ?? throw new MappaGeneratorException($"Cannot obtain named type symbol for '{type.FullName}'.");
    }

    /// <summary>
    /// Obtain the <see cref="INamedTypeSymbol"/> for type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type to be obtained.</typeparam>
    /// <param name="compilation">The current compilation.</param>
    /// <returns>The <see cref="INamedTypeSymbol"/> for type <typeparamref name="TType"/>.</returns>
    /// <exception cref="MappaGeneratorException">When <typeparamref name="TType"/> cannot be loaded.</exception>
    internal static INamedTypeSymbol GetTypeSymbol<TType>(this Compilation compilation)
        => compilation.GetTypeSymbol(typeof(TType));

    /// <summary>
    /// Obtain all methods declared in the base types of <paramref name="typeSymbol"/>.
    /// Each base type is resolved via metadata name before its members are enumerated.
    /// Methods are sorted by most derived base class first.
    /// </summary>
    /// <param name="compilation">The current compilation.</param>
    /// <param name="typeSymbol">The type symbol whose base types will be investigated.</param>
    /// <returns>The methods in the base type hierarchy.</returns>
    internal static IEnumerable<IMethodSymbol> GetMethodsInTypeHierarchyFromMetadata(
        this Compilation compilation,
        INamedTypeSymbol typeSymbol)
    {
        INamedTypeSymbol? currentType = typeSymbol.BaseType;
        while (currentType is not null)
        {
            var metadataName = currentType.GetMetadataName();
            var resolvedType = compilation.GetTypeByMetadataName(metadataName) ?? currentType;

            foreach (var method in resolvedType.GetMembers().OfType<IMethodSymbol>())
            {
                yield return method;
            }

            currentType = currentType.BaseType;
        }
    }

    /// <summary>
    /// Obtain all accessible <see cref="MappaDependencyAttribute"/> properties declared in the base types of <paramref name="mapperClassSymbol"/>.
    /// Each base type is resolved via metadata name before its members are enumerated.
    /// </summary>
    /// <param name="compilation">The current compilation.</param>
    /// <param name="mapperClassSymbol">The mapper class symbol whose base types will be investigated.</param>
    /// <returns>The properties in the mapper base type hierarchy.</returns>
    internal static IEnumerable<IPropertySymbol> GetMappaDependencyPropertiesInMapperBaseTypeHierarchy(
        this Compilation compilation,
        INamedTypeSymbol mapperClassSymbol)
    {
        INamedTypeSymbol? currentType = mapperClassSymbol.BaseType;
        while (currentType is not null)
        {
            var metadataName = currentType.GetMetadataName();
            var resolvedType = compilation.GetTypeByMetadataName(metadataName) ?? currentType;

            foreach (var property in resolvedType.GetMembers().OfType<IPropertySymbol>())
            {
                if (property.GetMethod is null)
                {
                    continue;
                }

                if (!property.GetAttributes().HasMappaDependencyAttribute(compilation))
                {
                    continue;
                }

                if (!compilation.IsSymbolAccessibleWithin(property, mapperClassSymbol))
                {
                    continue;
                }

                yield return property;
            }

            currentType = currentType.BaseType;
        }
    }

    /// <summary>
    /// Obtain all accessible <see cref="MappaDependencyAttribute"/> fields declared in the base types of <paramref name="mapperClassSymbol"/>.
    /// Each base type is resolved via metadata name before its members are enumerated.
    /// </summary>
    /// <param name="compilation">The current compilation.</param>
    /// <param name="mapperClassSymbol">The mapper class symbol whose base types will be investigated.</param>
    /// <returns>The fields in the mapper base type hierarchy.</returns>
    internal static IEnumerable<IFieldSymbol> GetMappaDependencyFieldsInMapperBaseTypeHierarchy(
        this Compilation compilation,
        INamedTypeSymbol mapperClassSymbol)
    {
        INamedTypeSymbol? currentType = mapperClassSymbol.BaseType;
        while (currentType is not null)
        {
            var metadataName = currentType.GetMetadataName();
            var resolvedType = compilation.GetTypeByMetadataName(metadataName) ?? currentType;

            foreach (var field in resolvedType.GetMembers().OfType<IFieldSymbol>())
            {
                if (!field.GetAttributes().HasMappaDependencyAttribute(compilation))
                {
                    continue;
                }

                if (!compilation.IsSymbolAccessibleWithin(field, mapperClassSymbol))
                {
                    continue;
                }

                yield return field;
            }

            currentType = currentType.BaseType;
        }
    }

    /// <summary>
    /// Locate an accessible field or property declared on <paramref name="typeSymbol"/> or one of its base types.
    /// Each type in the hierarchy is resolved via metadata name before its members are enumerated.
    /// Types are evaluated from most derived to least derived; the first level with a unique accessible match is returned.
    /// </summary>
    /// <param name="compilation">The current compilation.</param>
    /// <param name="typeSymbol">The type symbol whose hierarchy will be investigated.</param>
    /// <param name="name">The name of the field or property.</param>
    /// <param name="accessingType">The type from which the field or property must be accessible.</param>
    /// <returns>The field or property symbol, or <see langword="null"/> when none is found.</returns>
    internal static ISymbol? LocateAccessibleFieldOrPropertyInTypeHierarchy(
        this Compilation compilation,
        INamedTypeSymbol typeSymbol,
        string name,
        INamedTypeSymbol accessingType)
    {
        INamedTypeSymbol? currentType = typeSymbol;
        while (currentType is not null)
        {
            var metadataName = currentType.GetMetadataName();
            var resolvedType = compilation.GetTypeByMetadataName(metadataName) ?? currentType;

            var matchingProperties = resolvedType
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(property =>
                    property.Name.Equals(name, StringComparison.Ordinal) &&
                    compilation.IsSymbolAccessibleWithin(property, accessingType))
                .ToArray();

            var matchingFields = resolvedType
                .GetMembers()
                .OfType<IFieldSymbol>()
                .Where(field =>
                    field.Name.Equals(name, StringComparison.Ordinal) &&
                    compilation.IsSymbolAccessibleWithin(field, accessingType))
                .ToArray();

            if (matchingProperties.Length == 0 && matchingFields.Length == 0)
            {
                currentType = currentType.BaseType;
                continue;
            }

            var matchingSymbols = matchingProperties
                .Cast<ISymbol>()
                .Concat(matchingFields)
                .ToArray();

            if (matchingSymbols.Length != 1)
            {
                return null;
            }

            return matchingSymbols[0];
        }

        return null;
    }
}