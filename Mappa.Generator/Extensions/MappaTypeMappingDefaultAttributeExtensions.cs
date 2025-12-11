// <copyright file="MappaTypeMappingDefaultAttributeExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Exceptions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="MappaTypeMappingDefaultAttribute"/>.
/// </summary>
internal static class MappaTypeMappingDefaultAttributeExtensions
{
    /// <summary>
    /// Check if the attribute provided is valid or not.
    /// </summary>
    /// <param name="attribute">The attribute to validate.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="mapMethodParentClassSymbol">The symbol of the class containing the method being mapped.</param>
    /// <param name="nullableEnabled"><c>true</c> if nullability is enabled, <c>false</c> otherwise.</param>
    /// <param name="mapMethodHasTwoParameters"><c>true</c> if the mapping method has two parameters, <c>false</c> otherwise.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="location">The location on which the diagnostic should be pointing to.</param>
    /// <param name="diagnostics">The generated diagnostic in case of error or warnings.</param>
    /// <returns><c>true</c> if the attribute is valid, <c>false</c> otherwise.</returns>
    internal static bool IsValid(
        this MappaTypeMappingDefaultAttribute attribute,
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        ITypeSymbol mapMethodParentClassSymbol,
        bool nullableEnabled,
        bool mapMethodHasTwoParameters,
        Compilation compilation,
        Location? location,
        out ICollection<Diagnostic> diagnostics)
    {
        diagnostics = [];
        switch (attribute.Behavior)
        {
            case MappaTypeMappingDefaultBehavior.Undefined:
                // TODO [#49] Generate the error diagnostic: unsupported value.
                return false;
            case MappaTypeMappingDefaultBehavior.Throw:
                // Check the type is an exception and there is a constructor that can be used.
                if (attribute.Type is { } attributeType && !string.IsNullOrWhiteSpace(attributeType.FullName))
                {
                    var typeSymbol = compilation.GetTypeByMetadataName(attributeType.FullName.NormalizeType());
                    if (typeSymbol is null)
                    {
                        throw new MappaGeneratorException($"Type '{attributeType.FullName}' cannot be loaded at compile time.");
                    }

                    if (!typeSymbol.CanBeThrown(compilation))
                    {
                        // TODO [#49] Generate the error diagnostic: an exception type is expected.
                        return false;
                    }

                    if (typeSymbol.IsAbstract)
                    {
                        // TODO [#49] Generate the error diagnostic: the exception cannot be abstract.
                        return false;
                    }

                    if (!typeSymbol.HasNamedTypeSymbolAccessibleZeroParametersConstructor(compilation)
                        && !typeSymbol.HasNamedTypeSymbolAccessibleSingleStringParametersConstructor(compilation))
                    {
                        // TODO [#49] Generate the error diagnostic: expected to be able to access either empty constructor or constructor with one string parameter.
                        return false;
                    }
                }

                break;
            case MappaTypeMappingDefaultBehavior.Default:
            case MappaTypeMappingDefaultBehavior.Null:
                // Check the method name is not defined.
                if (!string.IsNullOrEmpty(attribute.MethodName))
                {
                    // TODO [#49] Generate the warning diagnostic: method name will not be used.
                }

                // Check the type is not set.
                if (attribute.Type is not null)
                {
                    // TODO [#49] Generate the warning diagnostic: type will not be used.
                }

                break;
            case MappaTypeMappingDefaultBehavior.MapSourceType:
                if (attribute.Type is { } attributeTargetType && !string.IsNullOrWhiteSpace(attributeTargetType.FullName))
                {
                    var typeSymbol = compilation.GetTypeByMetadataName(attributeTargetType.FullName.NormalizeType());
                    if (typeSymbol is null)
                    {
                        throw new MappaGeneratorException($"Type '{attributeTargetType.FullName}' cannot be loaded at compile time.");
                    }

                    if (!typeSymbol.IsImplementingOrIsDerivedFromClass(targetType))
                    {
                        diagnostics.Add(MappaDiagnostics.ExplicitTargetTypeDoesNotDeriveMapMethodTargetType(typeSymbol, targetType, location));
                        return false;
                    }

                    if (typeSymbol.TypeKind == TypeKind.Interface)
                    {
                        diagnostics.Add(MappaDiagnostics.CannotIdentifyStrategy(typeSymbol, sourceType, location));
                        return false;
                    }

                    if (typeSymbol.IsAbstract)
                    {
                        diagnostics.Add(MappaDiagnostics.CannotIdentifyStrategy(typeSymbol, sourceType, location));
                        return false;
                    }
                }

                break;
            case MappaTypeMappingDefaultBehavior.InvokeMethod:
                // Check the method name is not defined.
                if (string.IsNullOrWhiteSpace(attribute.MethodName))
                {
                    diagnostics.Add(MappaDiagnostics.MethodToInvokeUndefined(location));
                    return false;
                }

                var invokeMethodTypeSymbol =
                    (attribute.Type is { } invokingType && !string.IsNullOrWhiteSpace(invokingType.FullName))
                        ? compilation.GetTypeByMetadataName(invokingType.FullName)
                        : mapMethodParentClassSymbol;

                if (invokeMethodTypeSymbol is null)
                {
                    throw new MappaGeneratorException("Type that can be used to identify the method to invoke cannot be loaded.");
                }

                var methods = invokeMethodTypeSymbol.LocateMethods(attribute.MethodName!);
                var method = methods.FirstOrDefault(m => m.IsMethodValidToMapToTargetSymbolForPolymorphism(
                    sourceType,
                    compilation,
                    attribute.Type is not null,
                    nullableEnabled,
                    mapMethodHasTwoParameters));
                if (method is null)
                {
                    diagnostics.Add(MappaDiagnostics.CannotIdentifySuitableMethodToInvoke(
                        invokeMethodTypeSymbol.ToDisplayString(),
                        attribute.MethodName!,
                        location));
                    return false;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attribute));
        }

        return true;
    }
}