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
}