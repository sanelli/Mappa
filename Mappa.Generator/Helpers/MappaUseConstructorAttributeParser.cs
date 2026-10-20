// <copyright file="MappaUseConstructorAttributeParser.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Parses <see cref="Mappa.Attributes.MappaUseConstructorAttribute"/> from Roslyn attribute data.
/// </summary>
internal static class MappaUseConstructorAttributeParser
{
    /// <summary>
    /// Tries to parse a single attribute application.
    /// </summary>
    /// <param name="attributeData">The attribute data.</param>
    /// <param name="nonGenericAttributeSymbol">The non-generic attribute type symbol.</param>
    /// <returns>The parsed attribute, or <see langword="null"/> when the attribute is not a use-constructor attribute.</returns>
    internal static MappaUseConstructorAttributeData? TryParse(
        AttributeData attributeData,
        INamedTypeSymbol? nonGenericAttributeSymbol)
    {
        if (attributeData.AttributeClass is not { } attributeClass || nonGenericAttributeSymbol is null)
        {
            return null;
        }

        if (!TryClassify(attributeClass, nonGenericAttributeSymbol, out var isGeneric))
        {
            return null;
        }

        var location = attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation();
        return isGeneric
            ? ParseGenericForm(attributeData, attributeClass, location)
            : ParseNonGenericForm(attributeData, location);
    }

    private static bool TryClassify(
        INamedTypeSymbol attributeClass,
        INamedTypeSymbol nonGenericAttributeSymbol,
        out bool isGeneric)
    {
        var isNonGeneric = SymbolEqualityComparer.Default.Equals(attributeClass, nonGenericAttributeSymbol);
        isGeneric = attributeClass.IsGenericType
            && attributeClass.Arity is >= 1 and <= 8
            && SymbolEqualityComparer.Default.Equals(attributeClass.BaseType, nonGenericAttributeSymbol);
        return isGeneric || isNonGeneric;
    }

    private static MappaUseConstructorAttributeData ParseGenericForm(
        AttributeData attributeData,
        INamedTypeSymbol attributeClass,
        Location? location)
    {
        var parameterTypes = attributeClass.TypeArguments.ToArray();
        var targetType = TryReadOptionalTargetType(attributeData.ConstructorArguments);
        return new MappaUseConstructorAttributeData(
            targetType,
            Mappa.ConstructorType.WithParameters,
            parameterTypes,
            location);
    }

    private static MappaUseConstructorAttributeData? ParseNonGenericForm(
        AttributeData attributeData,
        Location? location)
    {
        var args = attributeData.ConstructorArguments;
        if (args.Length == 0)
        {
            return null;
        }

        if (TryParseConstructorTypeFirstForm(args, location, out var constructorTypeFirstForm))
        {
            return constructorTypeFirstForm;
        }

        return TryParseTargetTypeFirstForm(args, location, out var targetTypeFirstForm)
            ? targetTypeFirstForm
            : null;
    }

    private static bool TryParseConstructorTypeFirstForm(
        ImmutableArray<TypedConstant> args,
        Location? location,
        out MappaUseConstructorAttributeData? result)
    {
        result = null;
        if (!TryReadConstructorType(args[0], out var constructorType))
        {
            return false;
        }

        var parameterTypes = args.Length == 1
            ? []
            : ReadParameterTypeSymbols(args, startIndex: 1);
        result = new MappaUseConstructorAttributeData(null, constructorType, parameterTypes, location);
        return true;
    }

    private static bool TryParseTargetTypeFirstForm(
        ImmutableArray<TypedConstant> args,
        Location? location,
        out MappaUseConstructorAttributeData? result)
    {
        result = null;
        if (args.Length < 2
            || (!args[0].IsNull && args[0].Value is not ITypeSymbol)
            || !TryReadConstructorType(args[1], out var constructorType))
        {
            return false;
        }

        var targetType = args[0].Value as ITypeSymbol;
        var parameterTypes = args.Length == 2
            ? []
            : ReadParameterTypeSymbols(args, startIndex: 2);
        result = new MappaUseConstructorAttributeData(targetType, constructorType, parameterTypes, location);
        return true;
    }

    private static ITypeSymbol? TryReadOptionalTargetType(ImmutableArray<TypedConstant> constructorArguments)
    {
        if (constructorArguments.Length == 1
            && constructorArguments[0].Value is ITypeSymbol explicitTarget)
        {
            return explicitTarget;
        }

        return null;
    }

    private static bool TryReadConstructorType(TypedConstant typedConstant, out Mappa.ConstructorType constructorType)
    {
        // Roslyn stores enum constructor arguments as the underlying integral value.
        if (typedConstant.Value is int intValue)
        {
            constructorType = (Mappa.ConstructorType)intValue;
            return true;
        }

        constructorType = default;
        return false;
    }

    private static ITypeSymbol[] ReadParameterTypeSymbols(
        ImmutableArray<TypedConstant> arguments,
        int startIndex)
    {
        // params Type[] is always a single TypedConstantKind.Array argument in Roslyn.
        return
        [
            .. arguments[startIndex].Values
                .Select(value => value.Value)
                .OfType<ITypeSymbol>(),
        ];
    }
}