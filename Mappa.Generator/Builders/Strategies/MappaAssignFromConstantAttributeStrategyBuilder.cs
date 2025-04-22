// <copyright file="MappaAssignFromConstantAttributeStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;

using Mappa.Generator.Exceptions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="MappaAssignFromConstantAttributeStrategy"/>.
/// </summary>
internal sealed class MappaAssignFromConstantAttributeStrategyBuilder
    : IMappaStrategyBuilder
{
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
        var value = ValueToString(this.strategy.Attribute.Value);
        var code = $"{targetType} {temporary} = {value};";
        return (temporary, code);
    }

    private static string ValueToString(object? value)
    {
        if (value is null)
        {
            return "null";
        }

        var valueType = value.GetType();

        if (valueType.IsArray && valueType.GetArrayRank() == 1 && value is IEnumerable enumerable)
        {
            var items = new List<string>(from object? innerValue in enumerable select ValueToString(innerValue));

            var arrayInitialValues = string.Join(", ", items);
            var elementType = valueType.GetElementType()?.FullName ?? throw new MappaGeneratorException("Cannot detect array type for MappaAssignFromConstant attribute.");
            return $"new {elementType}[]{{ {arrayInitialValues} }}";
        }

        if (valueType.IsEnum)
        {
            return $"{valueType.FullName}.{value}";
        }

        return value switch
        {
            string s => $"\"{s}\"",
            sbyte or byte or short or ushort or int or uint or long or ulong or float or double => $"{value}",
            bool b => b ? "true" : "false",
            char c => $"'{c}'",
            Type t => $"typeof({t.FullName})",
            _ => throw new MappaGeneratorException("Unexpected MappaAssignFromConstant attribute value."),
        };
    }
}