// <copyright file="NullableStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

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
        // TTargetType target
        // if ( source.HasValue OR source is not null )
        // begin
        //   -> Mapping
        //   target := mapped
        // end else begin
        //   target.IsNullable -> mapped := null
        //  !target.IsNullable -> throw NullReference
        // end.
        PrettyCode.StringBuilder stringBuilder = new();
        var targetTemporary = context.NextTemporary();
        var originalSourceTemporary = source;

        stringBuilder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {targetTemporary};");

        // if block.
        stringBuilder.AppendLine(this.strategy.SourceType.IsReferenceType
            ? $"if ({source} is not null)"
            : $"if ({source}.HasValue)");
        using (stringBuilder.CurlyBracesBlock())
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

            var (elementTemporary, elementCode) = this.strategy.ElementStrategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);
            if (!string.IsNullOrEmpty(elementCode))
            {
                stringBuilder.AppendEmptyLine();
                stringBuilder.AppendLine(elementCode);
            }

            stringBuilder.AppendLine($"{targetTemporary} = {elementTemporary};");
        }

        // else block
        stringBuilder.AppendLine("else");
        using (stringBuilder.CurlyBracesBlock())
        {
            stringBuilder.AppendLine(this.strategy.TargetType.IsNullable()
                ? $"{targetTemporary} = ({this.strategy.TargetType.ToDisplayString()}) null;"
                : $"throw new System.NullReferenceException(\"\\\"{originalSourceTemporary}\\\" is null.\");");
        }

        return (targetTemporary, stringBuilder.ToString());
    }
}