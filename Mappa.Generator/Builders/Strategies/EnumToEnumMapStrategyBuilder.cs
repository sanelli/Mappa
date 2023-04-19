// <copyright file="EnumToEnumMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="EnumToEnumMapStrategy"/> strategy.
/// </summary>
internal sealed class EnumToEnumMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly EnumToEnumMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToEnumMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public EnumToEnumMapStrategyBuilder(EnumToEnumMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string StrategySource, string Header) BuildSource(MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new IndentStringBuilder();

        if (mappaGlobalOptions.MappaDebugComments)
        {
            builder.AppendLine($"/* Mappa Rule: {this.strategy.Rule} */ ");
        }

        var sourceEnumFullType = this.strategy.SourceType.ToDisplayString();
        var targetEnumFullType = this.strategy.TargetType.ToDisplayString();

        var temporary = context.NextTemporary();
        builder.AppendLine($"{targetEnumFullType} {temporary};");
        builder.AppendLine($"switch ({this.strategy.Source})");
        using (builder.CodeBlock())
        using (builder.Indent())
        {
            var sourceEnumNames = new HashSet<string>(this.strategy.SourceType.GetEnumValues().Select(enumValue => enumValue.Name));
            var targetEnumNames = new HashSet<string>(this.strategy.TargetType.GetEnumValues().Select(enumValue => enumValue.Name));
            var sharedEnumNames = sourceEnumNames.Intersect(targetEnumNames).OrderBy(enumValue => enumValue).ToArray();

            foreach (var enumName in sharedEnumNames)
            {
                builder.AppendLine($"case {sourceEnumFullType}.{enumName}:");
                using (builder.CodeBlock())
                using (builder.Indent())
                {
                    builder.AppendLine($"{temporary} = {targetEnumFullType}.{enumName};");
                    builder.AppendLine("break;");
                }
            }

            builder.AppendLine($"default:");
            using (builder.CodeBlock())
            using (builder.Indent())
            {
                builder.AppendLine($"throw new System.ArgumentOutOfRangeException(nameof({this.strategy.Source}));");
            }
        }

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} (source-enum is \"{sourceEnumFullType}\", target-enum is \"{targetEnumFullType}\") */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", builder.ToString());
    }
}