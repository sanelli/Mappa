// <copyright file="ProtobufOneofHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Helper methods to inspect protobuf <c>oneof</c> constructs on message types.
/// </summary>
internal static class ProtobufOneofHelper
{
    /// <summary>
    /// The name of the oneof case that represents "no arm set".
    /// </summary>
    internal const string NoneCaseName = "None";

    private const string OneofCaseSuffix = "OneofCase";
    private const string ByteStringFullName = "Google.Protobuf.ByteString";

    /// <summary>
    /// Finds the public instance readable property named <paramref name="propertyName"/> on the message type
    /// (or on one of its base types).
    /// </summary>
    /// <param name="messageType">The message type.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>The property, or <c>null</c> if it does not exist.</returns>
    internal static IPropertySymbol? FindProperty(ITypeSymbol messageType, string propertyName)
    {
        for (var current = messageType; current is not null; current = current.BaseType)
        {
            foreach (var member in current.GetMembers(propertyName))
            {
                if (member is IPropertySymbol { IsStatic: false, DeclaredAccessibility: Accessibility.Public, GetMethod: not null } property)
                {
                    return property;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the nested <c>*OneofCase</c> enum type of a oneof case property.
    /// </summary>
    /// <param name="caseProperty">The oneof case property.</param>
    /// <returns>The nested enum type, or <c>null</c> if the property type is not an enum named <c>*OneofCase</c> nested in the message type.</returns>
    internal static INamedTypeSymbol? GetCaseEnum(IPropertySymbol caseProperty)
        => caseProperty.Type is INamedTypeSymbol { TypeKind: TypeKind.Enum } enumType
           && enumType.Name.EndsWith(OneofCaseSuffix, StringComparison.Ordinal)
           && SymbolEqualityComparer.Default.Equals(enumType.ContainingType, caseProperty.ContainingType)
            ? enumType
            : null;

    /// <summary>
    /// Returns <c>true</c> when the type declares at least one nested <c>*OneofCase</c> enum.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns><c>true</c> if the type looks like a protobuf message with a oneof.</returns>
    internal static bool HasAnyOneofCaseEnum(ITypeSymbol type)
        => type.GetTypeMembers().Any(nested =>
            nested.TypeKind == TypeKind.Enum
            && nested.Name.EndsWith(OneofCaseSuffix, StringComparison.Ordinal));

    /// <summary>
    /// Returns <c>true</c> when the enum declares a constant member named <paramref name="memberName"/>.
    /// </summary>
    /// <param name="caseEnum">The oneof case enum.</param>
    /// <param name="memberName">The member name.</param>
    /// <returns><c>true</c> if the member exists.</returns>
    internal static bool HasCaseMember(INamedTypeSymbol caseEnum, string memberName)
        => caseEnum.GetMembers(memberName).OfType<IFieldSymbol>().Any(field => field.IsConst);

    /// <summary>
    /// Returns <c>true</c> when the type of a oneof arm is a message type
    /// (neither a value type, nor <see cref="string"/>, nor <c>ByteString</c>).
    /// </summary>
    /// <param name="armType">The arm property type.</param>
    /// <returns><c>true</c> if the arm is message-typed.</returns>
    internal static bool IsMessageType(ITypeSymbol armType)
        => armType is { IsValueType: false, IsReferenceType: true }
           && armType.SpecialType != SpecialType.System_String
           && !string.Equals(armType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat), ByteStringFullName, StringComparison.Ordinal);
}