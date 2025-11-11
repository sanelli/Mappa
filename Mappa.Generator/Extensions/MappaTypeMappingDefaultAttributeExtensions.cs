// <copyright file="MappaTypeMappingDefaultAttributeExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Models;

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
    /// <param name="mapMethod">The method being mapped.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="diagnostic">The optional diagnostic in case of errors or warnings.</param>
    /// <returns><c>true</c> if the attribute is valid, <c>false</c> otherwise.</returns>
    internal static bool IsValid(
        this MappaTypeMappingDefaultAttribute attribute,
        MapMethod mapMethod,
        Compilation compilation,
        out Diagnostic? diagnostic)
    {
        diagnostic = null;
        switch (attribute.Behavior)
        {
            case MappaTypeMappingDefaultBehavior.Undefined:
                // TODO [#49] Generate the error diagnostic: unsupported value.
                return false;
            case MappaTypeMappingDefaultBehavior.Throw:

                // Check the method name is not defined.
                if (!string.IsNullOrEmpty(attribute.MethodName))
                {
                    // TODO [#49] Generate the warning diagnostic: method name will not be used.
                }

                // Check the type is an exception and there is a constructor that can be used.
                if (attribute.Type is { } type && !string.IsNullOrWhiteSpace(type.FullName))
                {
                    var typeSymbol = compilation.GetTypeByMetadataName(type.FullName);
                    if (typeSymbol is null)
                    {
                        // TODO [#49] Generate the error diagnostic: the type cannot be loaded.
                        return false;
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
                // Check the method name is not defined.
                if (!string.IsNullOrEmpty(attribute.MethodName))
                {
                    // TODO [#49] Generate the warning diagnostic: method name will not be used.
                }

                if (attribute.Type is { } targetType && !string.IsNullOrWhiteSpace(targetType.FullName))
                {
                    var typeSymbol = compilation.GetTypeByMetadataName(targetType.FullName);
                    if (typeSymbol is null)
                    {
                        // TODO [#49] Generate the error diagnostic: the type cannot be loaded.
                        return false;
                    }

                    if (!typeSymbol.IsImplementingOrIsDerivedFromClass(mapMethod.TargetType))
                    {
                        // TODO [#49] Generate the error diagnostic: the type is not deriving/implementing target type.
                        return false;
                    }

                    if (typeSymbol.TypeKind == TypeKind.Interface)
                    {
                        // TODO [#49] Generate the error diagnostic: the type is not deriving/implementing target type.
                        return false;
                    }

                    if (typeSymbol.IsAbstract)
                    {
                        // TODO [#49] Generate the error diagnostic: the target type cannot be abstract.
                        return false;
                    }
                }

                break;
            case MappaTypeMappingDefaultBehavior.InvokeMethod:
                // Check the method name is not defined.
                if (string.IsNullOrWhiteSpace(attribute.MethodName))
                {
                    // TODO [#49] Generate the error diagnostic: method name is mandatory.
                    return false;
                }

                var invokeMethodTypeSymbol =
                    (attribute.Type is { } invokingType && !string.IsNullOrWhiteSpace(invokingType.FullName))
                        ? compilation.GetTypeByMetadataName(invokingType.FullName)
                        : mapMethod.MethodSymbol.ContainingSymbol as ITypeSymbol;

                if (invokeMethodTypeSymbol is null)
                {
                    // TODO [#49] Generate the error diagnostic: cannot load the type on which invoke the method.
                    return false;
                }

                var methods = invokeMethodTypeSymbol.LocateMethods(compilation, attribute.MethodName!);
                var method = methods.FirstOrDefault(m => IsMethodValidToMapToTargetSymbol(m, attribute.Type is not null));
                if (method is null)
                {
                    // TODO [#49] Generate the error diagnostic: a suitable method with the given name cannot be found.
                    return false;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attribute));
        }

        return true;

        bool IsMethodValidToMapToTargetSymbol(IMethodSymbol methodSymbol, bool mustBeStatic)
        {
            switch (methodSymbol.Parameters.Length)
            {
                case 1:
                    return methodSymbol.AreParametersRefModifiersValid()
                           && methodSymbol.Parameters[0].Type.IsEqualTo(mapMethod.SourceType, mapMethod.NullableEnabled)
                           && (!mustBeStatic || methodSymbol.IsStatic);

                // Accept 2 parameters only when the original method support 2 parameters too.
                case 2 when mapMethod.MethodSymbol.Parameters.Length == 2:
                    var isFirstParameterOk = methodSymbol.Parameters[0].Type.IsEqualTo(mapMethod.SourceType, mapMethod.NullableEnabled);
                    var isSecondParameterOk = methodSymbol.SecondParameterIsMappaContext(compilation);
                    return methodSymbol.AreParametersRefModifiersValid()
                           && isFirstParameterOk
                           && isSecondParameterOk
                           && (!mustBeStatic || methodSymbol.IsStatic);
            }

            return false;
        }
    }
}