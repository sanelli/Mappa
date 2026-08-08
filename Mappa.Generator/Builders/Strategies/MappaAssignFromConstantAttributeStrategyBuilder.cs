// <copyright file="MappaAssignFromConstantAttributeStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="MappaAssignFromConstantAttributeStrategy"/>.
/// </summary>
internal sealed class MappaAssignFromConstantAttributeStrategyBuilder
    : IMappaStrategyBuilder
{
    private static readonly HashSet<Type> NumericPrimitiveTypes =
    [
        typeof(sbyte),
        typeof(byte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
    ];

    private readonly MappaAssignFromConstantAttributeStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaAssignFromConstantAttributeStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public MappaAssignFromConstantAttributeStrategyBuilder(MappaAssignFromConstantAttributeStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var targetType = this.strategy.TargetType.ToDisplayString();
        var value = ValueToCode(this.strategy.Attribute.Value);
        var code = $"{targetType} {temporary} = {value};";
        return (temporary, code);
    }

    private static string ValueToCode(object? value)
    {
        if (value is null)
        {
            return "null";
        }

        if (value is string stringValue)
        {
            return CSharpLiteralHelper.ToStringLiteral(stringValue);
        }

        if (value is bool boolValue)
        {
            return boolValue ? "true" : "false";
        }

        if (value is char charValue)
        {
            return CSharpLiteralHelper.ToCharLiteral(charValue);
        }

        if (IsNumericPrimitive(value))
        {
            return $"{value}";
        }

        if (value is TypedConstant typedConstant)
        {
            return TypedConstantToCode(typedConstant);
        }

        throw new MappaGeneratorException("Unexpected MappaAssignFromConstant attribute value.");
    }

    private static bool IsNumericPrimitive(object value)
        => NumericPrimitiveTypes.Contains(value.GetType());

    private static string TypedConstantToCode(TypedConstant typedConstant) =>
        typedConstant.Kind switch
        {
            TypedConstantKind.Primitive => ValueToCode(typedConstant.Value),
            TypedConstantKind.Array => GetCodeForArrays(typedConstant),
            TypedConstantKind.Type when typedConstant.Value is ITypeSymbol typeSymbol =>
                $"typeof({typeSymbol.ToDisplayString()})",
            TypedConstantKind.Enum when
                typedConstant.Value is not null &&
                typedConstant.Type is not null &&
                typedConstant.Type.IsEnum() =>
                GetCodeForEnum(typedConstant.Type, typedConstant.Value),
            _ => throw new MappaGeneratorException("Unexpected MappaAssignFromConstant attribute value."),
        };

    private static string GetCodeForEnum(ITypeSymbol typeSymbol, object integerEnumValue)
    {
        #pragma warning disable S3267 // Loops should be simplified using the "Where" method
        foreach (var enumValue in typeSymbol.GetEnumValues())
        #pragma warning restore S3267 // Loops should be simplified using the "Where" method
        {
            if (enumValue.Value is not null && integerEnumValue.Equals(enumValue.Value))
            {
                return $"{typeSymbol.ToDisplayString()}.{enumValue.Name}";
            }
        }

        throw new MappaGeneratorException("Unexpected enumeration value");
    }

    private static string GetCodeForArrays(TypedConstant array)
    {
        var arrayValues = array.Values;
        var valuesAsCode = new string[arrayValues.Length];
        for (var i = 0; i < arrayValues.Length; i++)
        {
            valuesAsCode[i] = ValueToCode(arrayValues[i]);
        }

        return $"new {array.Type?.ToDisplayString()}{{ {string.Join(", ", valuesAsCode)} }}";
    }
}