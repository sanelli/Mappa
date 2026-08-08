// <copyright file="MappaDependencyInjectionAttributeData.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describes a parsed <see cref="Mappa.Attributes.MappaDependencyInjectionAttribute"/>.
/// </summary>
internal sealed class MappaDependencyInjectionAttributeData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaDependencyInjectionAttributeData"/> class.
    /// </summary>
    /// <param name="constructorMethodName">The method name from the constructor, if any.</param>
    /// <param name="methodName">The method name override from the named property, if any.</param>
    /// <param name="extensionMethod">Whether to generate an extension method on static classes.</param>
    /// <param name="accessibility">The accessibility of the generated method.</param>
    /// <param name="serviceLifetime">The DI service lifetime.</param>
    /// <param name="injectInterfaces">How classes and interfaces are registered.</param>
    /// <param name="ignoreTypes">Types to exclude from registration.</param>
    /// <param name="injectFromAssemblies">Marker types whose assemblies are also scanned.</param>
    /// <param name="location">The attribute location.</param>
    internal MappaDependencyInjectionAttributeData(
        string? constructorMethodName,
        string? methodName,
        bool extensionMethod,
        MappaDependencyInjectionMethodAccessibility accessibility,
        MappaDependencyInjectionServiceLifetime serviceLifetime,
        MappaDependencyInjectionInjectInterfaces injectInterfaces,
        ImmutableArray<INamedTypeSymbol> ignoreTypes,
        ImmutableArray<INamedTypeSymbol> injectFromAssemblies,
        Location? location)
    {
        this.ConstructorMethodName = constructorMethodName;
        this.MethodName = methodName;
        this.ExtensionMethod = extensionMethod;
        this.Accessibility = accessibility;
        this.ServiceLifetime = serviceLifetime;
        this.InjectInterfaces = injectInterfaces;
        this.IgnoreTypes = ignoreTypes;
        this.InjectFromAssemblies = injectFromAssemblies;
        this.Location = location;
    }

    /// <summary>
    /// Gets the method name supplied via the constructor, if any.
    /// </summary>
    internal string? ConstructorMethodName { get; }

    /// <summary>
    /// Gets the method name override from the named property, if any.
    /// </summary>
    internal string? MethodName { get; }

    /// <summary>
    /// Gets a value indicating whether a static registrar should generate an extension method.
    /// </summary>
    internal bool ExtensionMethod { get; }

    /// <summary>
    /// Gets the accessibility of the generated method.
    /// </summary>
    internal MappaDependencyInjectionMethodAccessibility Accessibility { get; }

    /// <summary>
    /// Gets the service lifetime used when registering mapper types.
    /// </summary>
    internal MappaDependencyInjectionServiceLifetime ServiceLifetime { get; }

    /// <summary>
    /// Gets how mapper classes and their interfaces are registered.
    /// </summary>
    internal MappaDependencyInjectionInjectInterfaces InjectInterfaces { get; }

    /// <summary>
    /// Gets the types to exclude from dependency injection registration.
    /// </summary>
    internal ImmutableArray<INamedTypeSymbol> IgnoreTypes { get; }

    /// <summary>
    /// Gets marker types whose assemblies are scanned in addition to the registrar's assembly.
    /// </summary>
    internal ImmutableArray<INamedTypeSymbol> InjectFromAssemblies { get; }

    /// <summary>
    /// Gets the attribute location.
    /// </summary>
    internal Location? Location { get; }

    /// <summary>
    /// Resolves the effective registration method name for the given registrar class name.
    /// </summary>
    /// <param name="className">The registrar class name.</param>
    /// <returns>The method name to generate.</returns>
    internal string ResolveMethodName(string className)
    {
        var methodName = this.MethodName;
        if (methodName is not null && !string.IsNullOrWhiteSpace(methodName))
        {
            return methodName;
        }

        var constructorMethodName = this.ConstructorMethodName;
        if (constructorMethodName is not null && !string.IsNullOrWhiteSpace(constructorMethodName))
        {
            return constructorMethodName;
        }

        return $"Register{className}";
    }

    /// <summary>
    /// Returns <c>true</c> when <paramref name="type"/> is listed in <see cref="IgnoreTypes"/>.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> when the type should be ignored; otherwise <c>false</c>.</returns>
    internal bool IsIgnored(INamedTypeSymbol type)
        => this.IgnoreTypes.Any(ignored => SymbolEqualityComparer.Default.Equals(ignored, type));
}