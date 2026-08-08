// <copyright file="NullableStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="NullableStrategy"/>.
/// </summary>
internal sealed class NullableStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly NullableStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public NullableStrategyBuilder(NullableStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        PrettyCode.StringBuilder stringBuilder = new();
        var targetTemporary = context.NextTemporary();
        var originalSourceTemporary = source;

        stringBuilder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {targetTemporary};");
        stringBuilder.AppendLine(this.strategy.SourceType.IsReferenceType
            ? $"if ({source} is not null)"
            : $"if ({source}.HasValue)");
        using (stringBuilder.CurlyBracesBlock())
        {
            this.AppendNullableIfBlock(stringBuilder, context, mappaGlobalOptions, originalSourceTemporary, targetTemporary, ref source);
        }

        stringBuilder.AppendLine("else");
        using (stringBuilder.CurlyBracesBlock())
        {
            this.AppendNullableElseBlock(stringBuilder, context, originalSourceTemporary, targetTemporary);
        }

        return (targetTemporary, stringBuilder.ToString());
    }

    private static string GetNullableSuppressSuffix(MappaBuilderContext context, ITypeSymbol targetType)
    {
        if (!context.GetMapMethod().NullableEnabled)
        {
            return string.Empty;
        }

        if (targetType is not { IsReferenceType: true, NullableAnnotation: NullableAnnotation.None })
        {
            return string.Empty;
        }

        return "!";
    }

    private void AppendNullableIfBlock(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        MappaGlobalOptions mappaGlobalOptions,
        string originalSourceTemporary,
        string targetTemporary,
        ref string source)
    {
        source = context.NextTemporary();

        if (this.strategy.SourceType.IsValueType)
        {
            stringBuilder.AppendLine($"{this.strategy.SourceType.GetTypeInsideNullable()} {source} = {originalSourceTemporary}.Value;");
        }
        else
        {
            var type = this.strategy.SourceType.ToDisplayString();
            if (type.EndsWith("?", StringComparison.Ordinal))
            {
                type = type.Substring(0, type.Length - 1);
            }

            stringBuilder.AppendLine($"{type} {source} = {originalSourceTemporary};");
        }

        var (elementTemporary, elementCode) = ReferenceHandlingCodeGenerator.BuildNestedSource(
            this.strategy.ElementStrategy,
            source,
            context,
            mappaGlobalOptions);
        if (!string.IsNullOrEmpty(elementCode))
        {
            stringBuilder.AppendEmptyLine();
            stringBuilder.AppendLine(elementCode);
        }

        stringBuilder.AppendLine($"{targetTemporary} = {elementTemporary};");
    }

    private void AppendNullableElseBlock(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        string originalSourceTemporary,
        string targetTemporary)
    {
        var suppressNullableWarningNull = GetNullableSuppressSuffix(context, this.strategy.TargetType);
        stringBuilder.AppendLine(this.strategy.TargetType.IsNullable()
            ? $"{targetTemporary} = ({this.strategy.TargetType.ToDisplayString()}) null{suppressNullableWarningNull};"
            : $"throw new System.NullReferenceException(\"\\\"{originalSourceTemporary}\\\" is null.\");");
    }
}