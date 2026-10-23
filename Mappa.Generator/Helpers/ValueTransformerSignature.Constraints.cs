// <copyright file="ValueTransformerSignature.Constraints.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Verification of the generic constraints of open generic transformers against the bound site type.
/// </summary>
internal static partial class ValueTransformerSignature
{
    private static bool SatisfiesConstraints(
        ITypeParameterSymbol typeParameter,
        ITypeSymbol boundType,
        Compilation compilation)
    {
        return SatisfiesKindConstraints(typeParameter, boundType)
               && SatisfiesConstraintTypes(typeParameter, boundType, compilation);
    }

    private static bool SatisfiesKindConstraints(ITypeParameterSymbol typeParameter, ITypeSymbol boundType)
    {
        if (typeParameter.HasReferenceTypeConstraint && !boundType.IsReferenceType)
        {
            return false;
        }

        if (typeParameter.HasValueTypeConstraint && (!boundType.IsValueType || boundType.IsValueTypeNullable()))
        {
            return false;
        }

        if (typeParameter.HasUnmanagedTypeConstraint && !boundType.IsUnmanagedType)
        {
            return false;
        }

        return !typeParameter.HasConstructorConstraint || HasParameterlessConstructor(boundType);
    }

    private static bool SatisfiesConstraintTypes(
        ITypeParameterSymbol typeParameter,
        ITypeSymbol boundType,
        Compilation compilation)
    {
        // Constraint types that refer to other type parameters cannot be verified safely: do not match.
        return typeParameter.ConstraintTypes.All(constraintType =>
            !ContainsTypeParameter(constraintType)
            && compilation.ClassifyCommonConversion(boundType, constraintType).IsImplicit);
    }

    private static bool HasParameterlessConstructor(ITypeSymbol type)
    {
        return type.IsValueType
               || (type is INamedTypeSymbol { IsAbstract: false } namedType
                   && namedType.InstanceConstructors.Any(IsPublicParameterlessConstructor));
    }

    private static bool IsPublicParameterlessConstructor(IMethodSymbol constructor)
        => constructor.Parameters.Length == 0
           && constructor.DeclaredAccessibility == Accessibility.Public;
}