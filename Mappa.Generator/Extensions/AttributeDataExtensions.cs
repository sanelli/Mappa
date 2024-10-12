// <copyright file="AttributeDataExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Exceptions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="AttributeData"/>.
/// </summary>
internal static class AttributeDataExtensions
{
    private static readonly string MappaInvokeMethodAttributeFullName = typeof(MappaInvokeMethodAttribute).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(MappaInvokeMethodAttribute)}");

    /// <summary>
    /// Gets the <see cref="MappaInvokeMethodAttribute"/>s applied to the method.
    /// </summary>
    /// <param name="methodSymbol">The method.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaInvokeMethodAttribute"/> applied to the class.</returns>
    internal static MappaInvokeMethodAttribute[] GetInvokeMethodAttributes(this IMethodSymbol methodSymbol, Compilation compilation)
    {
        var mappaInvokeMethodAttributeSymbol = compilation.GetTypeByMetadataName(MappaInvokeMethodAttributeFullName);
        List<MappaInvokeMethodAttribute> results = new();
        foreach (var constructorArguments in methodSymbol
                     .GetAttributes()
                     .Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappaInvokeMethodAttributeSymbol))
                     .Select(attributeData => attributeData.ConstructorArguments))
        {
                switch (constructorArguments.Length)
                {
                    case 2: // (targetPropertyName, methodName)
                        {
                            if (constructorArguments[0].Value is string targetParameterName &&
                                constructorArguments[1].Value is string methodName)
                            {
                                results.Add(new MappaInvokeMethodAttribute(targetParameterName, methodName));
                            }
                        }

                        break;

                    case 3: // (targetPropertyName, classType, methodName) or (targetPropertyName, fieldName, methodName)
                        {
                            if (constructorArguments[0].Value is string targetParameterName &&
                                constructorArguments[2].Value is string methodName)
                            {
                                switch (constructorArguments[1].Value)
                                {
                                    case string fieldName:
                                        results.Add(new MappaInvokeMethodAttribute(targetParameterName, fieldName, methodName));
                                        break;
                                    case Type classType:
                                        results.Add(new MappaInvokeMethodAttribute(targetParameterName, classType, methodName));
                                        break;
                                }
                            }
                        }

                        break;
                }
        }

        return [.. results];
    }
}